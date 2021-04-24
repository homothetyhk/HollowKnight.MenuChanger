using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuElements
{
    public class BigButton : BaseButton
    {
        private BigButton(MenuPage page, MenuButton button) : base(page, button)
        {
            button.transform.localScale = new Vector2(0.45f, 0.45f);
            button.transform.Find("Selector").localScale = new Vector2(0.3f, 0.5f);
        }

        public BigButton(MenuPage page, string title) 
            : this(page, PrefabMenuObjects.BuildBigButtonOneTextNoSprite(title)) { }
        public BigButton(MenuPage page, string title, string desc) 
            : this(page, PrefabMenuObjects.BuildBigButtonTwoTextNoSprite(title, desc)) { }
        public BigButton(MenuPage page, Sprite sprite, string title, string desc) 
            : this(page, PrefabMenuObjects.BuildBigButtonTwoTextAndSprite(sprite, title, desc)) { }
        public BigButton(MenuPage page, Sprite sprite, string title)
            : this(page, PrefabMenuObjects.BuildBigButtonOneTextAndSprite(sprite, title)) { }
        public BigButton(MenuPage page, Sprite sprite)
            : this(page, PrefabMenuObjects.BuildBigButtonSpriteOnly(sprite)) { }
        public BigButton(MenuPage page, Mode mode, bool removeAction = false)
            : this(page, PrefabMenuObjects.CloneBigButton(mode).button)
        {
            if (removeAction) Button.ClearEvents();
        }

    }
}
