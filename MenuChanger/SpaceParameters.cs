namespace MenuChanger
{
    /// <summary>
    /// Static class with various constants for positioning and spacing elements in the (-960, 960) x (-540, 540) MenuPage coordinates
    /// </summary>
    public static class SpaceParameters
    {
        /// <summary>
        /// Recommended minimum vertical spacing for SmallButtons.
        /// </summary>
        public const float VSPACE_SMALL = 50f;
        /// <summary>
        /// Recommended minimum vertical spacing for SmallButtons and/or labelled EntryFields.
        /// </summary>
        public const float VSPACE_MEDIUM = 75f;
        /// <summary>
        /// Recommended minimum vertical spacing for BigButtons.
        /// </summary>
        public const float VSPACE_LARGE = 150f;

        /// <summary>
        /// Recommended minimum horizontal spacing for SmallButtons or EntryFields with short (1-2 word) labels.
        /// </summary>
        public const float HSPACE_SMALL = 400f;
        /// <summary>
        /// Recommended horizontal spacing between three large columns.
        /// </summary>
        public const float HSPACE_MEDIUM = 650f;
        /// <summary>
        /// Recommended horizontal spacing between two large columns.
        /// </summary>
        public const float HSPACE_LARGE = 800f;

        /// <summary>
        /// Recommended positioning for a centered title. Value is (0, 400), but there is room to increase y if space is needed.
        /// </summary>
        public static Vector2 TOP_CENTER { get => new(0, 400f); }

        /// <summary>
        /// Recommended positioning for a centered panel or element just below a page title. Value is (0, 300).
        /// </summary>
        public static Vector2 TOP_CENTER_UNDER_TITLE { get => new(0, 300f); }
    }
}
