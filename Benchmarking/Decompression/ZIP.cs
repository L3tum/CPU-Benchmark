using System;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;

namespace Benchmarking.Decompression
{
    public class ZIP : BaseDecompression
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    using Stream s = new MemoryStream(ZIPData.RANDOM_DATA);
                    using var stream = new ZipInputStream(s);
                    var zipEntry = stream.GetNextEntry();
                    zipEntry.DateTime = DateTime.Now;
                    using var sr = new StreamReader(stream);
                    sr.ReadToEnd();
                }

                iterations++;
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "Decompressing data with ZIP";
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
            return new[] {"decompression", "zip", "all"};
        }

        public override string GetName()
        {
            return $"{base.GetName()}-decompression";
        }
    }
}