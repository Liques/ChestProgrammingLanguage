using System;
using System.Collections.Generic;

namespace Chest.Compiler;

/// <summary>
/// Base AST node with source code position information
/// </summary>
public abstract class Node 
{
    public SourceSpan Span { get; set; }
}

/// <summary>
/// Program root node - contains all buildings
/// </summary>
public class ProgramNode : Node 
{
    public List<BuildingNode> Buildings { get; set; } = new();
}

/// <summary>
/// Building - agrupador/namespace da linguagem Chest
/// </summary>
public class BuildingNode : Node 
{
    public string Name { get; set; } = "";
    public List<Node> Members { get; set; } = new();
}

/// <summary>
/// Office - equivalente a uma classe/tipo
/// </summary>
public class OfficeNode : Node 
{
    public string Name { get; set; } = "";
    public List<Node> Members { get; set; } = new();
}

/// <summary>
/// Employee - equivalent to a method/function
/// </summary>
public class EmployeeNode : Node 
{
    public string Name { get; set; } = "";
    public List<Parameter> Parameters { get; set; } = new();
    public List<Node> Body { get; set; } = new();
}

/// <summary>
/// Variable declaration (chest)
/// </summary>
public class VarDeclNode : Node 
{
    public string Name { get; set; } = "";
    public ExprNode? Init { get; set; }
    public TypeRef TypeHint { get; set; }
}

/// <summary>
/// Comando show - imprime valor no console
/// </summary>
public class ShowNode : Node 
{
    public ExprNode Expr { get; set; } = null!;
}

/// <summary>
/// Comando decide - estrutura condicional if/else
/// </summary>
public class DecideNode : Node 
{
    public ExprNode Cond { get; set; } = null!;
    public List<Node> Then { get; set; } = new();
    public List<Node>? Else { get; set; }
}

/// <summary>
/// Base node for expressions
/// </summary>
public abstract class ExprNode : Node { }

/// <summary>
/// Literal - constant value (string, number, bool)
/// </summary>
public class LiteralNode : ExprNode 
{
    public object? Value { get; set; }
    public ChestType Type { get; set; }
}

/// <summary>
/// Identificador - referência a variável
/// </summary>
public class IdentNode : ExprNode 
{
    public string Name { get; set; } = "";
}

/// <summary>
/// Operação binária (+ - * / < > == != <= >=)
/// </summary>
public class BinaryNode : ExprNode 
{
    public string Op { get; set; } = "";
    public ExprNode Left { get; set; } = null!;
    public ExprNode Right { get; set; } = null!;
}

/// <summary>
/// Chamada de método go/poke (futuro)
/// </summary>
public class CallNode : ExprNode 
{
    public ExprNode Target { get; set; } = null!;
    public string MethodName { get; set; } = "";
    public List<ExprNode> Arguments { get; set; } = new();
}

/// <summary>
/// Referência de tipo
/// </summary>
public struct TypeRef 
{
    public string Name { get; set; }
    
    public TypeRef(string name)
    {
        Name = name;
    }
    
    public static implicit operator TypeRef(string name) => new(name);
}

/// <summary>
/// Posição no código fonte para mensagens de erro
/// </summary>
public struct SourceSpan 
{
    public int Line { get; set; }
    public int Col { get; set; }
    public int EndLine { get; set; }
    public int EndCol { get; set; }
    
    public SourceSpan(int line, int col, int endLine, int endCol)
    {
        Line = line;
        Col = col;
        EndLine = endLine;
        EndCol = endCol;
    }
    
    public override string ToString() => $"({Line},{Col})-({EndLine},{EndCol})";
}

/// <summary>
/// Parâmetro de método
/// </summary>
public class Parameter 
{
    public string Name { get; set; } = "";
    public TypeRef TypeHint { get; set; }
}

/// <summary>
/// Tipos primitivos da linguagem Chest
/// </summary>
public enum ChestType 
{
    Number,  // double
    Text,    // string  
    Bool     // boolean
}

/// <summary>
/// Extensões para trabalhar com tipos
/// </summary>
public static class ChestTypeExtensions
{
    public static Type ToSystemType(this ChestType chestType) => chestType switch
    {
        ChestType.Number => typeof(double),
        ChestType.Text => typeof(string),
        ChestType.Bool => typeof(bool),
        _ => throw new ArgumentException($"Tipo Chest desconhecido: {chestType}")
    };
    
    public static ChestType FromSystemType(Type systemType)
    {
        if (systemType == typeof(double) || systemType == typeof(float) || 
            systemType == typeof(int) || systemType == typeof(long))
            return ChestType.Number;
        if (systemType == typeof(string))
            return ChestType.Text;
        if (systemType == typeof(bool))
            return ChestType.Bool;
        
        throw new ArgumentException($"Tipo do sistema não suportado: {systemType}");
    }
}
