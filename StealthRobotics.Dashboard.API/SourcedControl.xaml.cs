using Adorners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StealthRobotics.Dashboard.API
{
    /// <summary>
    /// Interaction logic for SourcedControl.xaml
    /// </summary>
    public partial class SourcedControl : AdornedControl
    {
        private const double maxFadeInTime = 0.25;
        private const double maxFadeOutTime = 0.625;
        private DoubleAnimation fadeOut = null;
        private Thickness defaultMargin;
        private int defaultZIndex;

        public SourcedControl()
        {
            InitializeComponent();
            defaultMargin = Margin;
            defaultZIndex = Panel.GetZIndex(this);
        }

        private void AdornedControl_MouseEnter(object sender, MouseEventArgs e)
        {
            //if we're editable to the TileGrid we can show our property adorner
            //if we're not in a TileGrid, true by default so ok to do this check
            if (TileGrid.GetEditable(this))
            {
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

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //put this in the foreground
            Panel.SetZIndex(this, 1);
            //shift by the amount the mouse moved
            Thickness currentMargin = Margin;
            currentMargin.Left += e.HorizontalChange;
            currentMargin.Top += e.VerticalChange;
            currentMargin.Right -= e.HorizontalChange;
            currentMargin.Bottom -= e.VerticalChange;
            Margin = currentMargin;
        }

        private void Thumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //todo: before snapping back, see if we move to a new column/row
            //reset back to no margin
            Margin = defaultMargin;
            //put back at the basic level
            Panel.SetZIndex(this, defaultZIndex);
        }
    }
}
