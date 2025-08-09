using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Chest.Compiler;

/// <summary>
/// Token da linguagem Chest
/// </summary>
public record Token(TokenType Type, string Value, SourceSpan Span);

/// <summary>
/// Tipos de token da linguagem Chest
/// </summary>
public enum TokenType
{
    // Literais
    Number,
    String,
    Bool,
    
    // Identificadores e palavras-chave
    Identifier,
    Building,
    Office,
    Employee,
    Chest,
    Show,
    Decide,
    Else,
    Go,
    Poke,
    
    // Operadores
    Plus,
    Minus,
    Multiply,
    Divide,
    Less,
    Greater,
    LessEqual,
    GreaterEqual,
    Equal,
    NotEqual,
    Assign,
    
    // Delimitadores
    LeftBrace,
    RightBrace,
    LeftParen,
    RightParen,
    Comma,
    Semicolon,
    
    // Especiais
    Newline,
    Indent,
    Dedent,
    EOF,
    
    // Erro
    Error
}

/// <summary>
/// Lexer (lexical analyzer) for the Chest language
/// </summary>
public class ChestLexer
{
    private readonly string _source;
    private int _position;
    private int _line = 1;
    private int _column = 1;
    private readonly Stack<int> _indentStack = new();
    
    private readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "building", TokenType.Building },
        { "office", TokenType.Office },
        { "employee", TokenType.Employee },
        { "chest", TokenType.Chest },
        { "show", TokenType.Show },
        { "decide", TokenType.Decide },
        { "else", TokenType.Else },
        { "go", TokenType.Go },
        { "poke", TokenType.Poke },
        { "true", TokenType.Bool },
        { "false", TokenType.Bool },
        { "verdadeiro", TokenType.Bool },
        { "falso", TokenType.Bool }
    };
    
    public ChestLexer(string source)
    {
        _source = source;
        _indentStack.Push(0); // Base indentation level
    }
    
    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        
        while (!IsAtEnd())
        {
            var token = NextToken();
            if (token.Type != TokenType.Error)
            {
                tokens.Add(token);
            }
        }
        
        // Adicionar DEDENTs para fechar blocos pendentes
        while (_indentStack.Count > 1)
        {
            _indentStack.Pop();
            tokens.Add(CreateToken(TokenType.Dedent, ""));
        }
        
        tokens.Add(CreateToken(TokenType.EOF, ""));
        return tokens;
    }
    
    private Token NextToken()
    {
        SkipWhitespace();
        
        if (IsAtEnd())
            return CreateToken(TokenType.EOF, "");
        
        var start = _position;
        var startLine = _line;
        var startColumn = _column;
        
        var ch = Advance();
        
        return ch switch
        {
            '\n' => HandleNewline(),
            '"' => ScanString(),
            '{' => CreateToken(TokenType.LeftBrace, "{"),
            '}' => CreateToken(TokenType.RightBrace, "}"),
            '(' => CreateToken(TokenType.LeftParen, "("),
            ')' => CreateToken(TokenType.RightParen, ")"),
            ',' => CreateToken(TokenType.Comma, ","),
            ';' => CreateToken(TokenType.Semicolon, ";"),
            '+' => CreateToken(TokenType.Plus, "+"),
            '-' => CreateToken(TokenType.Minus, "-"),
            '*' => CreateToken(TokenType.Multiply, "*"),
            '/' when Peek() == '/' => SkipLineComment(),
            '/' => CreateToken(TokenType.Divide, "/"),
            '<' when Peek() == '=' => AdvanceAndCreateToken(TokenType.LessEqual, "<="),
            '<' => CreateToken(TokenType.Less, "<"),
            '>' when Peek() == '=' => AdvanceAndCreateToken(TokenType.GreaterEqual, ">="),
            '>' => CreateToken(TokenType.Greater, ">"),
            '=' when Peek() == '=' => AdvanceAndCreateToken(TokenType.Equal, "=="),
            '=' => CreateToken(TokenType.Assign, "="),
            '!' when Peek() == '=' => AdvanceAndCreateToken(TokenType.NotEqual, "!="),
            _ when char.IsDigit(ch) => ScanNumber(),
            _ when char.IsLetter(ch) || ch == '_' => ScanIdentifier(),
            _ => CreateToken(TokenType.Error, $"Unexpected character: '{ch}'")
        };
    }
    
    private Token HandleNewline()
    {
        _line++;
        _column = 1;
        
        // Check indentation on next non-empty line
        var nextIndent = MeasureIndentation();
        if (nextIndent == -1) // Empty line or EOF
            return CreateToken(TokenType.Newline, "\n");
        
        var currentIndent = _indentStack.Peek();
        
        if (nextIndent > currentIndent)
        {
            _indentStack.Push(nextIndent);
            return CreateToken(TokenType.Indent, new string(' ', nextIndent));
        }
        else if (nextIndent < currentIndent)
        {
            // There can be multiple DEDENTs
            while (_indentStack.Count > 1 && _indentStack.Peek() > nextIndent)
            {
                _indentStack.Pop();
            }
            
            if (_indentStack.Peek() != nextIndent)
            {
                return CreateToken(TokenType.Error, "Indentação inconsistente");
            }
            
            return CreateToken(TokenType.Dedent, "");
        }
        
        return CreateToken(TokenType.Newline, "\n");
    }
    
    private int MeasureIndentation()
    {
        var savedPos = _position;
        var savedLine = _line;
        var savedCol = _column;
        
        var indent = 0;
        
        while (!IsAtEnd())
        {
            var ch = Peek();
            if (ch == ' ')
            {
                indent++;
                Advance();
            }
            else if (ch == '\t')
            {
                indent += 4; // Tab = 4 spaces
                Advance();
            }
            else if (ch == '\n' || ch == '\r')
            {
                // Empty line, continue
                if (ch == '\r' && Peek(1) == '\n')
                    Advance();
                Advance();
                _line++;
                _column = 1;
                indent = 0;
            }
            else if (ch == '/' && Peek(1) == '/')
            {
                // Comment, skip line
                while (!IsAtEnd() && Peek() != '\n')
                    Advance();
                if (!IsAtEnd())
                {
                    Advance();
                    _line++;
                    _column = 1;
                    indent = 0;
                }
            }
            else
            {
                // Found content
                break;
            }
        }
        
        if (IsAtEnd())
        {
            // Restore position and return -1 for EOF
            _position = savedPos;
            _line = savedLine;
            _column = savedCol;
            return -1;
        }
        
        // Don't advance position, just measure
        _position = savedPos;
        _line = savedLine;
        _column = savedCol;
        
        return indent;
    }
    
    private Token ScanString()
    {
        var value = new StringBuilder();
        
        while (!IsAtEnd() && Peek() != '"')
        {
            if (Peek() == '\\')
            {
                Advance(); // Consumir '\'
                if (!IsAtEnd())
                {
                    var escaped = Advance();
                    value.Append(escaped switch
                    {
                        'n' => '\n',
                        't' => '\t',
                        'r' => '\r',
                        '\\' => '\\',
                        '"' => '"',
                        _ => escaped
                    });
                }
            }
            else
            {
                value.Append(Advance());
            }
        }
        
        if (IsAtEnd())
            return CreateToken(TokenType.Error, "String não terminada");
        
        Advance(); // Consumir '"' final
        return CreateToken(TokenType.String, value.ToString());
    }
    
    private Token ScanNumber()
    {
        var value = new StringBuilder();
        value.Append(Previous());
        
        while (!IsAtEnd() && char.IsDigit(Peek()))
        {
            value.Append(Advance());
        }
        
        // Verificar parte decimal
        if (!IsAtEnd() && Peek() == '.' && char.IsDigit(Peek(1)))
        {
            value.Append(Advance()); // Consumir '.'
            while (!IsAtEnd() && char.IsDigit(Peek()))
            {
                value.Append(Advance());
            }
        }
        
        return CreateToken(TokenType.Number, value.ToString());
    }
    
    private Token ScanIdentifier()
    {
        var value = new StringBuilder();
        value.Append(Previous());
        
        while (!IsAtEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
        {
            value.Append(Advance());
        }
        
        var text = value.ToString();
        var tokenType = _keywords.TryGetValue(text.ToLower(), out var keyword) ? keyword : TokenType.Identifier;
        
        return CreateToken(tokenType, text);
    }
    
    private Token SkipLineComment()
    {
        while (!IsAtEnd() && Peek() != '\n')
            Advance();
        
        return NextToken(); // Continue to next token
    }
    
    private void SkipWhitespace()
    {
        while (!IsAtEnd())
        {
            var ch = Peek();
            if (ch == ' ' || ch == '\r' || ch == '\t')
                Advance();
            else
                break;
        }
    }
    
    private Token CreateToken(TokenType type, string value)
    {
        var span = new SourceSpan(_line, _column - value.Length, _line, _column);
        return new Token(type, value, span);
    }
    
    private Token AdvanceAndCreateToken(TokenType type, string value)
    {
        Advance(); // Consumir segundo caractere
        return CreateToken(type, value);
    }
    
    private char Advance()
    {
        if (IsAtEnd()) return '\0';
        _column++;
        return _source[_position++];
    }
    
    private char Peek(int offset = 0)
    {
        var pos = _position + offset;
        return pos >= _source.Length ? '\0' : _source[pos];
    }
    
    private char Previous()
    {
        return _position > 0 ? _source[_position - 1] : '\0';
    }
    
    private bool IsAtEnd()
    {
        return _position >= _source.Length;
    }
}

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
        
        while (!Check(TokenType.Dedent) && !IsAtEnd())
        {
            if (Check(TokenType.Office))
            {
                members.Add(ParseOffice());
            }
            else if (Check(TokenType.Newline))
            {
                Advance();
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
        
        while (!Check(TokenType.Dedent) && !IsAtEnd())
        {
            if (Check(TokenType.Employee))
            {
                members.Add(ParseEmployee());
            }
            else if (Check(TokenType.Newline))
            {
                Advance();
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
        
        while (!Check(TokenType.Dedent) && !Check(TokenType.Else) && !IsAtEnd())
        {
            if (Check(TokenType.Newline))
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
            TokenType.Chest => ParseVarDeclaration(),
            TokenType.Show => ParseShowStatement(),
            TokenType.Decide => ParseDecideStatement(),
            _ => throw new ParseException($"Unexpected statement: {Peek().Type}", Peek().Span)
        };
    }
    
    private VarDeclNode ParseVarDeclaration()
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
    
    private ShowNode ParseShowStatement()
    {
        var show = new ShowNode();
        
        Consume(TokenType.Show, "Expected 'show'");
        show.Expr = ParseExpression();
        
        ConsumeStatementEnd();
        return show;
    }
    
    private DecideNode ParseDecideStatement()
    {
        var decide = new DecideNode();
        
        Consume(TokenType.Decide, "Expected 'decide'");
        decide.Cond = ParseExpression();
        
        ConsumeBlock();
        decide.Then = ParseStatements();
        
        if (Match(TokenType.Else))
        {
            ConsumeBlock();
            decide.Else = ParseStatements();
        }
        
        return decide;
    }
    
    private ExprNode ParseExpression()
    {
        return ParseComparison();
    }
    
    private ExprNode ParseComparison()
    {
        var expr = ParseAddition();
        
        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, 
                     TokenType.LessEqual, TokenType.Equal, TokenType.NotEqual))
        {
            var op = Previous().Value;
            var right = ParseAddition();
            expr = new BinaryNode { Op = op, Left = expr, Right = right };
        }
        
        return expr;
    }
    
    private ExprNode ParseAddition()
    {
        var expr = ParseMultiplication();
        
        while (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Previous().Value;
            var right = ParseMultiplication();
            expr = new BinaryNode { Op = op, Left = expr, Right = right };
        }
        
        return expr;
    }
    
    private ExprNode ParseMultiplication()
    {
        var expr = ParsePrimary();
        
        while (Match(TokenType.Multiply, TokenType.Divide))
        {
            var op = Previous().Value;
            var right = ParsePrimary();
            expr = new BinaryNode { Op = op, Left = expr, Right = right };
        }
        
        return expr;
    }
    
    private ExprNode ParsePrimary()
    {
        if (Match(TokenType.Number))
        {
            var value = double.Parse(Previous().Value, CultureInfo.InvariantCulture);
            return new LiteralNode { Value = value, Type = ChestType.Number };
        }
        
        if (Match(TokenType.String))
        {
            return new LiteralNode { Value = Previous().Value, Type = ChestType.Text };
        }
        
        if (Match(TokenType.Bool))
        {
            var value = Previous().Value.ToLower() is "true" or "verdadeiro";
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
        
        throw new ParseException($"Expressão inesperada: {Peek().Type}", Peek().Span);
    }
    
    private void ConsumeBlock()
    {
        if (Match(TokenType.LeftBrace))
        {
            // Syntax with braces
            return;
        }
        
        // Syntax with indentation
        if (Check(TokenType.Newline))
            Advance();
        
        Consume(TokenType.Indent, "Expected indentation or '{'");
    }
    
    private void ConsumeStatementEnd()
    {
        if (Match(TokenType.Semicolon))
            return;
        
        if (Check(TokenType.Newline) || Check(TokenType.Dedent) || IsAtEnd())
            return;
        
        throw new ParseException("Expected ';' or newline", Peek().Span);
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
        return !IsAtEnd() && Peek().Type == type;
    }
    
    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }
    
    private Token Peek()
    {
        return _tokens[_current];
    }
    
    private Token Previous()
    {
        return _tokens[_current - 1];
    }
    
    private bool IsAtEnd()
    {
        return _current >= _tokens.Count || Peek().Type == TokenType.EOF;
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
