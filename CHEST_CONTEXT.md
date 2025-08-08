# CHEST PROGRAMMING LANGUAGE - CONTEXT GUIDE

Este arquivo contém todas as informações necessárias para uma LLM dar suporte completo ao projeto compilador da linguagem Chest.

## VISÃO GERAL DO PROJETO

**Nome**: Chest Programming Language Compiler  
**Objetivo**: Compilador JIT que converte código Chest diretamente para IL (.NET MSIL/CIL) usando Reflection.Emit  
**Localização**: `c:\Projs\ChestLanguage\Chest.Compiler\`  
**Framework**: .NET 9.0  
**Linguagem**: C# com Nullable habilitado  

## FILOSOFIA DA LINGUAGEM CHEST

Chest é uma linguagem projetada para ser **intuitiva** usando metáforas do mundo real:

- **building** = namespace/agrupador (como um edifício empresarial)
- **office** = classe/tipo (como um escritório no edifício)
- **employee** = método/função (funcionário que executa tarefas)
- **chest** = variável (baú que guarda valores)
- **show** = imprimir/exibir (mostrar conteúdo do baú)
- **decide** = if/else (tomar uma decisão)
- **go** = acessar objeto/propriedade (ir até um local)
- **poke** = chamar método (cutucar alguém para fazer algo)

## SINTAXE BÁSICA

### Estrutura Mínima
```rust
building MinhaEmpresa
  office Recepção  
    employee Cumprimentar
      chest mensagem = "Olá Mundo"
      show mensagem
```

### Tipos Suportados (Fase 1)
- `number` → `System.Double`
- `text` → `System.String` 
- `bool` → `System.Boolean`

### Operadores
- Aritméticos: `+`, `-`, `*`, `/`
- Comparação: `<`, `>`, `==`, `!=`, `<=`, `>=`
- Lógicos: `&&`, `||`, `!` (futura implementação)

### Controle de Fluxo
```rust
decide condicao {
  // bloco então
} else {
  // bloco senão  
}
```

## GRAMÁTICA FORMAL

```
program        : buildingDecl+ EOF ;
buildingDecl   : 'building' IDENT block ;
block          : '{' stmt* '}' ;
stmt           : officeDecl
               | employeeDecl
               | varDecl ';'
               | showStmt ';'
               | decideStmt
               ;
officeDecl     : 'office' IDENT block ;
employeeDecl   : 'employee' IDENT paramList? block ;
paramList      : '(' (IDENT (',' IDENT)*)? ')' ;
varDecl        : 'chest' IDENT ('=' expr)? ;
showStmt       : 'show' expr ;
decideStmt     : 'decide' expr block ('else' block)? ;
expr           : literal
               | IDENT
               | expr binop expr
               | '(' expr ')'
               ;
literal        : NUMBER | STRING | 'true' | 'false' ;
binop          : '+' | '-' | '*' | '/' | '<' | '>' | '==' | '!=' | '<=' | '>=' ;
```

## ARQUITETURA DO COMPILADOR

### Pipeline de Compilação
1. **Lexer/Parser** → AST (Abstract Syntax Tree)
2. **Análise Semântica** → Verificação de tipos, escopos
3. **Geração IL** → Reflection.Emit para MSIL
4. **Runtime** → Biblioteca C# com funções nativas
5. **Execução** → JIT do CLR

### Estrutura de Arquivos
```
Chest.Compiler/
├── Ast.cs              // Classes do AST
├── ChestEmitter.cs     // Gerador de IL (Reflection.Emit)
├── ChestRuntime.cs     // Biblioteca runtime
├── Parser.cs           // Parser simples (sem ANTLR)
├── SymbolTable.cs      // Gerenciamento de escopos
├── Program.cs          // Entry point e demonstração
└── Tests.cs            // Testes xUnit
```

## CLASSES DO AST

```csharp
public abstract class Node 
{
    public SourceSpan Span { get; set; }
}

public class ProgramNode : Node 
{
    public List<BuildingNode> Buildings { get; set; } = new();
}

public class BuildingNode : Node 
{
    public string Name { get; set; } = "";
    public List<Node> Members { get; set; } = new();
}

public class OfficeNode : Node 
{
    public string Name { get; set; } = "";
    public List<Node> Members { get; set; } = new();
}

public class EmployeeNode : Node 
{
    public string Name { get; set; } = "";
    public List<Parameter> Parameters { get; set; } = new();
    public List<Node> Body { get; set; } = new();
}

public class VarDeclNode : Node 
{
    public string Name { get; set; } = "";
    public ExprNode? Init { get; set; }
    public TypeRef TypeHint { get; set; }
}

public class ShowNode : Node 
{
    public ExprNode Expr { get; set; } = null!;
}

public class DecideNode : Node 
{
    public ExprNode Cond { get; set; } = null!;
    public List<Node> Then { get; set; } = new();
    public List<Node>? Else { get; set; }
}

