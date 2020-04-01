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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;

namespace DTTRound2
{
    class MediaAct
    {
        public void Upload(MediaElement media, string mediaName)
        {
            try
            {
                string Filepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                media.Source = new Uri(Filepath + "\\Resources\\" + mediaName, UriKind.Relative);
                Play(media);
                Stop(media);
            }
            catch
            {
                Console.Write("Loi khong load duoc");
            }
        }
        public void Play(MediaElement media)
        {
            media.Stop();
            media.Visibility = Visibility.Visible;
            media.Play();
        }
        public void Stop(MediaElement media)
        {
            media.Visibility = Visibility.Hidden;
            media.Stop();
        }
        public void Upload(Image image, string imageName)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            string Filepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
                + "\\Resources\\" + imageName;
            logo.UriSource = new Uri(Filepath);
            logo.EndInit();
            image.Source = logo;
        }
        public void Upload(ImageBrush imageBrush, string imageBrushName)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            string Filepath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
                + "\\Resources\\" + imageBrushName;
            logo.UriSource = new Uri(Filepath);
            logo.EndInit();
            imageBrush.ImageSource = logo;
        }
    }
}
