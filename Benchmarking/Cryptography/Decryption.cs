#region using

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Util;

#endregion

namespace Benchmarking.Cryptography
{
	internal class Decryption : Benchmark
	{
		private readonly byte[][] aesTag;
		private readonly byte[][] datasAES;
		private readonly string[] datasSHA;
		private readonly uint volume = 10000000;
		private const uint runs = 200;
		private byte[] aesKey;
		private byte[] aesNonce;
		private byte[] sha512Key;

		public Decryption(Options options) : base(options)
		{
			datasSHA = new string[options.Threads];
			datasAES = new byte[options.Threads][];

			aesTag = new byte[options.Threads][];

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
					for (var j = 0; j < runs; j++)
					{
						using (Stream s = new MemoryStream())
						{
							using var stream = new CryptoStream(s, new HMACSHA512(sha512Key),
								CryptoStreamMode.Read);
							using var sw = new StreamWriter(s);

							sw.Write(datasSHA[i1]);
							sw.Flush();
							stream.Flush();
							stream.FlushFinalBlock();

							s.Seek(0, SeekOrigin.Begin);

							using var sr = new StreamReader(stream);
							sr.ReadToEnd();
						}

						using var aes = new AesGcm(aesKey);
						var plaintext = new byte[datasAES[i1].Length];
						aes.Decrypt(aesNonce, datasAES[i1], aesTag[i1], plaintext);
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Decrypting data with HMACSHA512 and AES-GCM";
		}

		public override void Initialize()
		{
			aesNonce = new byte[12];
			aesKey = new byte[32];

			RandomNumberGenerator.Fill(aesNonce);
			RandomNumberGenerator.Fill(aesKey);

			var tasks = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() =>
				{
					var data = Encoding.UTF8.GetBytes(DataGenerator.GenerateString((int) (volume / options.Threads)));
					var rand = new Random();
					sha512Key = new byte[64];

					for (var j = 0; j < 64; j++)
					{
						sha512Key[j] = (byte) rand.Next();
					}

					var hmac = new HMACSHA512(sha512Key);
					hmac.Initialize();

					datasSHA[i1] = Encoding.Default.GetString(hmac.ComputeHash(data));

					datasAES[i1] = new byte[data.Length];

					using var aes = new AesGcm(aesKey);
					aesTag[i1] = new byte[16];

					aes.Encrypt(aesNonce, data, datasAES[i1], aesTag[i1]);
				});
			}

			Task.WaitAll(tasks);
			GC.Collect();
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 1200.0d;
				}
				default:
				{
					return 545.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"cryptography", "int"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(char) * volume * 2 * runs / (timeInMillis / 1000);
		}
	}
}