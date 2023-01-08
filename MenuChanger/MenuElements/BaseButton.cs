using MenuChanger.Extensions;

namespace MenuChanger.MenuElements
{
    /// <summary>
    /// The base class for MenuChanger buttons such as SmallButton or BigButton.
    /// </summary>
    public class BaseButton : IMenuElement, ISelectable
    {
        public event Action OnClick;
        protected virtual void InvokeOnClick()
        {
            OnClick?.Invoke();
        }

        public void ClearOnClick() => OnClick = null;

        public Action OnCancel;
        protected virtual void InvokeOnCancel()
        {
            OnCancel?.Invoke();
        }

        public MenuPage Parent { get; }
        public MenuButton Button { get; }
        public GameObject GameObject { get; }
        public bool Hidden { get; private set; }

        public BaseButton(MenuPage page, MenuButton newButton)
        {
            Parent = page;
            Button = newButton;
            GameObject = Button.gameObject;

            Parent.Add(GameObject);
            GameObject.transform.localPosition = Vector3.zero;
            newButton.AddEvent(InvokeOnClick);
            newButton.customCancelAction = (s) => InvokeOnCancel();
            OnCancel = page.GoBack;
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
            UObject.Destroy(GameObject);
        }

        public void Translate(Vector2 delta)
        {
            GameObject.transform.localPosition += (Vector3)delta;
        }

        public void MoveTo(Vector2 pos)
        {
            GameObject.transform.localPosition = pos;
        }

        public void SetNeighbor(Neighbor neighbor, ISelectable selectable)
        {
            Navigation nv = Button.navigation;
            switch (neighbor)
            {
                case Neighbor.Up:
                    nv.selectOnUp = selectable?.GetSelectable(Neighbor.Down);
                    break;
                case Neighbor.Down:
                    nv.selectOnDown = selectable?.GetSelectable(Neighbor.Up);
                    break;
                case Neighbor.Right:
                    nv.selectOnRight = selectable?.GetSelectable(Neighbor.Left);
                    break;
                case Neighbor.Left:
                    nv.selectOnLeft = selectable?.GetSelectable(Neighbor.Right);
                    break;
            }
            Button.navigation = nv;
        }

        public ISelectable GetISelectable(Neighbor neighbor) => this;
        public Selectable GetSelectable(Neighbor neighbor) => Button;
    }
}
