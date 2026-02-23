using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CatClock.Behaviours
{
    public class ClockManager : MonoBehaviour
    {
        public TMP_Text clockTime;
        public TMP_Text clockAMPM;

        private bool use24HourFormat;

        public void Initialize(GameObject clockObject)
        {
            if (!clockObject.activeInHierarchy)
            {
                clockObject.SetActive(true);
            }

            Transform timeTransform = clockObject.transform.Find("Clock/ClockFace/Time");
            Transform ampmTransform = clockObject.transform.Find("Clock/ClockFace/AMPM");

            if (timeTransform == null || ampmTransform == null)
            {
                Debug.LogError("[CatClock] could not find text objects");
                return;
            }

            clockTime = timeTransform.GetComponent<TMP_Text>();
            clockAMPM = ampmTransform.GetComponent<TMP_Text>();

            if (clockTime == null || clockAMPM == null)
            {
                Debug.LogError("[CatClock] Time or AMPM objects are missing");
                return;
            }

            use24HourFormat = !System.Globalization.DateTimeFormatInfo.CurrentInfo.LongTimePattern.Contains("tt");

            StartCoroutine(UpdateClockCoroutine());
        }

        private IEnumerator UpdateClockCoroutine()
        {
            while (true)
            {
                DateTime now = DateTime.Now;

                if (use24HourFormat)
                {
                    clockTime.text = now.ToString("HH:mm");
                    clockAMPM.text = "";
                }
                else
                {
                    clockTime.text = now.ToString("hh:mm");
                    clockAMPM.text = now.ToString("tt").ToUpper();
                }

                yield return new WaitForSeconds(60 - now.Second);
            }
        }
    }
}
