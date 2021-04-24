using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MenuChanger.MenuElements
{
    public interface IMenuElement
    {
        MenuPage Parent { get; }
        void Hide();
        void Show();
        bool Hidden { get; }

        void Destroy();
        void MoveTo(Vector2 pos);
        void Translate(Vector2 delta);
    }
}
