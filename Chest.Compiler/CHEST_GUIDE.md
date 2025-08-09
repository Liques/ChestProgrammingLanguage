# CHEST PROGRAMMING LANGUAGE - CONTEXT GUIDE

This file contains all the information needed for an LLM to fully support the Chest language compiler project.

## PROJECT OVERVIEW

**Name**: Chest Programming Language Compiler  
**Goal**: JIT compiler that converts Chest code directly to IL (.NET MSIL/CIL) using Reflection.Emit  
**Location**: `c:\Projs\ChestLanguage\Chest.Compiler\`  
**Framework**: .NET 9.0  
**Language**: C# with Nullable enabled  

## CHEST LANGUAGE PHILOSOPHY

Chest is designed to be **intuitive** using real-world metaphors:

- **building** = namespace/group (like a business building)
- **office** = class/type (like an office in the building)
- **employee** = method/function (an employee who performs tasks)
- **chest** = variable (a chest that stores values)
- **show** = print/display (show chest contents)
- **decide** = if/else (make a decision)
- **attach** = import/using (attach external functionality)
- **ask** = get user input (ask for information)
- **go** = access object/property (go to a location)
- **poke** = call method (poke someone to do something)

## BASIC SYNTAX

### Minimal Structure
```rust
building MyCompany
  office Reception  
    employee Greet
      chest message = "Hello World"
      show message
```

### Supported Types (Phase 1)
- `number` → `System.Double`
- `text` → `System.String` 
- `bool` → `System.Boolean`

### Operators
- Arithmetic: `+`, `-`, `*`, `/`
- Comparison: `<`, `>`, `==`, `!=`, `<=`, `>=`
- Logical: `&&`, `||`, `!` (future implementation)

### Control Flow
```rust
decide condition {
  // then block
} else {
  // else block  
}
```

### User Input and Module Import
```rust
attach Console        // Import functionality (similar to 'using' in C#)
chest name = ask "Enter your name: "    // Get user input with prompt
chest value = ask                       // Get user input without prompt
show "Hello " + name
```

## FORMAL GRAMMAR

```
program        : buildingDecl+ EOF ;
buildingDecl   : 'building' IDENT block ;
block          : '{' stmt* '}' ;
stmt           : attachStmt
               | officeDecl
               | employeeDecl
               | varDecl ';'
               | showStmt ';'
               | decideStmt
               ;
attachStmt     : 'attach' IDENT ';' ;
officeDecl     : 'office' IDENT block ;
employeeDecl   : 'employee' IDENT paramList? block ;
paramList      : '(' (IDENT (',' IDENT)*)? ')' ;
varDecl        : 'chest' IDENT ('=' expr)? ;
showStmt       : 'show' expr ;
decideStmt     : 'decide' expr block ('else' block)? ;
expr           : literal
               | IDENT
               | askExpr
               | expr binop expr
               | '(' expr ')'
               ;
askExpr        : 'ask' STRING? ;
literal        : NUMBER | STRING | 'true' | 'false' ;
binop          : '+' | '-' | '*' | '/' | '<' | '>' | '==' | '!=' | '<=' | '>=' ;
```

## COMPILER ARCHITECTURE

### Compilation Pipeline
1. **Lexer/Parser** → AST (Abstract Syntax Tree)
2. **Semantic Analysis** → Type and scope checking
3. **IL Generation** → Reflection.Emit for MSIL
4. **Runtime** → C# library with native functions
5. **Execution** → CLR JIT

### File Structure (Updated)
```
Chest.Compiler/
├── Core/
│   ├── Types.cs           // Type definitions
│   └── AstNodes.cs        // AST node classes
├── Lexing/
│   ├── Token.cs           // Token definitions
│   └── ChestLexer.cs      // Lexer implementation
├── Parsing/
│   └── ChestParser.cs     // Parser implementation
├── CodeGen/
│   └── ChestEmitter.cs    // IL generator (Reflection.Emit)
├── Runtime/
│   └── ChestRuntime.cs    // Runtime library
├── Tests/
│   └── Tests.cs           // xUnit tests
├── Program.cs             // Entry point and demo
└── CHEST_GUIDE.md         // This guide
```

## AST CLASSES

```csharp
public abstract class Node 
{
    public SourceSpan Span { get; set; }
}

