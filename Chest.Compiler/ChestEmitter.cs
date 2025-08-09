using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Chest.Compiler;

/// <summary>
/// Symbol table to manage scopes and local variables
/// </summary>
public class SymbolTable
{
    private readonly Stack<Dictionary<string, (LocalBuilder Local, Type Type)>> _scopes = new();
    
    public void PushScope()
    {
        _scopes.Push(new Dictionary<string, (LocalBuilder, Type)>());
    }
    
    public void PopScope()
    {
        if (_scopes.Count > 0)
            _scopes.Pop();
    }
    
    public void DeclareVariable(string name, LocalBuilder local, Type type)
    {
        if (_scopes.Count == 0)
            throw new InvalidOperationException("No active scope to declare variable");
            
        var currentScope = _scopes.Peek();
        if (currentScope.ContainsKey(name))
            throw new InvalidOperationException($"Variable '{name}' has already been declared in this scope");
            
        currentScope[name] = (local, type);
    }
    
    public LocalBuilder? LookupVariable(string name)
    {
        foreach (var scope in _scopes)
        {
            if (scope.TryGetValue(name, out var info))
                return info.Local;
        }
        return null;
    }
    
    public Type? LookupVariableType(string name)
    {
        foreach (var scope in _scopes)
        {
            if (scope.TryGetValue(name, out var info))
                return info.Type;
        }
        return null;
    }
    
    public bool IsVariableDeclared(string name)
    {
        return LookupVariable(name) != null;
    }
}

/// <summary>
/// Generated assembly with information for execution
/// </summary>
public class ChestAssembly
{
    public Assembly Assembly { get; }
    public Type EntryType { get; }
    public MethodInfo EntryMethod { get; }
    
    public ChestAssembly(Assembly assembly, Type entryType, MethodInfo entryMethod)
    {
        Assembly = assembly;
        EntryType = entryType;
        EntryMethod = entryMethod;
    }
    
    /// <summary>
    /// Executes the compiled Chest program
    /// </summary>
    public void Execute()
    {
        EntryMethod.Invoke(null, null);
    }
}

/// <summary>
/// IL generator using Reflection.Emit to compile Chest AST to .NET IL
/// </summary>
public class ChestEmitter
{
    private AssemblyBuilder _assemblyBuilder = null!;
    private ModuleBuilder _moduleBuilder = null!;
    private readonly Dictionary<string, TypeBuilder> _officeTypes = new();
    private readonly Dictionary<string, Type> _finishedTypes = new();
    
    /// <summary>
    /// Compiles a Chest program to a .NET assembly
    /// </summary>
    public ChestAssembly Emit(ProgramNode program)
    {
        CreateAssembly();
        
        // First pass: create types (offices)
        foreach (var building in program.Buildings)
        {
            CreateOfficeTypes(building);
        }
        
        // Second pass: implement methods (employees)
        foreach (var building in program.Buildings)
        {
            ImplementOfficeMembers(building);
        }
        
        // Finalize types
        foreach (var kvp in _officeTypes)
        {
            _finishedTypes[kvp.Key] = kvp.Value.CreateType()!;
        }
        
        // Find entry point
        var (entryType, entryMethod) = FindEntryPoint(program);
        
        return new ChestAssembly(_assemblyBuilder, entryType, entryMethod);
    }
    
