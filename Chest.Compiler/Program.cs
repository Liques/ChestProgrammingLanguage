using System;
using Chest.Compiler;

namespace Chest.Compiler;

/// <summary>
/// Entry point do compilador Chest
/// Demonstra como usar o compilador para executar código Chest
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Compilador Chest v1.0 ===");
        Console.WriteLine("Compilando código Chest para IL (.NET)...\n");
        
        try
        {
            // Executar exemplos demonstrativos
            RunHelloWorldExample();
            RunArithmeticExample();
            RunConditionalExample();
            RunBusinessExample();
            
            Console.WriteLine("\n=== Todos os exemplos executados com sucesso! ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO: {ex.Message}");
            if (ex is ParseException parseEx)
            {
                Console.WriteLine($"Posição: {parseEx.Span}");
            }
        }
    }
    
    static void RunHelloWorldExample()
    {
        Console.WriteLine("🎯 Exemplo 1: Hello World");
        
        var code = """
            building Demo
              office Main
                employee Start
                  show "Olá, mundo Chest!"
            """;
        
        CompileAndRun("Hello World", code);
    }
    
    static void RunArithmeticExample()
    {
        Console.WriteLine("\n🔢 Exemplo 2: Operações Aritméticas");
        
        var code = """
            building Calculator
              office Math
                employee Calculate
                  chest a = 15
                  chest b = 25
                  chest soma = a + b
                  chest produto = a * b
                  chest divisao = b / 5
                  
                  show "Valores:"
                  show "a = " + a
                  show "b = " + b
                  show "Soma: " + soma
                  show "Produto: " + produto
                  show "Divisão b/5: " + divisao
            """;
        
        CompileAndRun("Aritmética", code);
    }
    
    static void RunConditionalExample()
    {
        Console.WriteLine("\n🤔 Exemplo 3: Estruturas Condicionais");
        
        var code = """
            building Logic
              office Decision
                employee CheckAge
                  chest age = 25
                  chest name = "Ana"
                  
                  show "Verificando idade de " + name + " (" + age + " anos)"
                  
                  decide age >= 18
                    show "✅ Maior de idade"
                    decide age >= 65
                      show "🎂 Aposentada"
                    else
                      show "💼 Em idade ativa"
                  else
                    show "🚫 Menor de idade"
            """;
        
        CompileAndRun("Condicionais", code);
    }
    
    static void RunBusinessExample()
    {
        Console.WriteLine("\n🏢 Exemplo 4: Cenário Empresarial");
        
        var code = """
            building Company
              office HR
                employee ProcessEmployee
                  chest employeeName = "Carlos"
                  chest baseSalary = 4500.0
                  chest experience = 8
                  
                  show "=== PROCESSAMENTO DE FUNCIONÁRIO ==="
                  show "Nome: " + employeeName
                  show "Salário base: R$ " + baseSalary
                  show "Experiência: " + experience + " anos"
                  
                  decide experience > 5
                    chest experienceBonus = baseSalary * 0.2
                    chest finalSalary = baseSalary + experienceBonus
                    show "🎉 Bônus por experiência: R$ " + experienceBonus
                    show " Salário alto: R$ " + finalSalary
                    chest tax = finalSalary * 0.275
                    show "💸 Imposto (27.5%): R$ " + tax
                  else
                    show "� Sem bônus de experiência ainda"
                    show "�💵 Salário padrão: R$ " + baseSalary
                    chest tax = baseSalary * 0.15
                    show "💸 Imposto (15%): R$ " + tax
                  
                  show "==============================="
            """;
        
        CompileAndRun("Cenário Empresarial", code);
    }
    
    static void CompileAndRun(string exampleName, string sourceCode)
    {
        try
        {
            Console.WriteLine($"📝 Código {exampleName}:");
            
            // Análise léxica
            var lexer = new ChestLexer(sourceCode);
            var tokens = lexer.Tokenize();
            Console.WriteLine($"   ✓ Lexer: {tokens.Count} tokens gerados");
            
            // Análise sintática
            var parser = new ChestParser(tokens);
            var ast = parser.Parse();
            Console.WriteLine($"   ✓ Parser: AST com {ast.Buildings.Count} building(s)");
            
            // Geração de código IL
            var emitter = new ChestEmitter();
            var assembly = emitter.Emit(ast);
            Console.WriteLine($"   ✓ Emitter: Assembly gerado com entry point '{assembly.EntryMethod.Name}'");
            
            // Execução
            Console.WriteLine($"   🚀 Executando {exampleName}:");
            Console.WriteLine("   " + new string('-', 40));
            
            assembly.Execute();
            
            Console.WriteLine("   " + new string('-', 40));
            Console.WriteLine($"   ✅ {exampleName} executado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Erro em {exampleName}: {ex.Message}");
            throw;
        }
    }
}
