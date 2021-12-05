using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MenuChanger.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// MenuElement which manages a "Mode Select" style button.
    /// </summary>
    public class BigButton : BaseButton
    {
        private BigButton(MenuPage page, MenuButton button) : base(page, button)
        {
            button.transform.localScale = new Vector2(0.45f, 0.45f);
            button.transform.Find("Selector").localScale = new Vector2(0.3f, 0.5f);
        }

        /// <summary>
        /// Creates a button on the page with the specified title text.
        /// </summary>
        public BigButton(MenuPage page, string title) 
            : this(page, PrefabMenuObjects.BuildBigButtonOneTextNoSprite(title)) { }
        /// <summary>
        /// Creates a button on the page with the specified title and subtitle.
        /// </summary>
        public BigButton(MenuPage page, string title, string desc) 
            : this(page, PrefabMenuObjects.BuildBigButtonTwoTextNoSprite(title, desc)) { }
        /// <summary>
        /// Creates a button on the page with the specified title, subtitle, and sprite background.
        /// </summary>
        public BigButton(MenuPage page, Sprite sprite, string title, string desc) 
            : this(page, PrefabMenuObjects.BuildBigButtonTwoTextAndSprite(sprite, title, desc)) { }
        /// <summary>
        /// Creates a button on the page with the specified title and sprite background.
        /// </summary>
        public BigButton(MenuPage page, Sprite sprite, string title)
            : this(page, PrefabMenuObjects.BuildBigButtonOneTextAndSprite(sprite, title)) { }
        /// <summary>
        /// Creates a button on the page with the specified sprite background.
        /// </summary>
        public BigButton(MenuPage page, Sprite sprite)
            : this(page, PrefabMenuObjects.BuildBigButtonSpriteOnly(sprite)) { }
        /// <summary>
        /// Creates a button on the page with the same style as the corresponding mode select button.
        /// </summary>
        public BigButton(MenuPage page, Mode mode, bool removeAction = false)
            : this(page, PrefabMenuObjects.CloneBigButton(mode).button)
        {
            if (removeAction) Button.ClearEvents();
        }

    }
}
