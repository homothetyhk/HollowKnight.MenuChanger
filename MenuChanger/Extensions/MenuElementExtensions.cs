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
        /// <summary>
        /// Causes the button to hide its MenuPage on click.
        /// </summary>
        public static void AddHideMenuPageEvent(this BaseButton self)
        {
            self.OnClick += self.Parent.Hide;
        }

        /// <summary>
        /// Causes the button to hide the given MenuPage on click.
        /// </summary>
        public static void AddHideMenuPageEvent(this BaseButton self, MenuPage prev)
        {
            self.OnClick += prev.Hide;
        }

        /// <summary>
        /// Causes the button to hide all MenuPages on click.
        /// </summary>
        public static void AddHideAllMenuPagesEvent(this BaseButton self)
        {
            self.OnClick += MenuChangerMod.HideAllMenuPages;
        }

        /// <summary>
        /// Causes the button to show the given MenuPage on click.
        /// </summary>
        public static void AddShowMenuPageEvent(this BaseButton self, MenuPage next)
        {
            self.OnClick += next.Show;
        }

        /// <summary>
        /// Causes the button to transition from its MenuPage to another on click.
        /// </summary>
        public static void AddHideAndShowEvent(this BaseButton self, MenuPage next)
        {
            self.OnClick += () =>
            {
                self.Parent.Hide();
                next.Show();
            };
        }

        /// <summary>
        /// Causes the button to transition from one MenuPage to another on click.
        /// </summary>
        public static void AddHideAndShowEvent(this BaseButton self, MenuPage prev, MenuPage next)
        {
            self.OnClick += () =>
            {
                prev.Hide();
                next.Show();
            };
        }

        /// <summary>
        /// Saves the string to local settings. MenuChanger will subsequently allow a ResumeMenu subscribed to the matching key to take control upon clicking the save slot button.
        /// </summary>
        public static void AddSetResumeKeyEvent(this BaseButton self, string key)
        {
            self.OnClick += () => MenuChangerMod.instance.Settings.resumeKey = key;
        }

        /// <summary>
        /// Calls SetNeighbor with the given parameters, and then calls the reverse SetNeighbor to ensure symmetric navigation.
        /// </summary>
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
