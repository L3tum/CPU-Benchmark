using System.Threading;

namespace Benchmarking.Arithmetic.Long
{
    internal class Multiplication : BaseLong
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var upticker = 1L;
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    upticker *= RandomInt;
                }

                iterations++;
                upticker = 1;
            }

            return iterations - (ulong) upticker + (ulong) upticker;
        }

        public override ulong GetComparison(Options options)
        {
            switch (options.Threads)
            {
                case 1:
                {
                    return 1292;
                }
                default:
                {
                    return 52;
                }
            }
        }

        public override string GetDescription()
        {
            return "Integer multiplication arithmetic performance";
        }

        public override string GetName()
        {
            return "long_mul";
        }

        public override string[] GetCategories()
        {
            return new[] {"long", "multiplication", "arithmetic", "arithmetic_long", "all"};
        }
    }
}