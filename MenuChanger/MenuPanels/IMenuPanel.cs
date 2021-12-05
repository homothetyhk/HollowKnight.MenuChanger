using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MenuChanger.MenuElements;

namespace MenuChanger.MenuPanels
{
    /// <summary>
    /// The common interface for MenuChanger types which arrange or display IMenuElements. IMenuPanel implements IMenuElement, and so panels can often be nested to some degree.
    /// </summary>
    public interface IMenuPanel : IMenuElement, ISelectable, ISelectableGroup
    {
        /// <summary>
        /// Adds the IMenuElement to the panel.
        /// </summary>
        void Add(IMenuElement obj);
        /// <summary>
        /// Removes the IMenuElement from the panel.
        /// </summary>
        bool Remove(IMenuElement obj);
        /// <summary>
        /// The collection of IMenuElements managed by the panel.
        /// </summary>
        List<IMenuElement> Items { get; }
    }
}
