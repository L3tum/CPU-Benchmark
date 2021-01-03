using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.AVX
{
    public class Addition : BaseAvx
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Avx.IsSupported)
            {
                return 0uL;
            }

            var randomFloatingSpan = new Span<float>(new[]
            {
                RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT,
                RANDOM_FLOAT
            });
            var dst = new Span<float>(new float[8]);
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
                            dstVector = Avx.Add(dstVector, srcVector);
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
            return "AVX benchmark of addition on 256-bit floats (8 floating numbers)";
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
            return "avx_add";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "avx", "addition", "all"};
        }
    }
}