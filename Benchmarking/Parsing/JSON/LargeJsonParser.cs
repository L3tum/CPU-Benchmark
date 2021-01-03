using System.Threading;
using Newtonsoft.Json;

namespace Benchmarking.Parsing.JSON
{
    public class LargeJsonParser : BaseJson
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                var doc = JsonConvert.DeserializeObject<dynamic>(LargeJsonFile.FILE);

                iterations++;
            }

            return iterations;
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(char) * (double) iterations * LargeJsonFile.FILE.Length;
        }
    }
}