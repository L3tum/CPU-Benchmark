#region using

#endregion

namespace Benchmarking
{
    public class Options
    {
        public enum Mode
        {
            SINGLE_THREADED,
            MULTI_THREADED,
            BOTH
        }

        public uint Runs { get; set; } = 1u;

        public string Benchmark { get; set; } = null!;

        public int Threads { get; internal set; }

        public Mode BenchmarkingMode { get; set; } = Mode.BOTH;

        public bool EnableProgressBar { get; set; } = true;

        public bool EnableFrequencyMeasurements { get; set; } = true;

        public int WarmupTime { get; set; } = 0;
    }
}