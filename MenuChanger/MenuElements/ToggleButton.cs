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
            _text.text = InvokeFormat(Name, string.Empty, string.Empty);
            SetColor();
            _align.AlignText();

            if (invokeEvent)
            {
                InvokeChanged();
            }
        }

        public void SetColor()
        {
            if (!(CurrentSelection is bool value))
            {
                if (Locked)
                {
                    _text.color = Colors.LOCKED_FALSE_COLOR;
                }
                else
                {
                    _text.color = Color.white;
                }
                return;
            }

            if (!Locked && value)
            {
                _text.color = Colors.TRUE_COLOR;
            }
            else if (!Locked && !value)
            {
                _text.color = Colors.FALSE_COLOR;
            }
            else if (Locked && value)
            {
                _text.color = Colors.LOCKED_TRUE_COLOR;
            }
            else if (Locked && value)
            {
                _text.color = Colors.LOCKED_FALSE_COLOR;
            }
            else
            {
                _text.color = Color.red;
            }
        }

        public void SetColor(Color c)
        {
            _text.color = c;
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
