using CommandLine;

namespace Benchmarking
{
	public class Options
	{
		[Option('t', "threads", Required = false, Default = 0, HelpText = "Manually set the number of threads to use (overrides multithreaded)")]
		public int Threads { get; set; } = 1;

		[Option('r', "runs", Required = false, HelpText = "Times to run the benchmark and average the result")]
		public int Runs { get; set; } = 3;

		[Option('m', "multithreaded", Required = false, HelpText = "Run benchmarks multithreaded (automatically uses all logical processors)")]
		public bool Multithreaded { get; set; } = false;

		[Option('b', "benchmark", Required = true, HelpText = "Choose the benchmark to run")]
		public string Benchmark { get; set; }
	}
}