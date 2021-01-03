using System.Threading;

namespace Benchmarking.Arithmetic.Double
{
    public class Multiplication : BaseDouble
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var upticker = 1d;
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    upticker *= RANDOM_DOUBLE;
                }

                iterations++;
            }

            return iterations - (ulong) upticker + (ulong) upticker;
        }

        public override ulong GetComparison(Options options)
        {
            switch (options.Threads)
            {
                case 1:
                {
                    return 1503;
                }
                default:
                {
                    return 60;
                }
            }
        }

        public override string GetDescription()
        {
            return "Double arithmetic multiplication performance";
        }

        public override string GetName()
        {
            return "double_mul";
        }

        public override string[] GetCategories()
        {
            return new[] {"double", "arithmetic", "all", "multiplication"};
        }
    }
}