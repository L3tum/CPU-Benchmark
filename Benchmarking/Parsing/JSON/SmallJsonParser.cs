using System.Threading;
using Benchmarking.Results;
using Newtonsoft.Json;

namespace Benchmarking.Parsing.JSON
{
    public class SmallJsonParser : BaseJson
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    var doc = JsonConvert.DeserializeObject<Save>(SmallJsonFile.FILE);
                }

                iterations++;
            }

            return iterations;
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return base.GetDataThroughput(iterations) * SmallJsonFile.FILE.Length;
        }
    }
}