public class ProgramNode : Node 
{
    public List<BuildingNode> Buildings { get; set; } = new();
}

public class BuildingNode : Node 
{
    public string Name { get; set; } = "";
    public List<Node> Members { get; set; } = new();
}

public class OfficeNode : Node 
{
    public string Name { get; set; } = "";
    public List<Node> Members { get; set; } = new();
}

public class EmployeeNode : Node 
{
    public string Name { get; set; } = "";
    public List<Parameter> Parameters { get; set; } = new();
    public List<Node> Body { get; set; } = new();
}

public class VarDeclNode : Node 
{
    public string Name { get; set; } = "";
    public ExprNode? Init { get; set; }
    public TypeRef TypeHint { get; set; }
}

public class ShowNode : Node 
{
    public ExprNode Expr { get; set; } = null!;
}

public class DecideNode : Node 
{
    public ExprNode Cond { get; set; } = null!;
    public List<Node> Then { get; set; } = new();
    public List<Node>? Else { get; set; }
}

public class AttachNode : Node 
{
    public string Module { get; set; } = "";
}

public abstract class ExprNode : Node { }

public class LiteralNode : ExprNode 
{
    public object? Value { get; set; }
    public ChestType Type { get; set; }
}

public class IdentNode : ExprNode 
{
    public string Name { get; set; } = "";
}

public class AskNode : ExprNode 
{
    public ExprNode? Prompt { get; set; }
}

public class BinaryNode : ExprNode 
{
    public string Op { get; set; } = "";
    public ExprNode Left { get; set; } = null!;
    public ExprNode Right { get; set; } = null!;
}

public struct TypeRef 
{
    public string Name { get; set; }
}

public struct SourceSpan 
{
    public int Line { get; set; }
    public int Col { get; set; }
    public int EndLine { get; set; }
    public int EndCol { get; set; }
}

public class Parameter 
{
    public string Name { get; set; } = "";
    public TypeRef TypeHint { get; set; }
}

public enum ChestType 
{
    Number,  // double
    Text,    // string  
    Bool     // boolean
}
```

## CHEST RUNTIME

```csharp
public static class ChestRuntime
{
    public static void Show(object? x)
    {
        Console.WriteLine(x?.ToString() ?? "null");
    }
    
    public static string Ask()
    {
        return Console.ReadLine() ?? "";
    }
    
    public static string Ask(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? "";
    }
    
    public static bool Lt(double a, double b) => a < b;
    public static bool Gt(double a, double b) => a > b;
    public static bool Le(double a, double b) => a <= b;
    public static bool Ge(double a, double b) => a >= b;
    public static bool Eq(object? a, object? b) => Equals(a, b);
    public static bool Ne(object? a, object? b) => !Equals(a, b);
    
    public static double Add(double a, double b) => a + b;
    public static double Sub(double a, double b) => a - b;
    public static double Mul(double a, double b) => a * b;
    public static double Div(double a, double b) => a / b;
    
