using System.Threading;

namespace Benchmarking.Arithmetic.Int32
{
    internal class Multiplication : BaseInteger
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var upticker = 1;
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
            return "int_mul";
        }

        public override string[] GetCategories()
        {
            return new[] {"int", "multiplication", "arithmetic", "arithmetic_int", "all"};
        }
    }
}