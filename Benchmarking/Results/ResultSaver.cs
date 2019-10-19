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

		public static void Init(Options options)
		{
			SaveResults();

			try
			{
				if (File.Exists("./save.json"))
				{
					save = JsonConvert.DeserializeObject<Save>(File.ReadAllText("./save.json"));

					File.Delete("./save.json");
				}

				if (File.Exists("./save.benchmark"))
				{
					using var stream = File.OpenRead("./save.benchmark");
					using var reader = new StreamReader(stream);
					save = JsonConvert.DeserializeObject<Save>(
						Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd())));
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Your save was invalid! Creating new one...");
			}

			if (save == null)
			{
				save = new Save();
			}

			save.MachineInformation = MachineInformationGatherer.GatherInformation(options.QuickRun);
			save.Version = Assembly.GetExecutingAssembly().GetName().Version;
			save.DotNetVersion = RuntimeInformation.FrameworkDescription;

			foreach (List<Result> resultsValue in save.Results.Values)
			{
				resultsValue.RemoveAll(item => item == null);
			}

			AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
		}

		public static void SaveResult(uint threads, Result result)
		{
			lock (save.Results)
			{
				if (!save.Results.ContainsKey(threads))
				{
					save.Results.Add(threads, new List<Result>());
				}

				var saved = save.Results[threads].FirstOrDefault(r => r?.Benchmark == result.Benchmark);

				if (saved != null)
				{
					save.Results[threads].Remove(saved);
				}

				save.Results[threads].Add(result);
			}
		}

		public static Dictionary<uint, List<Result>> GetResults()
		{
			return save.Results;
		}

		public static bool IsAllowedToUpload(Options options)
		{
			return save.MachineInformation.Platform == MachineInformation.Platforms.Windows && !options.QuickRun;
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

				using var stream = File.OpenWrite("./save.benchmark");
				using var writer = new StreamWriter(stream);
				var json = JsonConvert.SerializeObject(save);

				writer.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));
				writer.Flush();
				stream.Flush();
			}
		}

		private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
		{
			SaveResults();
		}
	}
}