#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Util;
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
			LoadSave();

			var machineInformation = MachineInformationGatherer.GatherInformation(options.QuickRun && !options.Upload);

			if (!CheckValidSave(machineInformation))
			{
				save.Results.Clear();
			}

			save.MachineInformation = machineInformation;
			save.Version = Assembly.GetExecutingAssembly().GetName().Version;
			save.DotNetVersion = RuntimeInformation.FrameworkDescription;

			foreach (var resultsValue in save.Results.Values)
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
					if (saved.Points > result.Points)
					{
						return;
					}

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
			return save.MachineInformation.Platform == MachineInformation.Platforms.Windows && options.Upload;
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

		public static string GetUUID()
		{
			return save.UUID;
		}

		private static bool CheckValidSave(MachineInformation machineInformation)
		{
			if (save != null && save.MachineInformation != null)
			{
				if (machineInformation.Platform != save.MachineInformation.Platform ||
				    machineInformation.Cpu.Family != save.MachineInformation.Cpu.Family ||
				    machineInformation.Cpu.Model != save.MachineInformation.Cpu.Model ||
				    machineInformation.Cpu.Stepping != save.MachineInformation.Cpu.Stepping ||
				    machineInformation.RAMSticks.Count != save.MachineInformation.RAMSticks.Count ||
				    Assembly.GetExecutingAssembly().GetName().Version != save.Version ||
				    RuntimeInformation.FrameworkDescription != save.DotNetVersion ||
				    machineInformation.SmBios.BIOSCodename !=
				    save.MachineInformation.SmBios.BIOSCodename ||
				    machineInformation.SmBios.BoardName != save.MachineInformation.SmBios.BoardName ||
				    machineInformation.SmBios.BoardVersion !=
				    save.MachineInformation.SmBios.BoardVersion)
				{
					return false;
				}

				foreach (var ram in machineInformation.RAMSticks)
				{
					if (save.MachineInformation.RAMSticks.FirstOrDefault(r =>
						    r.Capacity == ram.Capacity && r.FormFactor == ram.FormFactor &&
						    r.Manfucturer == ram.Manfucturer && r.PartNumber == ram.PartNumber &&
						    r.Speed == ram.Speed) == null)
					{
						return false;
					}
				}
			}

			var lastBootTime = GetLastBootTime();

			if (lastBootTime == DateTime.MinValue)
			{
				return false;
			}

			var currentBootTime = GetBootTime();

			if (!lastBootTime.Equals(currentBootTime))
			{
				return false;
			}

			return true;
		}

		private static void LoadSave()
		{
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
			catch
			{
				Console.WriteLine("Your save was invalid! Creating new one...");
			}

			if (save == null)
			{
				save = new Save();
			}
		}

		private static DateTime GetBootTime()
		{
			var ticks = Stopwatch.GetTimestamp();
			var uptime = (double) ticks / Stopwatch.Frequency;
			var uptimeSpan = TimeSpan.FromSeconds(uptime);

			var bootTime = DateTime.Now.Subtract(uptimeSpan).ToUniversalTime();

			// Trim to minutes
			return bootTime.Trim(TimeSpan.TicksPerMinute);
		}

		private static DateTime GetLastBootTime()
		{
			try
			{
				if (File.Exists("./time.benchmark"))
				{
					return DateTime.FromBinary(long.Parse(File.ReadAllText("./time.benchmark")));
				}
			}
			catch
			{
				return DateTime.MinValue;
			}

			return DateTime.MinValue;
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

				if (File.Exists("./time.benchmark"))
				{
					File.Delete("./time.benchmark");
				}

				File.AppendAllText("./time.benchmark", GetBootTime().ToBinary().ToString());
			}
		}

		private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
		{
			SaveResults();
		}
	}
}