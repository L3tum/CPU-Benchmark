using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Threading;
using Benchmarking.Results;
using Benchmarking.Util;
using Microsoft.Extensions.Logging;

namespace Benchmarking
{
    public class Runner
    {
        private readonly Lazy<List<Benchmark>> benchmarks = new(GetBenchmarks);
        private readonly FrequencyMeasurer? frequencyMeasurer;
        private readonly ILogger logger;
        private readonly Options options;

        public Runner(Options options, ILogger logger)
        {
            this.options = options;
            this.logger = logger;
            CurrentBenchmark = "";
            Results = new List<Result>().AsReadOnly();

            if (options.EnableFrequencyMeasurements)
            {
                frequencyMeasurer = new FrequencyMeasurer();
            }
        }

        private List<Benchmark> Benchmarks => benchmarks.Value;

        public string CurrentBenchmark { get; private set; }
        public IReadOnlyList<Result> Results { get; private set; }

        private static List<Benchmark> GetBenchmarks()
        {
            var marks = new List<Benchmark>();

            var benchmarkTypes = Assembly.GetAssembly(typeof(Runner))!.GetTypes()
                .Where(type => type.InheritsFrom(typeof(Benchmark)) && type.GetMethod("Run")?.DeclaringType == type)
                .ToArray();

            foreach (var benchmarkType in benchmarkTypes)
            {
                var benchmark = (Benchmark) Activator.CreateInstance(benchmarkType)!;

                if (marks.Any(bench => bench.GetName() == benchmark.GetName()))
                {
                    throw new DuplicateNameException(benchmark.GetName());
                }

                marks.Add(benchmark);
            }

            return marks;
        }

        public List<string> GetListOfBenchmarksAndCategories()
        {
            var hashSet = new HashSet<string>();

            foreach (var benchmark in Benchmarks)
            {
                hashSet.Add(benchmark.GetName());
                hashSet.UnionWith(benchmark.GetCategories().Select(category => category.ToUpperInvariant()));
            }

            var list = hashSet.ToList();

            list.Sort((a, b) =>
            {
                if (a == "ALL")
                {
                    return -1;
                }

                if (b == "ALL")
                {
                    return 1;
                }

                if (char.IsUpper(a, 0) && !char.IsUpper(b, 0))
                {
                    return -1;
                }

                if (char.IsUpper(a, 0) && char.IsUpper(b, 0))
                {
                    return 0;
                }

                return 1;
            });

            return list;
        }

        public List<Benchmark> GetBenchmarksToRun()
        {
            var benchmarksToRun = Benchmarks.Where(bench =>
                string.Equals(bench.GetName(), options.Benchmark, StringComparison.InvariantCultureIgnoreCase)
                ||
                bench.GetCategories().Any(category =>
                    string.Equals(category, options.Benchmark, StringComparison.InvariantCultureIgnoreCase))).ToList();

            return benchmarksToRun;
        }

        public double GetTotalTime()
        {
            var benchmarksToRun = GetBenchmarksToRun();
            var totalTime = benchmarksToRun.Select(benchmark => benchmark.GetRuntimeInMilliseconds()).Sum();

            if (options.BenchmarkingMode == Options.Mode.BOTH)
            {
                totalTime *= 2;
            }

            totalTime += benchmarksToRun.Count * options.WarmupTime;

            // 3000 are approx. for the PerformanceCounter
            return totalTime * options.Runs + 3000;
        }

