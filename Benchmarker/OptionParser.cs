#region using

using System;
using System.Linq;
using Benchmarking;
using Benchmarking.Results;
using Benchmarking.Util;

#endregion

namespace Benchmarker
{
	internal static class OptionParser
	{
		internal static bool ParseOptions(Options options)
		{
			if (options.ListBenchmarks)
			{
				Console.WriteLine(string.Join(Environment.NewLine, BenchmarkRunner.GetAvailableBenchmarks()));

				return false;
			}

			if (options.ListResults)
			{
				ResultSaver.Init(options);
				Console.WriteLine();
				Console.WriteLine(Util.FormatResults(ResultSaver.GetResults()));

				return false;
			}

			if (options.Upload)
			{
				if (!ResultSaver.Init(options))
				{
					Console.WriteLine(
						"A restart or hardware change has been detected. Please redo the benchmark and upload it without restarting.");

					return false;
				}

				var result = ResultSaver.IsAllowedToUpload(options);

				if (result == ErrorCode.NOT_WINDOWS)
				{
					Console.WriteLine("You can only upload your results on a Windows machine.");

					return false;
				}

				if (result == ErrorCode.NO_CATEGORY_ALL)
				{
					Console.WriteLine("You can only upload your results after having completed all benchmarks.");

					return false;
				}

				Console.WriteLine();
				Console.WriteLine("Uploading results...");

				var response = ResultSaver.UploadResults().Result;

				if (response != null)
				{
					Console.WriteLine("Done!");
					Console.WriteLine("You can view your raw save here: {0}", response.RawPath);
					Console.WriteLine("You can view your parsed save here: {0}", response.WebsitePath);

					Console.WriteLine(
						"Click 'a' to open raw, 'b' to open parsed or any other key to close the program!");

					while (true)
					{
						var key = Console.ReadKey(true);

						if (key.Key == ConsoleKey.A)
						{
							Helper.OpenBrowser(response.RawPath);
						}
						else if (key.Key == ConsoleKey.B)
						{
							Helper.OpenBrowser(response.WebsitePath);
						}
						else
						{
							return false;
						}
					}
				}

				Console.WriteLine("Failed uploading results!");

				return false;
			}

			if (options.Benchmark == null || string.IsNullOrWhiteSpace(options.Benchmark))
			{
				Console.WriteLine("Please specify a benchmark!");

				return false;
			}

			if (!BenchmarkRunner.GetAvailableBenchmarks()
				.Any(a => a.ToLowerInvariant().Equals(options.Benchmark.ToLowerInvariant())))
			{
				Console.WriteLine("Benchmark name not recognized!");

				return false;
			}

			return true;
		}
	}
}