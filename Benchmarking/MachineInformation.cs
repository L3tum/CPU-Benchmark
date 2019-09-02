#region using

using System;
using System.Management;
using System.Runtime.InteropServices;

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

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				try
				{
					var mos = new ManagementObjectSearcher("select * from Win32_Processor");

					foreach (var managementBaseObject in mos.Get())
					{
						foreach (var propertyData in managementBaseObject.Properties)
						{
							switch (propertyData.Name)
							{
								case "Name":
								{
									information.Cpu.Name = (string) propertyData.Value;

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
									information.Cpu.Socket = (string) propertyData.Value;

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

					mos = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");

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

					information.Ram.CapacityHRF = FormatBytes(information.Ram.Capacity);

					mos = new ManagementObjectSearcher("select * from Win32_BIOS");

					foreach (var managementBaseObject in mos.Get())
					{
						foreach (var propertyData in managementBaseObject.Properties)
						{
							switch (propertyData.Name)
							{
								case "Caption":
								{
									information.Cpu.BIOSVersion = propertyData.Value.ToString();

									break;
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}


			return information;
		}

		private static string FormatBytes(long bytes)
		{
			string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
			int i;
			double dblSByte = bytes;
			for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
			{
				dblSByte = bytes / 1024.0;
			}

			return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
		}
	}
}