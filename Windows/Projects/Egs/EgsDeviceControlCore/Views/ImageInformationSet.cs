namespace Egs.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using System.IO;
    using System.Globalization;

    /// <summary>
    /// Information about one image.  This class does not have image data itself.
    /// </summary>
    public class ImageInformation
    {
        public int Index { get; set; }
        public Size Size { get; set; }
        public Point Offset { get; set; }
        public string FileRelativePath { get; set; }
    }

    /// <summary>
    /// Information about multiple images in one folder.  This class does not have image data itself.
    /// </summary>
    public class ImageInformationSet
    {
        public int ImageSetIndex { get; set; }
        public string FolderPath { get; set; }
        public string Description { get; set; }
        public string SampleImageFileRelativePath { get; set; }
        public IList<ImageInformation> ImageInformationList { get; internal set; }

        public ImageInformationSet()
        {
            ImageInformationList = new List<ImageInformation>();
        }

        public void AddImage(int imageIndex, string imageFileRelativePath)
        {
            var newItem = new ImageInformation();
            newItem.Index = imageIndex;
            newItem.FileRelativePath = imageFileRelativePath;
            var fullPath = Path.Combine(FolderPath, imageFileRelativePath);
            if (File.Exists(fullPath) == false)
            {
                throw new FileNotFoundException("The image file does not exist.", fullPath);
            }
            ImageInformationList.Add(newItem);
        }

        public static IList<ImageInformationSet> CreateDefaultImageInformationSetList(string basePath)
        {
            var ret = new List<ImageInformationSet>();

            // TODO: Let it get not "C:\Windows\system32" but a correct path, also in editing XAML.
            var directoryPath = "";
            var projectDir = "";
            if (false)
            {
                // NOTE: The path is correct when the app is running.  But in editing XAML, this path become "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE"
                projectDir = System.AppDomain.CurrentDomain.BaseDirectory;
                projectDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                projectDir = Path.GetDirectoryName(typeof(ImageInformationSet).Assembly.Location);
                // NOTE: Maybe it is cannot be converted to a folder path.
                var uri = new Uri("pack://application:,,,/Assembly;component/" + ImageInformationSet.CursorImageSetListFolderRelativePath);
            }
            directoryPath = Path.Combine("", basePath);

            if (Directory.Exists(directoryPath))
            {
                var directories = Directory.GetDirectories(directoryPath);
                int i = 0;
                foreach (var directory in directories)
                {
                    var newSet = new ImageInformationSet();
                    newSet.ImageSetIndex = i++;
                    newSet.FolderPath = directory;
                    newSet.Description = directory.Split(Path.DirectorySeparatorChar).Last().Split('_').Last();
                    newSet.SampleImageFileRelativePath = newSet.FolderPath + "/Sample.png";
                    ret.Add(newSet);
                }
            }
            else
            {
                Debugger.Break();
                for (int i = 0; i < 12; i++)
                {
                    var newSet = new ImageInformationSet();
                    newSet.ImageSetIndex = i;
                    newSet.FolderPath = "";
                    newSet.Description = i.ToString("D2", CultureInfo.InvariantCulture);
                    ret.Add(newSet);
                }
            }
            return ret;
        }



        public static readonly string CursorImageSetListFolderRelativePath = "Resources/CursorImageSets";
        public static readonly string CameraViewUserControlImagesFolderRelativePath = "Resources/CameraViewImageSets";

        public static IList<ImageInformationSet> CreateDefaultRightCursorImageInformationSetList()
        {
            var src = ImageInformationSet.CreateDefaultImageInformationSetList(CursorImageSetListFolderRelativePath);
            var ret = src.Select(e =>
            {
                e.AddImage((int)CursorImageIndexLabels.OpenHand, "Right_OpenHand.png");
                e.AddImage((int)CursorImageIndexLabels.CloseHand, "Right_CloseHand.png");
                //e.AddImage((int)CursorImageIndexLabelsForPowerPointViewer.OpenHandWithRightArrow, "Right_OpenHandWithRightArrow.png");
                //e.AddImage((int)CursorImageIndexLabelsForPowerPointViewer.OpenHandWithLeftArrow, "Right_OpenHandWithLeftArrow.png");
                return e;
            }).ToList();
            return ret;
        }

        public static IList<ImageInformationSet> CreateDefaultLeftCursorImageInformationSetList()
        {
            var src = ImageInformationSet.CreateDefaultImageInformationSetList(CursorImageSetListFolderRelativePath);
            var ret = src.Select(e =>
            {
                e.AddImage((int)CursorImageIndexLabels.OpenHand, "Left_OpenHand.png");
                e.AddImage((int)CursorImageIndexLabels.CloseHand, "Left_CloseHand.png");
                //e.AddImage((int)CursorImageIndexLabelsForPowerPointViewer.OpenHandWithRightArrow, "Left_OpenHandWithRightArrow.png");
                //e.AddImage((int)CursorImageIndexLabelsForPowerPointViewer.OpenHandWithLeftArrow, "Left_OpenHandWithLeftArrow.png");
                return e;
            }).ToList();
            return ret;
        }

        internal static IList<ImageInformationSet> CreateDefaultCameraViewUserControlImagesFolderRelativePath()
        {
            var imageInfoSetList = ImageInformationSet.CreateDefaultImageInformationSetList(CameraViewUserControlImagesFolderRelativePath);
            var ret = imageInfoSetList.Select(oneImageSetInfo =>
            {
                var imageFilePaths = Directory.GetFiles(oneImageSetInfo.FolderPath);
                int imageIndex = 0;
                foreach (var imagefilePath in imageFilePaths)
                {
                    oneImageSetInfo.AddImage(imageIndex++, Path.GetFileName(imagefilePath));
                }
                return oneImageSetInfo;
            }).ToList();
            return ret;
        }
    }
}
