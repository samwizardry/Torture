using System.Diagnostics;
using System.Runtime;

using SDL3;

namespace Torture.Core;

public static class GarbageCollection
{
    /// <summary>Полная блокирующая сборка с компактификацией. Только для экранов загрузки!</summary>
    public static void FullBlockingCollect()
    {
        long before = GC.GetTotalMemory(forceFullCollection: false);
        long start = Stopwatch.GetTimestamp();

        // LOH по умолчанию не компактится; CompactOnce сам сбросится после следующего полного GC.
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

        // 1-й проход: недостижимые объекты с финализаторами попадают в очередь финализации.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
        // Ждём, пока отработает поток финализаторов (NativeList, COM-обёртки и т.п.).
        GC.WaitForPendingFinalizers();
        // 2-й проход: собираем то, что освободилось после финализаторов.
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);

        LogStats(before, Stopwatch.GetElapsedTime(start));
    }

    /// <summary>Режим «во время геймплея»: без блокирующих gen2, если это возможно.</summary>
    public static void EnterGameplayMode()
        => GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

    /// <summary>Режим «загрузка»: GC может работать агрессивно.</summary>
    public static void EnterLoadingMode()
        => GCSettings.LatencyMode = GCLatencyMode.Interactive;

    [Conditional("DEBUG")]
    private static void LogStats(long before, TimeSpan elapsed)
    {
        long after = GC.GetTotalMemory(false);
        SDL.LogInfo(SDL.LogCategory.Application,
            $"[GC] {before / 1024} KB -> {after / 1024} KB за {elapsed.TotalMilliseconds:F2} ms; " +
            $"gen0={GC.CollectionCount(0)} gen1={GC.CollectionCount(1)} gen2={GC.CollectionCount(2)}");
    }
}
