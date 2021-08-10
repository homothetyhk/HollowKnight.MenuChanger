using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuElements
{
    public class ToggleButton : MenuItem<bool>
    {
        public ToggleButton(MenuPage page, string text) : base(page, text, false, true)
        {
            
        }

        protected override void RefreshText(bool invokeEvent = true)
        {
            Text.text = InvokeFormat(Name, string.Empty, string.Empty);
            SetColor();
            _align.AlignText();

            if (invokeEvent)
            {
                InvokeChanged();
            }
        }

        public void SetColor()
        {
            switch (CurrentSelection)
            {
                case true when !Locked:
                    Text.color = Colors.TRUE_COLOR;
                    break;
                case true when Locked:
                    Text.color = Colors.LOCKED_TRUE_COLOR;
                    break;
                case false when Locked:
                    Text.color = Colors.LOCKED_FALSE_COLOR;
                    break;
                case false when !Locked:
                    Text.color = Colors.FALSE_COLOR;
                    break;
            }
        }

        public void SetColor(Color c)
        {
            Text.color = c;
        }

        public override void Lock()
        {
            Locked = true;
            SetColor();
        }

        public override void Unlock()
        {
            Locked = false;
            SetColor();
        }
    }
}
