using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.BZip2;

namespace Benchmarking.Decompression
{
    public class BZip2 : BaseDecompression
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    using Stream s = new MemoryStream(BZip2Data.RANDOM_DATA);
                    using var stream = new BZip2InputStream(s);
                    using var sr = new StreamReader(stream);
                    sr.ReadToEnd();
                }

                iterations++;
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "Decompressing data with BZip2";
        }

        public override ulong GetComparison(Options options)
        {
            switch (options.Threads)
            {
                case 1:
                {
                    return 1570;
                }
                default:
                {
                    return 1080;
                }
            }
        }

        public override string[] GetCategories()
        {
            return new[] {"decompression", "bzip2", "all"};
        }

        public override string GetName()
        {
            return $"{base.GetName()}-decompression";
        }
    }
}