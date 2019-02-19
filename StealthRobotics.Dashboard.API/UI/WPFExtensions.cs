using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace StealthRobotics.Dashboard.API.UI
{
    public static class WPFExtensions
    {
        public static void BringToFront(this FrameworkElement element)
        {
            if (element == null) return;

            if (!(element.Parent is Panel parent)) return;

            //get the highest z index
            IEnumerable<int> siblingZIndices = parent.Children.OfType<UIElement>()
              .Where(x => x != element)
              .Select(x => Panel.GetZIndex(x));
            if (siblingZIndices.Count() == 0)
                return;
            int maxZ = siblingZIndices.Max();
            //get this z index
            int thisZ = Panel.GetZIndex(element);
            //get others sharing this z index
            IEnumerable<UIElement> sameZ = parent.Children.OfType<UIElement>()
                .Where(x => x != element && Panel.GetZIndex(x) == thisZ);
            //if this is the only element at the max level, it stays. otherwise promote it
            if (sameZ.Count() != 0 || thisZ != maxZ)
                Panel.SetZIndex(element, maxZ + 1);
        }

        public static void SetBinding(this DependencyObject target, DependencyProperty targetProp, DependencyObject source, string path)
        {
            Binding b = new Binding(path) { Source = source };
            BindingOperations.SetBinding(target, targetProp, b);
        }

        // Helper to search up the VisualTree
        public static T FindAncestor<T>(this DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }
}
