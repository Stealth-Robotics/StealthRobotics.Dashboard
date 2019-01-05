﻿using Adorners;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.API.UI
{
    /// <summary>
    /// Interaction logic for SnapControl.xaml
    /// </summary>
    public partial class SnapControl : AdornedControl
    {
        private const double maxFadeInTime = 0.25;
        private const double maxFadeOutTime = 0.625;
        private DoubleAnimation fadeOut = null;
        private Thickness defaultMargin;

        public ObservableCollection<Button> PopupButtons { get; } = new ObservableCollection<Button>();

        public SnapControl()
        {
            InitializeComponent();
            defaultMargin = Margin;
            PopupButtons.CollectionChanged += PopupButtons_CollectionChanged;
        }

        private void PopupButtons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            menuOptions.Children.Clear();
            foreach(Button b in PopupButtons)
            {
                menuOptions.Children.Add(b);
            }
        }

        private void AlignAdorner()
        {
            //prefer the adorner to be on the left, but move to the right if we're in the first column
            menu.HorizontalAlignment = TileGrid.GetColumn(this) == 0 ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        private void AdornedControl_MouseEnter(object sender, MouseEventArgs e)
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
                adorner.BeginAnimation(Border.OpacityProperty, fadeIn);
            }
        }

        private void AdornedControl_MouseLeave(object sender, MouseEventArgs e)
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

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            //when done fading hide the adorner
            HideAdorner();
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
            if(parent is TileGrid tg)
            {
                //use relative end positioning to panel rather than to start position.
                Point endPos = Mouse.GetPosition(tg);
                //we know where the thumb is but really we would like to know where THIS is
                Point thumbOffset = Mouse.GetPosition(this);
                endPos.X -= thumbOffset.X;
                endPos.Y -= thumbOffset.Y;
                //get the width and height of each row
                double colWidth = tg.ActualWidth / tg.Columns;
                double rowHeight = tg.ActualHeight / tg.Rows;

                //get row and column based on position of top-left corner. round off to an integer to snap to nearest
                int newCol = (int)Math.Round(endPos.X / colWidth);
                int newRow = (int)Math.Round(endPos.Y / rowHeight);

                //if the rowspan and columnspan would exceed the number of rows or columns, fix it
                newCol = Math.Min(tg.Columns - TileGrid.GetColumnSpan(this), newCol);
                newRow = Math.Min(tg.Rows - TileGrid.GetRowSpan(this), newRow);

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