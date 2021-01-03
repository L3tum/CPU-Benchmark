using System.Threading;

namespace Benchmarking.Arithmetic.Int32
{
    internal class Addition : BaseInteger
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;
            var upticker = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    upticker += RandomInt;
                }

                iterations++;
                upticker = 0;
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
            return "Integer addition arithmetic performance";
        }

        public override string GetName()
        {
            return "int_add";
        }

        public override string[] GetCategories()
        {
            return new[] {"int", "addition", "arithmetic", "arithmetic_int", "all"};
        }
    }
}