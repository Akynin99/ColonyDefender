using System;
using System.Threading;
using ColonyDefender.Core;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

namespace ColonyDefender.Infrastructure.Services
{
    public class SaveService : ISaveService
    {
        private const string COLONY_DATA_KEY = "colony_data";
        private readonly ReactiveProperty<DateTime?> _lastSaveTime = new ReactiveProperty<DateTime?>();
        
        public IReadOnlyReactiveProperty<DateTime?> LastSaveTime => _lastSaveTime;

        public async UniTask SaveColonyDataAsync(ColonyData data, CancellationToken token)
        {
            try
            {
                string json = JsonConvert.SerializeObject(new SaveData
                {
                    Energy = data.Energy.Value,
                    Minerals = data.Minerals.Value,
                    Population = data.Population.Value,
                    SaveTime = DateTime.Now
                });

                await UniTask.RunOnThreadPool(() =>
                {
                    PlayerPrefs.SetString(COLONY_DATA_KEY, json);
                    PlayerPrefs.Save();
                }, cancellationToken: token);
                
                _lastSaveTime.Value = DateTime.Now;
                Debug.Log($"Colony data saved successfully at {_lastSaveTime.Value}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save colony data: {ex.Message}");
                throw;
            }
        }

        public IUniTaskAsyncEnumerable<ColonyData> LoadColonyDataAsync(CancellationToken token)
        {
            return UniTaskAsyncEnumerable.Create<ColonyData>(async (writer, cancelToken) =>
            {
                if (!PlayerPrefs.HasKey(COLONY_DATA_KEY))
                {
                    Debug.Log("No save data found. Creating new colony data.");
                    await writer.YieldAsync(new ColonyData());
                    return;
                }

                try
                {
                    string json = await UniTask.RunOnThreadPool(() => 
                        PlayerPrefs.GetString(COLONY_DATA_KEY),
                        cancellationToken: cancelToken);
                    
                    var saveData = JsonConvert.DeserializeObject<SaveData>(json);
                    
                    var colonyData = new ColonyData
                    {
                        Energy = { Value = saveData.Energy },
                        Minerals = { Value = saveData.Minerals },
                        Population = { Value = saveData.Population }
                    };
                    
                    _lastSaveTime.Value = saveData.SaveTime;
                    Debug.Log($"Colony data loaded successfully from {saveData.SaveTime}");
                    
                    await writer.YieldAsync(colonyData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load colony data: {ex.Message}");
                    await writer.YieldAsync(new ColonyData()); // Return default data in case of error
                }
            });
        }

        [Serializable]
        private class SaveData
        {
            public int Energy { get; set; }
            public int Minerals { get; set; }
            public int Population { get; set; }
            public DateTime SaveTime { get; set; }
        }
    }
} 