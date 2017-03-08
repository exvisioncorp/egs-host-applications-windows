namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Diagnostics;
    using NAudio.Wave;

    class NAudioWrapper : IAudioPlayer
    {
        public bool IsToShowMessageBoxOfExceptions { get; set; }
        public string AudioFileFullPath { get; private set; }
        public bool IsActuallyPlaying
        {
            get
            {
                if (Stream == null) { return false; }
                return Stream.Position < Stream.Length;
            }
        }

        WaveStream Stream { get; set; }
        WaveOut Output { get; set; }

        public NAudioWrapper()
        {
            IsToShowMessageBoxOfExceptions = true;
        }

        public bool StartAsync(string audioFilePath)
        {
            string fullPath = "";
            try
            {
                if (string.IsNullOrWhiteSpace(audioFilePath)) { throw new ArgumentNullException(audioFilePath); }
                fullPath = System.IO.Path.GetFullPath(audioFilePath);
                if (System.IO.File.Exists(fullPath) == false) { throw new System.IO.FileNotFoundException("Could not find the file", audioFilePath); }

                WaveStream newStream = null;

                var ext = System.IO.Path.GetExtension(fullPath).ToUpperInvariant();
                switch (ext)
                {
                    case ".OGG":
                        newStream = new NAudio.Vorbis.VorbisWaveReader(fullPath);
                        break;
                    case ".WAV":
                        newStream = new WaveFileReader(fullPath);
                        break;
                    default:
                        throw new ArgumentException("Extension must be .wav or .ogg");
                }
                if (newStream == null) { throw new ArgumentException("Could not open the file", fullPath); }
                Stop();
                Stream = newStream;
                if (Output == null) { Output = new WaveOut(); }
                // NOTE: this "true" means "convertTo16Bit", and in some audio devices, it seems to need this convert!!
                if (Output != null) { Output.Init(Stream.ToSampleProvider(), true); }
                if (Output != null) { Output.Play(); }
                AudioFileFullPath = fullPath;
                return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message + Environment.NewLine;
                msg += "the argument audioFilePath: " + audioFilePath + Environment.NewLine;
                msg += "fullPath: " + fullPath + Environment.NewLine;
                if (IsToShowMessageBoxOfExceptions) { MessageBox.Show(msg); }
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (Output == null) { return false; }

                if (Output.PlaybackState != PlaybackState.Stopped) { Output.Stop(); }
                if (true && Stream != null) { Stream.Dispose(); Stream = null; }
                GC.WaitForPendingFinalizers();
                if (true && Output != null) { Output.Dispose(); Output = null; }
                GC.WaitForPendingFinalizers();
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
