using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MenuChanger.MenuElements;
using MenuChanger.Extensions;

namespace MenuChanger
{
    public class MenuPage
    {
        public readonly GameObject self;
        public readonly RectTransform rt;
        public readonly CanvasGroup cg;
        public readonly MenuPageNavigation nav;
        public MenuPage backTo;


        public SmallButton backButton;
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

            this.backTo = backTo;
            if (backTo == null)
            {
                backButton = new SmallButton(this, "Back");
                backButton.OnClick += UIManager.instance.UIGoToProfileMenu;
                backButton.OnCancel += UIManager.instance.UIGoToProfileMenu;
            }
            else
            {
                backButton = new SmallButton(this, "Back");
                backButton.OnClick += GoBack;
            }
            
            Add(backButton.GameObject);
            backButton.MoveTo(new Vector2(0, -450));

            nav = new SimpleHorizontalNavigation(this);

            Hide();
        }

        public void Show()
        {
            BeforeShow?.Invoke();

            self.SetActive(true);
            cg.interactable = true;
            cg.alpha = 1f;
            isShowing = true;
            MenuChangerMod.displayedPages.Add(this);
            nav.SelectDefault();

            AfterShow?.Invoke();
        }

        public void Hide()
        {
            BeforeHide?.Invoke();
            
            cg.interactable = false;
            cg.alpha = 0f;
            self.SetActive(false);
            isShowing = false;
            MenuChangerMod.displayedPages.Remove(this);

            AfterHide?.Invoke();
        }

        public void GoBack()
        {
            if (backTo == null)
            {
                UIManager.instance.UIGoToProfileMenu();
            }
            else
            {
                TransitionTo(backTo);
            }
        }

        public void TransitionTo(MenuPage next)
        {
            Hide();
            next.Show();
        }

        internal void Add(GameObject obj)
        {
            obj.transform.SetParent(self.transform);
        }

        public void AddToNavigationControl(ISelectable selectable)
        {
            nav.Add(selectable);
        }

        public event Action BeforeShow;
        public event Action AfterShow;
        public event Action BeforeHide;
        public event Action AfterHide;
    }
}
