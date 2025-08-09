using System;

namespace Chest.Compiler.Runtime;

/// <summary>
/// Chest language runtime library
/// Contains all native functions that generated IL code will call
/// </summary>
public static class ChestRuntime
{
    /// <summary>
    /// Implements the 'show' command - prints value to console
    /// </summary>
    public static void Show(object? value)
    {
        Console.WriteLine(value?.ToString() ?? "null");
    }
    
    /// <summary>
    /// Implements the 'ask' command - gets user input from console
    /// </summary>
    public static string Ask()
    {
        return Console.ReadLine() ?? "";
    }
    
    /// <summary>
    /// Implements the 'ask' command with prompt - gets user input from console
    /// </summary>
    public static string Ask(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? "";
    }
    
    #region Comparison Operators
    
    /// <summary>
    /// Less than (<) operator for numbers
    /// </summary>
    public static bool Lt(double a, double b) => a < b;
    
    /// <summary>
    /// Greater than (>) operator for numbers
    /// </summary>
    public static bool Gt(double a, double b) => a > b;
    
    /// <summary>
    /// Less than or equal (<=) operator for numbers
    /// </summary>
    public static bool Le(double a, double b) => a <= b;
    
    /// <summary>
    /// Greater than or equal (>=) operator for numbers
    /// </summary>
    public static bool Ge(double a, double b) => a >= b;
    
    /// <summary>
    /// Equality (==) operator for any type
    /// </summary>
    public static bool Eq(object? a, object? b) => Equals(a, b);
    
    /// <summary>
    /// Inequality (!=) operator for any type
    /// </summary>
    public static bool Ne(object? a, object? b) => !Equals(a, b);
    
    #endregion
    
    #region Arithmetic Operators
    
    /// <summary>
    /// Addition (+) operator for numbers
    /// </summary>
    public static double Add(double a, double b) => a + b;
    
    /// <summary>
    /// Subtraction (-) operator for numbers
    /// </summary>
    public static double Sub(double a, double b) => a - b;
    
    /// <summary>
    /// Multiplication (*) operator for numbers
    /// </summary>
    public static double Mul(double a, double b) => a * b;
    
    /// <summary>
    /// Division (/) operator for numbers
    /// </summary>
    public static double Div(double a, double b) => a / b;
    
    #endregion
    
    #region String Operators
    
    /// <summary>
    /// Concatenation (+) for strings
    /// </summary>
    public static string Concat(string? a, string? b) => (a ?? "") + (b ?? "");
    
    /// <summary>
    /// Concatenation between string and number
    /// </summary>
    public static string Concat(string? a, double b) => (a ?? "") + b.ToString();
    
    /// <summary>
    /// Concatenation between number and string
    /// </summary>
    public static string Concat(double a, string? b) => a.ToString() + (b ?? "");
    
    /// <summary>
    /// Concatenation between string and bool
    /// </summary>
    public static string Concat(string? a, bool b) => (a ?? "") + (b ? "true" : "false");
    
    /// <summary>
    /// Concatenation between bool and string
    /// </summary>
    public static string Concat(bool a, string? b) => (a ? "true" : "false") + (b ?? "");
    
    #endregion
    
    #region Logical Operators (for future use)
    
    /// <summary>
    /// Logical AND (&&) operator
    /// </summary>
    public static bool And(bool a, bool b) => a && b;
    
    /// <summary>
    /// Logical OR (||) operator
    /// </summary>
    public static bool Or(bool a, bool b) => a || b;
    
    /// <summary>
    /// Logical NOT (!) operator
    /// </summary>
    public static bool Not(bool a) => !a;
    
    #endregion
    
    #region Type Conversions
    
    /// <summary>
    /// Converts number to string
    /// </summary>
    public static string NumberToText(double value) => value.ToString();
    
    /// <summary>
    /// Converts bool to string
    /// </summary>
    public static string BoolToText(bool value) => value ? "true" : "false";
    
    /// <summary>
    /// Tries to convert string to number
    /// </summary>
    public static double TextToNumber(string? text)
    {
        if (double.TryParse(text, out var result))
            return result;
        throw new InvalidOperationException($"Could not convert '{text}' to number");
    }
    
    /// <summary>
    /// Tries to convert string to bool
    /// </summary>
    public static bool TextToBool(string? text)
    {
        return text?.ToLower() switch
        {
            "true" or "verdadeiro" or "sim" or "1" => true,
            "false" or "falso" or "não" or "nao" or "0" => false,
            _ => throw new InvalidOperationException($"Could not convert '{text}' to bool")
        };
    }
    
    #endregion
    
    #region Utilities
    
    /// <summary>
    /// Checks if a value is "truthy" in the Chest language
    /// (similar to JavaScript: 0, "", null, false are falsy)
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
    /// Converts any value to string for debugging
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
    
    #region Dynamic Operations for Object
    
    /// <summary>
    /// Dynamic addition that handles objects and performs automatic conversions
    /// </summary>
    public static object AddDynamic(object? left, object? right)
    {
        try
        {
            // Protection against null
            left ??= 0;
            right ??= 0;
            
            // Special case: both are strings or one is string
            if (left is string || right is string)
            {
                return (left?.ToString() ?? "") + (right?.ToString() ?? "");
            }
            
            // Try as numbers
            var leftNum = Convert.ToDouble(left);
            var rightNum = Convert.ToDouble(right);
            
            return leftNum + rightNum;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error in addition: left={left}, right={right}, error={ex.Message}");
        }
    }
    
    /// <summary>
    /// Dynamic subtraction
    /// </summary>
    public static double SubDynamic(object? left, object? right)
    {
        try
        {
            var leftNum = Convert.ToDouble(left ?? 0);
            var rightNum = Convert.ToDouble(right ?? 0);
            return leftNum - rightNum;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error in subtraction: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Dynamic multiplication
    /// </summary>
    public static double MulDynamic(object? left, object? right)
    {
        try
        {
            var leftNum = Convert.ToDouble(left ?? 0);
            var rightNum = Convert.ToDouble(right ?? 0);
            var result = leftNum * rightNum;
            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error in multiplication: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Dynamic division
    /// </summary>
    public static double DivDynamic(object? left, object? right)
    {
        try
        {
            var leftNum = Convert.ToDouble(left ?? 0);
            var rightNum = Convert.ToDouble(right ?? 1); // Avoid division by zero
            return leftNum / rightNum;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error in division: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Dynamic less than comparison
    /// </summary>
    public static bool LtDynamic(object? left, object? right)
    {
        try
        {
            var leftNum = Convert.ToDouble(left ?? 0);
            var rightNum = Convert.ToDouble(right ?? 0);
            return leftNum < rightNum;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error in comparison <: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Dynamic greater than comparison
    /// </summary>
    public static bool GtDynamic(object? left, object? right)
    {
        try
        {
            var leftNum = Convert.ToDouble(left ?? 0);
            var rightNum = Convert.ToDouble(right ?? 0);
            return leftNum > rightNum;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error in comparison >: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Dynamic less than or equal comparison
    /// </summary>
    public static bool LeDynamic(object? left, object? right)
    {
        try
        {
            var leftNum = Convert.ToDouble(left ?? 0);
            var rightNum = Convert.ToDouble(right ?? 0);
            return leftNum <= rightNum;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error in comparison <=: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Dynamic greater than or equal comparison
    /// </summary>
    public static bool GeDynamic(object? left, object? right)
    {
        try
        {
            var leftNum = Convert.ToDouble(left ?? 0);
            var rightNum = Convert.ToDouble(right ?? 0);
            return leftNum >= rightNum;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error in comparison >=: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Tries to extract a number from an object
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
