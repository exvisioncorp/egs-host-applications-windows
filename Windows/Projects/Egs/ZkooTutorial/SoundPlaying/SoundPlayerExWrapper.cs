namespace Exvision.Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Media;
    using System.Threading;
    using System.Diagnostics;
    using System.Windows;
    using System.ComponentModel;
    using System.IO;

    class SoundPlayerExWrapper : IAudioPlayer
    {
        public bool IsToShowMessageBoxOfExceptions { get; set; }
        public string AudioFileFullPath { get; private set; }
        public bool IsActuallyPlaying
        {
            get
            {
                if (player == null) { return false; }
                return !(player.Finished);
            }
        }
        SoundPlayerEx player { get; set; }

        public SoundPlayerExWrapper()
        {
            IsToShowMessageBoxOfExceptions = true;
            AudioFileFullPath = "";
        }

        public bool StartAsync(string audioFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(audioFilePath)) { throw new ArgumentNullException(audioFilePath); }
                var fullPath = System.IO.Path.GetFullPath(audioFilePath);
                if (System.IO.File.Exists(fullPath) == false) { throw new System.IO.FileNotFoundException("Could not find the file", audioFilePath); }

                var ext = System.IO.Path.GetExtension(fullPath).ToLower();
                switch (ext)
                {
                    case ".wav":
                        break;
                    default:
                        throw new ArgumentException("Extension must be .wav");
                }

                AudioFileFullPath = fullPath;
                if (player != null && player.Finished == false) { player.Stop(); }
                player = new SoundPlayerEx(fullPath);
                player.PlayAsync();
                return true;
            }
            catch (Exception ex)
            {
                if (IsToShowMessageBoxOfExceptions) { MessageBox.Show(ex.Message); }
                return false;
            }
        }

#if false
        public void TogglePause()
        {
            throw new NotImplementedException();
        }
#endif

        public bool Stop()
        {
            try
            {
                if (player != null) { player.Stop(); }
                return true;
            }
            catch (Exception ex)
            {
                if (IsToShowMessageBoxOfExceptions) { MessageBox.Show(ex.Message); }
                return false;
            }
        }
    }
}
