using System;
using System.Collections.Generic;
using HardwareInformation;

namespace Benchmarking.Results
{
    /// <summary>
    ///     Benchmark save
    /// </summary>
    public class Save
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Save()
        {
            MultiThreadedResults = new List<Result>();
            SingleThreadedResults = new List<Result>();
            SingleThreadedPerCategory = new Dictionary<string, ulong>();
            MultiThreadedPerCategory = new Dictionary<string, ulong>();
        }

        /// <summary>
        ///     Version of .NET the benchmark was run with
        /// </summary>
        public string? DotNetVersion { get; set; }

        /// <summary>
        ///     Contains the information on the PC
        /// </summary>
        public MachineInformation? MachineInformation { get; set; }

        /// <summary>
        ///     Benchmark results
        /// </summary>
        public List<Result> MultiThreadedResults { get; set; }

        /// <summary>
        ///     Benchmark results
        /// </summary>
        public List<Result> SingleThreadedResults { get; set; }

        /// <summary>
        ///     MultiThreaded results per category
        /// </summary>
        public Dictionary<string, ulong> MultiThreadedPerCategory { get; set; }

        /// <summary>
        ///     SingleThreaded results per category
        /// </summary>
        public Dictionary<string, ulong> SingleThreadedPerCategory { get; set; }

        /// <summary>
        ///     Overall points for all multiThreaded results
        /// </summary>
        public ulong OverallMultiThreaded { get; set; }

        /// <summary>
        ///     Overall points for all singleThreaded results
        /// </summary>
        public ulong OverallSingleThreaded { get; set; }

        /// <summary>
        ///     Timestamp of when it was created
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        ///     Timestamp of when it was uploaded
        /// </summary>
        public long Uploaded { get; set; }

        /// <summary>
        ///     Unique identifier for this save
        /// </summary>
        public string? UUID { get; set; }

        /// <summary>
        ///     Version of the benchmark the save was created with
        /// </summary>
        public Version? Version { get; set; }
    }
}