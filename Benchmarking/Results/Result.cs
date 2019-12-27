#region using

using Newtonsoft.Json;

#endregion

namespace Benchmarking
{
	public class Result
	{
		public Result(string benchmark, double timing, double points, double referenceTiming, double referencePoints, double dataThroughput)
		{
			Benchmark = benchmark;
			Timing = timing;
			Points = points;
			ReferenceTiming = referenceTiming;
			ReferencePoints = referencePoints;
			DataThroughput = dataThroughput;
		}

		public Result()
		{
			// Stupid JSON
		}

		public string Benchmark { get; set; }
		public double Points { get; set; }

		[JsonIgnore] public double ReferencePoints { get; set; }

		[JsonIgnore] public double ReferenceTiming { get; set; }

		[JsonIgnore] public double DataThroughput { get; set; }

		public double Timing { get; set; }
	}
}