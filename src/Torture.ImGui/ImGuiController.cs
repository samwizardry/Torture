using ImGuiNET;

using Torture.Core.Graphics;

namespace Torture.ImGui;

//public class ImGuiController : IDisposable
//{
//    private readonly IGraphicsDevice _graphicsDevice;
//    private int _width;
//    private int _height;

//    public ImGuiController(IGraphicsDevice graphicsDevice, int width, int height)
//    {
//        _graphicsDevice = graphicsDevice;
//        _width = width;
//        _height = height;

//        ImGuiNET.ImGui.CreateContext();
//        var io = ImGuiNET.ImGui.GetIO();
//        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

//        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard | ImGuiConfigFlags.DockingEnable;
//        io.Fonts.Flags |= ImFontAtlasFlags.NoBakedLines;
//    }

//    public void Dispose()
//    {
//        throw new NotImplementedException();
//    }
//}
