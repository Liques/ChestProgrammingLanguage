using System;

namespace Chest.Compiler.Core;

/// <summary>
/// Type reference
/// </summary>
public struct TypeRef 
{
    public string Name { get; set; }
    
    public TypeRef(string name)
    {
        Name = name;
    }
    
    public static implicit operator TypeRef(string name) => new(name);
}

/// <summary>
/// Source code position for error messages
/// </summary>
public struct SourceSpan 
{
    public int Line { get; set; }
    public int Col { get; set; }
    public int EndLine { get; set; }
    public int EndCol { get; set; }
    
    public SourceSpan(int line, int col, int endLine, int endCol)
    {
        Line = line;
        Col = col;
        EndLine = endLine;
        EndCol = endCol;
    }
    
    public override string ToString() => $"({Line},{Col})-({EndLine},{EndCol})";
}

/// <summary>
/// Method parameter
/// </summary>
public class Parameter 
{
    public string Name { get; set; } = "";
    public TypeRef TypeHint { get; set; }
}

/// <summary>
/// Primitive types of the Chest language
/// </summary>
public enum ChestType 
{
    Number,  // double
    Text,    // string  
    Bool     // boolean
}

/// <summary>
/// Extensions to work with types
/// </summary>
public static class ChestTypeExtensions
{
    public static Type ToSystemType(this ChestType chestType) => chestType switch
    {
        ChestType.Number => typeof(double),
        ChestType.Text => typeof(string),
        ChestType.Bool => typeof(bool),
        _ => throw new ArgumentException($"Unknown Chest type: {chestType}")
    };
    
    public static ChestType FromSystemType(Type systemType)
    {
        if (systemType == typeof(double) || systemType == typeof(float) || 
            systemType == typeof(int) || systemType == typeof(long))
            return ChestType.Number;
        if (systemType == typeof(string))
            return ChestType.Text;
        if (systemType == typeof(bool))
            return ChestType.Bool;
        
        throw new ArgumentException($"Unsupported system type: {systemType}");
    }
}
