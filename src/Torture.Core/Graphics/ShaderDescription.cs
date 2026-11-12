namespace Torture.Core.Graphics;

public readonly record struct ShaderDescription
{
    public readonly ShaderType ShaderType;
    public readonly byte[] ShaderSource;
    public readonly string EntryPoint;
    public readonly bool Debug;
    public readonly string? DebugName;

    public ShaderDescription(ShaderType shaderType, byte[] shaderSource, string entryPoint, bool debug = false, string? debugName = null)
    {
        ShaderType = shaderType;
        ShaderSource = shaderSource;
        EntryPoint = entryPoint;
        Debug = debug;
        DebugName = debugName;
    }
}
