namespace MenuChanger
{
    public enum Neighbor
    {
        Left,
        Right,
        Up,
        Down
    }

    /// <summary>
    /// Interface used by MenuChanger types which manage one or more Selectables.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Sets the neighbor in the specified direction to be the given ISelectable.
        /// <br/>Often used recursively: e.g. setting a panel as a neighbor of a panel calls SetNeighbor on ISelectable elements of each panel.
        /// </summary>
        void SetNeighbor(Neighbor neighbor, ISelectable selectable);
        /// <summary>
        /// Returns the most extreme ISelectable subelement in the specified direction.
        /// <br/>For example, on a button, this would return itself. On a VerticalItemPanel, this would return the first item if called with Neighbor.Up.
        /// </summary>
        ISelectable GetISelectable(Neighbor neighbor);
        /// <summary>
        /// Returns the most extreme Selectable subelement in the specified direction.
        /// <br/>For example, on a button, this would return its Selectable. On a VerticalItemPanel, this would be equivalent to GetISelectable composed with GetSelectable.
        /// </summary>
        Selectable GetSelectable(Neighbor neighbor);
    }

    /// <summary>
    /// Interface used by MenuChanger types which manage one or more ISelectables.
    /// </summary>
    public interface ISelectableGroup
    {
        /// <summary>
        /// Resets the internal navigation of the group.
        /// </summary>
        void ResetNavigation();
    }
}
