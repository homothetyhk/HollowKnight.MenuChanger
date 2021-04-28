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

        public MenuPage(string name) : this(name, null) { }

        public MenuPage(string name, MenuPage backTo)
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
            BeforeShow?.Invoke();

            self.SetActive(true);
            cg.interactable = true;
            cg.alpha = 1f;
            isShowing = true;
            MenuChanger.displayedPages.Add(this);

            AfterShow?.Invoke();
        }

        public void Hide()
        {
            BeforeHide?.Invoke();
            
            cg.interactable = false;
            cg.alpha = 0f;
            self.SetActive(false);
            isShowing = false;
            MenuChanger.displayedPages.Remove(this);

            AfterHide?.Invoke();
        }

        internal void Add(GameObject obj)
        {
            obj.transform.SetParent(self.transform);
        }

        public event Action BeforeShow;
        public event Action AfterShow;
        public event Action BeforeHide;
        public event Action AfterHide;
    }
}
