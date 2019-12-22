#region using

using System;
using System.Collections.Generic;
using HardwareInformation;

#endregion

namespace Benchmarking.Results
{
	public class Save
	{
		public string DotNetVersion;
		public MachineInformation MachineInformation;
		public Dictionary<uint, List<Result>> Results;
		public string UUID;
		public Version Version;
		public long Uploaded;

		public Save()
		{
			Results = new Dictionary<uint, List<Result>>();
			Uploaded = 0;
		}
	}
}