using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.FMA
{
    public class Fma256 : BaseFma
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Fma.IsSupported && Avx.IsSupported)
            {
                return 0uL;
            }

            var randomFloatingSpan = new Span<float>(new[]
            {
                RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT,
                RANDOM_FLOAT
            });
            var dst = new Span<float>(Enumerable.Repeat(1f, 8).ToArray());
            var iterations = 0uL;

            unsafe
            {
                fixed (float* pdst = dst)
                fixed (float* psrc = randomFloatingSpan)
                {
                    var srcVector = Avx.LoadVector256(psrc);
                    var dstVector = Avx.LoadVector256(pdst);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            dstVector = Fma.MultiplyAdd(dstVector, srcVector, srcVector);
                        }

                        Avx.Store(pdst, dstVector);

                        iterations++;
                    }
                }
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "Benchmark of fma (fused multiply add) calculation on 256-bit floats (8 floating numbers)";
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
            return "fma_256";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "fma", "avx", "all"};
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(float) * 8 * (double) (LENGTH * iterations);
        }
    }
}