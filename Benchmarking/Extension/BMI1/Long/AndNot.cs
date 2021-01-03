using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.BMI1.Long
{
    public class AndNot : BaseBmi1
    {
        private ulong anotherRandomInt;

        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Bmi1.X64.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var bfe = randomInt;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    bfe = Bmi1.X64.AndNot(bfe, anotherRandomInt);
                }

                iterations++;
            }

            return iterations + bfe - bfe;
        }

        public override void Initialize()
        {
            base.Initialize();
            var rand = new Random();
            anotherRandomInt = ((ulong) rand.Next(int.MinValue, int.MaxValue) << 32) +
                               (ulong) rand.Next(int.MinValue, int.MaxValue);
        }

        public override string GetDescription()
        {
            return "BMI1 benchmark of and-not on 64-bit ints";
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
            return "bmi1_andnot_long";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "bmi1", "andnot", "all"};
        }
    }
}