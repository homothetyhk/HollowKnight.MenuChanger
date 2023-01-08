namespace MenuChanger.MenuElements
{
    /// <summary>
    /// The common interface for MenuChanger types which are attached to a MenuPage, and have the abilities to be shown, hidden, and moved around.
    /// </summary>
    public interface IMenuElement
    {
        MenuPage Parent { get; }
        void Hide();
        void Show();

        bool Hidden { get; }

        /// <summary>
        /// Destroys the GameObject(s) attached to the element.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Moves the element to the specified position in standard MenuPage coordinates: approximately (-960, 960) x (-540, 540) scaled to the screen.
        /// </summary>
        void MoveTo(Vector2 pos);

        /// <summary>
        /// Translates the element by the specified amount in standard MenuPage coordinates: approximately (-960, 960) x (-540, 540) scaled to the screen.
        /// </summary>
        void Translate(Vector2 delta);
    }
}
