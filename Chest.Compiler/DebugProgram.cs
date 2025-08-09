using System;
using Chest.Compiler.Core;
using Chest.Compiler.Lexing;
using Chest.Compiler.Parsing;
using Chest.Compiler.CodeGen;

// Programa de debug para testar geração de IL
class DebugProgram
{
    static void Main()
    {
        try
        {
            Console.WriteLine("=== DEBUG: Teste simples ===");
            
            var code = """
                building Demo
                  office Main
                    employee Start
                      chest x = 42
                      show x
                """;
            
            Console.WriteLine("Código:");
            Console.WriteLine(code);
            
            var lexer = new ChestLexer(code);
            var tokens = lexer.Tokenize();
            Console.WriteLine($"Tokens: {tokens.Count}");
            
            var parser = new ChestParser(tokens);
            var ast = parser.Parse();
            Console.WriteLine("AST gerado com sucesso");
            
            var emitter = new ChestEmitter();
            var assembly = emitter.Emit(ast);
            Console.WriteLine("Assembly gerado com sucesso");
            
            Console.WriteLine("Executando...");
            assembly.Execute();
            Console.WriteLine("Execução concluída");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO: {ex}");
        }
    }
}