        public void RunBenchmarks()
        {
            var results = new List<Result>();
            var benchmarksToRun = GetBenchmarksToRun();

            if (benchmarksToRun.Count == 0)
            {
                logger.LogError("No benchmarks with category or name {0} found", options.Benchmark);
                return;
            }

            logger.LogInformation("Running benchmarks {0}",
                string.Join(", ", benchmarksToRun.Select(bench => bench.GetName())));
            logger.LogInformation("Run will take approx. {0}", Helper.FormatTime(GetTotalTime()));

            // Check measurements thread is ready
            while (frequencyMeasurer != null && !frequencyMeasurer.MeasuringThreadReady)
            {
                Thread.Sleep(10);
            }

            if (options.BenchmarkingMode == Options.Mode.BOTH)
            {
                options.Threads = 1;
                results.AddRange(RunBenchmarksToRun(benchmarksToRun));

                options.Threads = Environment.ProcessorCount;
                results.AddRange(RunBenchmarksToRun(benchmarksToRun));
            }
            else
            {
                options.Threads = options.BenchmarkingMode == Options.Mode.SINGLE_THREADED
                    ? 1
                    : Environment.ProcessorCount;

                results.AddRange(RunBenchmarksToRun(benchmarksToRun));
            }

            Results = results.AsReadOnly();
            logger.LogInformation("Finished running benchmarks");
            logger.LogInformation($"Results: {string.Join("; ", results.Select(result => $"{result.Benchmark}: {result.Iterations}"))}");
        }

        private List<Result> RunBenchmarksToRun(IEnumerable<Benchmark> benchmarksToRun)
        {
            var results = new List<Result>();

            foreach (var benchmark in benchmarksToRun)
            {
                // Status
                CurrentBenchmark = benchmark.GetName();
                
                if (options.WarmupTime > 0)
                {
                    RunBenchmark(benchmark, options.WarmupTime, false);
                }
                
                frequencyMeasurer?.ClearMeasurements();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                results.Add(RunBenchmark(benchmark));
            }

            return results;
        }

        private Result RunBenchmark(Benchmark baseBenchmark, int? overwriteRuntime = null, bool log = true)
        {
            // Result
            var result = new Result
            {
                Benchmark = baseBenchmark.GetName(),
                Categories = baseBenchmark.GetCategories().ToList(),
                MultiThreaded = options.Threads > 1
            };

            // Benchmark running

            #region Running

            using var ctts = new CancellationTokenSource();
            var initializedBenchmarks = 0;
            var threads = new List<Thread>();
            var iterations = new List<ulong>();
            var runtime = overwriteRuntime ?? baseBenchmark.GetRuntimeInMilliseconds();
            var benchmarkType = baseBenchmark.GetType();
            var oldMode = GCSettings.LatencyMode;
            try
            {
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

                // Generation 2 garbage collection is now
                // deferred, except in extremely low-memory situations
                for (var i = 0; i < options.Threads; i++)
                {
                    var thread = new Thread(() =>
                    {
                        Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                        var benchmark = (Benchmark) Activator.CreateInstance(benchmarkType)! ??
                                        throw new Exception();

                        benchmark.Initialize();
                        Interlocked.Increment(ref initializedBenchmarks);
                        while (initializedBenchmarks != options.Threads)
                        {
                            Thread.Sleep(10);
                        }

                        var its = benchmark.Run(ctts.Token) / (ulong) (runtime / 1000);

                        lock (iterations)
                        {
                            iterations.Add(its);
                        }

                        Thread.CurrentThread.Priority = ThreadPriority.Normal;
                    });
                    thread.Start();
                    threads.Add(thread);
                }

                var sw = new Stopwatch();
                while (initializedBenchmarks != options.Threads)
                {
                    Thread.Sleep(10);
                }

                frequencyMeasurer?.ClearMeasurements();

                if (log)
                {
                    logger.LogInformation($"Initialized {baseBenchmark.GetName()}. Running now...");
                }
                sw.Start();
                ctts.CancelAfter(runtime);
                while (sw.ElapsedMilliseconds < runtime)
                {
                    Thread.Sleep(10);
                }

                sw.Stop();
                ctts.Cancel();
                foreach (var thread in threads)
                {
                    thread.Join();
                }
            }
            finally
            {
                // ALWAYS set the latency mode back
                GCSettings.LatencyMode = oldMode;
            }

            #endregion

            // Result reporting

            #region Results

            var totalIterations = (ulong) iterations.Select(kvp => (double) kvp).Sum();
            result.Iterations = totalIterations;
            result.ReferenceIterations = baseBenchmark.GetComparison(options);
            result.DataThroughput = baseBenchmark.GetDataThroughput(totalIterations);
            result.Frequency = frequencyMeasurer?.GetMeasurements();

            #endregion

            // Clearing
            frequencyMeasurer?.ClearMeasurements();

            return result;
        }
    }
}