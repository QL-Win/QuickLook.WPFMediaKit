using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Test_Application
{
    /// <summary>
    /// Publishes the process identity the system media flyout resolves to a display name.
    /// The flyout shows the Start menu shortcut name; the AppUserModelID itself is only the lookup key.
    /// </summary>
    internal static class AppUserModel
    {
        internal const string Id = "WPFMediaKit.TestApplication";
        internal const string DisplayName = "WPF-MediaKit Test Application";

        private static readonly PropertyKey AppUserModelIdKey = new PropertyKey
        {
            FormatId = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"),
            PropertyId = 5
        };

        public static void Register()
        {
            string shortcutPath = RegisterShortcut(out bool shortcutExisted);
            int hr = SetCurrentProcessExplicitAppUserModelID(Id);
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);

            NotifyShell(shortcutPath, shortcutExisted ? SHCNE_UPDATEITEM : SHCNE_CREATE);
        }

        private static string RegisterShortcut(out bool shortcutExisted)
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                DisplayName + ".lnk");
            shortcutExisted = File.Exists(shortcutPath);

            Directory.CreateDirectory(Path.GetDirectoryName(shortcutPath));

            var shellLink = (IShellLinkW)new ShellLink();
            try
            {
                shellLink.SetPath(exePath);
                shellLink.SetWorkingDirectory(Path.GetDirectoryName(exePath));
                shellLink.SetDescription(DisplayName);
                shellLink.SetIconLocation(exePath, 0);

                var store = (IPropertyStore)shellLink;
                var key = AppUserModelIdKey;
                InitString(Id, out PropVariant value);
                try
                {
                    store.SetValue(ref key, ref value);
                    store.Commit();
                }
                finally
                {
                    PropVariantClear(ref value);
                }

                ((IPersistFile)shellLink).Save(shortcutPath, true);
            }
            finally
            {
#pragma warning disable CA1416
                Marshal.FinalReleaseComObject(shellLink);
#pragma warning restore CA1416
            }

            return shortcutPath;
        }

        private static void InitString(string value, out PropVariant variant)
        {
            variant = new PropVariant
            {
                VariantType = 31,
                Value = Marshal.StringToCoTaskMemUni(value)
            };
        }

        private static void NotifyShell(string shortcutPath, int eventId)
        {
            IntPtr path = Marshal.StringToCoTaskMemUni(shortcutPath);
            try
            {
                SHChangeNotify(eventId, SHCNF_PATHW | SHCNF_FLUSHNOWAIT, path, IntPtr.Zero);
            }
            finally
            {
                Marshal.FreeCoTaskMem(path);
            }
        }

        private const int SHCNE_CREATE = 0x00000002;
        private const int SHCNE_UPDATEITEM = 0x00002000;
        private const uint SHCNF_PATHW = 0x0005;
        private const uint SHCNF_FLUSHNOWAIT = 0x2000;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SetCurrentProcessExplicitAppUserModelID(string appId);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void SHChangeNotify(int eventId, uint flags, IntPtr item1, IntPtr item2);

        [DllImport("ole32.dll")]
        private static extern int PropVariantClear(ref PropVariant variant);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct PropertyKey
        {
            public Guid FormatId;
            public uint PropertyId;
        }

        [StructLayout(LayoutKind.Explicit, Size = 24)]
        private struct PropVariant
        {
            [FieldOffset(0)]
            public ushort VariantType;

            [FieldOffset(8)]
            public IntPtr Value;
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLink
        {
        }

        [ComImport]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellLinkW
        {
            void GetPath(IntPtr file, int fileCapacity, IntPtr findData, int flags);
            void GetIDList(out IntPtr idList);
            void SetIDList(IntPtr idList);
            void GetDescription(IntPtr name, int nameCapacity);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string name);
            void GetWorkingDirectory(IntPtr directory, int directoryCapacity);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string directory);
            void GetArguments(IntPtr arguments, int argumentsCapacity);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string arguments);
            void GetHotkey(out short hotkey);
            void SetHotkey(short hotkey);
            void GetShowCmd(out int showCommand);
            void SetShowCmd(int showCommand);
            void GetIconLocation(IntPtr iconPath, int iconPathCapacity, out int iconIndex);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string iconPath, int iconIndex);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string relativePath, int reserved);
            void Resolve(IntPtr window, int flags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string file);
        }

        [ComImport]
        [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertyStore
        {
            uint GetCount();
            PropertyKey GetAt(uint index);
            void GetValue(ref PropertyKey key, out PropVariant value);
            void SetValue(ref PropertyKey key, ref PropVariant value);
            void Commit();
        }
    }
}
