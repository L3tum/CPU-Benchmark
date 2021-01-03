using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Benchmarking.Decompression
{
    public class GZip : BaseDecompression
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    using Stream s = new MemoryStream(GZipData.RANDOM_DATA);
                    using var stream = new GZipInputStream(s);
                    using var sr = new StreamReader(stream);
                    sr.ReadToEnd();
                }

                iterations++;
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "Decompressing data with GZip";
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
            return new[] {"decompression", "gzip", "all"};
        }

        public override string GetName()
        {
            return $"{base.GetName()}-decompression";
        }
    }
}