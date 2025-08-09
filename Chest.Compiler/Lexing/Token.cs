using Chest.Compiler.Core;

namespace Chest.Compiler.Lexing;

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
