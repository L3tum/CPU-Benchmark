#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HardwareInformation;
using Newtonsoft.Json;

#endregion

namespace Benchmarking.Results
{
	public static class ResultSaver
	{
		private static Save save;

		public static void Init()
		{
			SaveResults();

			if (File.Exists("./save.json"))
			{
				save = JsonConvert.DeserializeObject<Save>(File.ReadAllText("./save.json"));

				File.Delete("./save.json");
			}

			if (File.Exists("./save.benchmark"))
			{
				using (var stream = File.OpenRead("./save.benchmark"))
				{
					using (var reader = new StreamReader(stream))
					{
						save = JsonConvert.DeserializeObject<Save>(
							Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd())));
					}
				}
			}

			if (save == null)
			{
				save = new Save();
			}

			save.MachineInformation = MachineInformationGatherer.GatherInformation();
			save.Version = Assembly.GetExecutingAssembly().GetName().Version;
			save.DotNetVersion = RuntimeInformation.FrameworkDescription;

			AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
		}

		public static void SaveResult(Result result)
		{
			var saved = save.Results.FirstOrDefault(r => r.Benchmark == result.Benchmark);

			if (saved != null)
			{
				save.Results.Remove(saved);
			}

			save.Results.Add(result);
		}

		public static List<Result> GetResults()
		{
			return save.Results;
		}

		public static async Task<bool> UploadResults()
		{
			try
			{
				var uuid = await ResultUploader.UploadResult(save).ConfigureAwait(false);

				save.UUID = uuid;
			}
			catch (HttpRequestException)
			{
				return false;
			}

			return true;
		}

		private static void SaveResults()
		{
			if (save != null)
			{
				if (File.Exists("./save.benchmark"))
				{
					File.Delete("./save.benchmark");
				}

				using (var stream = File.OpenWrite("./save.benchmark"))
				{
					using (var writer = new StreamWriter(stream))
					{
						var json = JsonConvert.SerializeObject(save);

						writer.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));
						writer.Flush();
						stream.Flush();
					}
				}
			}
		}

		private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
		{
			SaveResults();
		}
	}
}