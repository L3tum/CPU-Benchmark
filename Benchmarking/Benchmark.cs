using System.Threading;

namespace Benchmarking
{
    public abstract class Benchmark
    {
        protected const int LENGTH = int.MaxValue / 64;
        
        public virtual string GetDescription()
        {
            return "Base Benchmark Class - You should never see this!";
        }

        public virtual ulong Run(CancellationToken cancellationToken)
        {
            return 0uL;
        }

        public virtual void Initialize()
        {
        }

        public virtual ulong GetComparison(Options options)
        
        {
            return 0;
        }

        /// <summary>
        ///     Returns the data throughput achieved per second adjusted to the time the benchmark took, in bytes
        /// </summary>
        /// <returns></returns>
        public virtual double GetDataThroughput(ulong iterations)
        {
            return 0.0d;
        }

        public virtual string[] GetCategories()
        {
            return new[] {"none"};
        }

        public virtual string GetName()
        {
            return GetType().Name.ToLowerInvariant();
        }

        public virtual int GetRuntimeInMilliseconds()
        {
            return 5000;
        }
    }
}