#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Benchmarking.Arithmetic;
using Benchmarking.Compression;
using Benchmarking.Cryptography;
using Benchmarking.Extension;

#endregion

namespace Benchmarking
{
	public class BenchmarkRunner
	{
		public static readonly string[] AvailableBenchmarks =
		{
			"ZIP", "GZIP", "BZIP2", "DEFLATE", "BROTLI", "ARITHMETIC_INT", "ARITHMETIC_FLOAT", "AVX", "SSE",
			"ENCRYPTION", "DECRYPTION", "CSRPNG",
			"ALL", "COMPRESSION", "ARITHMETIC", "EXTENSION", "CRYPTOGRAPHY",
			"INT", "FLOAT"
		};

		private static int finished;
		private static int total;
		private static readonly object _lock = new object();
		private readonly List<Benchmark> benchmarksToRun = new List<Benchmark>();
		private readonly Options options;
		public readonly List<Result> Results = new List<Result>();
		private readonly long[] timings;

		public BenchmarkRunner(Options options)
		{
			this.options = options;
			timings = new long[options.Runs];

			total = options.Runs * options.Threads;
		}

		public static double CurrentProgress { get; private set; }

		public void RunBenchmark()
		{
			switch (options.Benchmark.ToUpper())
			{
				case "ZIP":
				{
					benchmarksToRun.Add(new ZIP(options));

					break;
				}

				case "GZIP":
				{
					benchmarksToRun.Add(new GZip(options));

					break;
				}

				case "BZIP2":
				{
					benchmarksToRun.Add(new BZip2(options));

					break;
				}

				case "DEFLATE":
				{
					benchmarksToRun.Add(new Deflate(options));

					break;
				}

				case "BROTLI":
				{
					benchmarksToRun.Add(new Brotli(options));

					break;
				}

				case "ARITHMETIC_INT":
				{
					benchmarksToRun.Add(new Integer(options));

					break;
				}

				case "ARITHMETIC_FLOAT":
				{
					benchmarksToRun.Add(new Float(options));

					break;
				}

				case "AVX":
				{
					benchmarksToRun.Add(new AVX(options));

					break;
				}

				case "SSE":
				{
					benchmarksToRun.Add(new SSE(options));

					break;
				}

				case "ENCRYPTION":
				{
					benchmarksToRun.Add(new Encryption(options));

					break;
				}

				case "DECRYPTION":
				{
					benchmarksToRun.Add(new Decryption(options));

					break;
				}

				case "CSRPNG":
				{
					benchmarksToRun.Add(new CSRPNG(options));

					break;
				}

				case "COMPRESSION":
				{
					benchmarksToRun.Add(new ZIP(options));
					benchmarksToRun.Add(new GZip(options));
					benchmarksToRun.Add(new BZip2(options));
					benchmarksToRun.Add(new Deflate(options));
					benchmarksToRun.Add(new Brotli(options));

					break;
				}

				case "ARITHMETIC":
				{
					benchmarksToRun.Add(new Integer(options));
					benchmarksToRun.Add(new Float(options));

					break;
				}

				case "EXTENSION":
				{
					benchmarksToRun.Add(new AVX(options));
					benchmarksToRun.Add(new SSE(options));

					break;
				}

				case "CRYPTOGRAPHY":
				{
					benchmarksToRun.Add(new Encryption(options));
					benchmarksToRun.Add(new Decryption(options));
					benchmarksToRun.Add(new CSRPNG(options));

					break;
				}

				case "INT":
				{
					benchmarksToRun.Add(new Integer(options));
					benchmarksToRun.Add(new Encryption(options));
					benchmarksToRun.Add(new Decryption(options));
					benchmarksToRun.Add(new CSRPNG(options));

					break;
				}

				case "FLOAT":
				{
					benchmarksToRun.Add(new Float(options));

					break;
				}

				case "ALL":
				{
					benchmarksToRun.Add(new ZIP(options));
					benchmarksToRun.Add(new GZip(options));
					benchmarksToRun.Add(new BZip2(options));
					benchmarksToRun.Add(new Deflate(options));
					benchmarksToRun.Add(new Brotli(options));
					benchmarksToRun.Add(new Integer(options));
					benchmarksToRun.Add(new Encryption(options));
					benchmarksToRun.Add(new Decryption(options));
					benchmarksToRun.Add(new CSRPNG(options));
					benchmarksToRun.Add(new Float(options));
					benchmarksToRun.Add(new AVX(options));
					benchmarksToRun.Add(new SSE(options));

					break;
				}

				default:
				{
					throw new ArgumentException($"Benchmark {options.Benchmark} not recognized!");
				}
			}

			RunGenericBenchmark();
		}

		internal static void ReportProgress()
		{
			lock (_lock)
			{
				finished++;
				CurrentProgress = (double) finished / total;
			}
		}

		private void RunGenericBenchmark()
		{
			while (benchmarksToRun.Count > 0)
			{
				benchmarksToRun[0].Initialize();
				var timing = ExecuteBenchmark();

				Results.Add(new Result(benchmarksToRun[0].GetDescription(), timing,
					BenchmarkRater.RateBenchmark(timing), benchmarksToRun[0].GetReferenceValue(),
					BenchmarkRater.RateBenchmark(benchmarksToRun[0].GetReferenceValue())));

				benchmarksToRun.RemoveAt(0);
				GC.Collect();
			}
		}

		private double ExecuteBenchmark()
		{
			var sw = new Stopwatch();

			for (var i = 0; i < options.Runs; i++)
			{
				sw.Start();
				benchmarksToRun[0].Run();
				sw.Stop();

				timings[i] = sw.ElapsedMilliseconds;
				sw.Reset();
			}

			return timings.Average();
		}
	}
}