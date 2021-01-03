using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.SSE4
{
    public class DotProduct : BaseSse4
    {
        private const float ANOTHER_RANDOM_FLOAT = MathF.PI;

        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Sse41.IsSupported)
            {
                return 0uL;
            }

            var randomFloatingSpan = new Span<float>(new[] {RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT});
            var dst = new Span<float>(new[]
                {ANOTHER_RANDOM_FLOAT, ANOTHER_RANDOM_FLOAT, ANOTHER_RANDOM_FLOAT, ANOTHER_RANDOM_FLOAT});
            var iterations = 0uL;

            unsafe
            {
                fixed (float* pdst = dst)
                fixed (float* psrc = randomFloatingSpan)
                {
                    var srcVector = Sse.LoadVector128(psrc);
                    var dstVector = Sse.LoadVector128(pdst);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            // Bit 4-7 (F): Which parts should be multiplied -> F: All
                            // Bit 0-3 (F): Where the result should be placed -> F: Everywhere
                            dstVector = Sse41.DotProduct(dstVector, srcVector, 0xFF);
                        }

                        Sse.Store(pdst, dstVector);

                        iterations++;
                    }
                }
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "SSE4.1 benchmark of dot-product calculation on 128-bit floats (4 floating numbers)";
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
            return "sse4_dp";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "sse4", "dot_product", "all"};
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(float) * 4 * (double) (LENGTH * iterations);
        }
    }
}