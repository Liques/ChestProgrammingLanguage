using System;
using Chest.Compiler.Core;
using Chest.Compiler.Lexing;
using Chest.Compiler.Parsing;
using Chest.Compiler.CodeGen;
using Chest.Compiler.Runtime;

namespace Chest.Compiler;

/// <summary>
/// Entry point of the Chest compiler
/// Demonstrates how to use the compiler to run Chest code
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Chest Compiler v1.0 ===");
        Console.WriteLine("Compiling Chest code to IL (.NET)...\n");
        
        try
        {
            // Check if a file argument was provided
            if (args.Length > 0)
            {
                var filePath = args[0];
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"ERROR: File '{filePath}' not found.");
                    ShowUsage();
                    return;
                }
                
                if (!filePath.EndsWith(".chest"))
                {
                    Console.WriteLine($"WARNING: File '{filePath}' does not have .chest extension.");
                }
                
                CompileFile(filePath);
            }
            else
            {
                // Run demo examples if no file provided
                RunHelloWorldExample();
                RunArithmeticExample();
                RunConditionalExample();
                RunBusinessExample();
                
                Console.WriteLine("\n=== All examples executed successfully! ===");
                Console.WriteLine("\nTo compile a .chest file, use:");
                Console.WriteLine("  dotnet run <filename.chest>");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            if (ex is ParseException parseEx)
            {
                Console.WriteLine($"Position: {parseEx.Span}");
            }
        }
    }
    
    static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run                    - Run demo examples");
        Console.WriteLine("  dotnet run <file.chest>       - Compile and run a .chest file");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run hello.chest");
        Console.WriteLine("  dotnet run examples/demo.chest");
    }
    
    static void CompileFile(string filePath)
    {
        Console.WriteLine($"📁 Compiling file: {filePath}");
        
        var sourceCode = File.ReadAllText(filePath);
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        
        CompileAndRun(fileName, sourceCode);
    }
    
    static void RunHelloWorldExample()
    {
        Console.WriteLine("🎯 Example 1: Hello World");
        
        var code = """
            building Demo
              office Main
                employee Start
                  show "Hello, Chest world!"
            """;
        
        CompileAndRun("Hello World", code);
    }
    
    static void RunArithmeticExample()
    {
        Console.WriteLine("\n🔢 Example 2: Arithmetic Operations");
        
        var code = """
            building Calculator
              office Math
                employee Calculate
                  chest a = 15
                  chest b = 25
                  chest sum = a + b
                  chest product = a * b
                  chest division = b / 5
                  
                  show "Values:"
                  show "a = " + a
                  show "b = " + b
                  show "Sum: " + sum
                  show "Product: " + product
                  show "Division b/5: " + division
            """;
        
        CompileAndRun("Arithmetic", code);
    }
    
    static void RunConditionalExample()
    {
        Console.WriteLine("\n🤔 Example 3: Conditional Structures");
        
        var code = """
            building Logic
              office Decision
                employee CheckAge
                  chest age = 25
                  chest name = "Ana"
                  
                  show "Checking age of " + name + " (" + age + " years)"
                  
                  decide age >= 18
                    show "✅ Adult"
                    decide age >= 65
                      show "🎂 Retired"
                    else
                      show "💼 Working age"
                  else
                    show "🚫 Underage"
            """;
        
        CompileAndRun("Conditionals", code);
    }
    
    static void RunBusinessExample()
    {
        Console.WriteLine("\n🏢 Example 4: Business Scenario");
        
        var code = """
            building Company
              office HR
                employee ProcessEmployee
                  chest employeeName = "Carlos"
                  chest baseSalary = 4500.0
                  chest experience = 8
                  
                  show "=== EMPLOYEE PROCESSING ==="
                  show "Name: " + employeeName
                  show "Base salary: R$ " + baseSalary
                  show "Experience: " + experience + " years"
                  
                  decide experience > 5
                    chest experienceBonus = baseSalary * 0.2
                    chest finalSalary = baseSalary + experienceBonus
                    show "🎉 Experience bonus: R$ " + experienceBonus
                    show "💰 High salary: R$ " + finalSalary
                    chest tax = finalSalary * 0.275
                    show "💸 Tax (27.5%): R$ " + tax
                  else
                    show "🚫 No experience bonus yet"
                    show "💵 Standard salary: R$ " + baseSalary
                    chest tax = baseSalary * 0.15
                    show "💸 Tax (15%): R$ " + tax
                  
                  show "==============================="
            """;
        
        CompileAndRun("Business Scenario", code);
    }
    
    static void CompileAndRun(string exampleName, string sourceCode)
    {
        try
        {
            Console.WriteLine($"📝 Code {exampleName}:");
            
            // Lexical analysis
            var lexer = new ChestLexer(sourceCode);
            var tokens = lexer.Tokenize();
            Console.WriteLine($"   ✓ Lexer: {tokens.Count} tokens generated");
            
            // Parsing
            var parser = new ChestParser(tokens);
            var ast = parser.Parse();
            Console.WriteLine($"   ✓ Parser: AST with {ast.Buildings.Count} building(s)");
            
            // IL code generation
            var emitter = new ChestEmitter();
            var assembly = emitter.Emit(ast);
            Console.WriteLine($"   ✓ Emitter: Assembly generated with entry point '{assembly.EntryMethod.Name}'");
            
            // Execution
            Console.WriteLine($"   🚀 Running {exampleName}:");
            Console.WriteLine("   " + new string('-', 40));
            
            assembly.Execute();
            
            Console.WriteLine("   " + new string('-', 40));
            Console.WriteLine($"   ✅ {exampleName} executed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error in {exampleName}: {ex.Message}");
            throw;
        }
    }
}
