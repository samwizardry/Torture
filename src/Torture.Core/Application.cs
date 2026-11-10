using System.Diagnostics;

using SDL3;

using Torture.Core.Graphics;
using Torture.Core.Input;

using Vortice.Mathematics;

namespace Torture.Core;

/// <summary>
/// Base application class that manages the window lifecycle, main loop, and graphics device.
/// </summary>
public abstract class Application<TApp> : SDL.IMainCallbacks<TApp>
    where TApp : Application<TApp>, new()
{
    private readonly Stopwatch _timer = new();
    private TimeSpan _lastFrameTime;
    private bool _isExiting;
    private bool _isMinimized;
    private string _title = "Torture";
    private WindowMode _windowMode = WindowMode.Windowed;

    protected nint Window { get; private set; } = IntPtr.Zero;
    public IGraphicsDevice GraphicsDevice { get; private set; } = null!;

    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            if (Window != IntPtr.Zero)
            {
                SDL.SetWindowTitle(Window, _title);
            }
        }
    }

    /// <summary>
    /// Gets or sets the window display mode (Windowed, BorderlessFullscreen, or Fullscreen).
    /// Can be pre-configured in the constructor or changed at runtime.
    /// </summary>
    public WindowMode WindowMode
    {
        get => _windowMode;
        set
        {
            if (_windowMode == value)
            {
                return;
            }

            _windowMode = value;
            if (Window != IntPtr.Zero)
            {
                ApplyWindowMode(_windowMode);
                GraphicsDevice?.OnWindowModeChanged(_windowMode);
                OnWindowModeChanged(_windowMode);
            }
        }
    }

    public string Version { get; protected init; } = "0.1";
    public string Identifier { get; protected init; } = "semisoft.torture.app";
    public SizeI PreferredWindowSize { get; protected init; } = new(800, 600);
    public bool IsResizable { get; protected init; } = true;
    public GraphicsBackend PreferredBackend { get; protected init; } = GraphicsBackend.Default;

    /// <summary>
    /// Signals the application to exit at the end of the current frame.
    /// </summary>
    public void Exit() => _isExiting = true;

    /// <summary>
    /// Called after the window and graphics device have been created. Calls <see cref="LoadContent"/> by default.
    /// </summary>
    protected virtual void Initialize()
    {
        LoadContent();
    }

    /// <summary>
    /// Called when graphics and application resources should be loaded.
    /// </summary>
    protected virtual void LoadContent() { }

    /// <summary>
    /// Called once right before the main loop starts running.
    /// </summary>
    protected virtual void BeginRun() { }

    /// <summary>
    /// Called every frame to update application logic.
    /// </summary>
    protected virtual void Update(GameTime gameTime) { }

    /// <summary>
    /// Called before rendering a frame. Returns false to skip rendering the current frame.
    /// </summary>
    protected virtual bool BeginDraw()
    {
        if (_isMinimized)
        {
            return false;
        }

        return GraphicsDevice.BeginFrame();
    }

    /// <summary>
    /// Called every frame to render the scene.
    /// </summary>
    protected virtual void Draw(GameTime gameTime) { }

    /// <summary>
    /// Called after rendering a frame has completed.
    /// </summary>
    protected virtual bool EndDraw()
    {
        return GraphicsDevice.EndFrame();
    }

    /// <summary>
    /// Called when the window pixel size changes.
    /// </summary>
    protected virtual void OnResize(int width, int height) { }
    /// <summary>
    /// Called when the window position on the desktop changes.
    /// </summary>
    protected virtual void OnWindowMoved(RectI bounds) { }
    /// <summary>
    /// Called when the window display mode changes (Windowed, BorderlessFullscreen, or Fullscreen).
    /// </summary>
    protected virtual void OnWindowModeChanged(WindowMode mode) { }
    /// <summary>
    /// Called when the window is minimized to the taskbar.
    /// </summary>
    protected virtual void OnMinimized() { }
    /// <summary>
    /// Called when the window is restored from a minimized state.
    /// </summary>
    protected virtual void OnRestored() { }

    /// <summary>
    /// Called when the main loop is stopping before content is unloaded.
    /// </summary>
    protected virtual void EndRun() { }

    /// <summary>
    /// Called when graphics and application resources should be released prior to device disposal.
    /// </summary>
    protected virtual void UnloadContent() { }

    /// <summary>
    /// Called during application shutdown for final cleanup.
    /// </summary>
    protected virtual void Cleanup() { }

    /// <summary>
    /// Creates the platform-specific graphics device instance.
    /// </summary>
    protected abstract IGraphicsDevice CreateGraphicsDevice(GraphicsBackend backend, NativeWindowInfo window);

    public static SDL.AppResult AppInit(out TApp? appState, string[] args)
    {
        appState = new TApp();
        return appState.InternalInit();
    }

    private SDL.AppResult InternalInit()
    {
        SDL.SetAppMetadata(Title, Version, Identifier);

        if (!SDL.Init(SDL.InitFlags.Video))
        {
            SDL.LogError(SDL.LogCategory.Application, $"Couldn't initialize SDL: {SDL.GetError()}");
            return SDL.AppResult.Failure;
        }

        SDL.WindowFlags windowFlags = BuildInitialWindowFlags();

        Window = SDL.CreateWindow(Title, PreferredWindowSize.Width, PreferredWindowSize.Height, windowFlags);
        if (Window == IntPtr.Zero)
        {
            SDL.LogError(SDL.LogCategory.Application, $"Couldn't create window: {SDL.GetError()}");
            return SDL.AppResult.Failure;
        }

        GraphicsDevice = CreateGraphicsDevice(PreferredBackend, GetNativeWindowInfo(Window));

        // Initialize systems
        Keyboard.Initialize();
        Mouse.Initialize(Window);

        Initialize();
        BeginRun();

        _timer.Start();

        return SDL.AppResult.Continue;
    }

    public SDL.AppResult AppIterate()
    {
        if (_isExiting)
        {
            return SDL.AppResult.Success;
        }

        TimeSpan totalTime = _timer.Elapsed;
        TimeSpan elapsedTime = totalTime - _lastFrameTime;
        _lastFrameTime = totalTime;
        var gameTime = new GameTime(totalTime, elapsedTime);

        // Update systems
        Keyboard.Update();
        Mouse.Update();

        Update(gameTime);

        if (_isExiting)
        {
            return SDL.AppResult.Success;
        }

        if (BeginDraw())
        {
            Draw(gameTime);
            EndDraw();
        }

        return SDL.AppResult.Continue;
    }

    public SDL.AppResult AppEvent(ref SDL.Event e)
    {
        switch ((SDL.EventType)e.Type)
        {
            case SDL.EventType.Quit:
                return SDL.AppResult.Success;

            case SDL.EventType.WindowMinimized:
                _isMinimized = true;
                GraphicsDevice?.OnMinimized();
                OnMinimized();
                break;

            case SDL.EventType.WindowRestored:
                _isMinimized = false;
                GraphicsDevice?.OnRestored();
                OnRestored();
                break;

            case SDL.EventType.WindowPixelSizeChanged:
                int w = e.Window.Data1;
                int h = e.Window.Data2;
                if (w > 0 && h > 0)
                {
                    GraphicsDevice?.OnResize(w, h);
                    OnResize(w, h);
                }
                break;

            case SDL.EventType.WindowMoved:
                int x = e.Window.Data1;
                int y = e.Window.Data2;
                SizeI currentSize = GraphicsDevice?.Size ?? PreferredWindowSize;
                var bounds = new RectI(x, y, currentSize.Width, currentSize.Height);
                GraphicsDevice?.OnWindowMoved(bounds);
                OnWindowMoved(bounds);
                break;

            case SDL.EventType.WindowEnterFullscreen:
                if (_windowMode == WindowMode.Windowed)
                {
                    _windowMode = WindowMode.BorderlessFullscreen;
                    GraphicsDevice?.OnWindowModeChanged(_windowMode);
                    OnWindowModeChanged(_windowMode);
                }
                break;

            case SDL.EventType.WindowLeaveFullscreen:
                if (_windowMode != WindowMode.Windowed)
                {
                    _windowMode = WindowMode.Windowed;
                    GraphicsDevice?.OnWindowModeChanged(_windowMode);
                    OnWindowModeChanged(_windowMode);
                }
                break;

            case SDL.EventType.MouseWheel:
                Mouse.OnScrollWheel(e.Wheel.X, e.Wheel.Y);
                break;
        }

        return SDL.AppResult.Continue;
    }

    public void AppQuit(SDL.AppResult result)
    {
        GraphicsDevice?.WaitIdle();

        EndRun();
        UnloadContent();
        Cleanup();

        // Shutdown systems
        Mouse.Shutdown();
        Keyboard.Shutdown();

        GraphicsDevice?.Dispose();
        GraphicsDevice = null!;

        if (Window != IntPtr.Zero)
        {
            SDL.DestroyWindow(Window);
            Window = IntPtr.Zero;
        }

        SDL.Quit();
    }

    private SDL.WindowFlags BuildInitialWindowFlags()
    {
        SDL.WindowFlags flags = 0;

        if (IsResizable)
        {
            flags |= SDL.WindowFlags.Resizable;
        }

        switch (_windowMode)
        {
            case WindowMode.BorderlessFullscreen:
                flags |= SDL.WindowFlags.Borderless | SDL.WindowFlags.Fullscreen;
                break;

            case WindowMode.Fullscreen:
                flags |= SDL.WindowFlags.Fullscreen;
                break;
        }

        return flags;
    }

    private void ApplyWindowMode(WindowMode mode)
    {
        switch (mode)
        {
            case WindowMode.Windowed:
                SDL.SetWindowFullscreen(Window, false);
                SDL.SetWindowBordered(Window, true);
                SDL.SetWindowResizable(Window, IsResizable);
                SDL.SetWindowSize(Window, PreferredWindowSize.Width, PreferredWindowSize.Height);
                break;

            case WindowMode.BorderlessFullscreen:
                SDL.SetWindowBordered(Window, false);
                SDL.SetWindowFullscreen(Window, true);
                break;

            case WindowMode.Fullscreen:
                SDL.SetWindowFullscreen(Window, true);
                break;
        }
    }

    protected NativeWindowInfo GetNativeWindowInfo(nint window)
    {
        uint props = SDL.GetWindowProperties(window);
        if (props == 0)
        {
            throw new InvalidOperationException("Couldn't get window properties.");
        }

        if (!SDL.GetWindowSizeInPixels(window, out int w, out int h))
        {
            throw new InvalidOperationException("Couldn't get window size in pixels.");
        }

        if (!SDL.GetWindowPosition(window, out int x, out int y))
        {
            throw new InvalidOperationException("Couldn't get window position.");
        }

        var size = new SizeI(w, h);
        var bounds = new RectI(x, y, w, h);

#if PLATFORM_WINDOWS
        nint hwnd = SDL.GetPointerProperty(props, SDL.Props.WindowWin32HWNDPointer, IntPtr.Zero);
        return hwnd == IntPtr.Zero
            ? throw new InvalidOperationException("Couldn't get Win32 HWND.")
            : new NativeWindowInfo(window, hwnd, size, bounds);
#else
        return new NativeWindowInfo(window, IntPtr.Zero, size, bounds);
#endif
    }
}