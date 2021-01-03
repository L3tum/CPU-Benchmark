using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.SSE4.Long
{
    public class LzCount : BaseSse4
    {
        private ulong data;

        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Lzcnt.X64.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    data = Lzcnt.X64.LeadingZeroCount(data);
                }

                iterations++;
            }

            return iterations;
        }

        public override void Initialize()
        {
            var rand = new Random();

            data = ((ulong) rand.Next(int.MinValue, int.MaxValue) << 32) +
                   (ulong) rand.Next(int.MinValue, int.MaxValue);
        }

        public override string GetDescription()
        {
            return "SSE4.2 benchmark of lzcnt calculation on 64-bit ints";
        }

        public override ulong GetComparison(Options options)
        {
            switch (options.Threads)
            {
                case 1:
                {
                    return 1010;
                }
                default:
                {
                    return 200;
                }
            }
        }

        public override string GetName()
        {
            return "sse4_lzcnt_long";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "sse4", "lzcnt", "all"};
        }
        
        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(ulong) * (double) (LENGTH * iterations);
        }
    }
}