using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

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

        public TileSizingMode TileSizingMode
        {
            get { return (TileSizingMode)GetValue(TileSizingModeProperty); }
            set { SetValue(TileSizingModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridSizingMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileSizingModeProperty =
            DependencyProperty.Register("TileSizingMode", typeof(TileSizingMode), typeof(TileGrid),
                new FrameworkPropertyMetadata(TileSizingMode.RowColumn, FrameworkPropertyMetadataOptions.AffectsMeasure 
                    | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double TileDimension
        {
            get { return (double)GetValue(TileDimensionProperty); }
            set { SetValue(TileDimensionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelDimension.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileDimensionProperty =
            DependencyProperty.Register("TileDimension", typeof(double), typeof(TileGrid), 
                new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsArrange));

        private TileGridlineAdorner gridlines;

        public bool ShowGridlines
        {
            get { return (bool)GetValue(ShowGridlinesProperty); }
            set { SetValue(ShowGridlinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGridlines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowGridlinesProperty =
            DependencyProperty.Register("ShowGridlines", typeof(bool), typeof(TileGrid),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

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
            double width = availableSize.Width;
            double height = availableSize.Height;
            //if neither are infinite, leave as is
            if (!double.IsPositiveInfinity(width) && !double.IsPositiveInfinity(height))
                return availableSize;
            //if we're uniform and at least one dimension is infinite (given), clip it to the other one
            //this makes easy squares
            if (TileSizingMode == TileSizingMode.Uniform)
            {
                width = Math.Min(width, height);
                height = width;
            }
            double rowHeight;
            double colWidth;
            //compute possible row and column dimensions. may be infinite
            if (TileSizingMode == TileSizingMode.Uniform)
            {
                //at this point, either both are finite or both are infinite.
                //that means everything rounds the same
                //if the height is infinite, take the desired size
                //if not, get an integer row number and divide the height by it
                double dim = double.IsPositiveInfinity(height) ? TileDimension
                    : height / Math.Round(height / TileDimension);
                rowHeight = dim;
                colWidth = dim;
            }
            else
            {
                rowHeight = height / Rows;
                colWidth = width / Columns;
            }
            //in case either dimension of the panel is still infinite, use largest child trick
            //find the child with the largest footprint in case we have infinite size
            UIElement largestChild = null;
            double largestFootprint = 0;
            //also find the rightmost and bottommost child
            UIElement rightmostChild = null;
            UIElement bottommostChild = null;
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(colWidth * GetColumnSpan(child), rowHeight * GetRowSpan(child)));
                double childFootprint = child.DesiredSize.Width * child.DesiredSize.Height;
                if (childFootprint >= largestFootprint)
                {
                    largestChild = child;
                    largestFootprint = childFootprint;
                }
                int rightColumn = GetColumn(child) + GetColumnSpan(child);
                if (rightmostChild != null)
                {
                    int rightmostColumn = GetColumn(rightmostChild) + GetColumnSpan(rightmostChild);
                    if (rightColumn > rightmostColumn)
                    {
                        rightmostChild = child;
                    }
                }
                else rightmostChild = child;
                int bottomRow = GetRow(child) + GetRowSpan(child);
                if (bottommostChild != null)
                {
                    int bottommostRows = GetRow(bottommostChild) + GetRowSpan(bottommostChild);
                    if (bottomRow > bottommostRows)
                    {
                        bottommostChild = child;
                    }
                }
                else bottommostChild = child;
            }
            //compute possible number of rows/columns and a final size
            int rows;
            int cols;
            if (TileSizingMode == TileSizingMode.RowColumn)
            {
                rows = Rows;
                cols = Columns;
            }
            else
            {
                rows = GetRow(bottommostChild) + GetRowSpan(bottommostChild);
                cols = GetColumn(rightmostChild) + GetColumnSpan(rightmostChild);
            }
            return new Size
            {
                Width = double.IsPositiveInfinity(width) ? cols * colWidth : width,
                Height = double.IsPositiveInfinity(height) ? rows * rowHeight : height
            };
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //we will take all the size available to us. we know how large each panel is by number of rows and cols
            //get the number of rows and columns to display
            Tuple<int, int> rowCol = GetRowColDimensions(finalSize);
            double colWidth = finalSize.Width / rowCol.Item2;
            double rowHeight = finalSize.Height / rowCol.Item1;
            foreach(UIElement child in Children)
            {
                int colSpan = GetColumnSpan(child);
                int rowSpan = GetRowSpan(child);
                //force children not to overflow; if more rows are added later they can go there
                int placementCol = Math.Min(rowCol.Item2 - colSpan, GetColumn(child));
                int placementRow = Math.Min(rowCol.Item1 - rowSpan, GetRow(child));
                double x = placementCol * colWidth;
                double y = placementRow * rowHeight;
                double width = colSpan * colWidth;
                double height = rowSpan * rowHeight;
                child.Arrange(new Rect(x, y, width, height));
            }
            //make sure grid lines are updated as well!
            AdornerLayer.GetAdornerLayer(this).Update();
            return finalSize;
        }

        /// <summary>
        /// Gets the render width of each displayed column.
        /// </summary>
        public double GetColumnWidth()
        {
            Tuple<int, int> rowcol = GetRowColDimensions(new Size(ActualWidth, ActualHeight));
            return ActualWidth / rowcol.Item2;
        }

        /// <summary>
        /// Gets the render height of each displayed row.
        /// </summary>
        public double GetRowHeight()
        {
            Tuple<int, int> rowcol = GetRowColDimensions(new Size(ActualWidth, ActualHeight));
            return ActualHeight / rowcol.Item1;
        }

        /// <summary>
        /// Gets the number of rows and columns for layout
        /// </summary>
        /// <param name="finalSize">The finite size of the panel</param>
        internal Tuple<int, int> GetRowColDimensions(Size finalSize)
        {
            if (TileSizingMode == TileSizingMode.RowColumn)
            {
                return new Tuple<int, int>(Rows, Columns);
            }
            else
            {
                double availableWidth = finalSize.Width;
                double availableHeight = finalSize.Height;
                //divide these so that the boxes stay roughly squares
                double suggestedCols = availableWidth / TileDimension;
                double suggestedRows = availableHeight / TileDimension;
                //there's some fiddling to do. first, we want to round to the nearest integer number of rows and columns
                suggestedCols = Math.Round(suggestedCols);
                suggestedRows = Math.Round(suggestedRows);
                //then, we should make sure the grid is more square by getting the panel dimensions close to each other
                //we should be tending towards more rows/columns as we do not want to lose too many degrees of freedom
                double suggestedColWidth = availableWidth / suggestedCols;
                double suggestedRowHeight = availableHeight / suggestedCols;
                if (Math.Abs(suggestedColWidth - suggestedRowHeight) > TileDimension / 2.0)
                {
                    //if they're off by half the target panel size, add one to the lower value
                    if (suggestedCols < suggestedRows)
                        suggestedCols++;
                    else
                        suggestedRows++;
                }
                return new Tuple<int, int>((int)suggestedRows, (int)suggestedCols);
            }
        }

        public TileGrid() : base()
        {
            gridlines = new TileGridlineAdorner(this);
            Binding visBinding = new Binding("ShowGridlines")
            {
                Source = this,
                Converter = new BooleanToVisibilityConverter()
            };
            gridlines.SetBinding(Adorner.VisibilityProperty, visBinding);
            Loaded += TileGrid_Loaded;
        }

        private void TileGrid_Loaded(object sender, RoutedEventArgs e)
        {
            AdornerLayer.GetAdornerLayer(this).Add(gridlines);
            DataContext = this;
        }
    }
}
