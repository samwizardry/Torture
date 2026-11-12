namespace Torture.Core.Graphics;

public abstract class Texture : IDisposable
{
    protected bool _disposed;

    public abstract void Dispose();
}