    public static string Concat(string? a, string? b) => (a ?? "") + (b ?? "");
}
```

## CHEST → IL MAPPING

| Chest Construct | IL Action |
|-----------------|----------|
| `attach Module` | No IL generated (placeholder for future module system) |
| `ask` | `Call ChestRuntime.Ask()` |
| `ask "prompt"` | `Ldstr "prompt"`, `Call ChestRuntime.Ask(string)` |
| `text` literal | `Ldstr` with string |
| `number` literal | `Ldc_R8` (double) |
| `bool` literal | `Ldc_I4_0`/`Ldc_I4_1` |
| `chest x = expr` | `DeclareLocal(T)`, generate expr, `Stloc x` |
| `show expr` | generate expr, box if needed, `Call ChestRuntime.Show(object)` |
| `decide cond { A } else { B }` | generate cond → `Brfalse elseLbl`; generate A; `Br end`; `MarkLabel elseLbl`; generate B; `MarkLabel end` |
| `a + b` (numbers) | generate `a`, generate `b`, `Call ChestRuntime.Add` |
| `a + b` (strings) | generate `a`, generate `b`, `Call ChestRuntime.Concat` |
| `a < b` | generate `a`, `b`, `Call ChestRuntime.Lt` |
| Local identifier | `Ldloc` |

## IL GENERATION WITH REFLECTION.EMIT

### Main Structure
```csharp
public class ChestEmitter
{
    private AssemblyBuilder asmBuilder;
    private ModuleBuilder modBuilder;
    private Dictionary<string, TypeBuilder> officeTypes = new();
    
    public ChestAssembly Emit(ProgramNode program)
    {
        // Create dynamic assembly
        var asmName = new AssemblyName("ChestOut");
        asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
        modBuilder = asmBuilder.DefineDynamicModule("ChestOutModule");
        
        // Process buildings/offices/employees
        foreach (var building in program.Buildings)
        {
            EmitBuilding(building);
        }
        
        // Create types and return assembly
        return FinalizeAssembly();
    }
}
```

### Entry Point Generation
The compiler should automatically generate a `Main` method that calls the first public employee found:

```csharp
public static void Main(string[] args)
{
    Demo.Main.Start(); // or the first employee found
}
```

## SCOPE MANAGEMENT

```csharp
public class SymbolTable
{
    private Stack<Dictionary<string, LocalBuilder>> scopes = new();
    
    public void PushScope() => scopes.Push(new Dictionary<string, LocalBuilder>());
    public void PopScope() => scopes.Pop();
    
    public void DeclareVariable(string name, LocalBuilder local)
    {
        scopes.Peek()[name] = local;
    }
    
    public LocalBuilder? LookupVariable(string name)
    {
        foreach (var scope in scopes)
        {
            if (scope.TryGetValue(name, out var local))
                return local;
        }
        return null;
    }
}
```

## ERROR HANDLING

### During Parsing
- Report line/column of error using `SourceSpan`
- User-friendly messages in English
- Correction suggestions when possible

### During Emission
- Validate types before emitting IL
- Check if variables are declared
- Check operator compatibility

### During Execution
- Capture exceptions and map to source code
- Readable stack traces

## AUTOMATIC BOXING

For calls expecting `object` (like `ChestRuntime.Show`):
```csharp
// If expr is value type (double, bool)
EmitExpression(il, expr, scope);
if (exprType.IsValueType)
{
    il.Emit(OpCodes.Box, exprType);
}
il.Emit(OpCodes.Call, showMethodInfo);
```

## REQUIRED TESTS

### Basic Tests (xUnit)
1. **HelloWorld**: `show "Hello"` → output: "Hello"
2. **Numbers**: `chest x = 42; show x` → output: "42"  
3. **Arithmetic**: `show 40 + 2` → output: "42"
4. **IfTrue**: `decide 1 < 2 { show "ok" } else { show "nope" }` → output: "ok"
5. **IfFalse**: `decide 2 < 1 { show "ok" } else { show "nope" }` → output: "nope"
6. **StringConcat**: `show "Hello" + " World"` → output: "Hello World"
7. **UserInput**: `chest name = ask "Name: "; show "Hello " + name` → input: "Alice" → output: "Hello Alice"
8. **SimpleAsk**: `chest value = ask; show value` → input: "test" → output: "test"

### Test Structure
```csharp
[Test]
public void TestHelloWorld()
{
    var code = """
        building Demo
          office Main
            employee Start
              show "Hello"
        """;
    
    var output = CompileAndRun(code);
    Assert.AreEqual("Hello", output.Trim());
}
```

## REQUIRED DEPENDENCIES

Add to `Chest.Compiler.csproj`:
```xml
<PackageReference Include="xunit" Version="2.4.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
```

## CHEST CODE EXAMPLES

### Hello World
```rust
building Demo
  office Main
    employee Start
      show "Hello, Chest!"
