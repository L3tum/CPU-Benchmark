using CommandLine;

namespace Benchmarker
{
    public class Arguments
    {
        [Option('r', "runs", Default = 1u, HelpText = "Times to run the benchmark and average the result")]
        public uint Runs { get; set; } = 1u;

        [Option('m', "multithreaded", Default = false,
            HelpText = "Run benchmarks multiThreaded (automatically uses all logical processors)")]
        public bool MultiThreaded { get; set; } = false;

        [Option('t', "singlethreaded", Default = false,
            HelpText = "Run benchmarks singlethreaded")]
        public bool SingleThreaded { get; set; } = false;

        [Option('x', "singleMultiThreaded", Default = false,
            HelpText = "Run benchmarks single- and multiThreaded")]
        public bool SingleMultiThreaded { get; set; } = false;

        [Option('b', "benchmark", HelpText = "Choose the benchmark to run", Default = "all")]
        public string Benchmark { get; set; } = null!;

        [Option('l', "list-benchmarks", Default = false, HelpText = "List available benchmarks")]
        public bool ListBenchmarks { get; set; }

        [Option('i', "list-results", Default = false, HelpText = "List all benchmark results so far")]
        public bool ListResults { get; set; }

        [Option('e', "memory-efficient", Default = false,
            HelpText = "Runs benchmarks in a memory efficient mode. May be slower.")]
        public bool MemoryEfficient { get; set; }

        [Option('u', "upload", Default = false, HelpText = "Uploads your results to the benchmark database.")]
        public bool Upload { get; set; }

        [Option('c', "clear", Default = false, HelpText = "Clears all saved data.")]
        public bool ClearSave { get; set; }

        [Option('s', "stress", Default = false,
            HelpText = "Starts a stress test with the selected benchmark. Press any key to stop it.")]
        public bool Stress { get; set; }
        
        [Option('p', "disable-progress-bar", Default = false,
            HelpText = "Disables the progress bar. May improve performance in multiThreaded benchmarks.")]
        public bool DisableProgressBar { get; set; }
        
        [Option('w', "warmup", Default = 0, HelpText = "Runs the benchmark in warmup for x seconds to pre-heat watercooling systems or so.")]
        public uint WarmupTime { get; set; }
    }
}