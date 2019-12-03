#region using

using System;
using CommandLine;

#endregion

namespace Benchmarking
{
	public class Options
	{
		[Option('t', "threads", Required = false, Default = 0u,
			HelpText = "Manually set the number of threads to use (overrides multithreaded)")]
		public uint Threads { get; set; } = 1u;

		[Option('r', "runs", Default = 3u, HelpText = "Times to run the benchmark and average the result")]
		public uint Runs { get; set; } = 3u;

		[Option('m', "multithreaded", Default = false,
			HelpText = "Run benchmarks multithreaded (automatically uses all logical processors)")]
		public bool Multithreaded { get; set; } = false;

		[Option('b', "benchmark", HelpText = "Choose the benchmark to run")]
		public string Benchmark { get; set; }

		[Option('l', "list-benchmarks", Default = false, HelpText = "List available benchmarks")]
		public bool ListBenchmarks { get; set; }

		[Option('i', "list-results", Default = false, HelpText = "List all benchmark results so far")]
		public bool ListResults { get; set; }

		[Option('m', "memory-efficient", Default = false,
			HelpText = "Runs benchmarks in a memory efficient mode. May be slower.")]
		public bool MemoryEfficient { get; set; }

		[Obsolete]
		[Option('q', "quick", Default = true,
			HelpText =
				"[DEPRECATED] Skips a few additional checks and routines. Exempts you from uploading to the benchmark database.",
			Hidden = true)]
		public bool QuickRun { get; set; }

		[Option('u', "upload", Default = false, HelpText = "Uploads your results to the benchmark database.")]
		public bool Upload { get; set; }
	}
}