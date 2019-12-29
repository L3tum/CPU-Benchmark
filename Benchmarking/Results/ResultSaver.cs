#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Util;
using CPU_Benchmark_Common;
using HardwareInformation;
using Newtonsoft.Json;

#endregion

namespace Benchmarking.Results
{
	public static class ResultSaver
	{
		private const string SAVE_DIRECTORY = "_save";
		private static readonly string SAVE_FILE = $"{SAVE_DIRECTORY}/save.benchmark";
		private static readonly string TIME_FILE = $"{SAVE_DIRECTORY}/time.benchmark";
		private static readonly string HASH_FILE = $"{SAVE_DIRECTORY}/check.benchmark";

		private static Save save;

		public static bool Init(Options options)
		{
			if (!Directory.Exists(SAVE_DIRECTORY))
			{
				var directory = Directory.CreateDirectory(SAVE_DIRECTORY);

				directory.Attributes |= FileAttributes.Hidden;
			}

			LoadSave();

			// Prune invalid results
			foreach (var resultsValue in save.Results.Values)
			{
				resultsValue.RemoveAll(item => item == null);
			}

			AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;

			// Skip checks if they only want to view their results
			if (!options.ListResults)
			{
				var machineInformation = MachineInformationGatherer.GatherInformation();

				if (!CheckValidSave(machineInformation))
				{
					SaveResults($"save-{GetBootTime().ToBinary()}.old.benchmark");
					save.Results.Clear();
					save.MachineInformation = machineInformation;
					save.Version = Assembly.GetExecutingAssembly().GetName().Version;
					save.DotNetVersion = RuntimeInformation.FrameworkDescription;

					return false;
				}
			}

			return true;
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

		public static ErrorCode IsAllowedToUpload(Options options)
		{
			if (save.MachineInformation.Platform != MachineInformation.Platforms.Windows)
			{
				return ErrorCode.NOT_WINDOWS;
			}

			if (!save.Results.All(r =>
				r.Value.Any(v => v.Benchmark.ToLowerInvariant().Replace(" ", "") == "category:all")))
			{
				return ErrorCode.NO_CATEGORY_ALL;
			}

			return ErrorCode.OK;
		}

		public static async Task<UploadedResponse> UploadResults()
		{
			try
			{
				var machineInformation = MachineInformationGatherer.GatherInformation(false);

				if (!CheckValidSave(machineInformation))
				{
					return null;
				}

				save.MachineInformation = machineInformation;

				var response = await ResultUploader.UploadResult(save).ConfigureAwait(false);

				save.UUID = response.UUID;
				save.Uploaded = response.Uploaded;

				return response;
			}
			catch (HttpRequestException)
			{
				// Intentionally left empty
			}

			return null;
		}

		private static bool CheckValidSave(MachineInformation machineInformation)
		{
			if (save == null || save.MachineInformation == null)
			{
				return false;
			}

			if (machineInformation.Platform != save.MachineInformation.Platform ||
			    machineInformation.Cpu.Family != save.MachineInformation.Cpu.Family ||
			    machineInformation.Cpu.Model != save.MachineInformation.Cpu.Model ||
			    machineInformation.Cpu.Stepping != save.MachineInformation.Cpu.Stepping ||
			    machineInformation.RAMSticks.Count != save.MachineInformation.RAMSticks.Count ||
			    machineInformation.SmBios.BIOSCodename !=
			    save.MachineInformation.SmBios.BIOSCodename ||
			    machineInformation.SmBios.BoardName != save.MachineInformation.SmBios.BoardName ||
			    machineInformation.SmBios.BoardVersion !=
			    save.MachineInformation.SmBios.BoardVersion ||
			    machineInformation.Cpu.Cores.Count != save.MachineInformation.Cpu.Cores.Count ||
			    machineInformation.Cpu.Caches.Count != save.MachineInformation.Cpu.Caches.Count)
			{
				return false;
			}

			foreach (var ram in machineInformation.RAMSticks)
			{
				if (save.MachineInformation.RAMSticks.FirstOrDefault(r =>
					    r.Capacity == ram.Capacity && r.FormFactor == ram.FormFactor &&
					    r.Manufacturer == ram.Manufacturer && r.PartNumber == ram.PartNumber &&
					    r.Speed == ram.Speed) == null)
				{
					return false;
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

			var lastHash = GetLastHash();

			if (lastHash.Length == 0)
			{
				return false;
			}

			var currentHash = ComputeHash();

			if (!lastHash.SequenceEqual(currentHash))
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

					SaveResults();
				}

				if (File.Exists("./save.benchmark"))
				{
					save = JsonConvert.DeserializeObject<Save>(
						Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText("./save.benchmark"))));

					File.Delete("./save.benchmark");

					SaveResults();
				}

				if (File.Exists(SAVE_FILE))
				{
					save = JsonConvert.DeserializeObject<Save>(
						Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(SAVE_FILE))));
				}
			}
			catch
			{
				Console.WriteLine("Exception occured accessing your save. Repairing....");
			}

			if (save == null)
			{
				save = new Save
				{
					MachineInformation = MachineInformationGatherer.GatherInformation(),
					Version = Assembly.GetExecutingAssembly().GetName().Version,
					DotNetVersion = RuntimeInformation.FrameworkDescription
				};
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
				if (File.Exists(TIME_FILE))
				{
					return DateTime.FromBinary(long.Parse(File.ReadAllText(TIME_FILE)));
				}
			}
			catch
			{
				return DateTime.MinValue;
			}

			return DateTime.MinValue;
		}

		private static void SaveResults(string filename = "save.benchmark")
		{
			if (save != null)
			{
				if (File.Exists($"{SAVE_DIRECTORY}/{filename}"))
				{
					File.Delete($"{SAVE_DIRECTORY}/{filename}");
				}

				var json = JsonConvert.SerializeObject(save);

				File.AppendAllText($"{SAVE_DIRECTORY}/{filename}",
					Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));

				if (File.Exists(TIME_FILE))
				{
					File.Delete(TIME_FILE);
				}

				File.AppendAllText(TIME_FILE, GetBootTime().ToBinary().ToString());

				SaveHash();
			}
		}

		private static byte[] GetLastHash()
		{
			if (!File.Exists(HASH_FILE) || !File.Exists(SAVE_FILE) || !File.Exists(TIME_FILE))
			{
				return Array.Empty<byte>();
			}

			using var stream = File.OpenRead(HASH_FILE);
			using var reader = new BinaryReader(stream);
			return reader.ReadAllBytes();
		}

		private static byte[] ComputeHash()
		{
			if (!File.Exists(HASH_FILE) || !File.Exists(SAVE_FILE) || !File.Exists(TIME_FILE))
			{
				return Array.Empty<byte>();
			}

			var combinedHash = new List<byte>();

			using var sha1 = HashAlgorithm.Create("SHA1");
			using (var stream = File.OpenRead(SAVE_FILE))
			{
				combinedHash.AddRange(sha1.ComputeHash(stream));
			}

			using (var stream = File.OpenRead(TIME_FILE))
			{
				combinedHash.AddRange(sha1.ComputeHash(stream));
			}

			combinedHash.AddRange(Encoding.UTF8.GetBytes(GetBootTime().ToBinary().ToString()));

			return sha1.ComputeHash(combinedHash.ToArray());
		}

		private static void SaveHash()
		{
			if (File.Exists(HASH_FILE))
			{
				File.Delete(HASH_FILE);
			}

			using var stream = File.OpenWrite(HASH_FILE);
			using var writer = new BinaryWriter(stream, Encoding.UTF8, false);
			writer.Write(ComputeHash());
			writer.Close();
		}

		/// <summary>
		///     Only works if ResultSaver.Init hasn't been called yet!
		/// </summary>
		public static void Clear()
		{
			Directory.Delete(SAVE_DIRECTORY, true);
		}

		private static void CurrentDomainOnProcessExit(object sender, EventArgs e)
		{
			SaveResults();
		}
	}
}