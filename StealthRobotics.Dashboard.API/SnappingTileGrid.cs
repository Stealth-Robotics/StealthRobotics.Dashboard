using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StealthRobotics.Dashboard.API
{
    public class SnappingTileGrid : Panel
    {
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(SnappingTileGrid), new PropertyMetadata(1));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(SnappingTileGrid), new PropertyMetadata(1));

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
            DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(SnappingTileGrid), new PropertyMetadata(1));

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
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(SnappingTileGrid), new PropertyMetadata(1));

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
            DependencyProperty.RegisterAttached("Row", typeof(int), typeof(SnappingTileGrid), new PropertyMetadata(0));

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
            DependencyProperty.RegisterAttached("Column", typeof(int), typeof(SnappingTileGrid), new PropertyMetadata(0));

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }
}
