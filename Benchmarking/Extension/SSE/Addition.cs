using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.SSE
{
    public class Addition : BaseSse
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Sse.IsSupported)
            {
                return 0uL;
            }

            var randomFloatingSpan = new Span<float>(new[] {RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT, RANDOM_FLOAT});
            var dst = new Span<float>(new float[4]);
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
                            dstVector = Sse.Add(dstVector, srcVector);
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
            return "SSE benchmark of addition on 128-bit floats (4 floating numbers)";
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
            return "sse_add";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "sse", "addition", "all"};
        }
    }
}