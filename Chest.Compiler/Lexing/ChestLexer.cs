using System;
using System.Collections.Generic;
using System.Text;
using Chest.Compiler.Core;

namespace Chest.Compiler.Lexing;

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
        { "attach", TokenType.Attach },
        { "ask", TokenType.Ask },
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
