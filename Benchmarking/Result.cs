namespace Benchmarking
{
	public class Result
	{
		public Result(string benchmark, double timing, double points, double referenceTiming, double referencePoints)
		{
			Benchmark = benchmark;
			Timing = timing;
			Points = points;
			ReferenceTiming = referenceTiming;
			ReferencePoints = referencePoints;
		}

		public string Benchmark { get; }
		public double Points { get; }
		public double ReferencePoints { get; }
		public double ReferenceTiming { get; }
		public double Timing { get; }
	}
}