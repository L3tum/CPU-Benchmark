namespace Benchmarking.Decompression
{
    public class BaseDecompression : Benchmark
    {
        // 100 Kilobytes
        internal const int VOLUME = 12500;
        protected new const int LENGTH = 30;

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(char) * (double) (VOLUME * iterations) * LENGTH;
        }

        public override int GetRuntimeInMilliseconds()
        {
            return 2000;
        }
    }
}