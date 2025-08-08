using System;

namespace Chest.Compiler;

/// <summary>
/// Biblioteca runtime da linguagem Chest
/// Contém todas as funções nativas que o código IL gerado irá chamar
/// </summary>
public static class ChestRuntime
{
    /// <summary>
    /// Implementa o comando 'show' - imprime valor no console
    /// </summary>
    public static void Show(object? value)
    {
        Console.WriteLine(value?.ToString() ?? "null");
    }
    
    #region Operadores de Comparação
    
    /// <summary>
    /// Operador menor que (&lt;) para números
    /// </summary>
    public static bool Lt(double a, double b) => a < b;
    
    /// <summary>
    /// Operador maior que (&gt;) para números
    /// </summary>
    public static bool Gt(double a, double b) => a > b;
    
    /// <summary>
    /// Operador menor ou igual (&lt;=) para números
    /// </summary>
    public static bool Le(double a, double b) => a <= b;
    
    /// <summary>
    /// Operador maior ou igual (&gt;=) para números
    /// </summary>
    public static bool Ge(double a, double b) => a >= b;
    
    /// <summary>
    /// Operador igualdade (==) para qualquer tipo
    /// </summary>
    public static bool Eq(object? a, object? b) => Equals(a, b);
    
    /// <summary>
    /// Operador diferença (!=) para qualquer tipo
    /// </summary>
    public static bool Ne(object? a, object? b) => !Equals(a, b);
    
    #endregion
    
    #region Operadores Aritméticos
    
    /// <summary>
    /// Operador adição (+) para números
    /// </summary>
    public static double Add(double a, double b) => a + b;
    
    /// <summary>
    /// Operador subtração (-) para números
    /// </summary>
    public static double Sub(double a, double b) => a - b;
    
    /// <summary>
    /// Operador multiplicação (*) para números
    /// </summary>
    public static double Mul(double a, double b) => a * b;
    
    /// <summary>
    /// Operador divisão (/) para números
    /// </summary>
    public static double Div(double a, double b) => a / b;
    
    #endregion
    
    #region Operadores de String
    
    /// <summary>
    /// Concatenação (+) para strings
    /// </summary>
    public static string Concat(string? a, string? b) => (a ?? "") + (b ?? "");
    
    /// <summary>
    /// Concatenação entre string e número
    /// </summary>
    public static string Concat(string? a, double b) => (a ?? "") + b.ToString();
    
    /// <summary>
    /// Concatenação entre número e string
    /// </summary>
    public static string Concat(double a, string? b) => a.ToString() + (b ?? "");
    
    /// <summary>
    /// Concatenação entre string e bool
    /// </summary>
    public static string Concat(string? a, bool b) => (a ?? "") + (b ? "true" : "false");
    
    /// <summary>
    /// Concatenação entre bool e string
    /// </summary>
    public static string Concat(bool a, string? b) => (a ? "true" : "false") + (b ?? "");
    
    #endregion
    
    #region Operadores Lógicos (para futuro)
    
    /// <summary>
    /// Operador E lógico (&amp;&amp;)
    /// </summary>
    public static bool And(bool a, bool b) => a && b;
    
    /// <summary>
    /// Operador OU lógico (||)
    /// </summary>
    public static bool Or(bool a, bool b) => a || b;
    
    /// <summary>
    /// Operador NÃO lógico (!)
    /// </summary>
    public static bool Not(bool a) => !a;
    
    #endregion
    
    #region Conversões de Tipo
    
    /// <summary>
    /// Converte número para string
    /// </summary>
    public static string NumberToText(double value) => value.ToString();
    
    /// <summary>
    /// Converte bool para string
    /// </summary>
    public static string BoolToText(bool value) => value ? "true" : "false";
    
    /// <summary>
    /// Tenta converter string para número
    /// </summary>
    public static double TextToNumber(string? text)
    {
        if (double.TryParse(text, out var result))
            return result;
        throw new InvalidOperationException($"Não foi possível converter '{text}' para número");
    }
    
    /// <summary>
    /// Tenta converter string para bool
    /// </summary>
    public static bool TextToBool(string? text)
    {
        return text?.ToLower() switch
        {
            "true" or "verdadeiro" or "sim" or "1" => true,
            "false" or "falso" or "não" or "nao" or "0" => false,
            _ => throw new InvalidOperationException($"Não foi possível converter '{text}' para bool")
        };
    }
    
    #endregion
    
    #region Utilitários
    
