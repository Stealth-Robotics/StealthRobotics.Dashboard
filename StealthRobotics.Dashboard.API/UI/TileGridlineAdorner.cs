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
            double colWidth = target.ActualWidth / target.Columns;
            double rowHeight = target.ActualHeight / target.Rows;
            Pen stroke = new Pen(new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80)), 1);
            for (int row = 1; row < target.Rows; row++)
            {
                drawingContext.DrawLine(stroke, new Point(0, row * rowHeight), new Point(target.ActualWidth, row * rowHeight));
            }
            for (int col = 1; col < target.Columns; col++)
            {
                drawingContext.DrawLine(stroke, new Point(col * colWidth, 0), new Point(col * colWidth, target.ActualHeight));
            }
        }
    }
}