public abstract class ExprNode : Node { }

public class LiteralNode : ExprNode 
{
    public object? Value { get; set; }
    public ChestType Type { get; set; }
}

public class IdentNode : ExprNode 
{
    public string Name { get; set; } = "";
}

public class BinaryNode : ExprNode 
{
    public string Op { get; set; } = "";
    public ExprNode Left { get; set; } = null!;
    public ExprNode Right { get; set; } = null!;
}

public struct TypeRef 
{
    public string Name { get; set; }
}

public struct SourceSpan 
{
    public int Line { get; set; }
    public int Col { get; set; }
    public int EndLine { get; set; }
    public int EndCol { get; set; }
}

public class Parameter 
{
    public string Name { get; set; } = "";
    public TypeRef TypeHint { get; set; }
}

public enum ChestType 
{
    Number,  // double
    Text,    // string  
    Bool     // boolean
}
```

## RUNTIME CHEST

```csharp
public static class ChestRuntime
{
    public static void Show(object? x)
    {
        Console.WriteLine(x?.ToString() ?? "null");
    }
    
    public static bool Lt(double a, double b) => a < b;
    public static bool Gt(double a, double b) => a > b;
    public static bool Le(double a, double b) => a <= b;
    public static bool Ge(double a, double b) => a >= b;
    public static bool Eq(object? a, object? b) => Equals(a, b);
    public static bool Ne(object? a, object? b) => !Equals(a, b);
    
    public static double Add(double a, double b) => a + b;
    public static double Sub(double a, double b) => a - b;
    public static double Mul(double a, double b) => a * b;
    public static double Div(double a, double b) => a / b;
    
    public static string Concat(string? a, string? b) => (a ?? "") + (b ?? "");
}
```

## MAPEAMENTO CHEST → IL

| Construção Chest | Ação IL |
|------------------|---------|
| `text` literal | `Ldstr` com a string |
| `number` literal | `Ldc_R8` (double) |
| `bool` literal | `Ldc_I4_0`/`Ldc_I4_1` |
| `chest x = expr` | `DeclareLocal(T)`, gerar expr, `Stloc x` |
| `show expr` | gerar expr, box se necessário, `Call ChestRuntime.Show(object)` |
| `decide cond { A } else { B }` | gerar cond → `Brfalse elseLbl`; gerar A; `Br end`; `MarkLabel elseLbl`; gerar B; `MarkLabel end` |
| `a + b` (números) | gerar `a`, gerar `b`, `Call ChestRuntime.Add` |
| `a + b` (strings) | gerar `a`, gerar `b`, `Call ChestRuntime.Concat` |
| `a < b` | gerar `a`, `b`, `Call ChestRuntime.Lt` |
| Identificador local | `Ldloc` |

## GERAÇÃO DE IL COM REFLECTION.EMIT

### Estrutura Principal
```csharp
public class ChestEmitter
{
    private AssemblyBuilder asmBuilder;
    private ModuleBuilder modBuilder;
    private Dictionary<string, TypeBuilder> officeTypes = new();
    
    public ChestAssembly Emit(ProgramNode program)
    {
        // Criar assembly dinâmico
        var asmName = new AssemblyName("ChestOut");
        asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
        modBuilder = asmBuilder.DefineDynamicModule("ChestOutModule");
        
        // Processar buildings/offices/employees
        foreach (var building in program.Buildings)
        {
            EmitBuilding(building);
        }
        
        // Criar tipos e retornar assembly
        return FinalizeAssembly();
    }
}
```

### Geração de Entry Point
O compilador deve gerar automaticamente um método `Main` que chama o primeiro employee público encontrado:

```csharp
public static void Main(string[] args)
{
    Demo.Main.Start(); // ou o primeiro employee encontrado
}
```

## GERENCIAMENTO DE ESCOPOS

```csharp
public class SymbolTable
{
    private Stack<Dictionary<string, LocalBuilder>> scopes = new();
    
    public void PushScope() => scopes.Push(new Dictionary<string, LocalBuilder>());
    public void PopScope() => scopes.Pop();
    
    public void DeclareVariable(string name, LocalBuilder local)
    {
        scopes.Peek()[name] = local;
    }
    
