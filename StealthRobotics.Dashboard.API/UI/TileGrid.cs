using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StealthRobotics.Dashboard.API.UI
{
    public class TileGrid : Panel
    {
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(TileGrid),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(TileGrid), 
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public static int GetRowSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(RowSpanProperty);
        }

        public static void SetRowSpan(DependencyObject obj, int value)
        {
            obj.SetValue(RowSpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(TileGrid),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsParentMeasure 
                    | FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static int GetColumnSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnSpanProperty);
        }

        public static void SetColumnSpan(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnSpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(TileGrid), 
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsParentMeasure 
                    | FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static int GetRow(DependencyObject obj)
        {
            return (int)obj.GetValue(RowProperty);
        }

        public static void SetRow(DependencyObject obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        // Using a DependencyProperty as the backing store for Row.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached("Row", typeof(int), typeof(TileGrid), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static int GetColumn(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnProperty);
        }

        public static void SetColumn(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        // Using a DependencyProperty as the backing store for Column.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached("Column", typeof(int), typeof(TileGrid),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public bool IsEditable { get; set; } = true;

        protected override Size MeasureOverride(Size availableSize)
        {
            //if we have infinite size, we can offer each element infinite size and determine grid size by largest element
            //if we have finite size, we know we will take all of it, and can give available size to the child by # of rows and cols
            //these account for possible infinite dimension
            double colWidth = availableSize.Width / Columns;
            double rowHeight = availableSize.Height / Rows;
            //find the child with the largest footprint in case we have infinite size
            UIElement largestChild = null;
            //to reduce computation time
            double largestFootprint = 0;
            foreach (UIElement child in Children)
            {
                double availableChildWidth = GetColumnSpan(child) * colWidth;
                double availableChildHeight = GetRowSpan(child) * rowHeight;
                child.Measure(new Size(availableChildWidth, availableChildHeight));
                double childFootprint = child.DesiredSize.Width * child.DesiredSize.Height;
                if (childFootprint > largestFootprint)
                {
                    largestChild = child;
                    largestFootprint = childFootprint;
                }
            }
            //compute possible grid sizing by largest child, in case a dimension is infinite
            colWidth = largestChild.DesiredSize.Width / GetColumnSpan(largestChild);
            rowHeight = largestChild.DesiredSize.Height / GetRowSpan(largestChild);
            return new Size
            {
                //if we have an infinite dimension available, use the largest child. otherwise take all of what we were offered
                Width = double.IsPositiveInfinity(availableSize.Width) ?
                    colWidth * Columns : availableSize.Width,
                Height = double.IsPositiveInfinity(availableSize.Height) ?
                    rowHeight * Rows : availableSize.Height
            };
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //we will take all the size available to us. we know how large each panel is by number of rows and cols
            double colWidth = finalSize.Width / Columns;
            double rowHeight = finalSize.Height / Rows;
            foreach(UIElement child in Children)
            {
                double x = GetColumn(child) * colWidth;
                double y = GetRow(child) * rowHeight;
                double width = GetColumnSpan(child) * colWidth;
                double height = GetRowSpan(child) * rowHeight;
                //may need to cut off the object so that it won't bleed outside the allotted space
                if (x + width > finalSize.Width)
                    width = finalSize.Width - x;
                if (y + height > finalSize.Height)
                    height = finalSize.Height - y;
                child.Arrange(new Rect(x, y, width, height));
            }

            return finalSize;
        }
    }
}
