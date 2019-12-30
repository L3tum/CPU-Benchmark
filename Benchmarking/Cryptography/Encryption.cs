#region using

using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Util;

#endregion

namespace Benchmarking.Cryptography
{
	internal class Encryption : Benchmark
	{
		private readonly string[] datas;
		private readonly uint volume = 500000000;
		private byte[] aesKey;
		private byte[] aesNonce;

		public Encryption(Options options) : base(options)
		{
			datas = new string[options.Threads];

			volume *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
			var tasks = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;
				tasks[i] = ThreadAffinity.RunAffinity(1uL << i, () =>
				{
					using (Stream s = new MemoryStream())
					{
						using var stream = new CryptoStream(s, new HMACSHA512(), CryptoStreamMode.Write);
						using var sw = new StreamWriter(stream);
						sw.Write(datas[i1]);
						sw.Flush();
						stream.Flush();
					}

					using (var aes = new AesGcm(aesKey))
					{
						var cipher = new byte[datas[i1].Length];
						var tag = new byte[16];

						aes.Encrypt(aesNonce, Encoding.UTF8.GetBytes(datas[i1]), cipher, tag);
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Encrypting data with HMACSHA512 and AES-GCM";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			// 500 "MB" string -> 2 bytes per character -> 1 GB String
			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() =>
				{
					datas[i1] = DataGenerator.GenerateString((int) (volume / options.Threads));
				});
			}

			Task.WaitAll(tasks);

			// 12 byte nonce
			aesNonce = new byte[12];
			RandomNumberGenerator.Fill(aesNonce);

			aesKey = new byte[32];
			RandomNumberGenerator.Fill(aesKey);
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 2004.0d;
				}
				default:
				{
					return 250.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"cryptography", "int"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(char) * volume * 2 / (timeInMillis / 1000);
		}
	}
}