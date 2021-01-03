using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.AVX2
{
    public class Addition : BaseAvx2
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Avx2.IsSupported)
            {
                return 0uL;
            }

            var randomIntSpan = new Span<int>(new[]
            {
                randomInt, randomInt, randomInt, randomInt, randomInt, randomInt, randomInt, randomInt
            });
            var dst = new Span<int>(new int[8]);
            var iterations = 0uL;

            unsafe
            {
                fixed (int* pdst = dst)
                fixed (int* psrc = randomIntSpan)
                {
                    var srcVector = Avx.LoadVector256(psrc);
                    var dstVector = Avx.LoadVector256(pdst);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            dstVector = Avx2.Add(dstVector, srcVector);
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
            return "AVX2 benchmark of addition on 256-bit ints (8 numbers)";
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
            return "avx2_add";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "avx2", "addition", "all"};
        }
    }
}