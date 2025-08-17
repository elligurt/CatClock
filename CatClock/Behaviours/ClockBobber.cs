using UnityEngine;

namespace CatClock.Behaviours

{
    public class ClockBobber : MonoBehaviour
    {

        public static float BobbingAmplitude = 0.03f;
        public static float BobbingSpeed = 0.4f;

        private float amplitude;
        private float speed;

        private Transform clockTransform;
        private Vector3 initialPosition;
        private float interpolationTime = 5f;

        public void Initialize(GameObject clockPrefab)
        {
            clockTransform = clockPrefab.transform.Find("Clock");
            if (clockTransform == null)
            {
                Debug.LogWarning("[CatClock] Clock not found");
                return;
            }

            initialPosition = clockTransform.localPosition;

            amplitude = BobbingAmplitude;
            speed = BobbingSpeed;

            enabled = true;
        }

        private void Update()
        {
            if (clockTransform == null) return;

            interpolationTime += Time.deltaTime * speed;
            float t = Mathf.PingPong(interpolationTime, 1f); 
            float yOffset = Mathf.Lerp(-amplitude, amplitude, t);
            clockTransform.localPosition = initialPosition + new Vector3(0f, yOffset, 0f);
        }
    }
}
