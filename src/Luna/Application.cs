using System.Diagnostics;

using SDL3;

using Vortice.Mathematics;

namespace Luna;

[SDL.GenerateMain]
internal sealed partial class Application : SDL.IMainCallbacks<Application>
{
    private nint _window = IntPtr.Zero;
    private D3D11GraphicsDevice? _graphicsDevice;

    private readonly Stopwatch _fpsTimer = new();
    private int _frameCount;

    public static SDL.AppResult AppInit(out Application? app, string[] args)
    {
        app = new Application();
        SDL.SetAppMetadata("Luna", "0.1", "Semisoft.Luna");

        if (!SDL.Init(SDL.InitFlags.Video))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Couldn't initialize SDL: {SDL.GetError()}");
            return SDL.AppResult.Failure;
        }

        // Проверяем именно D3D11, а не D3D12 (и до создания окна)
        //if (!D3D11GraphicsDevice.IsSupported)
        //{
        //    SDL.LogError(SDL.LogCategory.Application, "DirectX 11 is not supported.");
        //    return SDL.AppResult.Failure;
        //}

        app._window = SDL.CreateWindow("Luna", 800, 600, SDL.WindowFlags.Resizable);
        if (app._window == IntPtr.Zero)
        {
            SDL.LogError(SDL.LogCategory.Application, $"Couldn't create window: {SDL.GetError()}");
            return SDL.AppResult.Failure;
        }

        var props = SDL.GetWindowProperties(app._window);
        nint hwnd = SDL.GetPointerProperty(props, SDL.Props.WindowWin32HWNDPointer, IntPtr.Zero);
        if (hwnd == IntPtr.Zero)
        {
            SDL.LogError(SDL.LogCategory.Application, "Couldn't get Win32 HWND from SDL window.");
            return SDL.AppResult.Failure;
        }

        SDL.GetWindowSizeInPixels(app._window, out int width, out int height);
        SDL.GetWindowPosition(app._window, out int x, out int y);

        var size = new SizeI(width, height);
        var bounds = new RectI(x, y, width, height);

        app._graphicsDevice = new D3D11GraphicsDevice(hwnd, size, bounds);
        //app._graphicsDevice = new D3D12GraphicsDevice(true, hwnd, size);
        app._fpsTimer.Start();

        return SDL.AppResult.Continue;
    }

    public SDL.AppResult AppIterate()
    {
        _graphicsDevice?.DrawFrame(OnDraw);

        // Считаем реальный FPS раз в секунду без спама в консоль на каждый кадр
        _frameCount++;
        double elapsed = _fpsTimer.Elapsed.TotalSeconds;
        if (elapsed >= 1.0)
        {
            double fps = _frameCount / elapsed;
            SDL.SetWindowTitle(_window, $"Luna | FPS: {fps:F1}");
            _frameCount = 0;
            _fpsTimer.Restart();
        }

        return SDL.AppResult.Continue;
    }

    public SDL.AppResult AppEvent(ref SDL.Event @event)
    {
        switch ((SDL.EventType)@event.Type)
        {
            case SDL.EventType.Quit:
                return SDL.AppResult.Success;

            case SDL.EventType.WindowPixelSizeChanged:
                // Если в вашем D3D11GraphicsDevice есть метод изменения размера:
                // _graphicsDevice?.Resize(@event.Window.Data1, @event.Window.Data2);
                break;
        }

        return SDL.AppResult.Continue;
    }

    public void AppQuit(SDL.AppResult result)
    {
        _graphicsDevice?.Dispose();
        _graphicsDevice = null;

        if (_window != IntPtr.Zero)
        {
            SDL.DestroyWindow(_window);
            _window = IntPtr.Zero;
        }

        SDL.Quit();
    }

    private static void OnDraw(int width, int height)
    {
    }
}
