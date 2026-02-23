using UnityEngine;
using CatClock.Tools;
using CatClock.Models;

namespace CatClock.Behaviours
{
    public class ClockActions : MonoBehaviour

        //i really need to improve this in the future it makes my head hurt...
    {
        private Transform _clockTransform;
        private Transform _head;

        public Vector3 HeadOffset = new Vector3(0.5f, -0.10f, -35f);

        public void Initialize(GameObject clockPrefab)
        {
            _clockTransform = clockPrefab.transform;

            var holdable = clockPrefab.GetComponent<DevHoldable>();
            if (holdable != null)
                holdable.OnPickUp += HandlePickUp;

            _head = GorillaTagger.Instance.offlineVRRig.head.rigTarget;
        }

        private void Update()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                BringClockToMonke();
            }
        }

        private void BringClockToMonke()
        {
            if (_clockTransform == null || _head == null)
                return;

            _clockTransform.SetParent(null);

            Vector3 forward = _head.forward;
            Vector3 up = _head.up;
            Vector3 right = _head.right;

            Vector3 pos =
                _head.position +
                forward * 0.34f +  
                up * -0.05f;      

            _clockTransform.position = pos;

            Quaternion rot = Quaternion.LookRotation(forward, up);
            _clockTransform.rotation = rot;

            AudioManager.Instance?.Play(EAudioType.bring, false, 0.35f);
        }


        private void HandlePickUp()
        {
            AudioManager.Instance?.Play(EAudioType.pick, isLeftHand: true, volume: 0.35f);
        }
    }
}
