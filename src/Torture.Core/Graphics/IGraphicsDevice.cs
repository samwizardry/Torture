using Vortice.Mathematics;

namespace Torture.Core.Graphics;

public interface IGraphicsDevice : IDisposable
{
    SizeI Size { get; }

    void Resize(int width, int height);

    bool DrawFrame(Action<int, int> draw, string? frameName = null);
}
