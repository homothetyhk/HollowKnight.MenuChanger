using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuChanger.MenuElements;

namespace MenuChanger.Extensions
{
    public static class MenuElementExtensions
    {
        public static void AddHideMenuPageEvent(this BaseButton self, MenuPage prev)
        {
            self.OnClick += prev.Hide;
        }

        public static void AddHideAllMenuPagesEvent(this BaseButton self)
        {
            self.OnClick += MenuChangerMod.HideAllMenuPages;
        }

        public static void AddShowMenuPageEvent(this BaseButton self, MenuPage next)
        {
            self.OnClick += next.Show;
        }

        public static void AddHideAndShowEvent(this BaseButton self, MenuPage prev, MenuPage next)
        {
            self.OnClick += () =>
            {
                prev.Hide();
                next.Show();
            };
        }

        public static void AddSetResumeKeyEvent(this BaseButton self, string key)
        {
            self.OnClick += () => MenuChangerMod.instance.Settings.resumeKey = key;
        }

        public static void SymSetNeighbor(this ISelectable self, Neighbor neighbor, ISelectable selectable)
        {
            self.SetNeighbor(neighbor, selectable);
            neighbor = neighbor switch
            {
                Neighbor.Down => Neighbor.Up,
                Neighbor.Up => Neighbor.Down,
                Neighbor.Left => Neighbor.Right,
                Neighbor.Right => Neighbor.Left,
                _ => neighbor
            };
            selectable.SetNeighbor(neighbor, self);
        }
    }
}
