﻿#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#endregion

namespace Benchmarking
{
	public class MachineInformation
	{
		public MachineInformation()
		{
			Cpu = new CPU();
			Ram = new RAM();
		}

		public OperatingSystem OperatingSystem { get; set; }

		public CPU Cpu { get; set; }

		public RAM Ram { get; set; }

		public class CPU
		{
			public int PhysicalCores { get; set; }
			public int LogicalCores { get; set; }

			public string Architecture { get; set; }

			public string Caption { get; set; }

			public string Name { get; set; }

			public int MaxClockSpeed { get; set; }

			public string Socket { get; set; }

			public long L2CacheSize { get; set; }

			public long L3CacheSize { get; set; }

			public string BIOSVersion { get; set; }
		}

		public class RAM
		{
			public long Speed { get; set; }

			public string Manfucturer { get; set; }

			public long Capacity { get; set; }

			public string CapacityHRF { get; set; }
		}
	}

	public static class MachineInformationGatherer
	{
		private static MachineInformation information;

		public static MachineInformation GatherInformation()
		{
			if (information != null)
			{
				return information;
			}

			information = new MachineInformation();

			information.OperatingSystem = Environment.OSVersion;
			information.Cpu.LogicalCores = Environment.ProcessorCount;
			information.Cpu.Architecture = RuntimeInformation.ProcessArchitecture.ToString();
			information.Cpu.Caption = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
			information.Cpu.Name = information.Cpu.Caption;

			try
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					var mos = new ManagementObjectSearcher(
						"select Name,L2CacheSize,L3CacheSize,NumberOfEnabledCore,NumberOfLogicalProcessors,SocketDesignation,MaxClockSpeed from Win32_Processor");

					foreach (var managementBaseObject in mos.Get())
					{
						foreach (var propertyData in managementBaseObject.Properties)
						{
							switch (propertyData.Name)
							{
								case "Name":
								{
									information.Cpu.Name = propertyData.Value.ToString().Trim();

									break;
								}

								case "L2CacheSize":
								{
									information.Cpu.L2CacheSize = int.Parse(propertyData.Value.ToString());

									break;
								}

								case "L3CacheSize":
								{
									information.Cpu.L3CacheSize = int.Parse(propertyData.Value.ToString());
									break;
								}

								// MIND THE SSSSSSSS
								case "NumberOfEnabledCore":
								{
									information.Cpu.PhysicalCores = int.Parse(propertyData.Value.ToString());

									break;
								}

								case "NumberOfLogicalProcessors":
								{
									information.Cpu.LogicalCores = int.Parse(propertyData.Value.ToString());

									break;
								}

								case "SocketDesignation":
								{
									information.Cpu.Socket = propertyData.Value.ToString().Trim();

									break;
								}

								case "MaxClockSpeed":
								{
									information.Cpu.MaxClockSpeed = int.Parse(propertyData.Value.ToString());

									break;
								}
							}
						}
					}

					mos = new ManagementObjectSearcher(
						"select ConfiguredClockSpeed,Manufacturer,Capacity from Win32_PhysicalMemory");

					foreach (var managementBaseObject in mos.Get())
					{
						foreach (var propertyData in managementBaseObject.Properties)
						{
							switch (propertyData.Name)
							{
								case "ConfiguredClockSpeed":
								{
									information.Ram.Speed = long.Parse(propertyData.Value.ToString());
									break;
								}

								case "Manufacturer":
								{
									information.Ram.Manfucturer = propertyData.Value.ToString();

									break;
								}

								case "Capacity":
								{
									information.Ram.Capacity += long.Parse(propertyData.Value.ToString());
									break;
								}
							}
						}
					}

					mos = new ManagementObjectSearcher("select Caption from Win32_BIOS");

					foreach (var managementBaseObject in mos.Get())
					{
						foreach (var propertyData in managementBaseObject.Properties)
						{
							if (propertyData.Name == "Caption")
							{
								information.Cpu.BIOSVersion = propertyData.Value.ToString();
							}
						}
					}
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					var info = File.ReadAllLines("/proc/cpuinfo");
					var modelNameRegex = new Regex(@"^model name\s+:\s+(.+)");
					var cpuSpeedRegex = new Regex(@"^cpu MHz\s+:\s+(.+)");
					var physicalCoresRegex = new Regex(@"^cpu cores\s+:\s+(.+)");
					var logicalCoresRegex = new Regex(@"^siblings\s+:\s+(.+)");

					foreach (var s in info)
					{
						var match = modelNameRegex.Match(s);

						if (match.Success)
						{
							information.Cpu.Name = match.Groups[1].Value.Trim();

							continue;
						}

						match = cpuSpeedRegex.Match(s);

						if (match.Success)
						{
							information.Cpu.MaxClockSpeed = int.Parse(match.Groups[1].Value);

							continue;
						}

						match = physicalCoresRegex.Match(s);

						if (match.Success)
						{
							information.Cpu.PhysicalCores = int.Parse(match.Groups[1].Value);

							continue;
						}

						match = logicalCoresRegex.Match(s);

						if (match.Success)
						{
							information.Cpu.LogicalCores = int.Parse(match.Groups[1].Value);
						}
					}

					try
					{
						information.Cpu.BIOSVersion = File.ReadAllText("/sys/class/dmi/id/bios_version").Trim();
					}
					catch (Exception)
					{
						// Intentionally left blank
					}

					try
					{
						var memInfo = File.ReadAllLines("/proc/meminfo");

						foreach (var s in memInfo)
						{
							if (s.Trim().StartsWith("MemTotal"))
							{
								var value = long.Parse(s.Replace("MemTotal", "").Replace("kB", "").Trim());

								value *= 1000;

								information.Ram.Capacity = value;
							}
						}
					}
					catch (Exception)
					{
						// Intentionally left blank
					}

					try
					{
						var memInfo = new List<string>();

						using (var p = Process.Start("dmidecode", "-t 17"))
						{
							using (var sr = p.StandardOutput)
							{
								p.WaitForExit();

								while (!sr.EndOfStream)
								{
									memInfo.Add(sr.ReadLine());
								}
							}
						}

						foreach (var s in memInfo)
						{
							if (s.Trim().StartsWith("Speed"))
							{
								var value = long.Parse(s.Replace("Speed", "").Replace("MHz", "").Trim());

								information.Ram.Speed = value;
							}
							else if (s.Trim().StartsWith("Manufacturer"))
							{
								var value = s.Replace("Manufacturer", "").Trim();

								information.Ram.Manfucturer = value;
							}
						}
					}
					catch (Exception)
					{
						// Intentionally left blank
					}
				}

				information.Ram.CapacityHRF = FormatBytes(information.Ram.Capacity);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			return information;
		}

		private static string FormatBytes(long bytes)
		{
			string[] Suffix = {"B", "KB", "MB", "GB", "TB"};
			int i;
			double dblSByte = bytes;
			for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
			{
				dblSByte = bytes / 1024.0;
			}

			return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
		}
	}
}