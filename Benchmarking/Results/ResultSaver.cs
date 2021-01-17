#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Benchmarking.Util;
using HardwareInformation;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;

#endregion

namespace Benchmarking.Results
{
    public class ResultSaver
    {
        private const string SAVE_DIRECTORY = "save";

        private readonly List<Save> saves;
        private Save? currentSave;

        public ResultSaver()
        {
            saves = new List<Save>();

            if (!Directory.Exists(SAVE_DIRECTORY))
            {
                Directory.CreateDirectory(SAVE_DIRECTORY);
            }

            LoadSaves();
        }

        private void LoadSaves()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver()
            };

            foreach (var saveFile in Directory.GetFiles(SAVE_DIRECTORY, "*.json"))
            {
                using var stream = File.OpenRead(saveFile);
                using var sr = new StreamReader(stream);
                var save = JsonConvert.DeserializeObject<Save>(sr.ReadToEnd(), settings)!;

                saves.Add(save);
            }
        }

        public void WriteSaves(bool saveCurrentSave = true)
        {
            if (saveCurrentSave && currentSave != null)
            {
                saves.Add(currentSave);
            }

            currentSave = null;

            foreach (var save in saves)
            {
                var saveFile = $"{SAVE_DIRECTORY}/{save.Created}.json";

                if (!File.Exists(saveFile))
                {
                    using var stream = File.OpenWrite(saveFile);
                    using var sw = new StreamWriter(stream);
                    sw.Write(JsonConvert.SerializeObject(save));
                }
            }
        }

        public void CreateOrUpdateSaveForCurrentRun(MachineInformation machineInformation,
            IEnumerable<Result> results)
        {
            currentSave ??= new Save
            {
                Version =
                    Assembly.GetExecutingAssembly().GetName().Version ?? throw new InvalidOperationException(),
                DotNetVersion = RuntimeInformation.FrameworkDescription,
                MachineInformation = machineInformation,
                Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            var singleThreaded = currentSave.SingleThreadedResults;
            var multiThreaded = currentSave.MultiThreadedResults;

            foreach (var result in results)
            {
                if (result.MultiThreaded)
                {
                    AddOrUpdateResult(result, ref multiThreaded);
                }
                else
                {
                    AddOrUpdateResult(result, ref singleThreaded);
                }
            }

            currentSave.SingleThreadedResults = singleThreaded;
            currentSave.MultiThreadedResults = multiThreaded;
            currentSave.OverallMultiThreaded = multiThreaded.Count > 0
                ? Helper.GetGeometricMean(multiThreaded.Select(result => result.Iterations))
                : 0uL;
            currentSave.OverallSingleThreaded = singleThreaded.Count > 0
                ? Helper.GetGeometricMean(singleThreaded.Select(result => result.Iterations))
                : 0uL;
            currentSave.SingleThreadedPerCategory = GetResultByCategory(singleThreaded);
            currentSave.MultiThreadedPerCategory = GetResultByCategory(multiThreaded);
        }

        private void AddOrUpdateResult(Result result, ref List<Result> results)
        {
            var existingResult = results.FirstOrDefault(res => res.Benchmark == result.Benchmark);

            if (existingResult is not null)
            {
                results.Remove(existingResult);
            }

            results.Add(result);
        }

        private Dictionary<string, ulong> GetResultByCategory(List<Result> results)
        {
            var resultsByCategoryList = new Dictionary<string, List<ulong>>();

            foreach (var result in results)
            {
                foreach (var category in result.Categories)
                {
                    if (!resultsByCategoryList.ContainsKey(category))
                    {
                        resultsByCategoryList.Add(category, new List<ulong>());
                    }

                    resultsByCategoryList[category].Add(result.Iterations);
                }
            }

            var resultsByCategory = new Dictionary<string, ulong>();

            foreach (var kvp in resultsByCategoryList)
            {
                resultsByCategory.Add(kvp.Key, Helper.GetGeometricMean(kvp.Value));
            }

            return resultsByCategory;
        }

        public List<string> GetListOfSaves()
        {
            var names = saves.Select(save => save.Created.ToString()).ToList();

            if (currentSave != null)
            {
                names.Add("current");
            }

            return names;
        }

        public Save? GetSave(string name)
        {
            if (name == "current")
            {
                return currentSave;
            }

            return saves.FirstOrDefault(sav => sav.Created.ToString() == name);
        }

        public void Clear()
        {
            saves.Clear();
            currentSave = null;
            Directory.Delete(SAVE_DIRECTORY);
        }
    }
}