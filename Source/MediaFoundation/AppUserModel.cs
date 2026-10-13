using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace WPFMediaKit.MediaFoundation
{
    /// <summary>
    /// Registers the process identity the Windows shell uses for taskbar grouping
    /// and the system media flyout. Call <see cref="Register"/> before any window is created.
    /// </summary>
    public static class AppUserModel
    {
        private static readonly PropertyKey AppUserModelIdKey = new PropertyKey
        {
            FormatId = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"),
            PropertyId = 5
        };

        /// <summary>
        /// Assigns <paramref name="appUserModelId"/> to this process and publishes
        /// <paramref name="displayName"/> through a per-user Start menu shortcut.
        /// The media flyout shows the shortcut name.
        /// </summary>
        /// <param name="appUserModelId">
        /// Process identity, such as <c>WPFMediaKit.TestApplication</c>.
        /// Shorter than 128 characters and without spaces.
        /// </param>
        /// <param name="displayName">
        /// Name shown for this app, such as <c>WPF-MediaKit Test Application</c>.
        /// </param>
        public static void Register(string appUserModelId, string displayName)
        {
            if (string.IsNullOrWhiteSpace(appUserModelId) || appUserModelId.Length >= 128 || appUserModelId.IndexOf(' ') >= 0)
                throw new ArgumentException("AppUserModelID must be shorter than 128 characters and cannot contain spaces.", nameof(appUserModelId));
            if (string.IsNullOrWhiteSpace(displayName) || displayName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("Display name must be a valid file name.", nameof(displayName));

            string shortcutPath = RegisterShortcut(appUserModelId, displayName, out bool shortcutExisted);
            int hr = SetCurrentProcessExplicitAppUserModelID(appUserModelId);
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);

            NotifyShell(shortcutPath, shortcutExisted ? SHCNE_UPDATEITEM : SHCNE_CREATE);
        }

        /// <summary>
        /// Deletes every shortcut under this user's Start menu Programs folder whose
        /// <c>System.AppUserModel.ID</c> property equals <paramref name="appUserModelId"/>.
        /// <see cref="Register"/> writes that property, so this removes every shortcut it created,
        /// including ones left under an older display name.
        /// </summary>
        public static void RemoveShortcuts(string appUserModelId)
        {
            if (string.IsNullOrWhiteSpace(appUserModelId) || appUserModelId.Length >= 128 || appUserModelId.IndexOf(' ') >= 0)
                throw new ArgumentException("AppUserModelID must be shorter than 128 characters and cannot contain spaces.", nameof(appUserModelId));

            string programs = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            if (!Directory.Exists(programs))
                return;

            foreach (string shortcutPath in Directory.EnumerateFiles(programs, "*.lnk", SearchOption.AllDirectories))
            {
                if (!ShortcutHasAppUserModelId(shortcutPath, appUserModelId))
                    continue;

                File.Delete(shortcutPath);
                NotifyShell(shortcutPath, SHCNE_DELETE);
            }
        }

        private static string RegisterShortcut(string appUserModelId, string displayName, out bool shortcutExisted)
        {
            ProcessModule module = Process.GetCurrentProcess().MainModule;
            string exePath = module == null ? null : module.FileName;
            if (string.IsNullOrEmpty(exePath))
                throw new InvalidOperationException("The current process has no executable path.");

            string shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                displayName + ".lnk");
            shortcutExisted = File.Exists(shortcutPath);
            Directory.CreateDirectory(Path.GetDirectoryName(shortcutPath));

            var shellLink = (IShellLinkW)new ShellLink();
            try
            {
                shellLink.SetPath(exePath);
                shellLink.SetWorkingDirectory(Path.GetDirectoryName(exePath));
                shellLink.SetDescription(displayName);
                shellLink.SetIconLocation(exePath, 0);

                var store = (IPropertyStore)shellLink;
                var key = AppUserModelIdKey;
                PropVariant value = PropVariant.FromString(appUserModelId);
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
                Marshal.FinalReleaseComObject(shellLink);
            }

            return shortcutPath;
        }

        private static bool ShortcutHasAppUserModelId(string shortcutPath, string appUserModelId)
        {
            var shellLink = (IShellLinkW)new ShellLink();
            try
            {
                ((IPersistFile)shellLink).Load(shortcutPath, StgmRead);

                var store = (IPropertyStore)shellLink;
                var key = AppUserModelIdKey;
                PropVariant value;
                store.GetValue(ref key, out value);
                try
                {
                    return string.Equals(value.ReadString(), appUserModelId, StringComparison.Ordinal);
                }
                finally
                {
                    PropVariantClear(ref value);
                }
            }
            catch (COMException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            finally
            {
                Marshal.FinalReleaseComObject(shellLink);
            }
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

        private const int SHCNE_DELETE = 0x00000004;
        private const int SHCNE_CREATE = 0x00000002;
        private const int SHCNE_UPDATEITEM = 0x00002000;
        private const int StgmRead = 0;
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
            private const ushort VtLpWstr = 31;

            [FieldOffset(0)]
            public ushort VariantType;

            [FieldOffset(8)]
            public IntPtr Value;

            public static PropVariant FromString(string value)
            {
                return new PropVariant
                {
                    VariantType = VtLpWstr,
                    Value = Marshal.StringToCoTaskMemUni(value)
                };
            }

            public string ReadString()
            {
                if (VariantType != VtLpWstr || Value == IntPtr.Zero)
                    return null;

                return Marshal.PtrToStringUni(Value);
            }
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
