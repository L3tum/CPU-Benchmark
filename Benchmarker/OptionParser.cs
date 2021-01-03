#region using

using System;
using System.Collections.Generic;
using Benchmarking;
using Benchmarking.Results;
using Microsoft.Extensions.Logging.Abstractions;

#endregion

namespace Benchmarker
{
    internal static class OptionParser
    {
        internal static Options? ParseOptions(Arguments arguments)
        {
            var options = new Options
            {
                Benchmark = arguments.Benchmark,
                Runs = arguments.Runs
            };

            if (arguments.MultiThreaded && (arguments.SingleThreaded || arguments.SingleMultiThreaded) ||
                arguments.SingleThreaded && arguments.SingleMultiThreaded)
            {
                Console.WriteLine("Only one mode can be specified");

                return null;
            }

            if (arguments.MultiThreaded)
            {
                options.BenchmarkingMode = Options.Mode.MULTI_THREADED;
            }
            else if (arguments.SingleThreaded)
            {
                options.BenchmarkingMode = Options.Mode.SINGLE_THREADED;
            }
            else
            {
                options.BenchmarkingMode = Options.Mode.BOTH;
            }

            if (arguments.ListBenchmarks)
            {
                Console.WriteLine(string.Join(Environment.NewLine,
                    new Runner(options, new NullLogger<Runner>()).GetListOfBenchmarksAndCategories()));

                return null;
            }

            if (arguments.ListResults)
            {
                var saver = new ResultSaver();

                foreach (var saveName in saver.GetListOfSaves())
                {
                    var save = saver.GetSave(saveName);

                    if (save is null)
                    {
                        continue;
                    }

                    Console.WriteLine();
                    Console.WriteLine(Util.FormatResults(new Dictionary<int, List<Result>>
                    {
                        {1, save.SingleThreadedResults},
                        {(int) save.MachineInformation!.Cpu.LogicalCores, save.MultiThreadedResults}
                    }));
                }

                return null;
            }

            if (options.Benchmark == null || string.IsNullOrWhiteSpace(options.Benchmark))
            {
                Console.WriteLine("Please specify a benchmark!");

                return null;
            }

            if (arguments.DisableProgressBar)
            {
                options.EnableProgressBar = false;
            }

            options.WarmupTime = (int) arguments.WarmupTime * 1000;

            return options;
        }
    }
}