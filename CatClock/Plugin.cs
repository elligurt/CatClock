using System;
using System.Threading.Tasks;
using BepInEx;
using CatClock.Tools;
using UnityEngine;
using Utilla;
using CatClock.Behaviours;

namespace CatClock
{
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private GameObject _clockPrefab;
        private CatClockManager _clockManager;

        void Start() => Utilla.Events.GameInitialized += OnGameInitialized;

        private async void OnGameInitialized(object sender, EventArgs e)
        {
            await SetupClock();
        }

        private async Task SetupClock()
        {
            try
            {
                _clockPrefab = await AssetLoader.LoadAsset<GameObject>("CatClock");
                if (_clockPrefab == null)
                {
                    Debug.LogError("[CatClock] Failed to load prefab.");
                    return;
                }

                GameObject clockInstance = Instantiate(_clockPrefab);
                clockInstance.SetActive(true);

                clockInstance.transform.position = new Vector3(-65.7865f, 11.7985f, - 79.762f);
                clockInstance.transform.rotation = Quaternion.Euler(358.6325f, 266.6017f, 359.3513f);
                clockInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                _clockManager = clockInstance.AddComponent<CatClockManager>();
                _clockManager.Initialize(clockInstance);

                ClockBobber bobber = clockInstance.AddComponent<ClockBobber>();
                bobber.Initialize(clockInstance);

                Debug.Log("[CatClock] Tick Tock Monke!!");
            }
            catch (Exception ex)
            {
                Debug.LogError("[CatClock] Error setting up clock: " + ex);
            }
        }
    }
}
