using CatClock.Tools;
using UnityEngine;
using System.Threading.Tasks;

namespace CatClock.Behaviours
{
    // this script manages bobbing, snapping to the init location, sounds, etc.
    public class ClockActions : MonoBehaviour
    {
        private const float BobbingAmplitude = 0.03f;
        private const float BobbingSpeed = 0.4f;
        private const float SnapDistance = 0.2f;

        private Transform _clockTransform;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private float _interpolationTime = 5f;

        private AudioSource _audioSource;
        private AudioClip _snapClip;
        private AudioClip _pickClip;

        private bool IsBeingGrabbed { get; set; }

        public async void Initialize(GameObject clockPrefab)
        {
            _clockTransform = clockPrefab.transform.Find("Clock");
            if (_clockTransform == null)
            {
                Debug.LogWarning("[CatClock] Clock not found");
                return;
            }

            _originalPosition = _clockTransform.position;
            _originalRotation = _clockTransform.rotation;
            _initialPosition = _originalPosition;
            _initialRotation = _originalRotation;

            _audioSource = _clockTransform.gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.volume = 0.3f;

            _snapClip = await AssetLoader.LoadAsset<AudioClip>("snap");
            _pickClip = await AssetLoader.LoadAsset<AudioClip>("pick");

            var holdable = _clockTransform.AddComponent<DevHoldable>();
            holdable.OnPickUp += HandlePickUp;
            holdable.OnPutDown += HandlePutDown;

            enabled = true;
        }

        private void HandlePickUp()
        {
            IsBeingGrabbed = true;
            
            if (Vector3.Distance(_clockTransform.position, _originalPosition) < 0.05f
                && _pickClip && _audioSource)
            {
                _audioSource.PlayOneShot(_pickClip);
            }
        }

        private void HandlePutDown()
        {
            IsBeingGrabbed = false;

            if (Vector3.Distance(_clockTransform.position, _originalPosition) < SnapDistance)
            {
                SnapToOriginal();
            }
            else
            {
                _initialPosition = _clockTransform.position;
                _initialRotation = _clockTransform.rotation;
            }
        }

        private void SnapToOriginal()
        {
            _clockTransform.position = _originalPosition;
            _clockTransform.rotation = _originalRotation;

            _initialPosition = _originalPosition;
            _initialRotation = _originalRotation;

            if (_snapClip && _audioSource)
            {
                _audioSource.PlayOneShot(_snapClip);
            }
        }

        private void Update()
        {
            if (!_clockTransform) return;

            if (IsBeingGrabbed)
            {
                _initialPosition = _clockTransform.position;
                Plugin.Instance.ClockPosition.Value = _clockTransform.position;
                Plugin.Instance.ClockRotation.Value = _clockTransform.rotation;
                Plugin.Instance.Config.Save();
                return;
            }
            
            _interpolationTime += Time.deltaTime * BobbingSpeed;
            var pingPong = Mathf.PingPong(_interpolationTime, 1f);
            var smoothT = Mathf.SmoothStep(0f, 1f, pingPong);

            var yOffset = Mathf.Lerp(-BobbingAmplitude, BobbingAmplitude, smoothT);
            _clockTransform.position = _initialPosition + new Vector3(0f, yOffset, 0f);
        }

        private void FixedUpdate()
        {
            if (!_clockTransform || !IsBeingGrabbed) return;

            transform.rotation = Quaternion.Euler(
                _initialRotation.eulerAngles.x,
                _clockTransform.rotation.eulerAngles.y,
                _initialRotation.eulerAngles.z
            );
        }
    }
}
