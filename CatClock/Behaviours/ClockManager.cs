using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CatClock.Behaviours
{
    public class CatClockManager : MonoBehaviour
    {
        public TextMeshPro clockTime;
        public TextMeshPro clockAMPM;

        public void Initialize(GameObject clockPrefab)
        {
            clockTime = clockPrefab.transform.Find("Clock/Time")?.GetComponent<TextMeshPro>();
            clockAMPM = clockPrefab.transform.Find("Clock/AMPM")?.GetComponent<TextMeshPro>();

            if (clockTime != null && clockAMPM != null)
            {
                StartCoroutine(UpdateClockCoroutine());
            }
        }

        private IEnumerator UpdateClockCoroutine()
        {
            while (true)
            {
                DateTime now = DateTime.Now;

                if (clockTime != null && clockAMPM != null)
                {
                    clockTime.text = now.ToString("hh:mm");
                    clockAMPM.text = now.ToString("tt");
                }

                int secondsUntilNextMinute = 60 - now.Second;
                yield return new WaitForSeconds(secondsUntilNextMinute);
            }
        }
    }
}
