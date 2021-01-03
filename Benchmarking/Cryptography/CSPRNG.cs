#region using

using System.Security.Cryptography;
using System.Threading;

#endregion

namespace Benchmarking.Cryptography
{
    internal class CSPRNG : Benchmark
    {
        private const int VOLUME = int.MaxValue / 64;

        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;
            var data = new byte[VOLUME];

            while (!cancellationToken.IsCancellationRequested)
            {
                var csrpng = RandomNumberGenerator.Create();
                csrpng.GetBytes(data);
                iterations++;
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "Generates cryptographically secure random data";
        }

        public override ulong GetComparison(Options options)
        {
            switch (options.Threads)
            {
                case 1:
                {
                    return 1157;
                }
                default:
                {
                    return 260;
                }
            }
        }

        public override string[] GetCategories()
        {
            return new[] {"cryptography", "int", "all"};
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(byte) * (double) (VOLUME * iterations);
        }
    }
}