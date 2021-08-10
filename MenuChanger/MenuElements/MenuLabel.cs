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
        public CanvasGroup CanvasGroup { get; private set; }
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
                    (GameObject, Text, CanvasGroup) = PrefabMenuObjects.BuildLabel(Parent, text);
                    break;
                case Style.Body:
                    (GameObject, Text, CanvasGroup) = PrefabMenuObjects.BuildDescText(Parent, text);
                    break;
            }
        }

        public MenuLabel(MenuPage page, string text, Vector2 dimensions)
        {
            Parent = page;
            (GameObject, Text, CanvasGroup) = PrefabMenuObjects.BuildDescText(Parent, text);
            GameObject.DestroyImmediate(GameObject.GetComponent<ContentSizeFitter>());
            GameObject.DestroyImmediate(GameObject.GetComponent<AutoLocalizeTextUI>());
            GameObject.DestroyImmediate(GameObject.GetComponent<FixVerticalAlign>());
            Text.rectTransform.anchorMin = Text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            Text.rectTransform.sizeDelta = dimensions;
        }

        public virtual void MoveTo(Vector2 pos)
        {
            GameObject.transform.localPosition = pos;
        }

        public virtual void Translate(Vector2 delta)
        {
            GameObject.transform.localPosition += (Vector3)delta;
        }

        public virtual void Show()
        {
            Hidden = false;
            GameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            Hidden = true;
            GameObject.SetActive(false);
        }

        public virtual void Destroy()
        {
            GameObject.Destroy(GameObject);
        }

        public void MakeTransparent()
        {
            CanvasGroup.alpha = 0f;
        }

        public void MakeOpaque()
        {
            CanvasGroup.alpha = 1f;
        }

        public void SetVisibleByAlpha(bool visible)
        {
            if (visible) MakeOpaque();
            else MakeTransparent();
        }
    }
}
