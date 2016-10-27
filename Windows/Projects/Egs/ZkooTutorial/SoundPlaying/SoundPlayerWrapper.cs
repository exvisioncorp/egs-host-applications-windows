namespace Exvision.Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Media;
    using System.Diagnostics;
    using System.Windows;
    using System.ComponentModel;
    using System.IO;

    class SoundPlayerWrapper : IAudioPlayer
    {
        MemoryStream stream { get; set; }
        SoundPlayer player { get; set; }
        Action playingAction { get; set; }
        Task playingTask { get; set; }

        public bool IsToShowMessageBoxOfExceptions { get; set; }
        public string AudioFileFullPath { get; private set; }
        public bool IsActuallyPlaying { get; private set; }

        public SoundPlayerWrapper()
        {
            IsToShowMessageBoxOfExceptions = true;
            AudioFileFullPath = "";
            IsActuallyPlaying = false;
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
                playingAction = new Action(() =>
                {
                    try
                    {
                        if (stream != null) { stream.Dispose(); stream = null; }
                        stream = new MemoryStream(File.ReadAllBytes(fullPath));

                        player = new SoundPlayer();
                        player.Stream = stream;
                        if (false)
                        {
                            // 全部関係ねえ
                            player.Disposed += (sender, e) => { Debugger.Break(); };
                            player.LoadCompleted += (sender, e) => { Debugger.Break(); };
                            player.SoundLocationChanged += (sender, e) => { Debugger.Break(); };
                            player.StreamChanged += (sender, e) => { Debugger.Break(); };
                        }
                        player.Load();
                        player.PlaySync();
                    }
                    catch (Exception ex)
                    {
                        if (IsToShowMessageBoxOfExceptions) { MessageBox.Show(ex.Message); }
                        Debug.WriteLine(ex.Message);
                    }
                });
                IsActuallyPlaying = true;
                playingTask = Task.Run(playingAction);
                playingTask.ContinueWith(e => { IsActuallyPlaying = false; });
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
                if (playingTask != null)
                {
                    playingTask.Dispose();
                }
                if (player != null)
                {
                    player.Stop();
                    if (stream != null) { stream.Dispose(); stream = null; }
                    player.Stream = null;
                    player.Dispose();
                    player = null;
                }
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
