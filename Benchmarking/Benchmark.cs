namespace Benchmarking
{
	public abstract class Benchmark
	{
		protected Options options;

		protected Benchmark(Options options)
		{
			this.options = options;
		}

		public virtual string GetDescription()
		{
			return "Base Benchmark Class - You should never see this!";
		}

		public virtual void Initialize()
		{
		}

		public virtual void Run()
		{
		}

		public virtual void Shutdown()
		{
		}

		public virtual double GetComparison()
		{
			return 0.0d;
		}

		/// <summary>
		///     Returns the data throughput achieved per second adjusted to the time the benchmark took, in bytes
		/// </summary>
		/// <param name="timeInMillis"></param>
		/// <returns></returns>
		public virtual double GetDataThroughput(double timeInMillis)
		{
			return 0.0d;
		}

		public virtual BenchmarkRater.RateMethod GetRatingMethod()
		{
			return BenchmarkRater.RateBenchmark;
		}

		public virtual string[] GetCategories()
		{
			return new[] {"none"};
		}

		public virtual string GetName()
		{
			return GetType().Name;
		}
	}
}