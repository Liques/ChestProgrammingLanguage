using System;
using System.Collections.Generic;
using Chest.Compiler.Core;
using Chest.Compiler.Lexing;

namespace Chest.Compiler.Parsing;

/// <summary>
/// Parser recursivo descendente para a linguagem Chest
/// </summary>
public class ChestParser
{
    private readonly List<Token> _tokens;
    private int _current = 0;
    
    public ChestParser(List<Token> tokens)
    {
        _tokens = tokens;
    }
    
    public ProgramNode Parse()
    {
        var program = new ProgramNode { Buildings = new List<BuildingNode>() };
        
        while (!IsAtEnd())
        {
            if (Check(TokenType.Building))
            {
                program.Buildings.Add(ParseBuilding());
            }
            else if (Check(TokenType.Newline) || Check(TokenType.Indent) || Check(TokenType.Dedent))
            {
                Advance(); // Skip formatting tokens at top level
            }
            else
            {
                throw new ParseException($"Expected 'building', found: {Peek().Type}", Peek().Span);
            }
        }
        
        return program;
    }
    
    private BuildingNode ParseBuilding()
    {
        var building = new BuildingNode();
        
        Consume(TokenType.Building, "Expected 'building'");
        building.Name = Consume(TokenType.Identifier, "Expected building name").Value;
        
        ConsumeBlock();
        building.Members = ParseBuildingMembers();
        
        return building;
    }
    
    private List<Node> ParseBuildingMembers()
    {
        var members = new List<Node>();
        
        while (!IsAtEnd() && !Check(TokenType.Dedent))
        {
            if (Check(TokenType.Office))
            {
                members.Add(ParseOffice());
            }
            else if (Check(TokenType.Newline) || Check(TokenType.Indent))
            {
                Advance(); // Skip formatting tokens
            }
            else
            {
                throw new ParseException($"Expected 'office', found: {Peek().Type}", Peek().Span);
            }
        }
        
        if (Check(TokenType.Dedent))
            Advance();
        
        return members;
    }
    
    private OfficeNode ParseOffice()
    {
        var office = new OfficeNode();
        
        Consume(TokenType.Office, "Expected 'office'");
        office.Name = Consume(TokenType.Identifier, "Expected office name").Value;
        
        ConsumeBlock();
        office.Members = ParseOfficeMembers();
        
        return office;
    }
    
    private List<Node> ParseOfficeMembers()
    {
        var members = new List<Node>();
        
        while (!IsAtEnd() && !Check(TokenType.Dedent))
        {
            if (Check(TokenType.Employee))
            {
                members.Add(ParseEmployee());
            }
            else if (Check(TokenType.Newline) || Check(TokenType.Indent))
            {
                Advance(); // Skip formatting tokens
            }
            else
            {
                throw new ParseException($"Expected 'employee', found: {Peek().Type}", Peek().Span);
            }
        }
        
        if (Check(TokenType.Dedent))
            Advance();
        
        return members;
    }
    
    private EmployeeNode ParseEmployee()
    {
        var employee = new EmployeeNode();
        
        Consume(TokenType.Employee, "Expected 'employee'");
        employee.Name = Consume(TokenType.Identifier, "Expected employee name").Value;
        
        ConsumeBlock();
        employee.Body = ParseStatements();
        
        return employee;
    }
    
    private List<Node> ParseStatements()
    {
        var statements = new List<Node>();
        
        while (!IsAtEnd() && !Check(TokenType.Dedent) && !Check(TokenType.Else))
        {
            if (Check(TokenType.Newline) || Check(TokenType.Indent))
            {
                Advance();
                continue;
            }
            
            statements.Add(ParseStatement());
        }
        
        if (Check(TokenType.Dedent))
            Advance();
        
        return statements;
    }
    
    private Node ParseStatement()
    {
        return Peek().Type switch
        {
            TokenType.Attach => ParseAttach(),
            TokenType.Chest => ParseVarDecl(),
            TokenType.Show => ParseShow(),
            TokenType.Decide => ParseDecide(),
            _ => throw new ParseException($"Unexpected statement: {Peek().Type}", Peek().Span)
        };
    }
    
    private VarDeclNode ParseVarDecl()
    {
        var varDecl = new VarDeclNode();
        
        Consume(TokenType.Chest, "Expected 'chest'");
        varDecl.Name = Consume(TokenType.Identifier, "Expected variable name").Value;
        
        if (Match(TokenType.Assign))
        {
            varDecl.Init = ParseExpression();
        }
        
        ConsumeStatementEnd();
        return varDecl;
    }
    
    private AttachNode ParseAttach()
    {
        var attach = new AttachNode();
        
        Consume(TokenType.Attach, "Expected 'attach'");
        attach.Module = Consume(TokenType.Identifier, "Expected module name").Value;
        
        ConsumeStatementEnd();
        return attach;
    }
    
