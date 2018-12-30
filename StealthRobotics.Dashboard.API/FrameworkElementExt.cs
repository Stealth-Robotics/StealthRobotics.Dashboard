using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StealthRobotics.Dashboard.API
{
    public static class FrameworkElementExt
    {
        public static void BringToFront(this FrameworkElement element)
        {
            if (element == null) return;

            if (!(element.Parent is Panel parent)) return;

            //get the highest z index
            int maxZ = parent.Children.OfType<UIElement>()
              .Where(x => x != element)
              .Select(x => Panel.GetZIndex(x))
              .Max();
            //get all children with a z index larger than this
            IEnumerable<UIElement> higherZ = parent.Children.OfType<UIElement>()
                .Where(x => Panel.GetZIndex(x) > Panel.GetZIndex(element));
            //move everything we're passing down by 1 to keep numbers reasonable
            foreach(UIElement e in higherZ)
            {
                Panel.SetZIndex(e, Panel.GetZIndex(e) - 1);
            }
            //fill the slot of the top element
            //if nothing is higher, we need to move up a slot
            Panel.SetZIndex(element, higherZ.Count() == 0 ? maxZ + 1 : maxZ);
        }
    }
}
