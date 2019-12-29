﻿#region using

using System.Threading.Tasks;
using Benchmarking.Results;
using CPU_Benchmark_Common;
using Newtonsoft.Json;

#endregion

namespace Benchmarking.Parsing
{
	internal class JSONParser : Benchmark
	{
		private readonly uint volume = 3000;

		public JSONParser(Options options) : base(options)
		{
			volume *= BenchmarkRater.ScaleVolume(options.Threads);
		}

		public override void Run()
		{
			var threads = new Task[options.Threads];

			for (var i = 0; i < options.Threads; i++)
			{
				var i1 = i;

				threads[i1] = Task.Run(() =>
				{
					for (var i = 0; i < volume / options.Threads; i++)
					{
						var doc = JsonConvert.DeserializeObject<Save>(JSONFile.File);

						doc.MachineInformation.Cpu.Name = "Serialized";
					}

					BenchmarkRunner.ReportProgress();
				});
			}

			Task.WaitAll(threads);
		}

		public override double GetComparison()
		{
			switch (options.Threads)
			{
				case 1:
				{
					return 884.0d;
				}
				default:
				{
					return 141.0d;
				}
			}
		}

		public override string[] GetCategories()
		{
			return new[] {"parsing"};
		}

		public override double GetDataThroughput(double timeInMillis)
		{
			return sizeof(char) * JSONFile.File.Length * volume / (timeInMillis / 1000);
		}
	}

