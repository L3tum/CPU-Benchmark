using System.Threading;

namespace Benchmarking.Arithmetic.Int32
{
    internal class Subtraction : BaseInteger
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var upticker = int.MaxValue;
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    upticker -= RandomInt;
                }

                iterations++;
                upticker = int.MaxValue;
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
            return "Integer subtraction arithmetic performance";
        }

        public override string GetName()
        {
            return "int_sub";
        }

        public override string[] GetCategories()
        {
            return new[] {"int", "subtraction", "arithmetic", "arithmetic_int", "all"};
        }
    }
}