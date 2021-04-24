using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MenuChanger.MenuElements;

namespace MenuChanger.MenuPanels
{
    public interface IMenuPanel : IMenuElement
    {
        void Add(IMenuElement obj);
        bool Remove(IMenuElement obj);
        List<IMenuElement> Items { get; }
    }
}
