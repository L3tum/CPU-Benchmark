using System.Threading;

namespace Benchmarking.Arithmetic.Int32
{
    internal class Division : BaseInteger
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var upticker = int.MaxValue;
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    upticker /= RandomInt;
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
            return "Integer division arithmetic performance";
        }

        public override string GetName()
        {
            return "int_div";
        }

        public override string[] GetCategories()
        {
            return new[] {"int", "division", "arithmetic", "arithmetic_int", "all"};
        }
    }
}