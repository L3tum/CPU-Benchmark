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
		public List<Result> Results;
		public string UUID;
		public Version Version;

		public Save()
		{
			Results = new List<Result>();
		}
	}
}