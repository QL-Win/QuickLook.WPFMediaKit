using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WPFMediaKit.DirectShow.Controls
{
    /* Hand written ABI of the Windows.Media WinRT types, so the library does not
     * need any WinRT metadata to build. Vtable slots must follow the declaration
     * order of the metadata, so unused members before a used one stay declared. */

    internal enum MediaPlaybackStatus
    {
        Closed = 0,
        Changing = 1,
        Stopped = 2,
        Playing = 3,
        Paused = 4
    }

    internal enum MediaPlaybackType
    {
        Unknown = 0,
        Music = 1,
        Video = 2,
        Image = 3
    }

    internal enum SystemMediaTransportControlsButton
    {
        Play = 0,
        Pause = 1,
        Stop = 2,
        Record = 3,
        FastForward = 4,
        Rewind = 5,
        Next = 6,
        Previous = 7,
        ChannelUp = 8,
        ChannelDown = 9
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("99fa3ff4-1742-42a6-902e-087d41f965ec"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface ISystemMediaTransportControls
    {
        MediaPlaybackStatus get_PlaybackStatus();
        void put_PlaybackStatus(MediaPlaybackStatus value);
        ISystemMediaTransportControlsDisplayUpdater get_DisplayUpdater();
        int get_SoundLevel();
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsEnabled();
        void put_IsEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsPlayEnabled();
        void put_IsPlayEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsStopEnabled();
        void put_IsStopEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsPauseEnabled();
        void put_IsPauseEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsRecordEnabled();
        void put_IsRecordEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsFastForwardEnabled();
        void put_IsFastForwardEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsRewindEnabled();
        void put_IsRewindEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsPreviousEnabled();
        void put_IsPreviousEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsNextEnabled();
        void put_IsNextEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsChannelUpEnabled();
        void put_IsChannelUpEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsChannelDownEnabled();
        void put_IsChannelDownEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        long add_ButtonPressed(IButtonPressedEventHandler handler);
        void remove_ButtonPressed(long token);
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("ea98d2f6-7f3c-4af2-a586-72889808efb1"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface ISystemMediaTransportControls2
    {
        int get_AutoRepeatMode();
        void put_AutoRepeatMode(int value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_ShuffleEnabled();
        void put_ShuffleEnabled([MarshalAs(UnmanagedType.U1)] bool value);
        double get_PlaybackRate();
        void put_PlaybackRate(double value);
        void UpdateTimelineProperties(ISystemMediaTransportControlsTimelineProperties timelineProperties);
        long add_PlaybackPositionChangeRequested(IPlaybackPositionChangeRequestedEventHandler handler);
        void remove_PlaybackPositionChangeRequested(long token);
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("8abbc53e-fa55-4ecf-ad8e-c984e5dd1550"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface ISystemMediaTransportControlsDisplayUpdater
    {
        MediaPlaybackType get_Type();
        void put_Type(MediaPlaybackType value);
        [return: MarshalAs(UnmanagedType.HString)] string get_AppMediaId();
        void put_AppMediaId([MarshalAs(UnmanagedType.HString)] string value);
        IntPtr get_Thumbnail();
        void put_Thumbnail(IntPtr value);
        IMusicDisplayProperties get_MusicProperties();
        IVideoDisplayProperties get_VideoProperties();
        IntPtr get_ImageProperties();
        IntPtr CopyFromFileAsync(MediaPlaybackType type, IntPtr source);
        void ClearAll();
        void Update();
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("6bbf0c59-d0a0-4d26-92a0-f978e1d18e7b"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface IMusicDisplayProperties
    {
        [return: MarshalAs(UnmanagedType.HString)] string get_Title();
        void put_Title([MarshalAs(UnmanagedType.HString)] string value);
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("5609fdb1-5d2d-4872-8170-45dee5bc2f5c"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface IVideoDisplayProperties
    {
        [return: MarshalAs(UnmanagedType.HString)] string get_Title();
        void put_Title([MarshalAs(UnmanagedType.HString)] string value);
    }

    /// <summary>
    /// Times are Windows.Foundation.TimeSpan values in 100ns units
    /// </summary>
    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("5125316a-c3a2-475b-8507-93534dc88f15"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface ISystemMediaTransportControlsTimelineProperties
    {
        long get_StartTime();
        void put_StartTime(long value);
        long get_EndTime();
        void put_EndTime(long value);
        long get_MinSeekTime();
        void put_MinSeekTime(long value);
        long get_MaxSeekTime();
        void put_MaxSeekTime(long value);
        long get_Position();
        void put_Position(long value);
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("b7f47116-a56f-4dc8-9e11-92031f4a87c2"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface ISystemMediaTransportControlsButtonPressedEventArgs
    {
        SystemMediaTransportControlsButton get_Button();
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("b4493f88-eb28-4961-9c14-335e44f3e125"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface IPlaybackPositionChangeRequestedEventArgs
    {
        long get_RequestedPlaybackPosition();
    }

    /// <summary>
    /// TypedEventHandler&lt;SystemMediaTransportControls, SystemMediaTransportControlsButtonPressedEventArgs&gt;,
    /// the IID is derived from the parameterized type signature
    /// </summary>
    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("0557e996-7b23-5bae-aa81-ea0d671143a4"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IButtonPressedEventHandler
    {
        void Invoke(IntPtr sender, ISystemMediaTransportControlsButtonPressedEventArgs args);
    }

    /// <summary>
    /// TypedEventHandler&lt;SystemMediaTransportControls, PlaybackPositionChangeRequestedEventArgs&gt;,
    /// the IID is derived from the parameterized type signature
    /// </summary>
    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("44e34f15-bdc0-50a7-ace4-39e91fb753f1"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPlaybackPositionChangeRequestedEventHandler
    {
        void Invoke(IntPtr sender, IPlaybackPositionChangeRequestedEventArgs args);
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("3c841218-2123-4fc5-9082-2f883f77bdf5"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface IMediaPlayer2
    {
        ISystemMediaTransportControls get_SystemMediaTransportControls();
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("ee0660da-031b-4feb-bd9b-92e0a0a8d299"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface IMediaPlayer3
    {
        long add_IsMutedChanged(IntPtr handler);
        void remove_IsMutedChanged(long token);
        long add_SourceChanged(IntPtr handler);
        void remove_SourceChanged(long token);
        double get_AudioBalance();
        void put_AudioBalance(double value);
        [return: MarshalAs(UnmanagedType.U1)] bool get_RealTimePlayback();
        void put_RealTimePlayback([MarshalAs(UnmanagedType.U1)] bool value);
        int get_StereoscopicVideoRenderMode();
        void put_StereoscopicVideoRenderMode(int value);
        IntPtr get_BreakManager();
        IMediaPlaybackCommandManager get_CommandManager();
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("5acee5a6-5cb6-4a5a-8521-cc86b1c1ed37"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface IMediaPlaybackCommandManager
    {
        [return: MarshalAs(UnmanagedType.U1)] bool get_IsEnabled();
        void put_IsEnabled([MarshalAs(UnmanagedType.U1)] bool value);
    }

    [ComVisible(true), ComImport, SuppressUnmanagedCodeSecurity,
     Guid("30d5a829-7fa4-4026-83bb-d75bae4ea99e"),
     InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    internal interface IClosable
    {
        void Close();
    }

    [ComVisible(true)]
    internal sealed class ButtonPressedEventHandler : IButtonPressedEventHandler
    {
        private readonly Action<SystemMediaTransportControlsButton> m_handler;

        public ButtonPressedEventHandler(Action<SystemMediaTransportControlsButton> handler)
        {
            m_handler = handler;
        }

        public void Invoke(IntPtr sender, ISystemMediaTransportControlsButtonPressedEventArgs args)
        {
            m_handler(args.get_Button());
        }
    }

    [ComVisible(true)]
    internal sealed class PlaybackPositionChangeRequestedEventHandler : IPlaybackPositionChangeRequestedEventHandler
    {
        private readonly Action<long> m_handler;

        public PlaybackPositionChangeRequestedEventHandler(Action<long> handler)
        {
            m_handler = handler;
        }

        public void Invoke(IntPtr sender, IPlaybackPositionChangeRequestedEventArgs args)
        {
            m_handler(args.get_RequestedPlaybackPosition());
        }
    }

    internal static class WindowsRuntimeActivator
    {
        [DllImport("combase.dll", PreserveSig = false)]
        private static extern void RoActivateInstance([MarshalAs(UnmanagedType.HString)] string activatableClassId,
                                                      [MarshalAs(UnmanagedType.Interface)] out object instance);

        public static object ActivateInstance(string activatableClassId)
        {
            RoActivateInstance(activatableClassId, out object instance);
            return instance;
        }
    }
}
