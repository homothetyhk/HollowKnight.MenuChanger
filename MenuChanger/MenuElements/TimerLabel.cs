using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuChanger.MenuElements
{
    public class TimerLabel : MenuLabel
    {
        private Stopwatch Stopwatch;
        private string Prefix;
        private string RestLabel;
        private IEnumerator maintainTime;

        public TimerLabel(MenuPage page, string prefix, string restLabel = null) : base(page, string.Empty, Style.Body)
        {
            Prefix = $"{prefix}: ";
            RestLabel = restLabel ?? string.Empty;
            Stopwatch = new Stopwatch();
            Text.alignment = TextAnchor.UpperCenter;
        }

        public void Start()
        {
            Stopwatch.Reset();
            Stopwatch.Start();

            maintainTime = MaintainTime();
            Text.StartCoroutine(maintainTime);
        }

        private void StopInternal()
        {
            if (maintainTime is IEnumerator)
            {
                Text.StopCoroutine(maintainTime);
                maintainTime = null;
            }
        }

        public void Stop(string stopLabel = null)
        {
            StopInternal();
            if (stopLabel is string s) Text.text = s;
        }

        public void Reset(string newRestLabel = null)
        {
            StopInternal();
            if (newRestLabel is string s) RestLabel = s;
            Text.text = RestLabel;
        }


        private IEnumerator MaintainTime()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                Text.text = ComputeLabelText();
            }
        }

        private string ComputeLabelText()
        {
            StringBuilder sb = new StringBuilder(Prefix);
            int hours = Stopwatch.Elapsed.Hours;
            int minutes = Stopwatch.Elapsed.Minutes;
            int seconds = Stopwatch.Elapsed.Seconds;
            int milli = Stopwatch.Elapsed.Milliseconds;

            bool hh = hours > 0;
            bool mm = hh || minutes > 0;

            if (hh)
            {
                sb.Append(hours);
                sb.Append(':');
            }
            if (mm)
            {
                sb.Append(minutes);
                sb.Append(':');
            }
            sb.Append(seconds);
            sb.Append('.');
            sb.Append(milli.ToString().PadLeft(3, '0'));

            return sb.ToString();
        }
    }
}
