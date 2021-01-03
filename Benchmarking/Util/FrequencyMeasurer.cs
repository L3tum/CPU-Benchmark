using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using Benchmarking.Results;

namespace Benchmarking.Util
{
    public class FrequencyMeasurer
    {
        private readonly double baseFrequency;
        private readonly List<double> measurements;
        private readonly Thread measuringThread;
        private readonly CancellationTokenSource measuringThreadTokenSource;

        public FrequencyMeasurer()
        {
            measurements = new List<double>();
            measuringThreadTokenSource = new CancellationTokenSource();
            measuringThread = new Thread(() => MeasureCpuFrequency(measuringThreadTokenSource.Token))
            {
                Priority = ThreadPriority.Highest, IsBackground = true
            };
            measuringThread.Start();

            foreach (ManagementBaseObject obj in
                new ManagementObjectSearcher("SELECT *, Name FROM Win32_Processor").Get())
            {
                baseFrequency = Convert.ToDouble(obj["MaxClockSpeed"]) / 1000;
            }
        }

        public bool MeasuringThreadReady { get; private set; }

        ~FrequencyMeasurer()
        {
            StopMeasurements();
        }

        public void StopMeasurements()
        {
            measuringThreadTokenSource.Cancel();
            measuringThread.Join();
        }

        public void ClearMeasurements()
        {
            lock (measurements)
            {
                measurements.Clear();
            }
        }

        public Result.FrequencyMeasurement GetMeasurements()
        {
            lock (measurements)
            {
                var frequencies = measurements.Select(frequency => baseFrequency * frequency / 100).ToList();
                var averageFrequency = frequencies.Average();
                var highestFrequency = frequencies.Max();
                var lowestFrequency = frequencies.Min();

                return new Result.FrequencyMeasurement
                {
                    BaseFrequency = (int) (baseFrequency * 1000), AverageFrequency = (int) (averageFrequency * 1000),
                    HighestFrequency = (int) (highestFrequency * 1000), LowestFrequency = (int) (lowestFrequency * 1000)
                };
            }
        }

        /// <summary>
        ///     Asynchronously measures CPU frequency and returns base, average, highest
        ///     and lowest frequency in MHz
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private void MeasureCpuFrequency(CancellationToken cancellationToken)
        {
            using var cpuCounter =
                new PerformanceCounter("Processor Information", "% Processor Performance", "_Total");
            cpuCounter.NextValue();
            MeasuringThreadReady = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                lock (measurements)
                {
                    measurements.Add(cpuCounter.NextValue());

                    // Keep the list of measurements kinda short
                    if (measurements.Count > 1000)
                    {
                        measurements.RemoveRange(0, 100);
                    }
                }

                Thread.Sleep(500);
            }
        }
    }
}