using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Benchmarking.Decompression
{
    public class Deflate : BaseDecompression
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                for (var i = 0; i < LENGTH; i++)
                {
                    using Stream s = new MemoryStream(DeflateData.RANDOM_DATA);
                    using var stream = new InflaterInputStream(s);
                    using var sr = new StreamReader(stream);
                    sr.ReadToEnd();
                }

                iterations++;
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "Decompressing data with Deflate";
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
            return new[] {"decompression", "deflate", "all"};
        }

        public override string GetName()
        {
            return $"{base.GetName()}-decompression";
        }
    }
}