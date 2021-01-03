#region using

using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Benchmarking.Util;

#endregion

namespace Benchmarking.Cryptography
{
    internal class Hashing : Benchmark
    {
        // 1 Megabyte
        private const int VOLUME = 5000000;
        private string? data;

        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                using Stream s = new MemoryStream();
                using var stream = new CryptoStream(s, SHA256.Create(), CryptoStreamMode.Write);
                using var sw = new StreamWriter(stream);
                sw.Write(data);
                sw.Flush();
                stream.Flush();

                iterations++;
            }

            return iterations;
        }

        public override void Initialize()
        {
            data = DataGenerator.GenerateString(VOLUME);
        }

        public override string GetDescription()
        {
            return "Hashing data with SHA256";
        }

        public override ulong GetComparison(Options options)
        {
            switch (options.Threads)
            {
                case 1:
                {
                    return 1500;
                }
                default:
                {
                    return 200;
                }
            }
        }

        public override string[] GetCategories()
        {
            return new[] {"cryptography", "int", "all"};
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(char) * (double) (VOLUME * iterations);
        }
    }
}