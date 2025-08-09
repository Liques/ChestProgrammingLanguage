using Chest.Compiler.Core;

namespace Chest.Compiler.Lexing;

/// <summary>
/// Token of the Chest language
/// </summary>
public record Token(TokenType Type, string Value, SourceSpan Span);

/// <summary>
/// Token types of the Chest language
/// </summary>
public enum TokenType
{
    // Literals
    Number,
    String,
    Bool,
    
    // Identifiers and keywords
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
    Attach,
    Ask,
    
    // Operators
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
    
    // Delimiters
    LeftBrace,
    RightBrace,
    LeftParen,
    RightParen,
    Comma,
    Semicolon,
    
    // Specials
    Newline,
    Indent,
    Dedent,
    EOF,
    
    // Error
    Error
}