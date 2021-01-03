using System;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.BMI2.Long
{
    public class UnsignedMultiplication : BaseBmi2
    {
        private uint anotherRandomInt;

        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Bmi2.X64.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var zhb = randomInt;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    zhb = Bmi2.X64.MultiplyNoFlags(zhb, anotherRandomInt);
                }

                iterations++;
            }

            return iterations + zhb - zhb;
        }

        public override void Initialize()
        {
            base.Initialize();
            var rand = new Random();
            anotherRandomInt = (uint) rand.Next();
        }

        public override string GetDescription()
        {
            return "BMI2 benchmark of unsigned multiplication on 64-bit ints";
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
            return "bmi2_mulx_long";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "bmi2", "mulx", "all"};
        }
    }
}