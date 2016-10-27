namespace Exvision.Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;

    class MediaPlayerWrapper : IAudioPlayer
    {
        MediaPlayer player { get; set; }

        public bool IsToShowMessageBoxOfExceptions { get; set; }
        public string AudioFileFullPath { get; private set; }
        TimeSpan NaturalDurationTimeSpan { get; set; }
        public bool IsActuallyPlaying
        {
            get
            {
                bool ret = false;
                //player.Dispatcher.Invoke(new Action(() =>
                //{
                ret = player.Position < NaturalDurationTimeSpan;
                //}));
                return ret;
            }
        }

        public MediaPlayerWrapper()
        {
            IsToShowMessageBoxOfExceptions = true;
            AudioFileFullPath = "";
            player = new MediaPlayer();
        }

        public bool StartAsync(string audioFilePath)
        {
            //player.Dispatcher.Invoke(new Action(() =>
            //{
            try
            {
                if (string.IsNullOrWhiteSpace(audioFilePath)) { throw new ArgumentNullException(audioFilePath); }
                var fullPath = System.IO.Path.GetFullPath(audioFilePath);
                if (System.IO.File.Exists(fullPath) == false) { throw new System.IO.FileNotFoundException("Could not find the file", audioFilePath); }

                player.Open(new Uri(fullPath));
                if (player.HasAudio == false)
                {
                    throw new ArgumentException("The file does not have audio.");
                }
                if (player.NaturalDuration.HasTimeSpan == false)
                {
                    throw new ArgumentException("The file does not have NaturalDuration.TimeSpan.");
                }
                NaturalDurationTimeSpan = player.NaturalDuration.TimeSpan;
                AudioFileFullPath = fullPath;
                player.Play();
                return true;
            }
            catch (Exception ex)
            {
                if (IsToShowMessageBoxOfExceptions) { MessageBox.Show(ex.Message); }
                return false;
            }
            //}));
        }

        public bool Stop()
        {
            //player.Dispatcher.Invoke(new Action(() =>
            //{
            try
            {
                player.Stop();
                return true;
            }
            catch (Exception ex)
            {
                if (IsToShowMessageBoxOfExceptions) { MessageBox.Show(ex.Message); }
                return false;
            }
            //}));
        }
    }
}
