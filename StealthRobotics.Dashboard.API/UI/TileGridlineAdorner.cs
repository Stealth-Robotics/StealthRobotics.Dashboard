using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace StealthRobotics.Dashboard.API.UI
{
    public class TileGridlineAdorner : Adorner
    {
        readonly TileGrid target;
        public TileGridlineAdorner(TileGrid adornedElement) : base(adornedElement)
        {
            target = adornedElement;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Tuple<int, int> rowCol = target.GetRowColDimensions(new Size(target.ActualWidth, target.ActualHeight));
            double colWidth = target.ActualWidth / rowCol.Item2;
            double rowHeight = target.ActualHeight / rowCol.Item1;
            Pen stroke = new Pen(new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80)), 1);
            for (int row = 1; row < rowCol.Item1; row++)
            {
                drawingContext.DrawLine(stroke, new Point(0, row * rowHeight), new Point(target.ActualWidth, row * rowHeight));
            }
            for (int col = 1; col < rowCol.Item2; col++)
            {
                drawingContext.DrawLine(stroke, new Point(col * colWidth, 0), new Point(col * colWidth, target.ActualHeight));
            }
        }
    }
}
