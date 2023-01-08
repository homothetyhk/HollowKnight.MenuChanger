namespace MenuChanger.MenuElements
{
    /// <summary>
    /// MenuItem&lt;bool&gt; which indicates its state using colors instead of displaying the value as text.
    /// </summary>
    public class ToggleButton : MenuItem<bool>
    {
        private static List<bool> GetBoolList() => new() { false, true };

        /// <summary>
        /// Creates a toggle button displaying the given text and initialized to false.
        /// </summary>
        public ToggleButton(MenuPage page, string text) : base(page, text, GetBoolList(), new ToggleButtonFormatter()) { }

        protected override void RefreshText()
        {
            SetColor();
            base.RefreshText();
        }

        public void SetColor()
        {
            switch (Value)
            {
                case true when !Locked:
                    Text.color = Colors.TRUE_COLOR;
                    break;
                case true when Locked:
                    Text.color = Colors.LOCKED_TRUE_COLOR;
                    break;
                case false when Locked:
                    Text.color = Colors.LOCKED_FALSE_COLOR;
                    break;
                case false when !Locked:
                    Text.color = Colors.FALSE_COLOR;
                    break;
            }
        }

        public void SetColor(Color c)
        {
            Text.color = c;
        }

        public override void Lock()
        {
            Locked = true;
            SetColor();
        }

        public override void Unlock()
        {
            Locked = false;
            SetColor();
        }
    }
}
