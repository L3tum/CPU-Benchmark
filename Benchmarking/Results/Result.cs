#region using

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Benchmarking.Results
{
	/// <summary>
	///     Singular Benchmark result
	/// </summary>
	public class Result
    {
	    /// <summary>
	    ///     Constructor
	    /// </summary>
	    /// <param name="benchmark"></param>
	    /// <param name="categories"></param>
	    /// <param name="iterations"></param>
	    /// <param name="referenceIterations"></param>
	    /// <param name="dataThroughput"></param>
	    /// <param name="multiThreaded"></param>
	    public Result(string benchmark, IEnumerable<string> categories, ulong iterations, ulong referenceIterations, double dataThroughput,
            bool multiThreaded)
        {
            Benchmark = benchmark;
            Categories = categories as List<string> ?? categories.ToList();
            Iterations = iterations;
            ReferenceIterations = referenceIterations;
            DataThroughput = dataThroughput;
            MultiThreaded = multiThreaded;
        }

	    /// <summary>
	    ///     JSON Constructor
	    /// </summary>
	    public Result()
        {
            // Stupid JSON
        }

	    /// <summary>
	    ///     The benchmark that was run
	    /// </summary>
	    public string Benchmark { get; set; } = null!;

	    /// <summary>
	    ///     The categories this benchmark belongs to
	    /// </summary>
	    public List<string> Categories { get; set; } = null!;

	    /// <summary>
	    ///     Time spent on the benchmark by the reference CPU, in ms
	    /// </summary>
	    public ulong ReferenceIterations { get; set; }

	    /// <summary>
	    ///     Approximate data throughput achieved
	    /// </summary>
	    public double DataThroughput { get; set; }

	    /// <summary>
	    ///     Time spent on the benchmark, in ms
	    /// </summary>
	    public ulong Iterations { get; set; }

	    /// <summary>
	    ///     The frequencies achieved during the run
	    /// </summary>
	    public FrequencyMeasurement? Frequency { get; set; } = null!;


	    /// <summary>
	    ///     If the result was for multiThreaded or singleThreaded
	    /// </summary>
	    public bool MultiThreaded { get; set; }

        public sealed class FrequencyMeasurement
        {
            public int BaseFrequency { get; set; }
            public int AverageFrequency { get; set; }
            public int HighestFrequency { get; set; }
            public int LowestFrequency { get; set; }
        }
    }
}