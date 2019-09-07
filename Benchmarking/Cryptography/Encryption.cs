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
		private byte[] aesNonce;
		private byte[] aesKey;

		public Encryption(Options options, string[] data = null) : base(options)
		{
			datas = new string[options.Threads];

			if (data != null)
			{
				datas = data;
			}
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
						using (var stream = new CryptoStream(s, new HMACSHA512(), CryptoStreamMode.Write))
						{
							using (var sw = new StreamWriter(stream))
							{
								sw.Write(datas[i1]);
								sw.Flush();
								stream.Flush();
							}
						}
					}

					using (var aes = new AesGcm(aesKey))
					{
						var cipher = new byte[datas[i1].Length];
						var tag = new byte[16];

						aes.Encrypt(aesNonce, Encoding.UTF8.GetBytes(datas[i1]), cipher, tag);
					}

					BenchmarkRunner.ReportProgress(GetName());
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Encrypting 1 GB of data with SHA512 and AES-GCM";
		}

		public override void Initialize()
		{
			var tasks = new Task[options.Threads];

			// 500 "MB" string -> 2 bytes per character -> 1 GB String
			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				tasks[i1] = Task.Run(() => { datas[i1] = DataGenerator.GenerateString(500000000 / options.Threads); });
			}

			Task.WaitAll(tasks);

			// 12 byte nonce
			aesNonce = new byte[12];
			RandomNumberGenerator.Fill(aesNonce);

			aesKey = new byte[32];
			RandomNumberGenerator.Fill(aesKey);
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 2028.0d;
			}

			return 548.0d;
		}
	}
}