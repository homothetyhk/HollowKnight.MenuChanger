using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuChanger.MenuElements
{
    public class SmallButton : BaseButton
    {
        public SmallButton(MenuPage page, string text) : base(page, PrefabMenuObjects.BuildNewButton(text))
        {
            GameObject.transform.localScale = new Vector2(0.7f, 0.7f);
        }
    }
}
