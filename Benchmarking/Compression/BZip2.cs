#region using

using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.BZip2;

#endregion

namespace Benchmarking.Compression
{
    internal class BZip2 : BaseCompression
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            var iterations = 0uL;

            while (!cancellationToken.IsCancellationRequested)
            {
                using (Stream s = new MemoryStream())
                {
                    using var stream = new BZip2OutputStream(s);
                    using var sw = new StreamWriter(stream);
                    sw.Write(Data);
                    sw.Flush();
                    stream.Flush();
                    s.Flush();
                }

                iterations++;
            }

            return iterations;
        }

        public override string GetDescription()
        {
            return "Compressing data with BZip2";
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
            return new[] {"compression", "bzip2", "all"};
        }

        public override string GetName()
        {
            return $"{base.GetName()}-compression";
        }
    }
}