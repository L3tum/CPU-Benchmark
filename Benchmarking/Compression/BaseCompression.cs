using Benchmarking.Util;

namespace Benchmarking.Compression
{
    public class BaseCompression : Benchmark
    {
        // 1 Megabyte
        protected const int VOLUME = 12500;
        protected string Data = null!;
        protected new const int LENGTH = 30;

        public override void Initialize()
        {
            Data = DataGenerator.GenerateString(VOLUME);
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(char) * (double) (VOLUME * iterations);
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 2000;
        }
    }
}