using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using CatClock.Models;
using CatClock.Tools;

namespace CatClock.Tools
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private readonly Dictionary<EAudioType, AudioClip> audioClips =
            new Dictionary<EAudioType, AudioClip>();

        private bool _loaded;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _ = LoadAudioClips();
        }

        public async Task LoadAudioClips()
        {
            if (_loaded) return;
            _loaded = true;

            foreach (EAudioType type in System.Enum.GetValues(typeof(EAudioType)))
            {
                AudioClip clip = await AssetLoader.LoadAsset<AudioClip>(type.ToString());
                if (clip != null)
                {
                    audioClips[type] = clip;
                    Debug.Log($"[CatClock] loaded audio clips");
                }
                else
                {
                    Debug.LogWarning($"[CatClock] failed to load audio clip");
                }
            }
        }

        public AudioClip Get(EAudioType type)
        {
            audioClips.TryGetValue(type, out var clip);
            return clip;
        }

        public void Play(EAudioType type, bool isLeftHand, float volume = 0.35f)
        {
            if (!audioClips.TryGetValue(type, out var clip) || clip == null)
            {
                Debug.LogWarning($"[CatClock] audio clip not found");
                return;
            }

            var rig = GorillaTagger.Instance?.offlineVRRig;
            if (rig == null)
            {
                Debug.LogWarning("[CatClock] hands can not play audio");
                return;
            }

            AudioSource device = isLeftHand ? rig.leftHandPlayer : rig.rightHandPlayer;

            if (device == null)
            {
                Debug.LogWarning("[CatClock] cannot play audio");
                return;
            }

            device.GTPlayOneShot(clip, volume);
        }
    }
}
