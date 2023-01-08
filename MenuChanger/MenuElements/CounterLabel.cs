﻿namespace MenuChanger.MenuElements
{
    /// <summary>
    /// Object which manages a text box displaying a counter that can be incremented.
    /// </summary>
    public class CounterLabel : MenuLabel
    {
        private int Counter;
        private string Prefix;

        /// <summary>
        /// Creates a label on the page with the specified prefix and initial counter value.
        /// </summary>
        public CounterLabel(MenuPage page, string prefix, int startValue = 0) : base(page, $"{prefix}: {startValue}", Style.Body)
        {
            Counter = startValue;
            Prefix = $"{prefix}: ";
            Text.alignment = UnityEngine.TextAnchor.UpperCenter;
        }

        public string ComputeText()
        {
            return $"{Prefix}{Counter}";
        }

        public void Update()
        {
            Text.text = ComputeText();
        }

        public void Incr(int val = 1)
        {
            Counter += val;
            Update();
        }

        public void Set(int val)
        {
            Counter = val;
            Update();
        }
    }
}
