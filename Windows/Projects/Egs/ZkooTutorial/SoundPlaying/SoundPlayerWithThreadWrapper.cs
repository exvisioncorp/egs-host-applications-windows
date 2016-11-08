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
    using System.Threading;

    class SoundPlayerWithThreadWrapper : IAudioPlayer
    {
        public bool IsToShowMessageBoxOfExceptions { get; set; }
        public string AudioFileFullPath { get; private set; }

        Thread thread { get; set; }

        public bool IsActuallyPlaying { get; private set; }

        public SoundPlayerWithThreadWrapper()
        {
            IsToShowMessageBoxOfExceptions = true;
            AudioFileFullPath = "";
            IsActuallyPlaying = false;
        }

        void PlaySoundInternal()
        {
            IsActuallyPlaying = true;
            try
            {
                using (var player = new SoundPlayer(AudioFileFullPath))
                {
                    player.Load();
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                if (IsToShowMessageBoxOfExceptions) { MessageBox.Show(ex.Message); }
            }
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

                thread = new Thread(new ThreadStart(PlaySoundInternal));
                thread.Start();
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
                if (thread == null && IsActuallyPlaying) { Debugger.Break(); }
                if (thread != null)
                {
                    //AsynchronousThreadAbort が検出されました。
                    //Message: スレッド 10744 で実行中のユーザー コードは、スレッド 5956 を中止しようとしています。中止されようとしているスレッドがグローバル状態を変更する操作、またはネイティブ リソースを使用する操作の途中だった場合、このスレッドの中止によって壊れた状態、またはリソース リークが生じる可能性があります。現在実行中のスレッド以外のスレッドを中止する操作は避けることを強く奨励します。
                    thread.Abort();
                    thread.Join();
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
