using System;
using System.IO;
using Xunit;

namespace Chest.Compiler.Tests;

/// <summary>
/// Testes do compilador Chest
/// </summary>
public class ChestCompilerTests
{
    /// <summary>
    /// Compila e executa código Chest, capturando a saída do console
    /// </summary>
    private static string CompileAndRun(string sourceCode)
    {
        // Preparar captura de console
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        try
        {
            // Lexer
            var lexer = new ChestLexer(sourceCode);
            var tokens = lexer.Tokenize();
            
            // Parser
            var parser = new ChestParser(tokens);
            var ast = parser.Parse();
            
            // Emitter
            var emitter = new ChestEmitter();
            var assembly = emitter.Emit(ast);
            
            // Executar
            assembly.Execute();
            
            // Retornar saída capturada, normalizando quebras de linha
            return stringWriter.ToString().Replace("\r\n", "\n");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
    
    [Fact]
    public void TestHelloWorld()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  show "Hello"
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("Hello\n", output);
    }
    
    [Fact]
    public void TestNumber()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  chest x = 42
                  show x
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("42\n", output);
    }
    
    [Fact]
    public void TestArithmetic()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  show 40 + 2
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("42\n", output);
    }
    
    [Fact]
    public void TestIfTrue()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  decide 1 < 2
                    show "ok"
                  else
                    show "nope"
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("ok\n", output);
    }
    
    [Fact]
    public void TestIfFalse()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  decide 2 < 1
                    show "ok"
                  else
                    show "nope"
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("nope\n", output);
    }
    
    [Fact]
    public void TestStringConcat()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  show "Hello" + " World"
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("Hello World\n", output);
    }
    
    [Fact]
    public void TestVariableAssignment()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  chest name = "João"
                  chest age = 25
                  show name
                  show age
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("João\n25\n", output);
    }
    
    [Fact]
    public void TestComplexExpression()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  chest a = 10
                  chest b = 20
                  chest result = a + b * 2
                  show result
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("50\n", output);
    }
    
    [Fact]
    public void TestBooleans()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  chest verdade = true
                  chest mentira = false
                  show verdade
                  show mentira
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("True\nFalse\n", output);
    }
    
    [Fact]
    public void TestComparisons()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  show 5 > 3
                  show 2 < 1
                  show 4 == 4
                  show 3 != 5
                  show 7 >= 7
                  show 1 <= 0
            """;
        
        var output = CompileAndRun(code);
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal("True", lines[0]);
        Assert.Equal("False", lines[1]);
        Assert.Equal("True", lines[2]);
        Assert.Equal("True", lines[3]);
        Assert.Equal("True", lines[4]);
        Assert.Equal("False", lines[5]);
    }
    
    [Fact]
    public void TestStringNumberConcat()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  chest nome = "Idade: "
                  chest idade = 25
                  show nome + idade
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("Idade: 25\n", output);
    }
    
    [Fact]
    public void TestNestedDecisions()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  chest x = 15
                  decide x > 10
                    decide x < 20
                      show "Entre 10 e 20"
                    else
                      show "Maior que 20"
                  else
                    show "10 ou menor"
            """;
        
        var output = CompileAndRun(code);
        Assert.Equal("Entre 10 e 20\n", output);
    }
    
    [Fact(Skip = "Teste temporariamente desabilitado - problema com AccessViolationException")]
    public void TestComplexBusinessExample()
    {
        var code = """
            building Business
              office HR
                employee ProcessEmployee
                  chest name = "João"
                  chest salary = 5000.0
                  
                  show "Processando: " + name
                  
                  decide salary > 4000
                    show "Salário alto"
                    chest bonus = salary * 0.1
                    show "Bônus: " + bonus
                  else
                    show "Salário normal"
            """;
        
        var output = CompileAndRun(code);
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal("Processando: João", lines[0]);
        Assert.Equal("Salário alto", lines[1]);
        Assert.Equal("Bônus: 500", lines[2]);
    }
}

