namespace Torture.Core.Graphics;

public abstract class Shader : IDisposable
{
    protected bool _disposed;

    public abstract void Dispose();
}
