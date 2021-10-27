using System;
using System.Text;
using System.Threading.Tasks;
using MenuChanger.Extensions;
using UnityEngine.UI;
using static MenuChanger.LogHelper;

namespace MenuChanger
{
    public enum Neighbor
    {
        Left,
        Right,
        Up,
        Down
    }

    public interface ISelectable
    {
        void SetNeighbor(Neighbor neighbor, ISelectable selectable);
        ISelectable GetISelectable(Neighbor neighbor);
        Selectable GetSelectable(Neighbor neighbor);
    }

    public interface ISelectableGroup
    {
        void ResetNavigation();
    }
}
