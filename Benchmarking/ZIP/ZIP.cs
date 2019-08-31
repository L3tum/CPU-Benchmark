#region using

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

#endregion

namespace Benchmarking.ZIP
{
	public class ZIP : Benchmark
	{
		// 125 MB Byte Array -> 1 GB
		private string[] datas;

		public ZIP(Options options) : base(options)
		{
			datas = new string[options.Threads];
		}

		public override void Run()
		{
			var tasks = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				tasks[i] = Task.Run(() =>
				{
					using (Stream s = new MemoryStream())
					{
						using (var stream = new GZipOutputStream(s))
						{
							stream.SetLevel(9);

							using (var sw = new StreamWriter(stream))
							{
								sw.Write(datas[i1]);
								sw.Flush();
								stream.Finish();
							}
						}
					}

					using (Stream s = new MemoryStream())
					{
						using (var stream = new BZip2OutputStream(s))
						{

							using (var sw = new StreamWriter(stream))
							{
								sw.Write(datas[i1]);
								sw.Flush();
								stream.Flush();
							}
						}
					}

					using (Stream s = new MemoryStream())
					{
						using (var stream = new DeflaterOutputStream(s, new Deflater(Deflater.BEST_COMPRESSION)))
						{
							using (var sw = new StreamWriter(stream))
							{
								sw.Write(datas[i1]);
								sw.Flush();
								stream.Finish();
							}
						}
					}

					using (Stream s = new MemoryStream())
					{
						using (var stream = new ZipOutputStream(s))
						{
							stream.SetLevel(9);
							stream.PutNextEntry(new ZipEntry("test.txt"));

							using (var sw = new StreamWriter(stream))
							{
								sw.Write(datas[i1]);
								sw.Flush();
								stream.CloseEntry();
								stream.Finish();
							}
						}
					}
				});
			}

			Task.WaitAll(tasks);
		}

		private void GenerateData(int index, int length)
		{
			var random = new Random();
			const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			datas[index] = new string(Enumerable.Range(1, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
		}

		public override string GetDescription()
		{
			return "Compressing 1 GB of data";
		}

		public override void Initialize()
		{
			// 500 "MB" string -> 2 bytes per character -> 1 GB String
			for (int i = 0; i < options.Threads; i++)
			{
				GenerateData(i, 500000000 / options.Threads);
			}
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 135984.0d;
			}
			else
			{
				return 12732.0d;
			}
		}
	}
}