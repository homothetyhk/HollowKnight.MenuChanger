using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MenuChanger.MenuElements
{
    public class BaseButton : IMenuElement
    {
        public event Action OnClick;
        protected virtual void InvokeOnClick()
        {
            OnClick?.Invoke();
        }

        public MenuPage Parent { get; private set; }
        public MenuButton Button { get; private set; }
        public GameObject GameObject { get; private set; }
        public bool Hidden { get; private set; }

        public BaseButton(MenuPage page, MenuButton newButton)
        {
            Parent = page;
            Button = newButton;
            GameObject = Button.gameObject;

            Parent.Add(GameObject);
            GameObject.transform.localPosition = Vector3.zero;
            newButton.AddEvent(InvokeOnClick);
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

        public void Translate(Vector2 delta)
        {
            GameObject.transform.localPosition += (Vector3)delta;
        }

        public void MoveTo(Vector2 pos)
        {
            GameObject.transform.localPosition = pos;
        }

    }
}
