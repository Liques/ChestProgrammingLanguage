using System;
using System.Collections.Generic;

namespace Chest.Compiler.Core;

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
/// Show statement
/// </summary>
public class ShowNode : Node 
{
    public ExprNode Expr { get; set; } = null!;
}

/// <summary>
/// Decide statement (if/else)
/// </summary>
public class DecideNode : Node 
{
    public ExprNode Cond { get; set; } = null!;
    public List<Node> Then { get; set; } = new();
    public List<Node>? Else { get; set; }
}

/// <summary>
/// Attach statement (import/using functionality)
/// </summary>
public class AttachNode : Node 
{
    public string Module { get; set; } = "";
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
/// Identifier - variable reference
/// </summary>
public class IdentNode : ExprNode 
{
    public string Name { get; set; } = "";
}

/// <summary>
/// Ask expression - get user input (equivalent to Console.ReadLine())
/// </summary>
public class AskNode : ExprNode 
{
    public ExprNode? Prompt { get; set; }
}

/// <summary>
/// Binary operation (+ - * / < > == != <= >=)
/// </summary>
public class BinaryNode : ExprNode 
{
    public string Op { get; set; } = "";
    public ExprNode Left { get; set; } = null!;
    public ExprNode Right { get; set; } = null!;
}

/// <summary>
/// Method call go/poke (future)
/// </summary>
public class CallNode : ExprNode 
{
    public string Target { get; set; } = "";
    public string Method { get; set; } = "";
    public List<ExprNode> Args { get; set; } = new();
}
