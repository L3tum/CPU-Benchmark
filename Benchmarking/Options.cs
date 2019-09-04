﻿using CommandLine;

namespace Benchmarking
{
	public class Options
	{
		[Option('t', "threads", Required = false, Default = 0, HelpText = "Manually set the number of threads to use (overrides multithreaded)")]
		public int Threads { get; set; } = 1;

		[Option('r', "runs", Default = 3, HelpText = "Times to run the benchmark and average the result")]
		public int Runs { get; set; } = 3;

		[Option('m', "multithreaded", Default = false, HelpText = "Run benchmarks multithreaded (automatically uses all logical processors)")]
		public bool Multithreaded { get; set; } = false;

		[Option('b', "benchmark", HelpText = "Choose the benchmark to run")]
		public string Benchmark { get; set; }

		[Option('l', "list-benchmarks", Default = false, HelpText = "List available benchmarks")]
		public bool ListBenchmarks { get; set; }
	}
}