	internal sealed class JSONFile
	{
		// Save generated by this Benchmarker
		internal const string File =
			@"
{
  ""DotNetVersion"": "".NET Core 3.0.0"",
  ""MachineInformation"": {
    ""OperatingSystem"": {
      ""Platform"": ""Win32NT"",
      ""ServicePack"": """",
      ""Version"": ""6.2.9200.0"",
      ""VersionString"": ""Microsoft Windows NT 6.2.9200.0""
    },
    ""Platform"": ""Windows"",
    ""Cpu"": {
      ""ExtendedFeatureFlagsF7One"": ""FSGSBASE, BMI1, AVX2, SMEP, BMI2, PQM, PGE, RDSEED, ADX, SMAP, CLFLUSHOPT, CLWB, SHA"",
      ""ExtendedFeatureFlagsF7Two"": ""UMIP, RDPID"",
      ""ExtendedFeatureFlagsF7Three"": ""NONE"",
      ""PhysicalCores"": 12,
      ""LogicalCores"": 24,
      ""Nodes"": 1,
      ""LogicalCoresPerNode"": 24,
      ""Architecture"": ""X64"",
      ""Caption"": ""AMD64 Family 23 Model 113 Stepping 0, AuthenticAMD"",
      ""Name"": ""AMD Ryzen 9 3900X 12-Core Processor            "",
      ""Vendor"": ""AuthenticAMD"",
      ""Stepping"": 0,
      ""Model"": 113,
      ""Family"": 23,
      ""Type"": ""Original_OEM"",
      ""FeatureFlagsOne"": ""FPU, VME, DE, PSE, TSC, MSR, PAE, MCE, CX8, APIC, SEP, MTRR, PGE, MCA, CMOV, PAT, PSE36, CLFSH, MMX, FXSR, SSE, SSE2, HTT"",
      ""FeatureFlagsTwo"": ""SSE3, PCLMULQDQ, SSSE3, FMA, CX16, SSE4_1, SSE4_2, MOVBE, POPCNT, AES, XSAVE, OS_XSAVE, AVX, F16C, RDRND, HV"",
      ""MaxCpuIdFeatureLevel"": 13,
      ""MaxCpuIdExtendedFeatureLevel"": 2147483678,
      ""MaxClockSpeed"": 4306,
      ""NormalClockSpeed"": 3793,
      ""Cores"": [
        {
          ""Number"": 0,
          ""MaxClockSpeed"": 4292,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 0
        },
        {
          ""Number"": 1,
          ""MaxClockSpeed"": 4306,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 0
        },
        {
          ""Number"": 2,
          ""MaxClockSpeed"": 4287,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 1
        },
        {
          ""Number"": 3,
          ""MaxClockSpeed"": 4268,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 1
        },
        {
          ""Number"": 4,
          ""MaxClockSpeed"": 4267,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 2
        },
        {
          ""Number"": 5,
          ""MaxClockSpeed"": 4258,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 2
        },
        {
          ""Number"": 6,
          ""MaxClockSpeed"": 4295,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 4
        },
        {
          ""Number"": 7,
          ""MaxClockSpeed"": 4285,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 4
        },
        {
          ""Number"": 8,
          ""MaxClockSpeed"": 4298,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 5
        },
        {
          ""Number"": 9,
          ""MaxClockSpeed"": 4291,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 5
        },
        {
          ""Number"": 10,
          ""MaxClockSpeed"": 4248,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 6
        },
        {
          ""Number"": 11,
          ""MaxClockSpeed"": 4246,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 6
        },
        {
          ""Number"": 12,
          ""MaxClockSpeed"": 4247,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 8
        },
        {
          ""Number"": 13,
          ""MaxClockSpeed"": 4247,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 8
        },
        {
          ""Number"": 14,
          ""MaxClockSpeed"": 4247,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 9
        },
        {
          ""Number"": 15,
          ""MaxClockSpeed"": 4243,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 9
        },
        {
          ""Number"": 16,
          ""MaxClockSpeed"": 4211,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 10
        },
        {
          ""Number"": 17,
          ""MaxClockSpeed"": 4210,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 10
        },
        {
          ""Number"": 18,
          ""MaxClockSpeed"": 4239,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 12
        },
        {
          ""Number"": 19,
          ""MaxClockSpeed"": 4246,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 12
        },
        {
          ""Number"": 20,
          ""MaxClockSpeed"": 4210,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 13
        },
        {
          ""Number"": 21,
          ""MaxClockSpeed"": 4210,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 13
        },
        {
          ""Number"": 22,
          ""MaxClockSpeed"": 4195,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 14
        },
        {
          ""Number"": 23,
          ""MaxClockSpeed"": 4197,
          ""NormalClockSpeed"": 3793,
          ""ReferenceMaxClockSpeed"": 0,
          ""ReferenceNormalClockSpeed"": 0,
          ""ReferenceBusSpeed"": 0,
          ""Node"": 0,
          ""CoreId"": 14
        }
      ],
      ""AMDFeatureFlags"": {
        ""ExtendedFeatureFlagsF81One"": ""LAHF_LM, CMP_LEGACY, CR8_LEGACY, ABM, SSE4A, MISALIGNSSE, THREEDNOW_PREFETCH, OS_VW, WDT, TOPOEXT"",
        ""ExtendedFeatureFlagsF81Two"": ""FPU, VME, DE, PSE, TSC, MSR, PAE, MCE, CX8, APIC, SYSCALL, MTRR, PGE, MCA, CMOV, PAT, PSE36, NX, MMXEXT, MMX, FXSR, FXSR_OPT, PDPE_1GB, RDTSCP, LM"",
        ""FeatureFlagsSvm"": ""NONE"",
        ""FeatureFlagsApm"": ""TS, TTP, HW_STATE, TSC_INVARIANT, CPB, EFF_FREQ_RO""
      },
      ""IntelFeatureFlags"": {
        ""TPMFeatureFlags"": ""NONE"",
        ""ExtendedFeatureFlagsF81One"": ""NONE"",
        ""ExtendedFeatureFlagsF81Two"": ""NONE"",
        ""FeatureFlagsApm"": ""NONE""
      },
      ""Socket"": ""AM4"",
      ""Caches"": [
        {
          ""Level"": ""LEVEL1"",
          ""Type"": ""DATA"",
          ""WBINVD"": true,
          ""CoresPerCache"": 1,
          ""TimesPresent"": 12,
          ""Capacity"": 32768,
          ""CapacityHRF"": ""32 KiB"",
          ""Associativity"": 8,
          ""LineSize"": 64,
          ""Partitions"": 1,
          ""Sets"": 64
        },
        {
          ""Level"": ""LEVEL1"",
          ""Type"": ""INSTRUCTION"",
          ""WBINVD"": true,
          ""CoresPerCache"": 1,
          ""TimesPresent"": 12,
          ""Capacity"": 32768,
          ""CapacityHRF"": ""32 KiB"",
          ""Associativity"": 8,
          ""LineSize"": 64,
          ""Partitions"": 1,
          ""Sets"": 64
        },
        {
          ""Level"": ""LEVEL2"",
          ""Type"": ""UNIFIED"",
          ""WBINVD"": true,
          ""CoresPerCache"": 1,
          ""TimesPresent"": 12,
          ""Capacity"": 524288,
          ""CapacityHRF"": ""512 KiB"",
          ""Associativity"": 8,
          ""LineSize"": 64,
          ""Partitions"": 1,
          ""Sets"": 1024
        },
        {
          ""Level"": ""LEVEL3"",
          ""Type"": ""UNIFIED"",
          ""WBINVD"": false,
          ""CoresPerCache"": 3,
          ""TimesPresent"": 4,
          ""Capacity"": 16777216,
          ""CapacityHRF"": ""16 MiB"",
          ""Associativity"": 16,
          ""LineSize"": 64,
          ""Partitions"": 1,
          ""Sets"": 16384
        }
      ]
    },
    ""SmBios"": {
      ""BIOSVersion"": ""1001"",
      ""BIOSVendor"": ""American Megatrends Inc."",
      ""BIOSCodename"": ""ALASKA - 1072009"",
      ""BoardVendor"": ""ASUSTeK COMPUTER INC."",
      ""BoardName"": ""ROG CROSSHAIR VIII HERO"",
      ""BoardVersion"": ""Rev X.0x""
    },
    ""RAMSticks"": [
      {
        ""Speed"": 3200,
        ""Manfucturer"": ""Corsair"",
        ""Capacity"": 17179869184,
        ""CapacityHRF"": ""16 GiB"",
        ""Name"": ""DIMM_A2"",
        ""PartNumber"": ""CMK32GX4M2B3200C16  "",
        ""FormFactor"": ""DIMM""
      },
      {
        ""Speed"": 3200,
        ""Manfucturer"": ""Corsair"",
        ""Capacity"": 17179869184,
        ""CapacityHRF"": ""16 GiB"",
        ""Name"": ""DIMM_B2"",
        ""PartNumber"": ""CMK32GX4M2B3200C16  "",
        ""FormFactor"": ""DIMM""
      }
    ]
  },
  ""Results"": {
    ""1"": [
      {
        ""Benchmark"": ""arithmetic_fp16"",
        ""Points"": 42801.0,
        ""Timing"": 22430.0
      },
      {
        ""Benchmark"": ""ZIP"",
        ""Points"": 42174.0,
        ""Timing"": 24558.0
      },
      {
        ""Benchmark"": ""GZip"",
        ""Points"": 42237.0,
        ""Timing"": 24342.0
      },
      {
        ""Benchmark"": ""BZip2"",
        ""Points"": 47558.0,
        ""Timing"": 7224.0
      },
      {
        ""Benchmark"": ""Deflate"",
        ""Points"": 42698.0,
        ""Timing"": 22776.0
      },
      {
        ""Benchmark"": ""ZIP-decompression"",
        ""Points"": 49402.0,
        ""Timing"": 1737.0
      },
      {
        ""Benchmark"": ""GZip-decompression"",
        ""Points"": 47496.0,
        ""Timing"": 7411.0
      },
      {
        ""Benchmark"": ""Deflate-decompression"",
        ""Points"": 48025.0,
        ""Timing"": 5813.0
      },
      {
        ""Benchmark"": ""BZip2-decompression"",
        ""Points"": 44705.0,
        ""Timing"": 16148.0
      },
      {
        ""Benchmark"": ""arithmetic_int"",
        ""Points"": 45424.0,
        ""Timing"": 13846.0
      },
      {
        ""Benchmark"": ""arithmetic_float"",
        ""Points"": 47396.0,
        ""Timing"": 7717.0
      },
      {
        ""Benchmark"": ""arithmetic_double"",
        ""Points"": 45003.0,
        ""Timing"": 15191.0
      },
      {
        ""Benchmark"": ""AVX"",
        ""Points"": 47461.0,
        ""Timing"": 7520.0
      },
      {
        ""Benchmark"": ""SSE"",
        ""Points"": 48111.0,
        ""Timing"": 5557.0
      },
      {
        ""Benchmark"": ""SSE2"",
        ""Points"": 48817.0,
        ""Timing"": 3454.0
      },
      {
        ""Benchmark"": ""AVX2Int"",
        ""Points"": 47483.0,
        ""Timing"": 7452.0
      },
      {
        ""Benchmark"": ""FMA"",
        ""Points"": 49162.0,
        ""Timing"": 2437.0
      },
      {
        ""Benchmark"": ""Encryption"",
        ""Points"": 49373.0,
        ""Timing"": 1820.0
      },
      {
        ""Benchmark"": ""Decryption"",
        ""Points"": 49916.0,
        ""Timing"": 244.0
      },
      {
        ""Benchmark"": ""CSPRNG"",
        ""Points"": 44776.0,
        ""Timing"": 15919.0
      },
      {
        ""Benchmark"": ""HTMLParser"",
        ""Points"": 61730.0,
        ""Timing"": 969.0
      },
      {
        ""Benchmark"": ""JSONParser"",
        ""Points"": 49309.0,
        ""Timing"": 2008.0
      },
      {
        ""Benchmark"": ""Category: all"",
        ""Points"": 47216.0,
        ""Timing"": 343448.0
      },
      {
        ""Benchmark"": ""Category: arithmetic"",
        ""Points"": 45135.0,
        ""Timing"": 14868.0
      },
      {
        ""Benchmark"": ""Category: compression"",
        ""Points"": 43667.0,
        ""Timing"": 19725.0
      },
      {
        ""Benchmark"": ""Category: decompression"",
        ""Points"": 47407.0,
        ""Timing"": 7777.0
      },
      {
        ""Benchmark"": ""Category: int"",
        ""Points"": 47632.0,
        ""Timing"": 7122.0
      },
      {
        ""Benchmark"": ""Category: float"",
        ""Points"": 46642.0,
        ""Timing"": 10190.0
      },
      {
        ""Benchmark"": ""Category: parsing"",
        ""Points"": 55520.0,
        ""Timing"": 1488.0
      },
      {
        ""Benchmark"": ""Category: ml"",
        ""Points"": 42716.0,
        ""Timing"": 22716.0
      },
      {
        ""Benchmark"": ""Category: extension"",
        ""Points"": 48207.0,
        ""Timing"": 5284.0
      },
      {
        ""Benchmark"": ""Category: cryptography"",
        ""Points"": 48022.0,
        ""Timing"": 5994.0
      }
    ],
    ""24"": [
      {
        ""Benchmark"": ""arithmetic_fp16"",
        ""Points"": 49661.0,
        ""Timing"": 982.0
      },
      {
        ""Benchmark"": ""ZIP"",
        ""Points"": 48759.0,
        ""Timing"": 3625.0
      },
      {
        ""Benchmark"": ""GZip"",
        ""Points"": 57829.0,
        ""Timing"": 3205.0
      },
      {
        ""Benchmark"": ""BZip2"",
        ""Points"": 62720.0,
        ""Timing"": 4270.0
      },
      {
        ""Benchmark"": ""Deflate"",
        ""Points"": 59230.0,
        ""Timing"": 3017.0
      },
      {
        ""Benchmark"": ""ZIP-decompression"",
        ""Points"": 49908.0,
        ""Timing"": 266.0
      },
      {
        ""Benchmark"": ""GZip-decompression"",
        ""Points"": 50417.0,
        ""Timing"": 3568.0
      },
      {
        ""Benchmark"": ""Deflate-decompression"",
        ""Points"": 60323.0,
        ""Timing"": 2748.0
      },
      {
        ""Benchmark"": ""BZip2-decompression"",
        ""Points"": 53194.0,
        ""Timing"": 4778.0
      },
      {
        ""Benchmark"": ""arithmetic_int"",
        ""Points"": 54614.0,
        ""Timing"": 541.0
      },
      {
        ""Benchmark"": ""arithmetic_float"",
        ""Points"": 50325.0,
        ""Timing"": 306.0
      },
      {
        ""Benchmark"": ""arithmetic_double"",
        ""Points"": 52126.0,
        ""Timing"": 608.0
      },
      {
        ""Benchmark"": ""AVX"",
        ""Points"": 50629.0,
        ""Timing"": 1099.0
      },
      {
        ""Benchmark"": ""SSE"",
        ""Points"": 49628.0,
        ""Timing"": 1076.0
      },
      {
        ""Benchmark"": ""SSE2"",
        ""Points"": 49527.0,
        ""Timing"": 1371.0
      },
      {
        ""Benchmark"": ""AVX2Int"",
        ""Points"": 56727.0,
        ""Timing"": 952.0
      },
      {
        ""Benchmark"": ""FMA"",
        ""Points"": 53527.0,
        ""Timing"": 448.0
      },
      {
        ""Benchmark"": ""Encryption"",
        ""Points"": 51551.0,
        ""Timing"": 531.0
      },
      {
        ""Benchmark"": ""Decryption"",
        ""Points"": 50000.0,
        ""Timing"": 64.0
      },
      {
        ""Benchmark"": ""CSPRNG"",
        ""Points"": 52498.0,
        ""Timing"": 3804.0
      },
      {
        ""Benchmark"": ""HTMLParser"",
        ""Points"": 51422.0,
        ""Timing"": 1230.0
      },
      {
        ""Benchmark"": ""JSONParser"",
        ""Points"": 55259.0,
        ""Timing"": 1021.0
      },
      {
        ""Benchmark"": ""Category: arithmetic"",
        ""Points"": 51681.0,
        ""Timing"": 612.0
      },
      {
        ""Benchmark"": ""Category: compression"",
        ""Points"": 57134.0,
        ""Timing"": 3529.0
      },
      {
        ""Benchmark"": ""Category: decompression"",
        ""Points"": 53460.0,
        ""Timing"": 2840.0
      },
      {
        ""Benchmark"": ""Category: int"",
        ""Points"": 52486.0,
        ""Timing"": 1210.0
      },
      {
        ""Benchmark"": ""Category: float"",
        ""Points"": 50982.0,
        ""Timing"": 755.0
      },
      {
        ""Benchmark"": ""Category: parsing"",
        ""Points"": 53340.0,
        ""Timing"": 1126.0
      },
      {
        ""Benchmark"": ""Category: ml"",
        ""Points"": 49658.0,
        ""Timing"": 991.0
      },
      {
        ""Benchmark"": ""Category: extension"",
        ""Points"": 52008.0,
        ""Timing"": 989.0
      },
      {
        ""Benchmark"": ""Category: cryptography"",
        ""Points"": 51350.0,
        ""Timing"": 1466.0
      },
      {
        ""Benchmark"": ""Category: all"",
        ""Points"": 52581.0,
        ""Timing"": 1494.0
      }
    ]
  },
  ""UUID"": ""ayrZREghJ4D7FPNN66dshQlbJSVDS/zWPYmI/5nuXSg="",
  ""Version"": ""0.0.0.0"",
  ""Uploaded"": -8586245513945705456
}";
	}
}