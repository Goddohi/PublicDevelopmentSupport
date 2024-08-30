using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevSup.Core
{
    public class WinBase : Window
    {

        public static string WconnectionString="";
        protected void MainLogo()
        {
            Uri iconUri = new Uri("pack://application:,,,/Images/MainLogo.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
        }

        protected void SemiLogo()
        {
            Uri iconUri = new Uri("pack://application:,,,/Images/Logo.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
        }

        protected void MaxScreen()
        {
            var w = SystemParameters.WorkArea.Width;
            var h = SystemParameters.WorkArea.Height;

            if (w > 1920)
            {
                w = 1920;
                h = 1080;
            }

            this.Width = w;
            this.Height = h;

            if (SystemParameters.WorkArea.Width > 1920)
            {
                this.Left = 100;
                this.Top = 100;
            }


            
        }

    }
}