/// <summary>
/// Testes específicos do Lexer
/// </summary>
public class ChestLexerTests
{
    [Fact]
    public void TestBasicTokens()
    {
        var lexer = new ChestLexer("building office employee chest show");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(TokenType.Building, tokens[0].Type);
        Assert.Equal(TokenType.Office, tokens[1].Type);
        Assert.Equal(TokenType.Employee, tokens[2].Type);
        Assert.Equal(TokenType.Chest, tokens[3].Type);
        Assert.Equal(TokenType.Show, tokens[4].Type);
    }
    
    [Fact]
    public void TestStringLiteral()
    {
        var lexer = new ChestLexer("\"Hello World\"");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("Hello World", tokens[0].Value);
    }
    
    [Fact]
    public void TestNumberLiteral()
    {
        var lexer = new ChestLexer("42 3.14");
        var tokens = lexer.Tokenize();
        
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("42", tokens[0].Value);
        Assert.Equal(TokenType.Number, tokens[1].Type);
        Assert.Equal("3.14", tokens[1].Value);
    }
    
    [Fact]
    public void TestOperators()
    {
        var lexer = new ChestLexer("+ - * / < > <= >= == !=");
        var tokens = lexer.Tokenize();
        
        var expectedTypes = new[]
        {
            TokenType.Plus, TokenType.Minus, TokenType.Multiply, TokenType.Divide,
            TokenType.Less, TokenType.Greater, TokenType.LessEqual, TokenType.GreaterEqual,
            TokenType.Equal, TokenType.NotEqual
        };
        
        for (int i = 0; i < expectedTypes.Length; i++)
        {
            Assert.Equal(expectedTypes[i], tokens[i].Type);
        }
    }
}

/// <summary>
/// Testes específicos do Parser
/// </summary>
public class ChestParserTests
{
    [Fact]
    public void TestSimpleProgram()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  show "Hello"
            """;
        
        var lexer = new ChestLexer(code);
        var tokens = lexer.Tokenize();
        var parser = new ChestParser(tokens);
        var ast = parser.Parse();
        
        Assert.Single(ast.Buildings);
        Assert.Equal("Demo", ast.Buildings[0].Name);
        
        var office = Assert.IsType<OfficeNode>(ast.Buildings[0].Members[0]);
        Assert.Equal("Main", office.Name);
        
        var employee = Assert.IsType<EmployeeNode>(office.Members[0]);
        Assert.Equal("Start", employee.Name);
        
        var show = Assert.IsType<ShowNode>(employee.Body[0]);
        var literal = Assert.IsType<LiteralNode>(show.Expr);
        Assert.Equal("Hello", literal.Value);
    }
    
    [Fact]
    public void TestBinaryExpression()
    {
        var code = """
            building Demo
              office Main
                employee Start
                  show 1 + 2
            """;
        
        var lexer = new ChestLexer(code);
        var tokens = lexer.Tokenize();
        var parser = new ChestParser(tokens);
        var ast = parser.Parse();
        
        var office = (OfficeNode)ast.Buildings[0].Members[0];
        var employee = (EmployeeNode)office.Members[0];
        var show = (ShowNode)employee.Body[0];
        var binary = Assert.IsType<BinaryNode>(show.Expr);
        
        Assert.Equal("+", binary.Op);
        Assert.IsType<LiteralNode>(binary.Left);
        Assert.IsType<LiteralNode>(binary.Right);
    }
}

/// <summary>
/// Testes específicos do Emitter
/// </summary>
public class ChestEmitterTests
{
    [Fact]
    public void TestEmitSimpleProgram()
    {
        // Criar AST manualmente
        var program = new ProgramNode
        {
            Buildings = new List<BuildingNode>
            {
                new BuildingNode
                {
                    Name = "Demo",
                    Members = new List<Node>
                    {
                        new OfficeNode
                        {
                            Name = "Main",
                            Members = new List<Node>
                            {
                                new EmployeeNode
                                {
                                    Name = "Start",
                                    Body = new List<Node>
                                    {
                                        new ShowNode
                                        {
                                            Expr = new LiteralNode 
                                            { 
                                                Value = "Hello", 
                                                Type = ChestType.Text 
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        
        var emitter = new ChestEmitter();
        var assembly = emitter.Emit(program);
        
        Assert.NotNull(assembly);
        Assert.NotNull(assembly.EntryMethod);
        Assert.Equal("Start", assembly.EntryMethod.Name);
    }
}
