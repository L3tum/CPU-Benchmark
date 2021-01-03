using System.Threading;

namespace Benchmarking.Arithmetic.Float
{
    public class Addition : BaseFloat
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var upticker = 1f;
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    upticker += RANDOM_FLOAT;
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
                    return 1550;
                }
                default:
                {
                    return 68;
                }
            }
        }

        public override string GetDescription()
        {
            return "Float arithmetic addition performance";
        }

        public override string GetName()
        {
            return "float_add";
        }

        public override string[] GetCategories()
        {
            return new[] {"float", "arithmetic", "all", "addition"};
        }
    }
}