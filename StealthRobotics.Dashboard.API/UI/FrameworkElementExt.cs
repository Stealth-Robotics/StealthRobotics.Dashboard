using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StealthRobotics.Dashboard.API.UI
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
            //get this z index
            int thisZ = Panel.GetZIndex(element);
            //get others sharing this z index
            IEnumerable<UIElement> sameZ = parent.Children.OfType<UIElement>()
                .Where(x => x != element && Panel.GetZIndex(x) == thisZ);
            //if this is the only element at the max level, it stays. otherwise promote it
            if(sameZ.Count() != 0 || thisZ != maxZ)
                Panel.SetZIndex(element, maxZ + 1);
        }
    }
}
