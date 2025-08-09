# Chest Calculator Examples

This folder contains calculator programs written in the Chest programming language, demonstrating both static calculations and interactive user input.

## Files

- `calculator.chest` - Interactive calculator with user input using `ask` functionality
- `advanced_calculator.chest` - Advanced calculator with area, volume, temperature, and financial calculations
- `interactive_calculator.chest` - Calculator demonstrating mixed input and calculations
- `user_calculator.chest` - User input focused calculator
- `simple_test.chest` - Simple test file for debugging
- `test.chest` - Basic test file

## How to Run

To compile and run any Chest program, use the Chest compiler:

```pwsh
cd C:\Projs\ChestLanguage\Samples\Calculator
C:\Projs\ChestLanguage\Chest.Compiler\bin\Debug\net9.0\Chest.Compiler.exe .\calculator.chest
```

For interactive programs, you can provide input via pipe:

```pwsh
echo "Alice
25
17
Hello World!" | C:\Projs\ChestLanguage\Chest.Compiler\bin\Debug\net9.0\Chest.Compiler.exe .\calculator.chest
```

## New Features Demonstrated

### Interactive User Input (`ask`)
The Chest language now supports user input through the `ask` command:

```rust
chest name = ask "Enter your name: "     // Ask with prompt
chest value = ask                        // Ask without prompt
show "Hello " + name
```

### Module Import (`attach`)
Import functionality (placeholder for future expansion):

```rust
attach Console        // Similar to 'using' in C#
```

## Chest Language Features Demonstrated

### Interactive Calculator (`calculator.chest`)
- User input with `ask` command
- Variable declarations with `chest`
- String concatenation and manipulation
- Mathematical operations with hardcoded numbers
- Interactive prompts and responses

### Advanced Calculator (`advanced_calculator.chest`)
- Complex mathematical calculations
- Nested conditional statements
- Scientific calculations (area, volume, temperature)
- Financial calculations (simple interest)
- Performance metrics

### Key Language Features
- **Variables**: `chest variable = value`
- **User Input**: `ask "prompt"` or `ask`
- **Module Import**: `attach ModuleName`
- **Arithmetic**: `+`, `-`, `*`, `/`
- **Comparisons**: `<`, `>`, `==`
- **Conditionals**: `decide condition` / `else`
- **String operations**: Concatenation with `+`
- **Console output**: `show expression`
- **Indentation-based syntax**: Python-style blocks

## Chest Language Syntax

The Chest language uses Python-style indentation and real-world metaphors:

- `building` = namespace/module
- `office` = class/type
- `employee` = method/function
- `chest` = variable
- `show` = print/output
- `decide` = if/conditional
- `attach` = import/using
- `ask` = get user input

Example structure:
```rust
building MyProgram
  office MyClass
    employee MyFunction
      attach Console
      chest userName = ask "Enter name: "
      chest myVariable = 42
      show "Hello " + userName
      decide myVariable > 40
        show "Large number"
      else
        show "Small number"
```

## Building the Compiler

Before running the examples, make sure the Chest compiler is built:

```pwsh
cd C:\Projs\ChestLanguage\Chest.Compiler
dotnet build
```

The compiler will be available at:
`C:\Projs\ChestLanguage\Chest.Compiler\bin\Debug\net9.0\Chest.Compiler.exe`