    public LocalBuilder? LookupVariable(string name)
    {
        foreach (var scope in scopes)
        {
            if (scope.TryGetValue(name, out var local))
                return local;
        }
        return null;
    }
}
```

## TRATAMENTO DE ERROS

### Durante Parsing
- Reportar linha/coluna do erro usando `SourceSpan`
- Mensagens em português, user-friendly
- Sugestões de correção quando possível

### Durante Emissão
- Validar tipos antes de emitir IL
- Verificar se variáveis foram declaradas
- Verificar compatibilidade de operadores

### Durante Execução
- Capturar exceções e mapear para código fonte
- Stack traces legíveis

## BOXING AUTOMÁTICO

Para chamadas que esperam `object` (como `ChestRuntime.Show`):
```csharp
// Se expr é value type (double, bool)
EmitExpression(il, expr, scope);
if (exprType.IsValueType)
{
    il.Emit(OpCodes.Box, exprType);
}
il.Emit(OpCodes.Call, showMethodInfo);
```

## TESTES OBRIGATÓRIOS

### Testes Básicos (xUnit)
1. **HelloWorld**: `show "Hello"` → output: "Hello"
2. **Numbers**: `chest x = 42; show x` → output: "42"  
3. **Arithmetic**: `show 40 + 2` → output: "42"
4. **IfTrue**: `decide 1 < 2 { show "ok" } else { show "nope" }` → output: "ok"
5. **IfFalse**: `decide 2 < 1 { show "ok" } else { show "nope" }` → output: "nope"
6. **StringConcat**: `show "Hello" + " World"` → output: "Hello World"

### Estrutura de Teste
```csharp
[Test]
public void TestHelloWorld()
{
    var code = """
        building Demo
          office Main
            employee Start
              show "Hello"
        """;
    
    var output = CompileAndRun(code);
    Assert.AreEqual("Hello", output.Trim());
}
```

## DEPENDÊNCIAS NECESSÁRIAS

Adicionar ao `Chest.Compiler.csproj`:
```xml
<PackageReference Include="xunit" Version="2.4.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
```

## EXEMPLOS DE CÓDIGO CHEST

### Hello World
```rust
building Demo
  office Main
    employee Start
      show "Hello, Chest!"
```

### Variáveis e Operações
```rust
building Calc
  office Math
    employee Calculate
      chest a = 10
      chest b = 20
      chest result = a + b
      show result
```

### Condicionais
```rust
building Logic
  office Decision
    employee Check
      chest age = 18
      decide age >= 18 {
        show "Adulto"
      } else {
        show "Menor"
      }
```

### Exemplo Complexo
```rust
building Business
  office HR
    employee ProcessEmployee
      chest name = "João"
      chest salary = 5000.0
      
      show "Processando: " + name
      
      decide salary > 4000 {
        show "Salário alto"
        chest bonus = salary * 0.1
        show "Bônus: " + bonus
      } else {
        show "Salário normal"
      }
```

## INSTRUÇÕES PARA LLM

### Ao Implementar Parser
- Use parsing recursivo descendente simples
- Não use bibliotecas externas (ANTLR, etc.) na primeira versão
- Trate indentação como delimitador de blocos (Python-style)
- Suporte comentários com `//`

### Ao Implementar Emitter
- Sempre use tipos .NET correspondentes (double, string, bool)
- Implemente boxing automático para chamadas com object
- Gere métodos static para employees
- Crie entry point automático

### Ao Escrever Testes
- Teste cada construção da linguagem isoladamente
- Capture stdout para validar output
- Use nomes de teste descritivos em português
- Inclua testes de erro (parsing inválido, etc.)

### Tratamento de Tipos
- `number` → sempre `System.Double` 
- `text` → sempre `System.String`
- `bool` → sempre `System.Boolean`
- Conversões automáticas quando necessário

### Convenções de Código C#
- Use nullable habilitado (`<Nullable>enable</Nullable>`)
- Prefira records para estruturas imutáveis
- Use pattern matching quando apropriado
- Métodos pequenos e responsabilidade única
- Nomes em inglês para código, comentários em português

## ROADMAP FUTURO

### Fase 2 - Funcionalidades Avançadas
- Parâmetros em employees
- Chamadas entre employees  
- Tipos personalizados (offices como classes)
- Arrays/listas
- Loops (repetir/enquanto)

### Fase 3 - Otimizações
- Análise de fluxo de dados
- Otimizações de IL
- Suporte a debugging
- Compilation cache

### Fase 4 - Tooling
- Language Server Protocol (LSP)
- Syntax highlighting
- IntelliSense
- Depurador visual

## COMANDOS ÚTEIS

```bash
# Build do projeto
dotnet build

# Executar testes
dotnet test

# Executar compilador
dotnet run

# Adicionar dependências
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
```

## DEBUGGING TIPS

### Para Debug de IL
1. Use `ildasm.exe` para visualizar IL gerado
2. Salve assembly em disco para análise: `asmBuilder.Save("output.dll")`
3. Compare com IL de código C# equivalente

### Para Debug de Runtime
1. Capture exceções e mostre stack trace
2. Log cada operação de IL emitida
3. Use breakpoints em `ChestRuntime` methods

---

**IMPORTANTE**: Este contexto deve ser mantido atualizado conforme o projeto evolui. Sempre inclua exemplos práticos e mantenha a documentação sincronizada com o código.
