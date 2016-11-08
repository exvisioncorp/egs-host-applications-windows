namespace Exvision.Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Diagnostics;

    class CsCoreWrapper : IAudioPlayer
    {
        public bool IsToShowMessageBoxOfExceptions { get; set; }
        public string AudioFileFullPath { get; private set; }
        public bool IsActuallyPlaying
        {
            get
            {
                if (soundOut == null) { return false; }
                return soundOut.PlaybackState == CSCore.SoundOut.PlaybackState.Playing;
            }
        }

        CSCore.IWaveSource waveSource { get; set; }
        CSCore.SoundOut.ISoundOut soundOut { get; set; }

        public CsCoreWrapper()
        {
            IsToShowMessageBoxOfExceptions = true;
        }

        public bool StartAsync(string audioFilePath)
        {
            try
            {
                Stop();

                if (string.IsNullOrWhiteSpace(audioFilePath)) { throw new ArgumentNullException(audioFilePath); }
                var fullPath = System.IO.Path.GetFullPath(audioFilePath);
                if (System.IO.File.Exists(fullPath) == false) { throw new System.IO.FileNotFoundException("Could not find the file", audioFilePath); }

                var ext = System.IO.Path.GetExtension(fullPath).ToLower();
                switch (ext)
                {
                    case ".wav":
                        break;
                    default:
                        throw new InvalidOperationException("Extension must be .wav or .ogg");
                }
                waveSource = CSCore.Codecs.CodecFactory.Instance.GetCodec(fullPath);
                if (waveSource == null) { throw new ArgumentException("Could not open the file", fullPath); }
                AudioFileFullPath = fullPath;
                if (CSCore.SoundOut.WasapiOut.IsSupportedOnCurrentPlatform)
                {
                    soundOut = new CSCore.SoundOut.WasapiOut();
                }
                else
                {
                    soundOut = new CSCore.SoundOut.DirectSoundOut();
                }
                soundOut.Initialize(waveSource);
                soundOut.Play();
                return true;
            }
            catch (Exception ex)
            {
                if (IsToShowMessageBoxOfExceptions) { MessageBox.Show(ex.Message); }
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (soundOut == null) { return false; }
                if (soundOut.PlaybackState != CSCore.SoundOut.PlaybackState.Stopped) { soundOut.Stop(); }
                if (waveSource != null) { waveSource.Dispose(); waveSource = null; }
                GC.WaitForPendingFinalizers();
                if (soundOut != null) { soundOut.Dispose(); soundOut = null; }
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
