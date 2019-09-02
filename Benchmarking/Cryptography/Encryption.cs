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
		private byte[] aesIV;
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

					using (Stream s = new MemoryStream())
					{
						using (var aes = new AesManaged())
						{
							aes.Mode = CipherMode.CBC;
							aes.KeySize = 256;
							aes.IV = aesIV;
							aes.Key = aesKey;

							using (var stream = new CryptoStream(s, aes.CreateEncryptor(), CryptoStreamMode.Write))
							{
								using (var sw = new StreamWriter(stream))
								{
									sw.Write(datas[i1]);
									sw.Flush();
									stream.Flush();
								}
							}
						}
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(tasks);
		}

		public override string GetDescription()
		{
			return "Encrypting 1 GB of data with SHA512 and AES26";
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

			using (var aes = new AesManaged())
			{
				aes.Mode = CipherMode.CBC;
				aes.KeySize = 256;

				aes.GenerateIV();
				aes.GenerateKey();

				aesIV = aes.IV;
				aesKey = aes.Key;
			}
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 2538.0d;
			}

			return 548.0d;
		}
	}
}