    private ShowNode ParseShow()
    {
        var show = new ShowNode();
        
        Consume(TokenType.Show, "Expected 'show'");
        show.Expr = ParseExpression();
        
        ConsumeStatementEnd();
        return show;
    }
    
    private DecideNode ParseDecide()
    {
        var decide = new DecideNode();
        
        Consume(TokenType.Decide, "Expected 'decide'");
        decide.Cond = ParseExpression();
        
        ConsumeBlock();
        decide.Then = ParseStatements();
        
        if (Check(TokenType.Else))
        {
            Advance();
            ConsumeBlock();
            decide.Else = ParseStatements();
        }
        
        return decide;
    }
    
    private ExprNode ParseExpression()
    {
        return ParseEquality();
    }
    
    private ExprNode ParseEquality()
    {
        var expr = ParseComparison();
        
        while (Match(TokenType.Equal, TokenType.NotEqual))
        {
            var op = Previous().Value;
            var right = ParseComparison();
            expr = new BinaryNode { Op = op, Left = expr, Right = right };
        }
        
        return expr;
    }
    
    private ExprNode ParseComparison()
    {
        var expr = ParseTerm();
        
        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            var op = Previous().Value;
            var right = ParseTerm();
            expr = new BinaryNode { Op = op, Left = expr, Right = right };
        }
        
        return expr;
    }
    
    private ExprNode ParseTerm()
    {
        var expr = ParseFactor();
        
        while (Match(TokenType.Minus, TokenType.Plus))
        {
            var op = Previous().Value;
            var right = ParseFactor();
            expr = new BinaryNode { Op = op, Left = expr, Right = right };
        }
        
        return expr;
    }
    
    private ExprNode ParseFactor()
    {
        var expr = ParseUnary();
        
        while (Match(TokenType.Divide, TokenType.Multiply))
        {
            var op = Previous().Value;
            var right = ParseUnary();
            expr = new BinaryNode { Op = op, Left = expr, Right = right };
        }
        
        return expr;
    }
    
    private ExprNode ParseUnary()
    {
        return ParsePrimary();
    }
    
    private ExprNode ParsePrimary()
    {
        if (Match(TokenType.Ask))
        {
            var ask = new AskNode();
            
            // Check if there's a prompt (optional)
            if (Match(TokenType.String))
            {
                ask.Prompt = new LiteralNode { Value = Previous().Value, Type = ChestType.Text };
            }
            
            return ask;
        }
        
        if (Match(TokenType.Number))
        {
            var value = double.Parse(Previous().Value);
            return new LiteralNode { Value = value, Type = ChestType.Number };
        }
        
        if (Match(TokenType.String))
        {
            return new LiteralNode { Value = Previous().Value, Type = ChestType.Text };
        }
        
        if (Match(TokenType.Bool))
        {
            var value = Previous().Value.ToLower() == "true" || Previous().Value.ToLower() == "verdadeiro";
            return new LiteralNode { Value = value, Type = ChestType.Bool };
        }
        
        if (Match(TokenType.Identifier))
        {
            return new IdentNode { Name = Previous().Value };
        }
        
        if (Match(TokenType.LeftParen))
        {
            var expr = ParseExpression();
            Consume(TokenType.RightParen, "Expected ')' after expression");
            return expr;
        }
        
        throw new ParseException($"Unexpected token: {Peek().Type}", Peek().Span);
    }
    
    private void ConsumeBlock()
    {
        if (Match(TokenType.LeftBrace))
        {
            // Syntax with braces
            return;
        }
        
        // Syntax with indentation - just consume newline and indent tokens
        while (Match(TokenType.Newline)) { } // Skip newlines
        
        if (!Check(TokenType.Indent))
        {
            throw new ParseException("Expected indentation", Peek().Span);
        }
        
        Advance(); // Consume the indent
    }
    
    private void ConsumeStatementEnd()
    {
        if (Match(TokenType.Semicolon) || Match(TokenType.Newline))
            return;
        
        // For statements that don't end with semicolon or newline, that's ok too
        // This handles cases where we're at the end of a block or similar
    }
    
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }
    
    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }
    
    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }
    
    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }
    
    private Token Peek()
    {
        return _tokens[_current];
    }
    
    private Token Previous()
    {
        return _tokens[_current - 1];
    }
    
    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
            return Advance();
        
        throw new ParseException(message, Peek().Span);
    }
}

/// <summary>
/// Parsing error exception
/// </summary>
public class ParseException : Exception
{
    public SourceSpan Span { get; }
    
    public ParseException(string message, SourceSpan span) : base(message)
    {
        Span = span;
    }
}
