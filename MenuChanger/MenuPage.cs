using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MenuChanger.MenuElements;

namespace MenuChanger
{
    public class MenuPage
    {
        public readonly GameObject self;
        public readonly RectTransform rt;
        public readonly CanvasGroup cg;

        public MenuButton backButton;
        public bool isShowing = false;

        public static MenuPage Create(string name, MenuPage backTo) => new MenuPage(name, backTo);
        public static MenuPage Create(string name = "Menu Page") => new MenuPage(name, null);

        private MenuPage(string name, MenuPage backTo)
        {
            self = new GameObject(name);
            self.transform.position = UIManager.instance.UICanvas.transform.position;
            self.transform.SetParent(UIManager.instance.UICanvas.transform);

            rt = self.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.sizeDelta = new Vector2(0f, 0f);
            rt.localScale = new Vector3(1f, 1f, 1f);
            cg = self.AddComponent<CanvasGroup>();

            // back leads to profile select
            if (backTo == null)
            {
                backButton = PrefabMenuObjects.CloneBackButton($"Back");
            }
            else
            {
                backButton = PrefabMenuObjects.BuildNewButton("Back");
                backButton.AddHideMenuPageEvent(this);
                backButton.AddShowMenuPageEvent(backTo);
            }
            
            Add(backButton.gameObject);
            PrefabMenuObjects.RescaleBackButton(backButton.gameObject);
            backButton.transform.localPosition = new Vector2(0, -450);

            Hide();
        }

        public void Show()
        {
            self.SetActive(true);
            cg.interactable = true;
            cg.alpha = 1f;
            isShowing = true;
            MenuChanger.displayedPages.Add(this);
        }

        public void Hide()
        {
            cg.interactable = false;
            cg.alpha = 0f;
            self.SetActive(false);
            isShowing = false;
            MenuChanger.displayedPages.Remove(this);
        }

        public void Add(GameObject obj)
        {
            obj.transform.SetParent(self.transform);
        }

        public MenuButton AddNewModeButton(Sprite sprite)
        {
            MenuButton button = PrefabMenuObjects.BuildBigButtonSpriteOnly(sprite).GetComponent<MenuButton>();
            button.transform.SetParent(self.transform);
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = new Vector2(0.6f, 0.6f);
            button.transform.Find("Selector").localScale = new Vector2(0.5f, 0.5f);

            return button;
        }

        public MenuButton AddNewModeButton(Sprite sprite, string title, string desc)
        {
            MenuButton button = PrefabMenuObjects.BuildBigButtonTwoTextAndSprite(sprite, title, desc).GetComponent<MenuButton>();
            button.transform.SetParent(self.transform);
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = new Vector2(0.6f, 0.6f);
            button.transform.Find("Selector").localScale = new Vector2(0.5f, 0.5f);

            return button;
        }

    }
}
