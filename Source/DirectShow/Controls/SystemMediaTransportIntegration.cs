using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace WPFMediaKit.DirectShow.Controls
{
    /// <summary>
    /// Publishes the playback of a <see cref="MediaUriElement"/> to the Windows
    /// System Media Transport Controls (media keys and the system media flyout)
    /// and routes their commands back to the element.
    /// </summary>
    internal sealed class SystemMediaTransportIntegration
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SystemMediaTransportIntegration));

        private readonly MediaUriElement m_owner;

        /// <summary>
        /// Throttles timeline updates while the media position keeps changing
        /// </summary>
        private readonly DispatcherTimer m_timelineTimer;

        /// <summary>
        /// A Windows.Media.Playback.MediaPlayer that only hosts the
        /// SystemMediaTransportControls, it never plays anything
        /// </summary>
        private object m_player;

        private ISystemMediaTransportControls m_controls;
        private ISystemMediaTransportControlsTimelineProperties m_timeline;
        private long m_buttonPressedToken;
        private long m_positionChangeRequestedToken;
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
            m_player = WindowsRuntimeActivator.ActivateInstance("Windows.Media.Playback.MediaPlayer");
            ((IMediaPlayer3)m_player).get_CommandManager().put_IsEnabled(false);

            m_timeline = (ISystemMediaTransportControlsTimelineProperties)WindowsRuntimeActivator.ActivateInstance(
                "Windows.Media.SystemMediaTransportControlsTimelineProperties");

            m_controls = ((IMediaPlayer2)m_player).get_SystemMediaTransportControls();
            m_controls.put_IsEnabled(true);
            m_controls.put_IsPlayEnabled(true);
            m_controls.put_IsPauseEnabled(true);
            m_controls.put_IsStopEnabled(true);
            m_buttonPressedToken = m_controls.add_ButtonPressed(
                new ButtonPressedEventHandler(OnButtonPressed));
            m_positionChangeRequestedToken = ((ISystemMediaTransportControls2)m_controls).add_PlaybackPositionChangeRequested(
                new PlaybackPositionChangeRequestedEventHandler(OnPlaybackPositionChangeRequested));
        }

        private void ReleaseControls()
        {
            var player = m_player;
            var controls = m_controls;
            long buttonPressedToken = m_buttonPressedToken;
            long positionChangeRequestedToken = m_positionChangeRequestedToken;
            m_player = null;
            m_controls = null;
            m_timeline = null;
            m_buttonPressedToken = 0;
            m_positionChangeRequestedToken = 0;

            try
            {
                if (controls != null)
                {
                    if (buttonPressedToken != 0)
                        controls.remove_ButtonPressed(buttonPressedToken);
                    if (positionChangeRequestedToken != 0)
                        ((ISystemMediaTransportControls2)controls).remove_PlaybackPositionChangeRequested(positionChangeRequestedToken);
                    controls.put_PlaybackStatus(MediaPlaybackStatus.Closed);
                    controls.put_IsEnabled(false);
                }
            }
            finally
            {
                ((IClosable)player).Close();
                Marshal.FinalReleaseComObject(player);
            }
        }

        private void UpdateDisplay()
        {
            var updater = m_controls.get_DisplayUpdater();
            var title = GetTitle(m_owner.Source);

            updater.ClearAll();
            if (m_owner.HasVideo)
            {
                updater.put_Type(MediaPlaybackType.Video);
                updater.get_VideoProperties().put_Title(title);
            }
            else
            {
                updater.put_Type(MediaPlaybackType.Music);
                updater.get_MusicProperties().put_Title(title);
            }
            updater.Update();
        }

        private void UpdatePlaybackStatus()
        {
            m_controls.put_PlaybackStatus(GetPlaybackStatus());
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
            long duration = 0;
            long position = 0;

            if (m_owner.CurrentPositionFormat == MediaPositionFormat.MediaTime)
            {
                duration = Math.Max(0, m_owner.MediaDuration);
                position = Math.Max(0, Math.Min(m_owner.MediaPosition, duration));
            }

            m_timeline.put_StartTime(0);
            m_timeline.put_EndTime(duration);
            m_timeline.put_MinSeekTime(0);
            m_timeline.put_MaxSeekTime(duration);
            m_timeline.put_Position(position);
            ((ISystemMediaTransportControls2)m_controls).UpdateTimelineProperties(m_timeline);
        }

        /// <summary>
        /// Raised by the system on a thread pool thread
        /// </summary>
        private void OnButtonPressed(SystemMediaTransportControlsButton button)
        {
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

        /// <summary>
        /// Raised by the system on a thread pool thread
        /// </summary>
        private void OnPlaybackPositionChangeRequested(long position)
        {
            m_owner.Dispatcher.BeginInvoke((Action)(() => Seek(position)));
        }

        private void Seek(long position)
        {
            if (m_controls == null || m_owner.CurrentPositionFormat != MediaPositionFormat.MediaTime)
                return;

            m_owner.MediaPosition = Math.Max(0, Math.Min(position, m_owner.MediaDuration));
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
