using System;
using System.Threading.Tasks;
using BepInEx;
using CatClock.Tools;
using UnityEngine;
using CatClock.Behaviours;

namespace CatClock
{
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        private GameObject _clockPrefab;
        private bool _initialized;

        private void Awake()
        {
            Instance = this;

            if (gameObject.GetComponent<AudioManager>() == null)
            {
                gameObject.AddComponent<AudioManager>();
            }

            GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
        }

        private void OnPlayerSpawned()
        {
            if (_initialized) return;
            _initialized = true;
            _ = SetupClock();
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
                clockInstance.SetActive(false);

                var holdable = clockInstance.AddComponent<DevHoldable>();
                holdable.enabled = false;

                var actions = clockInstance.AddComponent<ClockActions>();
                actions.enabled = false;

                var manager = clockInstance.AddComponent<ClockManager>();
                manager.enabled = false;

                var bobber = clockInstance.AddComponent<ClockBobber>();
                bobber.enabled = false;

                clockInstance.transform.position = new Vector3(-65.8154f, 11.846f, -80.2366f);
                clockInstance.transform.rotation = Quaternion.Euler(0f, 17.8007f, 0f);
                clockInstance.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                bobber.enabled = true;
                holdable.enabled = true;
                actions.enabled = true;
                manager.enabled = true;

                manager.Initialize(clockInstance);
                bobber.Initialize(clockInstance);
                actions.Initialize(clockInstance);

                clockInstance.SetActive(true);

                Debug.Log("[CatClock] Tick Tock Monke!!");
            }
            catch (Exception ex)
            {
                Debug.LogError("[CatClock] Error setting up clock: " + ex);
            }
        }
    }
}
