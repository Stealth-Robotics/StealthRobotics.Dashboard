using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;

namespace StealthRobotics.Dashboard
{
    /// <summary>
    /// Interaction logic for CameraStream.xaml
    /// </summary>
    public class CameraStream : System.Windows.Controls.Image
    {
        public string StreamSource
        {
            get { return (string)GetValue(StreamSourceProperty); }
            set { SetValue(StreamSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StreamSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StreamSourceProperty =
            DependencyProperty.Register("StreamSource", typeof(string), typeof(CameraStream), new PropertyMetadata("", OnStreamChanged));

        private MJPEGStream stream;

        public CameraStream()
        {
            Loaded += CameraStream_Loaded;
        }

        private void CameraStream_Loaded(object sender, RoutedEventArgs e)
        {
            //make sure we know when the program is done with this
            Window w = Window.GetWindow(this);
            if(w != null)
                w.Closed += Window_Closed;
            //start the stream in case we happen to be ready
            HookStream();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //clean up the event handler for safety
            stream.NewFrame -= Stream_NewFrame;
            //end the stream to stop that thread
            stream.Stop();
        }

        private void Stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                System.Drawing.Image img = (Bitmap)eventArgs.Frame.Clone();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                    {
                        Source = bi;
                    }));
            }
            catch (Exception)
            {
            }
        }

        private static void OnStreamChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CameraStream s = sender as CameraStream;
            s.stream.NewFrame -= s.Stream_NewFrame;
            s.stream.Stop();
            s.HookStream();
        }

        private void HookStream()
        {
            stream = new MJPEGStream(StreamSource);
            stream.NewFrame += Stream_NewFrame;
            if (!string.IsNullOrEmpty(StreamSource)) stream.Start();
        }
    }
}
