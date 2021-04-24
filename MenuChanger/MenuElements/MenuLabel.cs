using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuElements
{
    public class MenuLabel : IMenuElement
    {
        public MenuPage Parent { get; private set; }
        public GameObject GameObject { get; private set; }
        public Text Text { get; private set; }
        public bool Hidden { get; private set; }

        public enum Style
        {
            Title,
            Body
        }

        public MenuLabel(MenuPage page, string text, Style style = Style.Title)
        {
            Parent = page;
            switch (style)
            {
                default:
                case Style.Title:
                    (GameObject, Text) = PrefabMenuObjects.BuildLabel(Parent, text);
                    break;
                case Style.Body:
                    (GameObject, Text) = PrefabMenuObjects.BuildDescText(Parent, text);
                    break;
            }
        }

        public void MoveTo(Vector2 pos)
        {
            GameObject.transform.localPosition = pos;
        }

        public void Translate(Vector2 delta)
        {
            GameObject.transform.localPosition += (Vector3)delta;
        }

        public void Show()
        {
            Hidden = false;
            GameObject.SetActive(true);
        }

        public void Hide()
        {
            Hidden = true;
            GameObject.SetActive(false);
        }

        public void Destroy()
        {
            GameObject.Destroy(GameObject);
        }
    }
}
