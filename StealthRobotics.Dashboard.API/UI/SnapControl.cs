using Adorners;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.API.UI
{
    public class SnapControl : AdornedControl
    {
        //resources
        private SolidColorBrush HoverBrush = new SolidColorBrush(Color.FromArgb(0xF0, 0xC0, 0xC0, 0xC0));
        private Style contentOnlyButton;
        private Style thumbHoverStyle;
        private ControlTemplate thumbTemplate;

        //controls
        protected Grid adorner;
        protected Border menu;
        protected StackPanel menuOptions;

        //other stuff
        private const double maxFadeInTime = 0.25;
        private const double maxFadeOutTime = 0.75;
        private DoubleAnimation fadeOut = null;
        private Thickness defaultMargin;

        public ObservableCollection<Button> PopupButtons { get; } = new ObservableCollection<Button>();

        public SnapControl()
        {
            InitializeComponent();
            defaultMargin = Margin;
            PopupButtons.CollectionChanged += PopupButtons_CollectionChanged;
        }

        private void InitializeComponent()
        {
            //build resources
            InitializeResources();

            //build children
            InitializeContent();

            //initialize this control
            AdornerContent = adorner;
            HorizontalAdornerPlacement = AdornerPlacement.Outside;
            IsAdornerVisible = false;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
        }

        private void InitializeResources()
        {
            //transparent tool brush
            SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);

            //initialize resources: contentOnlyButton
            contentOnlyButton = new Style(typeof(Button));
            contentOnlyButton.Setters.Add(new Setter(Button.BackgroundProperty, transparent));
            contentOnlyButton.Setters.Add(new Setter(Button.BorderBrushProperty, transparent));
            ControlTemplate buttonTemplate = (ControlTemplate)XamlReader.Parse(
                @"<ControlTemplate TargetType=""Button"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <Border Background=""{TemplateBinding Background}"">
                        <ContentPresenter HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
                    </Border>
                </ControlTemplate>");
            contentOnlyButton.Setters.Add(new Setter(Button.TemplateProperty, buttonTemplate));
            Trigger buttonHoverTrigger = new Trigger()
            {
                Property = Button.IsMouseOverProperty,
                Value = true
            };
            buttonHoverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, HoverBrush));
            contentOnlyButton.Triggers.Add(buttonHoverTrigger);

            //initialize resources: thumbHoverStyle
            thumbHoverStyle = new Style(typeof(Thumb));
            thumbHoverStyle.Setters.Add(new Setter(Thumb.BackgroundProperty, transparent));
            Trigger thumbHoverTrigger = new Trigger()
            {
                Property = Thumb.IsMouseOverProperty,
                Value = true
            };
            thumbHoverTrigger.Setters.Add(new Setter(Thumb.CursorProperty, Cursors.SizeAll));
            thumbHoverStyle.Triggers.Add(thumbHoverTrigger);

            //initialize resources: thumbTemplate
            thumbTemplate = (ControlTemplate)XamlReader.Parse(
                @"<ControlTemplate TargetType=""Thumb"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <Border Background=""{TemplateBinding Background}"">
                        <Path HorizontalAlignment=""Center"" Fill=""{TemplateBinding Foreground}"" Stroke=""{TemplateBinding Foreground}""
                            Data=""M 2,2 A .5,.5 180 0 1 2,1 A .5,.5 180 0 1 2,2 
                                M 6, 2 A .5, .5 180 0 1 6, 1 A .5, .5 180 0 1 6, 2
                                M 10, 2 A .5, .5 180 0 1 10, 1 A .5, .5 180 0 1 10, 2
                                M 14, 2 A .5, .5 180 0 1 14, 1 A .5, .5 180 0 1 14, 2
                                M 4, 5 A .5, .5 180 0 1 4, 4 A .5, .5 180 0 1 4, 5
                                M 8, 5 A .5, .5 180 0 1 8, 4 A .5, .5 180 0 1 8, 5
                                M 12, 5 A .5, .5 180 0 1 12, 4 A .5, .5 180 0 1 12, 5
                                M 2, 8 A .5, .5 180 0 1 2, 7 A .5, .5 180 0 1 2, 8
                                M 6, 8 A .5, .5 180 0 1 6, 7 A .5, .5 180 0 1 6, 8
                                M 10, 8 A .5, .5 180 0 1 10, 7 A .5, .5 180 0 1 10, 8
                                M 14, 8 A .5, .5 180 0 1 14, 7 A .5, .5 180 0 1 14, 8""/>
                    </Border>
                </ControlTemplate>");
        }

        private void InitializeContent()
        {
            //initialize adorner content: adorner
            adorner = new Grid() { Opacity = 0, Margin = new Thickness(-22, 0, -22, 0)};
            //turns out binding was broken on the old one. no need to bind the width and height of grid

            SolidColorBrush bestColor = new SolidColorBrush(Color.FromArgb(0xF0, 0x80, 0x80, 0x80));

            //initialize adorner content: outer frame
            Rectangle rect = new Rectangle()
            {
                Stroke = bestColor
            };
            rect.SetBinding(Rectangle.WidthProperty, new Binding("ActualWidth") { Source = this });
            rect.SetBinding(Rectangle.HeightProperty, new Binding("ActualHeight") { Source = this });
            adorner.Children.Add(rect);

            //intitialize border content: menu
            menu = new Border()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = bestColor,
                //give it a bunch of space to expand beyond the control if the menu is large
                Margin = new Thickness(1, 1, 1, double.MinValue),
                CornerRadius = new CornerRadius(5),
                Width = 20
            };
            menu.MouseEnter += OnMouseEnter;
            menu.MouseLeave += OnMouseLeave;
            adorner.Children.Add(menu);

            //initialize adorner content: menu internals
            StackPanel p = new StackPanel() { Margin = new Thickness(2, 5, 2, 5) };
            menu.Child = p;

            //initialize adorner content: menu items
            menuOptions = new StackPanel();
            menuOptions.Resources.Add(typeof(Button), new Style(typeof(Button), contentOnlyButton));
            p.Children.Add(menuOptions);

            //initialize adorner content: thumb
            Thumb t = new Thumb()
            {
                Height = 10,
                Margin = new Thickness(0, 3, 0, 0),
                Template = thumbTemplate,
                Style = thumbHoverStyle,
                Foreground = new SolidColorBrush(Color.FromRgb(0x48, 0x48, 0x48))
            };
            t.DragDelta += Thumb_DragDelta;
            t.DragCompleted += Thumb_DragCompleted;
            p.Children.Add(t);
        }

        private void AlignAdorner()
        {
            //prefer the adorner to be on the left, but move to the right if we're in the first column
            menu.HorizontalAlignment = TileGrid.GetColumn(this) == 0 ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            //this is usually the case when the mouse leaves, but we also don't want to start fading out if:
            //  -not editable
            //  -something else went wrong
            if (fadeOut == null)
            {
                //compute time to finish fade; should always fade out at same rate
                double fadeTime = maxFadeOutTime * adorner.Opacity;
                //fade to full transparency
                fadeOut = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(fadeTime)));
                fadeOut.Completed += FadeOut_Completed;
                adorner.BeginAnimation(Border.OpacityProperty, fadeOut);
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            //this is specifically designed for a tilegrid.
            //if we're in one, we need to also check whether it can be edited
            DependencyObject parent = LogicalTreeHelper.GetParent(this);
            if (!(parent is TileGrid tg) || tg.IsEditable)
            {
                AlignAdorner();
                //cancel the fade out
                if (fadeOut != null)
                {
                    fadeOut.Completed -= FadeOut_Completed;
                    fadeOut = null;
                }
                //show the adorner and set the animation up
                ShowAdorner();

                //compute time to finish fade; should always fade in at same rate
                double fadeTime = maxFadeInTime * (1 - adorner.Opacity);
                //fade to full opacity
                DoubleAnimation fadeIn = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(fadeTime)));
                adorner.BeginAnimation(Grid.OpacityProperty, fadeIn);
            }
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            //when done fading hide the adorner
            HideAdorner();
        }

        private void PopupButtons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            menuOptions.Children.Clear();
            foreach (Button b in PopupButtons)
            {
                menuOptions.Children.Add(b);
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //put this in the foreground
            this.BringToFront();
            //shift by the amount the mouse moved
            Thickness currentMargin = Margin;
            currentMargin.Left += e.HorizontalChange;
            currentMargin.Top += e.VerticalChange;
            currentMargin.Right -= e.HorizontalChange;
            currentMargin.Bottom -= e.VerticalChange;
            Margin = currentMargin;
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //need to alter the drop code for new tilegrid behavior
            //before snapping back, see if we move to a new column/row
            DependencyObject parent = LogicalTreeHelper.GetParent(this);
            if (parent is TileGrid tg)
            {
                Tuple<int, int> rowCol = tg.GetRowColDimensions(new Size(tg.ActualWidth, tg.ActualHeight));
                //use relative end positioning to panel rather than to start position.
                Point endPos = Mouse.GetPosition(tg);
                //we know where the thumb is but really we would like to know where THIS is
                Point thumbOffset = Mouse.GetPosition(this);
                endPos.X -= thumbOffset.X;
                endPos.Y -= thumbOffset.Y;
                //get the width and height of each row
                double colWidth = tg.ActualWidth / rowCol.Item2;
                double rowHeight = tg.ActualHeight / rowCol.Item1;

                //get row and column based on position of top-left corner. round off to an integer to snap to nearest
                int newCol = (int)Math.Round(endPos.X / colWidth);
                int newRow = (int)Math.Round(endPos.Y / rowHeight);

                //if the rowspan and columnspan would exceed the number of rows or columns, fix it
                newCol = Math.Min(rowCol.Item2 - TileGrid.GetColumnSpan(this), newCol);
                newRow = Math.Min(rowCol.Item1 - TileGrid.GetRowSpan(this), newRow);

                //snap to the correct row/column
                //this doesn't need to change
                TileGrid.SetColumn(this, newCol);
                TileGrid.SetRow(this, newRow);
                AlignAdorner();
                //reset back to no margin
                //only do this if snapping; presumably we actually want to stay dragged otherwise
                Margin = defaultMargin;
            }
        }
    }
}
