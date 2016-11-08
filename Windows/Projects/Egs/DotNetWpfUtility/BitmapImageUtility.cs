namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    public static class BitmapImageUtility
    {
        /// <summary>
        /// Load a bitmap image from a file more safely
        /// </summary>
        public static BitmapImage LoadBitmapImageFromFile(string filePath)
        {
            // NOTE) 
            // You have to be careful in loading some bitmap images from files in your WPF application.  This method provides the way.
            // In some conditions, These ways show the images on XAML editor, but the images do not appear in the runtime application!
            //  * <Image Source=""Images/MyImageFile.png"" />
            //  * <Image Source=""{Binding MyBitmapImage}"" /> and DataBinding
            //  * <BitmapImage x:Key=""MyBitmapImageKey"" UriSource=""Images/MyImageFile.png"" /> and <Image Source=""{StaticResource MyBitmapImageKey}"" />
            //  * <Image x:Name=""MyImage"" /> ... "MyImage.Source = new BitmapImage(new Uri("Images/MyImageFile.png", , UriKind.Relative));" in code-behind.
            if (File.Exists(filePath) == false) { return null; }
            var ret = new BitmapImage();
            using (var stream = File.OpenRead(filePath))
            {
                ret.BeginInit();
                ret.CacheOption = BitmapCacheOption.OnLoad;
                ret.StreamSource = stream;
                ret.EndInit();
            }
            return ret;
        }
    }
}
