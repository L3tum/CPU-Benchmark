#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Benchmarking;
using Newtonsoft.Json;

#endregion

namespace Benchmarker
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
			}

			if (save == null)
			{
				save = new Save();
			}

			save.MachineInformation = MachineInformationGatherer.GatherInformation();
			save.Version = Assembly.GetExecutingAssembly().GetName().Version;

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

		private static void SaveResults()
		{
			if (save != null)
			{
				if (File.Exists("./save.json"))
				{
					File.Delete("./save.json");
				}

				using (var stream = File.OpenWrite("./save.json"))
				{
					using (var writer = new StreamWriter(stream))
					{
						var json = JsonConvert.SerializeObject(save);

						writer.Write(json);
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

		public class Save
		{
			public MachineInformation MachineInformation;
			public List<Result> Results;
			public Version Version;

			public Save()
			{
				Results = new List<Result>();
			}
		}
	}
}