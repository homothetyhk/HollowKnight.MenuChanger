using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// MenuElement which manages a standard button.
    /// </summary>
    public class SmallButton : BaseButton, ILockable
    {
        /// <summary>
        /// The Text component on the button.
        /// </summary>
        public Text Text { get; }
        /// <summary>
        /// Creates a button on the page with the specified text.
        /// </summary>
        public SmallButton(MenuPage page, string text) : base(page, PrefabMenuObjects.BuildNewButton(text))
        {
            GameObject.transform.localScale = new Vector2(0.7f, 0.7f);
            Text = Button.transform.Find("Text").GetComponent<Text>();
            Text.fontSize = 36;
        }

        protected override void InvokeOnClick()
        {
            if (!Locked)
            {
                base.InvokeOnClick();
            }
        }


        public bool Locked { get; protected set; }

        public virtual void Lock()
        {
            Locked = true;
            Text.color = Colors.LOCKED_FALSE_COLOR;
        }
        public virtual void Unlock()
        {
            Locked = false;
            Text.color = Colors.DEFAULT_COLOR;
        }

        public override string ToString()
        {
            return $"Small Button {{ \"{Text.text}\" }}";
        }

    }
}
