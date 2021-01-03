using System.Threading;

namespace Benchmarking.Arithmetic.Double
{
    public class Division : BaseDouble
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var upticker = double.MaxValue;
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    upticker /= RANDOM_DOUBLE;
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
            return "Double arithmetic division performance";
        }

        public override string GetName()
        {
            return "double_div";
        }

        public override string[] GetCategories()
        {
            return new[] {"double", "arithmetic", "all", "division"};
        }
    }
}