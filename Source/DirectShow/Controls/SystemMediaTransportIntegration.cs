using System;
using System.Windows;
using System.Windows.Threading;
using Windows.Media;
using Windows.Media.Playback;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace WPFMediaKit.DirectShow.Controls
{
    /// <summary>
    /// Publishes the playback of a <see cref="MediaUriElement"/> to the Windows
    /// System Media Transport Controls (media keys and the system media flyout)
    /// and routes their commands back to the element.
    /// </summary>
    /// <remarks>
    /// Members that touch WinRT types must only be invoked through <see cref="Run"/>,
    /// so systems without the WinRT media APIs fail inside its catch instead of
    /// failing to JIT the caller.
    /// </remarks>
    internal sealed class SystemMediaTransportIntegration
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SystemMediaTransportIntegration));

        private readonly MediaUriElement m_owner;

        /// <summary>
        /// Throttles timeline updates while the media position keeps changing
        /// </summary>
        private readonly DispatcherTimer m_timelineTimer;

        /// <summary>
        /// Only hosts the SystemMediaTransportControls, it never plays anything
        /// </summary>
        private MediaPlayer m_player;

        private SystemMediaTransportControls m_controls;
        private bool m_isMediaOpen;
        private bool m_hasEnded;

        public SystemMediaTransportIntegration(MediaUriElement owner)
        {
            m_owner = owner;

            m_timelineTimer = new DispatcherTimer(DispatcherPriority.Background, owner.Dispatcher)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            m_timelineTimer.Tick += OnTimelineTimerTick;

            owner.MediaOpened += OnMediaOpened;
            owner.MediaClosed += OnMediaClosed;
            owner.MediaEnded += OnMediaEnded;
            owner.MediaPositionChanged += OnMediaPositionChanged;
            owner.Loaded += OnOwnerLoaded;
            owner.Unloaded += OnOwnerUnloaded;
        }

        /// <summary>
        /// Removes the media from the system media transport controls
        /// </summary>
        public void Close()
        {
            m_isMediaOpen = false;
            m_hasEnded = false;
            Release();
        }

        public void OnEnabledChanged()
        {
            if (m_owner.EnableSystemMediaTransportControls)
                Refresh();
            else
                Release();
        }

        /// <summary>
        /// Called when the PlayerState or IsPlaying of the owner has changed
        /// </summary>
        public void OnPlaybackStateChanged()
        {
            if (m_owner.IsPlaying || m_owner.PlayerState != PlayerState.Playing)
                m_hasEnded = false;

            if (m_controls == null)
                return;

            Run(UpdatePlaybackStatus);
            Run(UpdateTimeline);
        }

        private void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            m_isMediaOpen = true;
            m_hasEnded = false;
            Refresh();
        }

        private void OnMediaClosed(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            m_hasEnded = true;

            if (m_controls == null)
                return;

            Run(UpdatePlaybackStatus);
            Run(UpdateTimeline);
        }

        private void OnMediaPositionChanged(object sender, RoutedEventArgs e)
        {
            if (m_controls != null && !m_timelineTimer.IsEnabled)
                m_timelineTimer.Start();
        }

        private void OnTimelineTimerTick(object sender, EventArgs e)
        {
            m_timelineTimer.Stop();

            if (m_controls != null)
                Run(UpdateTimeline);
        }

        private void OnOwnerLoaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void OnOwnerUnloaded(object sender, RoutedEventArgs e)
        {
            Release();
        }

        private void Refresh()
        {
            if (!m_isMediaOpen || !m_owner.EnableSystemMediaTransportControls)
                return;

            if (m_controls == null && !Run(CreateControls))
            {
                Release();
                return;
            }

            Run(UpdateDisplay);
            Run(UpdatePlaybackStatus);
            Run(UpdateTimeline);
        }

        private void Release()
        {
            m_timelineTimer.Stop();

            if (m_player != null)
                Run(ReleaseControls);
        }

        private void CreateControls()
        {
            m_player = new MediaPlayer();
            m_player.CommandManager.IsEnabled = false;

            m_controls = m_player.SystemMediaTransportControls;
            m_controls.IsEnabled = true;
            m_controls.IsPlayEnabled = true;
            m_controls.IsPauseEnabled = true;
            m_controls.IsStopEnabled = true;
            m_controls.ButtonPressed += OnButtonPressed;
            m_controls.PlaybackPositionChangeRequested += OnPlaybackPositionChangeRequested;
        }

        private void ReleaseControls()
        {
            var player = m_player;
            var controls = m_controls;
            m_player = null;
            m_controls = null;

            try
            {
                if (controls != null)
                {
                    controls.ButtonPressed -= OnButtonPressed;
                    controls.PlaybackPositionChangeRequested -= OnPlaybackPositionChangeRequested;
                    controls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    controls.IsEnabled = false;
                }
            }
            finally
            {
                player.Dispose();
            }
        }

        private void UpdateDisplay()
        {
            var updater = m_controls.DisplayUpdater;
            var title = GetTitle(m_owner.Source);

            updater.ClearAll();
            if (m_owner.HasVideo)
            {
                updater.Type = MediaPlaybackType.Video;
                updater.VideoProperties.Title = title;
            }
            else
            {
                updater.Type = MediaPlaybackType.Music;
                updater.MusicProperties.Title = title;
            }
            updater.Update();
        }

        private void UpdatePlaybackStatus()
        {
            m_controls.PlaybackStatus = GetPlaybackStatus();
        }

        private MediaPlaybackStatus GetPlaybackStatus()
        {
            switch (m_owner.PlayerState)
            {
                case PlayerState.Playing:
                    if (m_owner.IsPlaying)
                        return MediaPlaybackStatus.Playing;
                    return m_hasEnded ? MediaPlaybackStatus.Stopped : MediaPlaybackStatus.Paused;
                case PlayerState.Paused:
                case PlayerState.SteppingFrames:
                    return MediaPlaybackStatus.Paused;
                default:
                    return MediaPlaybackStatus.Stopped;
            }
        }

        private void UpdateTimeline()
        {
            var timeline = new SystemMediaTransportControlsTimelineProperties();

            if (m_owner.CurrentPositionFormat == MediaPositionFormat.MediaTime)
            {
                long duration = Math.Max(0, m_owner.MediaDuration);
                long position = Math.Max(0, Math.Min(m_owner.MediaPosition, duration));

                timeline.StartTime = TimeSpan.Zero;
                timeline.EndTime = TimeSpan.FromTicks(duration);
                timeline.MinSeekTime = TimeSpan.Zero;
                timeline.MaxSeekTime = TimeSpan.FromTicks(duration);
                timeline.Position = TimeSpan.FromTicks(position);
            }

            m_controls.UpdateTimelineProperties(timeline);
        }

        private void OnButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            var button = args.Button;
            m_owner.Dispatcher.BeginInvoke((Action)(() => ExecuteButton(button)));
        }

        private void ExecuteButton(SystemMediaTransportControlsButton button)
        {
            if (m_controls == null)
                return;

            switch (button)
            {
                case SystemMediaTransportControlsButton.Play:
                    m_owner.Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    m_owner.Pause();
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    m_owner.Stop();
                    break;
            }
        }

        private void OnPlaybackPositionChangeRequested(SystemMediaTransportControls sender, PlaybackPositionChangeRequestedEventArgs args)
        {
            var position = args.RequestedPlaybackPosition;
            m_owner.Dispatcher.BeginInvoke((Action)(() => Seek(position)));
        }

        private void Seek(TimeSpan position)
        {
            if (m_controls == null || m_owner.CurrentPositionFormat != MediaPositionFormat.MediaTime)
                return;

            m_owner.MediaPosition = Math.Max(0, Math.Min(position.Ticks, m_owner.MediaDuration));
            Run(UpdateTimeline);
        }

        private static string GetTitle(Uri source)
        {
            if (source == null)
                return string.Empty;

            string path = !source.IsAbsoluteUri ? source.OriginalString
                : source.IsFile ? source.LocalPath
                : Uri.UnescapeDataString(source.AbsolutePath);

            string name = path.Substring(path.LastIndexOfAny(new[] { '/', '\\' }) + 1);
            int extension = name.LastIndexOf('.');
            if (extension > 0)
                name = name.Substring(0, extension);

            return name.Length > 0 ? name : source.OriginalString;
        }

        private static bool Run(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex, "System media transport controls call {0} failed", action.Method.Name);
                return false;
            }
        }
    }
}
