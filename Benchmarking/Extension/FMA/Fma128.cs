using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.FMA
{
    public class Fma128 : BaseFma
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Fma.IsSupported)
            {
                return 0uL;
            }

            var randomFloatingSpan = new Span<float>(new[] {RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT});
            var dst = new Span<float>(Enumerable.Repeat(1f, 4).ToArray());
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
                            dstVector = Fma.MultiplyAdd(dstVector, srcVector, srcVector);
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
            return "Benchmark of fma (fused multiply add) calculation on 128-bit floats (4 floating numbers)";
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
            return "fma_128";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "fma", "sse4", "all"};
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(float) * 4 * (double) (LENGTH * iterations);
        }
    }
}