using Torture.Core.Graphics;

using Vortice.Mathematics;

namespace Torture.Graphics.Vulkan;

public sealed class VulkanGraphicsDevice : IGraphicsDevice
{
    public SizeI Size => throw new NotImplementedException();

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool DrawFrame(Action<int, int> draw, string? frameName = null)
    {
        throw new NotImplementedException();
    }

    public void Resize(int width, int height)
    {
        throw new NotImplementedException();
    }
}
