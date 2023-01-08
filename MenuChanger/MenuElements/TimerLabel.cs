using System.Collections;
using System.Diagnostics;
using System.Text;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// MenuElement which manages a text box displaying a timer tied to a Stopwatch.
    /// </summary>
    public class TimerLabel : MenuLabel
    {
        private readonly Stopwatch _stopwatch;
        private string _prefix;
        private string _restLabel;
        private IEnumerator maintainTime;
        private readonly StringBuilder _sb = new();

        /// <summary>
        /// Creates a new TimerLabel. The optional rest label will be used until the timer is started, or else the label will be blank.
        /// <br/>When the timer is started, the label will display the prefix, followed by the current time.
        /// </summary>
        public TimerLabel(MenuPage page, string prefix, string restLabel = null) : base(page, string.Empty, Style.Body)
        {
            SetPrefix(prefix);
            _restLabel = restLabel ?? string.Empty;
            _stopwatch = new Stopwatch();
            Text.alignment = TextAnchor.UpperCenter;
        }

        /// <summary>
        /// Resets and starts the timer.
        /// </summary>
        public void Start()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            maintainTime = MaintainTime();
            Text.StartCoroutine(maintainTime);
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
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

        public void SetPrefix(string newPrefix)
        {
            _prefix = $"{newPrefix}: ";
        }

        /// <summary>
        /// Stops the timer, sets the prefix to a new value, and restarts the timer. Returns the final time as a string, without prefix.
        /// </summary>
        public string Split(string newPrefix)
        {
            StopInternal();
            string time = ComputeTimeText();
            SetPrefix(newPrefix);
            Start();
            return time;
        }

        public TimeSpan Elapsed { get => _stopwatch.Elapsed; }

        private IEnumerator MaintainTime()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                Text.text = ComputeLabelText();
            }
        }

        private string ComputeTimeText()
        {
            _sb.Clear();

            int hours = _stopwatch.Elapsed.Hours;
            int minutes = _stopwatch.Elapsed.Minutes;
            int seconds = _stopwatch.Elapsed.Seconds;
            int milli = _stopwatch.Elapsed.Milliseconds;

            bool hh = hours > 0;
            bool mm = hh || minutes > 0;
            bool ss = true;

            if (hh)
            {
                _sb.Append(hours);
                _sb.Append(':');
            }
            if (mm)
            {
                if (hh) _sb.Append(minutes.ToString().PadLeft(2, '0'));
                else _sb.Append(minutes);
                _sb.Append(':');
            }
            if (ss)
            {
                if (mm) _sb.Append(seconds.ToString().PadLeft(2, '0'));
                else _sb.Append(seconds);
            }
            _sb.Append('.');
            _sb.Append(milli.ToString().PadLeft(3, '0'));

            return _sb.ToString();
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
            bool ss = true;

            if (hh)
            {
                _sb.Append(hours);
                _sb.Append(':');
            }
            if (mm)
            {
                if (hh) _sb.Append(minutes.ToString().PadLeft(2, '0'));
                else _sb.Append(minutes);
                _sb.Append(':');
            }
            if (ss)
            {
                if (mm) _sb.Append(seconds.ToString().PadLeft(2, '0'));
                else _sb.Append(seconds);
            }
            _sb.Append('.');
            _sb.Append(milli.ToString().PadLeft(3, '0'));

            return _sb.ToString();
        }
    }
}
