namespace Egs.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.Diagnostics;
    using Egs.DotNetUtility;

    /// <summary>
    /// When you play multiple different videos on the same Application at the same time, 
    /// create a new instance of "class MediaElementReusableInMultipleUserControls", 
    /// and call SetTheOtherMediaElementReusableInMultipleUserControls method of this class.
    /// </summary>
    public partial class MediaElementUserControl : UserControl
    {
        public static MediaElementReusableInMultipleUserControls DefaultSharedReusableMediaElement { get; private set; }
        static MediaElementUserControl()
        {
            DefaultSharedReusableMediaElement = new MediaElementReusableInMultipleUserControls();
        }

        public static readonly DependencyProperty IsToShowMessageBoxWhenFileNotFoundProperty = DependencyProperty.Register("IsToShowMessageBoxWhenFileNotFound", typeof(bool), typeof(MediaElementUserControl), new PropertyMetadata(false));
        public bool IsToShowMessageBoxWhenFileNotFound
        {
            get { return (bool)GetValue(IsToShowMessageBoxWhenFileNotFoundProperty); }
            set { SetValue(IsToShowMessageBoxWhenFileNotFoundProperty, value); }
        }

        public static readonly DependencyProperty TimelineSliderVisibilityProperty = DependencyProperty.Register("TimelineSliderVisibility", typeof(Visibility), typeof(MediaElementUserControl), new FrameworkPropertyMetadata(Visibility.Visible));
        public Visibility TimelineSliderVisibility
        {
            get { return (Visibility)GetValue(TimelineSliderVisibilityProperty); }
            set { SetValue(TimelineSliderVisibilityProperty, value); }
        }

        MediaElementReusableInMultipleUserControls mediaElement { get; set; }

        bool _IsPlaying = false;
        public event EventHandler IsPlayingChanged;
        protected virtual void OnIsPlayingChanged(EventArgs e)
        {
            var t = IsPlayingChanged; if (t != null) { t(this, EventArgs.Empty); }
        }
        public bool IsPlaying
        {
            get { return _IsPlaying; }
            private set
            {
                _IsPlaying = value;
                OnIsPlayingChanged(EventArgs.Empty);
                UpdateResumeButtonImageVisibility();
            }
        }

        protected static void MediaElementSourceUriChangedForFramework(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as MediaElementUserControl).OnMediaElementSourceUriChanged(e);
        }
        public static readonly DependencyProperty MediaElementSourceUriProperty = DependencyProperty.Register("MediaElementSourceUri", typeof(Uri), typeof(MediaElementUserControl),
            new FrameworkPropertyMetadata(default(Uri), new PropertyChangedCallback(MediaElementSourceUriChangedForFramework)));
        public Uri MediaElementSourceUri
        {
            get { return (Uri)GetValue(MediaElementSourceUriProperty); }
            set { SetValue(MediaElementSourceUriProperty, value); }
        }
        public event EventHandler MediaElementSourceUriChanged;
        protected virtual void OnMediaElementSourceUriChanged(DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("[OnMediaElementSourceUriChanged] e: " + e.ToString());
            if (resumeButtonImage.Source == null)
            {
                resumeButtonImage.Source = BitmapImageUtility.LoadBitmapImageFromFile(@".\Resources\PlayButton.png");
            }
            try
            {
                UnloadVideoData();
                IsEnabled = true;
                _HasMediaEnded = false;
                Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsEnabled = false;
            }
            var t = MediaElementSourceUriChanged; if (t != null) { t(this, EventArgs.Empty); }
        }

        public void SetMediaElementSourceUriByFilePath(string path, UriKind uriKind)
        {
            if (System.IO.File.Exists(path) == false)
            {
                if (IsToShowMessageBoxWhenFileNotFound)
                {
                    MessageBox.Show("Could not find the video file." + Environment.NewLine + "File Path: " + path);
                }
                IsEnabled = false;
                return;
            }
            MediaElementSourceUri = new Uri(path, uriKind);
        }

        protected static void IsToShowResumeButtonChangedForFramework(DependencyObject sender, DependencyPropertyChangedEventArgs e) { (sender as MediaElementUserControl).OnIsToShowResumeButtonChanged(e); }
        public static readonly DependencyProperty IsToShowResumeButtonProperty = DependencyProperty.Register("IsToShowResumeButton", typeof(bool), typeof(MediaElementUserControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsToShowResumeButtonChangedForFramework)));
        public bool IsToShowResumeButton { get { return (bool)GetValue(IsToShowResumeButtonProperty); } set { SetValue(IsToShowResumeButtonProperty, value); } }
        protected virtual void OnIsToShowResumeButtonChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateResumeButtonImageVisibility();
        }

        public static readonly DependencyProperty CanPauseAndResumeByMouseDownProperty = DependencyProperty.Register("CanPauseAndResumeByMouseDown", typeof(bool), typeof(MediaElementUserControl), new FrameworkPropertyMetadata(true));
        public bool CanPauseAndResumeByMouseDown { get { return (bool)GetValue(CanPauseAndResumeByMouseDownProperty); } set { SetValue(CanPauseAndResumeByMouseDownProperty, value); } }

        public static readonly DependencyProperty IsToReplayAutomaticallyProperty = DependencyProperty.Register("IsToReplayAutomatically", typeof(bool), typeof(MediaElementUserControl), new FrameworkPropertyMetadata(true));
        public bool IsToReplayAutomatically { get { return (bool)GetValue(IsToReplayAutomaticallyProperty); } set { SetValue(IsToReplayAutomaticallyProperty, value); } }


        System.Windows.Threading.DispatcherTimer _SeekUpdateTimer;
        bool _IsUpdatingSeekBarFromModel = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _HasMediaEnded;
        public event EventHandler HasMediaEndedChanged;
        protected virtual void OnHasMediaEndedChanged(EventArgs e)
        {
            var t = HasMediaEndedChanged; if (t != null) { t(this, e); }
        }
        public bool HasMediaEnded
        {
            get { return _HasMediaEnded; }
            internal set { _HasMediaEnded = value; OnHasMediaEndedChanged(EventArgs.Empty); }
        }

        void UpdateResumeButtonImageVisibility()
        {
            resumeButtonImage.Visibility = (IsToShowResumeButton && (IsPlaying == false)) ? Visibility.Visible : Visibility.Hidden;
        }

        public MediaElementUserControl()
        {
            InitializeComponent();

            mediaElement = DefaultSharedReusableMediaElement;

            _SeekUpdateTimer = new System.Windows.Threading.DispatcherTimer();
            _SeekUpdateTimer.Interval = TimeSpan.FromSeconds(0.1);
            _SeekUpdateTimer.Tick += delegate
            {
                _IsUpdatingSeekBarFromModel = true;
                timelineSlider.Value = mediaElement.Position.TotalMilliseconds;
                _IsUpdatingSeekBarFromModel = false;
            };

            resumeButtonImage.MouseDown += delegate
            {
                Play();
            };

            timelineSlider.ValueChanged += delegate
            {
                if (_IsUpdatingSeekBarFromModel) { return; }
                var ts = new TimeSpan(0, 0, 0, 0, (int)timelineSlider.Value);
                mediaElement.Position = ts;
            };

            volumeSlider.ValueChanged += delegate
            {
                mediaElement.Volume = (double)volumeSlider.Value;
            };
        }

        public void SetTheOtherMediaElementReusableInMultipleUserControls(MediaElementReusableInMultipleUserControls value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            mediaElement = value;
        }

        /// <summary>
        /// Even if you just change the Source URI, the movie will not be changed.  In such case, you need to call this method.
        /// When you call this method before you replay the same video, MediaElement become black for a while, so you should not call this method.
        /// </summary>
        public void UnloadVideoData()
        {
            Trace.Assert(mediaElement != null);

            if (mediaElement.Source != null)
            {
                Debug.WriteLine("Update mediaElement.Source by null");
                mediaElement.Stop();
                mediaElement.Source = null;
            }
        }

        public void Play()
        {
            Trace.Assert(mediaElement != null);

            // This code is called in this.Replay() method from every MediaEnded event.
            if (HasMediaEnded)
            {
                mediaElement.Position = TimeSpan.Zero;
            }
            // Re-set mediaElement to mediaElement.OwnerGrid.Children, before re-set mediaElement itself.
            // The timing of changing the properties of mediaElement, should be here.  Not in SetTheOtherMediaElementReusableInMultipleUserControls()
            //            if (mediaElement.Owner != this)
            {
                Debug.WriteLine("Update mediaElement.Owner");
                UnloadVideoData();
                if (mediaElement.OwnerGrid != null) { mediaElement.OwnerGrid.Children.Remove(mediaElement); }
                mediaElementOwnerGrid.Children.Add(mediaElement);
                mediaElement.Owner = this;
            }
            Debug.WriteLine("Update mediaElement.Source by " + MediaElementSourceUri.ToString());
            mediaElement.Source = MediaElementSourceUri;
            mediaElement.Volume = (double)volumeSlider.Value;
            var waitBufferingStopwatch = Stopwatch.StartNew();
            while (waitBufferingStopwatch.ElapsedMilliseconds < 2000)
            {
                if (mediaElement.IsBuffering == false) { break; }
                Debug.WriteLine("MediaElementReusableInMultipleUserControls.IsBuffering == true");
                System.Threading.Thread.Sleep(50);
            }
            mediaElement.Play();
            _SeekUpdateTimer.Start();
            HasMediaEnded = false;
            IsPlaying = true;
        }

        public void Pause()
        {
            Trace.Assert(mediaElement != null);

            mediaElement.Pause();
            _SeekUpdateTimer.Stop();
            IsPlaying = false;
        }

        public void Replay()
        {
            Trace.Assert(mediaElement != null);

            mediaElement.Position = TimeSpan.Zero;
            Play();
        }
    }

    /// <summary>
    /// In some PCs, a driver error can occur when more than 4 MediaElements are created, even if they are not displayed.  
    /// But in various case, you need to create some UserControl which has MediaElement inside, and create more than 4 instances of the UserControl in multiple Pages.  
    /// So, in that case, make the instances of this class which can run at the same time on a Application, 
    /// and call MediaElementUserControl.SetTheOtherMediaElementReusableInMultipleUserControls() with a related instance selected from the instances.  
    /// </summary>
    public class MediaElementReusableInMultipleUserControls : MediaElement
    {
        public static int AllInstancesCount { get; private set; }
        public MediaElementUserControl Owner { get; set; }
        public Grid OwnerGrid { get { return (Owner != null) ? Owner.mediaElementOwnerGrid : null; } }

        public MediaElementReusableInMultipleUserControls()
        {
            AllInstancesCount++;
            if (ApplicationCommonSettings.IsDebugging && AllInstancesCount >= 4)
            {
                MessageBox.Show("In some PCs, a driver error can occur when more than 4 MediaElements are created, even if they are not displayed.");
            }

            LoadedBehavior = MediaState.Manual;
            UnloadedBehavior = MediaState.Stop;

            this.MediaOpened += delegate
            {
                if (Owner == null) { return; }
                if (this.NaturalDuration.HasTimeSpan == false) { return; }
                Owner.timelineSlider.Maximum = this.NaturalDuration.TimeSpan.TotalMilliseconds;
            };
            this.MediaEnded += delegate
            {
                if (Owner == null) { return; }
                Owner.HasMediaEnded = true;
                if (Owner.IsToReplayAutomatically) { Owner.Replay(); } else { Owner.Pause(); }
            };
            this.MouseDown += delegate
            {
                if (Owner == null) { return; }
                if (Owner.CanPauseAndResumeByMouseDown == false) { return; }
                if (Owner.HasMediaEnded) { return; }
                if (Owner.IsPlaying) { Owner.Pause(); } else { Owner.Play(); }
            };
        }
    }
}
