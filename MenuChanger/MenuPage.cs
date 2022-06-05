using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MenuChanger.MenuElements;
using MenuChanger.Extensions;
using MenuChanger;
using MenuChanger.NavigationTypes;

namespace MenuChanger
{
    /// <summary>
    /// Wrapper for the standard root object used by most MenuElements. MenuPages are fullscreen pages which can be shown and hidden independently.
    /// </summary>
    public class MenuPage
    {
        public readonly GameObject self;
        public readonly RectTransform rt;
        public readonly CanvasGroup cg;
        public readonly MenuPageNavigation nav;

        /// <summary>
        /// The MenuPage which the GoBack method and the back button directs to. Can be modified after creation. If null, defaults to going back to the profile menu.
        /// </summary>
        public MenuPage backTo;

        /// <summary>
        /// The MenuPage's back button. A back button is always created by the constructor, but can be safely destroyed if desired.
        /// </summary>
        public SmallButton backButton;

        /// <summary>
        /// Set true while the MenuPage is displayed.
        /// </summary>
        public bool isShowing = false;

        /// <summary>
        /// Creates a MenuPage with the given name, equipped with a back button that returns to the profile menu.
        /// </summary>
        public MenuPage(string name) : this(name, null) { }

        /// <summary>
        /// Creates a MenuPage with the given name, equipped with a back button that returns to the given page.
        /// </summary>
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
            backButton = new SmallButton(this, "Back");
            backButton.OnClick += GoBack;

            Add(backButton.GameObject);
            backButton.MoveTo(new Vector2(0, -450));

            nav = new SimpleHorizontalNavigation(this);

            Hide();
        }

        /// <summary>
        /// Displays the MenuPage, and selects its default element.
        /// </summary>
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

        /// <summary>
        /// Hides the MenuPage.
        /// </summary>
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

        /// <summary>
        /// The method invoked by the back button. Returns to the backTo page, or if backTo is null returns to the profile menu.
        /// </summary>
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

        /// <summary>
        /// Hides this MenuPage and shows the next page.
        /// </summary>
        public void TransitionTo(MenuPage next)
        {
            Hide();
            next.Show();
        }

        internal void Add(GameObject obj)
        {
            obj.transform.SetParent(self.transform);
        }

        /// <summary>
        /// Adds the selectable to be managed directly by the page's navigation.
        /// </summary>
        public void AddToNavigationControl(ISelectable selectable)
        {
            nav.Add(selectable);
        }

        /// <summary>
        /// Resets navigation for each selectable managed directly by the page's navigation.
        /// Note that this acts recursively on any ISelectableGroups.        
        /// </summary>
        public void ResetNavigation()
        {
            nav.ResetNavigation();
        }

        public event Action BeforeShow;
        public event Action AfterShow;
        public event Action BeforeHide;
        public event Action AfterHide;
    }
}
