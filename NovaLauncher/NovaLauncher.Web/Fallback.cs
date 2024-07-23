using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NovaLauncher.Web
{
    public class Fallback : StackPanel
    {
        public Fallback()
        {
            base.Background = new SolidColorBrush(Color.FromRgb(19, 19, 19));

            var imagePath = @"C:\Users\viper\Desktop\Launcherr\novalogo.png";
            base.Children.Add(new Grid
            {
                Height = 720.0,
                Children = { (UIElement)new Image
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute))
                    {
                        CreateOptions = BitmapCreateOptions.IgnoreColorProfile
                    },
                    Width = 64.0,
                    Height = 64.0,
                    VerticalAlignment = VerticalAlignment.Center
                } }
            });
        }
    }
}
