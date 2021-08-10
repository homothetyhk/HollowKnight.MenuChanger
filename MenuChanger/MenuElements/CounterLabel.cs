using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuChanger.MenuElements
{
    public class CounterLabel : MenuLabel
    {
        private int Counter;
        private string Prefix;

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
