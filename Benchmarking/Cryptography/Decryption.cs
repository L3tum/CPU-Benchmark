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
		private readonly string[] datas;
		private readonly string[] datasAES;
		private readonly string[] datasSHA;
		private byte[] aesIV;
		private byte[] aesKey;
		private byte[] sha512Key;

		public Decryption(Options options) : base(options)
		{
			datas = new string[options.Threads];
			datasSHA = new string[options.Threads];
			datasAES = new string[options.Threads];
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
						using (var stream = new CryptoStream(s, new HMACSHA512(sha512Key),
							CryptoStreamMode.Read))
						{
							using (var sw = new StreamWriter(s))
							{
								sw.Write(datasSHA[i1]);
								sw.Flush();
								stream.Flush();
								stream.FlushFinalBlock();

								s.Seek(0, SeekOrigin.Begin);

								using (var sr = new StreamReader(stream))
								{
									datasSHA[i1] = sr.ReadToEnd();
								}
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

							using (var stream = new CryptoStream(s, aes.CreateDecryptor(), CryptoStreamMode.Read))
							{
								using (var sw = new StreamWriter(s))
								{
									sw.Write(datasAES[i1]);
									sw.Flush();
									stream.Flush();
									stream.FlushFinalBlock();

									s.Seek(0, SeekOrigin.Begin);

									using (var sr = new StreamReader(stream))
									{
										datasAES[i1] = sr.ReadToEnd();
									}
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
			return "Decrypting 1 GB of data with HMACSHA512 and AES26";
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
					datas[i1] = DataGenerator.GenerateString(500000000 / options.Threads);
					var rand = new Random();
					sha512Key = new byte[64];

					for (var j = 0; j < 64; j++)
					{
						sha512Key[j] = (byte) rand.Next();
					}

					var hmac = new HMACSHA512(sha512Key);
					hmac.Initialize();

					datasSHA[i1] = Encoding.Default.GetString(hmac.ComputeHash(Encoding.Default.GetBytes(datas[i1])));

					using (var s = new MemoryStream())
					{
						using (var aes = new AesManaged())
						{
							aes.Mode = CipherMode.CBC;
							aes.KeySize = 256;
							aes.GenerateIV();
							aes.GenerateKey();

							aesIV = aes.IV;
							aesKey = aes.Key;

							using (var stream = new CryptoStream(s, aes.CreateEncryptor(), CryptoStreamMode.Write))
							{
								using (var sw = new StreamWriter(stream))
								{
									sw.Write(datas[i1]);
									sw.Flush();
									stream.Flush();
									stream.FlushFinalBlock();

									datasAES[i1] = Encoding.Default.GetString(s.GetBuffer(), 0, (int) s.Length);
								}
							}
						}
					}

					datas[i1] = null;
				});
			}

			Task.WaitAll(tasks);
		}

		public override double GetReferenceValue()
		{
			if (options.Threads == 1)
			{
				return 930.0d;
			}

			return 170.0d;
		}
	}
}