    private void CreateAssembly()
    {
        var assemblyName = new AssemblyName("ChestProgram");
        _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            assemblyName, 
            AssemblyBuilderAccess.RunAndCollect);
        _moduleBuilder = _assemblyBuilder.DefineDynamicModule("ChestModule");
    }
    
    private void CreateOfficeTypes(BuildingNode building)
    {
        foreach (var member in building.Members)
        {
            if (member is OfficeNode office)
            {
                var typeName = $"{building.Name}.{office.Name}";
                var typeBuilder = _moduleBuilder.DefineType(
                    typeName,
                    TypeAttributes.Public | TypeAttributes.Class | 
                    TypeAttributes.Abstract | TypeAttributes.Sealed); // static class
                
                _officeTypes[typeName] = typeBuilder;
            }
        }
    }
    
    private void ImplementOfficeMembers(BuildingNode building)
    {
        foreach (var member in building.Members)
        {
            if (member is OfficeNode office)
            {
                var typeName = $"{building.Name}.{office.Name}";
                var typeBuilder = _officeTypes[typeName];
                
                foreach (var officeMember in office.Members)
                {
                    if (officeMember is EmployeeNode employee)
                    {
                        EmitEmployee(typeBuilder, employee);
                    }
                }
            }
        }
    }
    
    private void EmitEmployee(TypeBuilder typeBuilder, EmployeeNode employee)
    {
        var methodBuilder = typeBuilder.DefineMethod(
            employee.Name,
            MethodAttributes.Public | MethodAttributes.Static,
            typeof(void),
            Type.EmptyTypes); // For now, no parameters
        
        var il = methodBuilder.GetILGenerator();
        var symbolTable = new SymbolTable();
        symbolTable.PushScope();
        
        try
        {
            foreach (var stmt in employee.Body)
            {
                EmitStatement(il, stmt, symbolTable);
            }
            
            il.Emit(OpCodes.Ret);
        }
        finally
        {
            symbolTable.PopScope();
        }
    }
    
    private void EmitStatement(ILGenerator il, Node stmt, SymbolTable symbolTable)
    {
        switch (stmt)
        {
            case VarDeclNode varDecl:
                EmitVarDeclaration(il, varDecl, symbolTable);
                break;
                
            case ShowNode show:
                EmitShow(il, show, symbolTable);
                break;
                
            case DecideNode decide:
                EmitDecide(il, decide, symbolTable);
                break;
                
            default:
                throw new NotSupportedException($"Tipo de statement não suportado: {stmt.GetType()}");
        }
    }
    
    private void EmitVarDeclaration(ILGenerator il, VarDeclNode varDecl, SymbolTable symbolTable)
    {
        // For simplicity, always use object as variable type
        // This avoids boxing/unboxing problems in the first version
        var varType = typeof(object);
        
        // Declarar local
        var local = il.DeclareLocal(varType);
        symbolTable.DeclareVariable(varDecl.Name, local, varType);
        
        // If there's initialization, generate code
        if (varDecl.Init != null)
        {
            EmitExpression(il, varDecl.Init, symbolTable, varType);
            il.Emit(OpCodes.Stloc, local);
        }
    }
    
    private void EmitShow(ILGenerator il, ShowNode show, SymbolTable symbolTable)
    {
        // Generate expression
        EmitExpression(il, show.Expr, symbolTable, typeof(object));
        
        // Chamar ChestRuntime.Show
        var showMethod = typeof(ChestRuntime).GetMethod("Show", new[] { typeof(object) })!;
        il.EmitCall(OpCodes.Call, showMethod, null);
    }
    
    private void EmitDecide(ILGenerator il, DecideNode decide, SymbolTable symbolTable)
    {
        var elseLabel = il.DefineLabel();
        var endLabel = il.DefineLabel();
        
        // Generate condition
        EmitExpression(il, decide.Cond, symbolTable, typeof(bool));
        
        // Se falso, pular para else
        il.Emit(OpCodes.Brfalse, elseLabel);
        
        // Bloco then
        symbolTable.PushScope();
        try
        {
            foreach (var stmt in decide.Then)
            {
                EmitStatement(il, stmt, symbolTable);
            }
        }
        finally
        {
            symbolTable.PopScope();
        }
        
        il.Emit(OpCodes.Br, endLabel);
        
        // Bloco else (se existir)
        il.MarkLabel(elseLabel);
        if (decide.Else != null)
        {
            symbolTable.PushScope();
            try
            {
                foreach (var stmt in decide.Else)
                {
                    EmitStatement(il, stmt, symbolTable);
                }
            }
            finally
            {
                symbolTable.PopScope();
            }
        }
        
        il.MarkLabel(endLabel);
    }
    
    private void EmitExpression(ILGenerator il, ExprNode expr, SymbolTable symbolTable, Type expectedType)
    {
        switch (expr)
        {
            case LiteralNode literal:
                EmitLiteral(il, literal, expectedType);
                break;
                
            case IdentNode ident:
                EmitIdentifier(il, ident, symbolTable);
                break;
                
            case BinaryNode binary:
                EmitBinaryOperation(il, binary, symbolTable);
                break;
                
            default:
                throw new NotSupportedException($"Unsupported statement type:: {expr.GetType()}");
        }
    }
    
    private void EmitLiteral(ILGenerator il, LiteralNode literal, Type expectedType)
    {
        switch (literal.Type)
        {
            case ChestType.Number:
                il.Emit(OpCodes.Ldc_R8, Convert.ToDouble(literal.Value));
                if (expectedType == typeof(object))
                    il.Emit(OpCodes.Box, typeof(double));
                break;
                
            case ChestType.Text:
                il.Emit(OpCodes.Ldstr, literal.Value?.ToString() ?? "");
                break;
                
            case ChestType.Bool:
                il.Emit(Convert.ToBoolean(literal.Value) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                if (expectedType == typeof(object))
                    il.Emit(OpCodes.Box, typeof(bool));
                break;
        }
    }
    
    private void EmitIdentifier(ILGenerator il, IdentNode ident, SymbolTable symbolTable)
    {
        var local = symbolTable.LookupVariable(ident.Name);
        if (local == null)
            throw new InvalidOperationException($"Variable '{ident.Name}' has not been declared");
        
        il.Emit(OpCodes.Ldloc, local);
    }
    
    private void EmitBinaryOperation(ILGenerator il, BinaryNode binary, SymbolTable symbolTable)
    {
        // Always use object to simplify - force dynamic methods
        EmitExpression(il, binary.Left, symbolTable, typeof(object));
        EmitExpression(il, binary.Right, symbolTable, typeof(object));
        
        // Get dynamic method
        var runtimeType = typeof(ChestRuntime);
        MethodInfo? method = binary.Op switch
        {
            "+" => runtimeType.GetMethod("AddDynamic", new[] { typeof(object), typeof(object) }),
            "-" => runtimeType.GetMethod("SubDynamic", new[] { typeof(object), typeof(object) }),
            "*" => runtimeType.GetMethod("MulDynamic", new[] { typeof(object), typeof(object) }),
            "/" => runtimeType.GetMethod("DivDynamic", new[] { typeof(object), typeof(object) }),
            "<" => runtimeType.GetMethod("LtDynamic", new[] { typeof(object), typeof(object) }),
            ">" => runtimeType.GetMethod("GtDynamic", new[] { typeof(object), typeof(object) }),
            "<=" => runtimeType.GetMethod("LeDynamic", new[] { typeof(object), typeof(object) }),
            ">=" => runtimeType.GetMethod("GeDynamic", new[] { typeof(object), typeof(object) }),
            "==" => runtimeType.GetMethod("Eq", new[] { typeof(object), typeof(object) }),
            "!=" => runtimeType.GetMethod("Ne", new[] { typeof(object), typeof(object) }),
            _ => null
        };
        
        Console.WriteLine($"DEBUG EmitBinaryOperation: op={binary.Op}, method={method?.Name}");
        
        if (method != null)
        {
            il.Emit(OpCodes.Call, method);
        }
        else
        {
            throw new InvalidOperationException($"Operation '{binary.Op}' is not supported");
        }
    }
    
    private Type InferExpressionType(ExprNode expr, SymbolTable? symbolTable = null)
    {
        return expr switch
        {
            LiteralNode literal => literal.Type.ToSystemType(),
            IdentNode ident when symbolTable != null => 
                symbolTable.LookupVariableType(ident.Name) ?? typeof(object),
            BinaryNode binary => InferBinaryOperationType(binary.Op, 
                InferExpressionType(binary.Left, symbolTable), 
                InferExpressionType(binary.Right, symbolTable)),
            _ => typeof(object)
        };
    }
    
    private Type InferBinaryOperationType(string op, Type leftType, Type rightType)
    {
        return op switch
        {
            "+" when leftType == typeof(string) || rightType == typeof(string) => typeof(string),
            "+" or "-" or "*" or "/" => typeof(double),
            "<" or ">" or "<=" or ">=" or "==" or "!=" => typeof(bool),
            _ => typeof(object)
        };
    }
    
    private MethodInfo? GetBinaryOperationMethod(string op, Type leftType, Type rightType)
    {
        var runtimeType = typeof(ChestRuntime);
        
        // If at least one is object, use dynamic methods
        if (leftType == typeof(object) || rightType == typeof(object))
        {
            return op switch
            {
                "+" => runtimeType.GetMethod("AddDynamic", new[] { typeof(object), typeof(object) }),
                "-" => runtimeType.GetMethod("SubDynamic", new[] { typeof(object), typeof(object) }),
                "*" => runtimeType.GetMethod("MulDynamic", new[] { typeof(object), typeof(object) }),
                "/" => runtimeType.GetMethod("DivDynamic", new[] { typeof(object), typeof(object) }),
                "<" => runtimeType.GetMethod("LtDynamic", new[] { typeof(object), typeof(object) }),
                ">" => runtimeType.GetMethod("GtDynamic", new[] { typeof(object), typeof(object) }),
                "<=" => runtimeType.GetMethod("LeDynamic", new[] { typeof(object), typeof(object) }),
                ">=" => runtimeType.GetMethod("GeDynamic", new[] { typeof(object), typeof(object) }),
                "==" => runtimeType.GetMethod("Eq", new[] { typeof(object), typeof(object) }),
                "!=" => runtimeType.GetMethod("Ne", new[] { typeof(object), typeof(object) }),
                _ => null
            };
        }
        
        return op switch
        {
            "+" when leftType == typeof(double) && rightType == typeof(double) => 
                runtimeType.GetMethod("Add", new[] { typeof(double), typeof(double) }),
            "+" when leftType == typeof(string) || rightType == typeof(string) => 
                GetConcatMethod(leftType, rightType),
            "-" => runtimeType.GetMethod("Sub", new[] { typeof(double), typeof(double) }),
            "*" => runtimeType.GetMethod("Mul", new[] { typeof(double), typeof(double) }),
            "/" => runtimeType.GetMethod("Div", new[] { typeof(double), typeof(double) }),
            "<" => runtimeType.GetMethod("Lt", new[] { typeof(double), typeof(double) }),
            ">" => runtimeType.GetMethod("Gt", new[] { typeof(double), typeof(double) }),
            "<=" => runtimeType.GetMethod("Le", new[] { typeof(double), typeof(double) }),
            ">=" => runtimeType.GetMethod("Ge", new[] { typeof(double), typeof(double) }),
            "==" => runtimeType.GetMethod("Eq", new[] { typeof(object), typeof(object) }),
            "!=" => runtimeType.GetMethod("Ne", new[] { typeof(object), typeof(object) }),
            _ => null
        };
    }
    
    private MethodInfo? GetConcatMethod(Type leftType, Type rightType)
    {
        var runtimeType = typeof(ChestRuntime);
        
        // Try to find specific method for the types
        return runtimeType.GetMethod("Concat", new[] { leftType, rightType }) ??
               runtimeType.GetMethod("Concat", new[] { typeof(string), typeof(string) });
    }
    
    private (Type entryType, MethodInfo entryMethod) FindEntryPoint(ProgramNode program)
    {
        // Procurar por Demo.Main.Start ou o primeiro employee encontrado
        foreach (var building in program.Buildings)
        {
            foreach (var member in building.Members)
            {
                if (member is OfficeNode office)
                {
                    var typeName = $"{building.Name}.{office.Name}";
                    var finishedType = _finishedTypes[typeName];
                    
                    foreach (var officeMember in office.Members)
                    {
                        if (officeMember is EmployeeNode employee)
                        {
                            var method = finishedType.GetMethod(employee.Name);
                            if (method != null)
                            {
                                return (finishedType, method);
                            }
                        }
                    }
                }
            }
        }

        throw new InvalidOperationException("No entry point found in the program");
    }
}
