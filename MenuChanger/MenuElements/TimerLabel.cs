using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// MenuElement which manages a text box displaying a timer tied to a Stopwatch.
    /// </summary>
    public class TimerLabel : MenuLabel
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _prefix;
        private string _restLabel;
        private IEnumerator maintainTime;
        private readonly StringBuilder _sb = new();

        /// <summary>
        /// Creates a new TimerLabel. The optional rest label will be used until the timer is started, or else the label will be blank.
        /// <br/>When the timer is started, the label will display the prefix, followed by the current time.
        /// </summary>
        public TimerLabel(MenuPage page, string prefix, string restLabel = null) : base(page, string.Empty, Style.Body)
        {
            _prefix = $"{prefix}: ";
            _restLabel = restLabel ?? string.Empty;
            _stopwatch = new Stopwatch();
            Text.alignment = TextAnchor.UpperCenter;
        }

        public void Start()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            maintainTime = MaintainTime();
            Text.StartCoroutine(maintainTime);
        }

        private void StopInternal()
        {
            if (maintainTime is not null)
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
            if (newRestLabel is string s) _restLabel = s;
            Text.text = _restLabel;
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
            _sb.Clear();
            _sb.Append(_prefix);

            int hours = _stopwatch.Elapsed.Hours;
            int minutes = _stopwatch.Elapsed.Minutes;
            int seconds = _stopwatch.Elapsed.Seconds;
            int milli = _stopwatch.Elapsed.Milliseconds;

            bool hh = hours > 0;
            bool mm = hh || minutes > 0;

            if (hh)
            {
                _sb.Append(hours);
                _sb.Append(':');
            }
            if (mm)
            {
                _sb.Append(minutes);
                _sb.Append(':');
            }
            _sb.Append(seconds);
            _sb.Append('.');
            _sb.Append(milli.ToString().PadLeft(3, '0'));

            return _sb.ToString();
        }
    }
}
