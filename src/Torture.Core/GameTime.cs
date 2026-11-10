namespace Torture.Core;

/// <summary>
/// Holds the time state of the application loop.
/// </summary>
public readonly record struct GameTime(TimeSpan TotalGameTime, TimeSpan ElapsedGameTime)
{
    /// <summary>
    /// Elapsed time since the previous frame in seconds.
    /// </summary>
    public float DeltaSeconds => (float)ElapsedGameTime.TotalSeconds;
    /// <summary>
    /// Total time elapsed since the application started in seconds.
    /// </summary>
    public float TotalSeconds => (float)TotalGameTime.TotalSeconds;
}