```

### User Input and Interaction
```rust
building Interactive
  office Main
    employee Start
      attach Console
      chest name = ask "Enter your name: "
      show "Hello " + name + "!"
      
      chest age = ask "Enter your age: "
      show "You are " + age + " years old"
```

### Variables and Operations
```rust
building Calc
  office Math
    employee Calculate
      chest a = 10
      chest b = 20
      chest result = a + b
      show result
```

### Conditionals
```rust
building Logic
  office Decision
    employee Check
      chest age = 18
      decide age >= 18 {
        show "Adult"
      } else {
        show "Minor"
      }
```

### Complex Example
```rust
building InteractiveApp
  office UserInterface
    employee RunApp
      attach Console
      
      chest userName = ask "Welcome! Enter your name: "
      show "Hello " + userName + "!"
      
      chest operation = ask "Enter operation (add/multiply): "
      chest num1 = ask "Enter first number: "
      chest num2 = ask "Enter second number: "
      
      show "You chose: " + operation
      show "Numbers: " + num1 + " and " + num2
      
      decide operation == "add"
        show "Note: This would add the numbers if we had conversion"
      else
        decide operation == "multiply"
          show "Note: This would multiply if we had conversion"
        else
          show "Unknown operation"
      
      show "String concatenation: " + num1 + num2
      show "Thank you " + userName + "!"
```

## INSTRUCTIONS FOR LLM

### When Implementing Parser
- Use simple recursive descent parsing
- Do not use external libraries (ANTLR, etc.) in the first version
- Treat indentation as block delimiter (Python-style)
- Support comments with `//`

### When Implementing Emitter
- Always use corresponding .NET types (double, string, bool)
- Implement automatic boxing for calls with object
- Generate static methods for employees
- Create automatic entry point

### When Writing Tests
- Test each language construct in isolation
- Capture stdout to validate output
- Use descriptive test names in English
- Include error tests (invalid parsing, etc.)

### Type Handling
- `number` → always `System.Double` 
- `text` → always `System.String`
- `bool` → always `System.Boolean`
- Automatic conversions when needed

### C# Code Conventions
- Use nullable enabled (`<Nullable>enable</Nullable>`)
- Prefer records for immutable structures
- Use pattern matching when appropriate
- Small, single-responsibility methods
- English names for code and comments

## FUTURE ROADMAP

### Phase 2 - Advanced Features
- Employee parameters
- Calls between employees  
- Custom types (offices as classes)
- Arrays/lists
- Loops (repeat/while)

### Phase 3 - Optimizations
- Data flow analysis
- IL optimizations
- Debugging support
- Compilation cache

### Phase 4 - Tooling
- Language Server Protocol (LSP)
- Syntax highlighting
- IntelliSense
- Visual debugger

## USEFUL COMMANDS

```pwsh
# Build the project
dotnet build

# Run tests
dotnet test

# Run compiler
dotnet run

# Add dependencies
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
```

## DEBUGGING TIPS

### For IL Debugging
1. Use `ildasm.exe` to view generated IL
2. Save assembly to disk for analysis: `asmBuilder.Save("output.dll")`
3. Compare with IL from equivalent C# code

### For Runtime Debugging
1. Capture exceptions and show stack trace
2. Log each emitted IL operation
3. Use breakpoints in `ChestRuntime` methods

---

**IMPORTANT**: This guide should be kept up to date as the project evolves. Always include practical examples and keep documentation synchronized with the code.