    /// <summary>
    /// Verifica se um valor é "verdadeiro" na linguagem Chest
    /// (similar ao JavaScript: 0, "", null, false são falsy)
    /// </summary>
    public static bool IsTruthy(object? value)
    {
        return value switch
        {
            null => false,
            bool b => b,
            double d => d != 0.0,
            string s => !string.IsNullOrEmpty(s),
            _ => true
        };
    }
    
    /// <summary>
    /// Converte qualquer valor para string para debugging
    /// </summary>
    public static string Debug(object? value)
    {
        return value switch
        {
            null => "null",
            string s => $"\"{s}\"",
            double d => d.ToString(),
            bool b => b ? "true" : "false",
            _ => value.ToString() ?? "unknown"
        };
    }
    
    #endregion
    
    #region Operações Dinâmicas para Object
    
    /// <summary>
    /// Adição dinâmica que lida com objects e faz conversões automaticamente
    /// </summary>
    public static object AddDynamic(object? left, object? right)
    {
        try
        {
            // Proteção contra null
            left ??= 0;
            right ??= 0;
            
            // Caso especial: ambos são strings ou um é string
            if (left is string || right is string)
            {
                return (left?.ToString() ?? "") + (right?.ToString() ?? "");
            }
            
            // Tentar como números
            var leftNum = Convert.ToDouble(left);
            var rightNum = Convert.ToDouble(right);
            
            return leftNum + rightNum;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro na adição: left={left}, right={right}, error={ex.Message}");
        }
    }
    
    /// <summary>
    /// Subtração dinâmica
    /// </summary>
    public static double SubDynamic(object? left, object? right)
    {
        if (TryGetNumber(left, out var leftNum) && TryGetNumber(right, out var rightNum))
        {
            return leftNum - rightNum;
        }
        throw new InvalidOperationException($"Não é possível subtrair '{right}' de '{left}'");
    }
    
    /// <summary>
    /// Multiplicação dinâmica
    /// </summary>
    public static double MulDynamic(object? left, object? right)
    {
        if (TryGetNumber(left, out var leftNum) && TryGetNumber(right, out var rightNum))
        {
            return leftNum * rightNum;
        }
        throw new InvalidOperationException($"Não é possível multiplicar '{left}' por '{right}'");
    }
    
    /// <summary>
    /// Divisão dinâmica
    /// </summary>
    public static double DivDynamic(object? left, object? right)
    {
        if (TryGetNumber(left, out var leftNum) && TryGetNumber(right, out var rightNum))
        {
            return leftNum / rightNum;
        }
        throw new InvalidOperationException($"Não é possível dividir '{left}' por '{right}'");
    }
    
    /// <summary>
    /// Comparação menor que dinâmica
    /// </summary>
    public static bool LtDynamic(object? left, object? right)
    {
        if (TryGetNumber(left, out var leftNum) && TryGetNumber(right, out var rightNum))
        {
            return leftNum < rightNum;
        }
        throw new InvalidOperationException($"Não é possível comparar '{left}' < '{right}'");
    }
    
    /// <summary>
    /// Comparação maior que dinâmica
    /// </summary>
    public static bool GtDynamic(object? left, object? right)
    {
        if (TryGetNumber(left, out var leftNum) && TryGetNumber(right, out var rightNum))
        {
            return leftNum > rightNum;
        }
        throw new InvalidOperationException($"Não é possível comparar '{left}' > '{right}'");
    }
    
    /// <summary>
    /// Comparação menor ou igual dinâmica
    /// </summary>
    public static bool LeDynamic(object? left, object? right)
    {
        if (TryGetNumber(left, out var leftNum) && TryGetNumber(right, out var rightNum))
        {
            return leftNum <= rightNum;
        }
        throw new InvalidOperationException($"Não é possível comparar '{left}' <= '{right}'");
    }
    
    /// <summary>
    /// Comparação maior ou igual dinâmica
    /// </summary>
    public static bool GeDynamic(object? left, object? right)
    {
        if (TryGetNumber(left, out var leftNum) && TryGetNumber(right, out var rightNum))
        {
            return leftNum >= rightNum;
        }
        throw new InvalidOperationException($"Não é possível comparar '{left}' >= '{right}'");
    }
    
    /// <summary>
    /// Tenta extrair um número de um object
    /// </summary>
    private static bool TryGetNumber(object? value, out double number)
    {
        number = 0;
        
        if (value == null) return false;
        
        switch (value)
        {
            case double d:
                number = d;
                return true;
            case float f:
                number = f;
                return true;
            case int i:
                number = i;
                return true;
            case long l:
                number = l;
                return true;
            case decimal dec:
                number = (double)dec;
                return true;
            case string s when double.TryParse(s, out var parsed):
                number = parsed;
                return true;
            default:
                return false;
        }
    }
    
    #endregion
}
