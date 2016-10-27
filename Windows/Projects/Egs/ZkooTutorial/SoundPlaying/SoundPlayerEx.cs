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

    class SoundPlayerEx : SoundPlayer
    {
        public bool Finished { get; private set; }

        private Task _playTask;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private CancellationToken _ct;
        private string _fileName;
        private bool _playingAsync = false;

        public event EventHandler SoundFinished;

        public SoundPlayerEx(string soundLocation)
            : base(soundLocation)
        {
            _fileName = soundLocation;
            _ct = _tokenSource.Token;
        }

        public void PlayAsync()
        {
            Finished = false;
            _playingAsync = true;
            Task.Run(() =>
            {
                try
                {
                    double lenMs = NativeMethods.GetSoundLength(_fileName);
                    DateTime stopAt = DateTime.Now.AddMilliseconds(lenMs);
                    this.Play();
                    while (DateTime.Now < stopAt)
                    {
                        _ct.ThrowIfCancellationRequested();
                        //The delay helps reduce processor usage while "spinning"
                        Task.Delay(10).Wait();
                    }
                }
                catch (OperationCanceledException)
                {
                    base.Stop();
                }
                finally
                {
                    OnSoundFinished();
                }

            }, _ct);
        }

        public new void Stop()
        {
            if (_playingAsync)
                _tokenSource.Cancel();
            else
                base.Stop();
        }

        protected virtual void OnSoundFinished()
        {
            Finished = true;
            _playingAsync = false;

            EventHandler handler = SoundFinished;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
