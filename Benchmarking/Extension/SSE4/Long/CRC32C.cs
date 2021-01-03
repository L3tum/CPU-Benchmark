using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using Benchmarking.Util;

namespace Benchmarking.Extension.SSE4.Long
{
    public class CRC32C : BaseSse4
    {
        private ulong datas;

        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Sse42.X64.IsSupported)
            {
                return 0uL;
            }
            
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                var crc = 0uL;

                for (var i = 0; i < LENGTH; i++)
                {
                    crc = Sse42.X64.Crc32(crc, datas);
                }

                iterations++;
            }

            return iterations;
        }

        public override string GetName()
        {
            return "sse4_crc_long";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "sse4", "crc", "all"};
        }

        public override string GetDescription()
        {
            return "SSE4.2 calculation of crc";
        }

        public override ulong GetComparison(Options options)
        {
            return 0ul;
        }

        public override void Initialize()
        {
            var data = DataGenerator.GenerateString(8);
            var bytes = Encoding.ASCII.GetBytes(data);
            var j = 0;

            datas = (ulong) ((long) bytes[j] << 56) + ((ulong) bytes[j + 1] << 48) +
                    ((ulong) bytes[j + 2] << 40) + ((ulong) bytes[j + 3] << 32) +
                    ((ulong) bytes[j + 4] << 24) + ((ulong) bytes[j + 5] << 16) +
                    ((ulong) bytes[j + 6] << 8) + bytes[j + 7];
        }
        
        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(ulong) * (double) (LENGTH * iterations);
        }
    }
}