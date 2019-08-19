using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChangeToMaxResolution
{
    #region Struct Pointl
    [StructLayout(LayoutKind.Sequential)]
    public struct Pointl
    {
        [MarshalAs(UnmanagedType.I4)]
        public int x;
        [MarshalAs(UnmanagedType.I4)]
        public int y;
    }
    #endregion

    #region Struct Devmode
    [StructLayout(LayoutKind.Sequential,
        CharSet = CharSet.Ansi)]
    public struct Devmode
    {
        // You can define the following constant
        // but OUTSIDE the structure because you know
        // that size and layout of the structure
        // is very important
        // CCHDEVICENAME = 32 = 0x50
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        // In addition you can define the last character array
        // as following:
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        //public Char[] dmDeviceName;

        // After the 32-bytes array
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmSpecVersion;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmDriverVersion;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmSize;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmDriverExtra;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmFields;

        public Pointl dmPosition;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayOrientation;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFixedOutput;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmColor;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmDuplex;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmYResolution;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmTTOption;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmCollate;

        // CCHDEVICENAME = 32 = 0x50
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        // Also can be defined as
        //[MarshalAs(UnmanagedType.ByValArray,
        //    SizeConst = 32, ArraySubType = UnmanagedType.U1)]
        //public Byte[] dmFormName;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmLogPixels;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmBitsPerPel;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPelsWidth;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPelsHeight;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFlags;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFrequency;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmICMMethod;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmICMIntent;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmMediaType;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDitherType;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmReserved1;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmReserved2;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPanningWidth;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPanningHeight;
    }
    #endregion

    public static class StringExtensions
    {
        public static byte[] ToLPTStr(this string str)
        {
            var lptArray = new byte[str.Length + 1];

            var index = 0;
            foreach (char c in str.ToCharArray())
                lptArray[index++] = Convert.ToByte(c);

            lptArray[index] = Convert.ToByte('\0');

            return lptArray;
        }
    }

    enum DISP_CHANGE : int
    {
        Successful = 0,
        Restart = 1,
        Failed = -1,
        BadMode = -2,
        NotUpdated = -3,
        BadFlags = -4,
        BadParam = -5,
        BadDualView = -6
    }

    [Flags()]
    public enum ChangeDisplaySettingsFlags : uint
    {
        CDS_NONE = 0,
        CDS_UPDATEREGISTRY = 0x00000001,
        CDS_TEST = 0x00000002,
        CDS_FULLSCREEN = 0x00000004,
        CDS_GLOBAL = 0x00000008,
        CDS_SET_PRIMARY = 0x00000010,
        CDS_VIDEOPARAMETERS = 0x00000020,
        CDS_ENABLE_UNSAFE_MODES = 0x00000100,
        CDS_DISABLE_UNSAFE_MODES = 0x00000200,
        CDS_RESET = 0x40000000,
        CDS_RESET_EX = 0x20000000,
        CDS_NORESET = 0x10000000
    }

    internal class Resolution
    {
        #region Fields
        private static Devmode _oldDevmode;
        #endregion

        #region Consts
        private const int EnumCurrentSettings = -1;     // Retrieves the current display mode.
        private const int DispChangeSuccessful = 0;     // Indicates that the function succeeded.
        private const int DispChangeBadmode = -2;       // The graphics mode is not supported.
        private const int DispChangeRestart = 1;        // The computer must be restarted 
                                                        // for the graphics mode to work.
        #endregion

        #region DllImport
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean EnumDisplaySettings(
            byte[] lpszDeviceName,
            [param: MarshalAs(UnmanagedType.U4)] int iModeNum,
            [In, Out] ref Devmode lpDevMode);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern int ChangeDisplaySettings(
            [In, Out] ref Devmode lpDevMode,
            [param: MarshalAs(UnmanagedType.U4)] uint dwflags);

        [DllImport("user32.dll")]
        static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, ref Devmode lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);
        #endregion

        #region ChangeToMaxResolution
        public bool ChangeToMaxResolution(string displayName)
        {
            Devmode CreateDevMove()
            {
                var _mode = new Devmode();
                _mode.dmSize = (ushort)Marshal.SizeOf(_mode);
                return _mode;
            }

            Console.WriteLine("Trying with monitor " + displayName);

            var modeLst = new List<Devmode>();
            var modeIndex = 0; // 0 = The first mode
            Devmode mode = CreateDevMove();
            while (EnumDisplaySettings(displayName.ToLPTStr(), modeIndex, ref mode))
            {
                modeLst.Add(mode);
                //Console.WriteLine(String.Format("{2} {0}x{1}", mode.dmPelsWidth, mode.dmPelsHeight, mode.dmDeviceName));
                mode = CreateDevMove();
                modeIndex++;
            }

            for (int i = modeLst.Count - 1; i >= 0; i--)
            {
                mode = modeLst[i];
                Console.WriteLine(String.Format("{2} {0}x{1}", mode.dmPelsWidth, mode.dmPelsHeight, mode.dmDeviceName));
                var ret = ChangeDisplaySettingsEx(displayName, ref mode, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);
                switch (ret)
                {
                    case DISP_CHANGE.Successful:
                        Console.WriteLine("Successful");
                        break;
                    case DISP_CHANGE.Restart:
                        Console.WriteLine("Restart");
                        break;
                    case DISP_CHANGE.Failed:
                        Console.WriteLine("Failed");
                        break;
                    case DISP_CHANGE.BadMode:
                        Console.WriteLine("BadMode");
                        break;
                    case DISP_CHANGE.NotUpdated:
                        break;
                    case DISP_CHANGE.BadFlags:
                        break;
                    case DISP_CHANGE.BadParam:
                        break;
                    case DISP_CHANGE.BadDualView:
                        break;
                    default:
                        break;
                }
                if (ret == DISP_CHANGE.Successful)
                    return true;
            }
            return false;
        }
        #endregion

        #region IsDisplayModeSupported
        /// <summary>
        /// Checks if the given display mode is supported
        /// </summary>
        /// <param name="width" />
        /// <param name="height" />
        /// <param name="supportedModes" />
        /// <returns>
        public bool IsDisplayModeSupported(int width, int height, out string supportedModes)
        {
            var mode = new Devmode();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);

            var modeIndex = 0; // 0 = The first mode
            supportedModes = string.Empty;
            var previousSupportedMode = string.Empty;

            while (EnumDisplaySettings(null,
                modeIndex,
                ref mode)) // Mode found
            {
                if (mode.dmPelsWidth == (uint)width && mode.dmPelsHeight == (uint)height)
                    return true;

                var newSupportedMode = mode.dmPelsWidth + "x" + mode.dmPelsHeight;
                if (newSupportedMode != previousSupportedMode)
                {
                    if (supportedModes == string.Empty)
                        supportedModes += newSupportedMode;
                    else
                        supportedModes += ", " + newSupportedMode;

                    previousSupportedMode = newSupportedMode;
                }

                modeIndex++; // The next mode
            }

            return false;
        }
        #endregion

        #region ChangeDisplaySettings
        /// <summary>
        /// Changes the display settings
        /// </summary>
        /// <param name="width" />
        /// <param name="height" />
        public bool ChangeDisplaySettings(int width, int height)
        {
            _oldDevmode = new Devmode();
            _oldDevmode.dmSize = (ushort)Marshal.SizeOf(_oldDevmode);

            // Retrieving current settings
            // to edit them
            EnumDisplaySettings(null,
                EnumCurrentSettings,
                ref _oldDevmode);

            // Making a copy of the current settings
            // to allow reseting to the original mode
            var newMode = _oldDevmode;

            // Changing the settings
            newMode.dmPelsWidth = (uint)width;
            newMode.dmPelsHeight = (uint)height;

            // Capturing the operation result, 1 = update registry
            var result =
                ChangeDisplaySettings(ref newMode, 1);

            switch (result)
            {
                case DispChangeSuccessful:
                    return true;

                case DispChangeBadmode:
                    MessageBox.Show("Mode not supported.");
                    return false;

                case DispChangeRestart:
                    MessageBox.Show("Restart required.");
                    return false;

                default:
                    MessageBox.Show("Failed. Error code = " + result);
                    return false;
            }
        }
        #endregion

        #region RestoreDisplaySettings
        /// <summary>
        /// Restores the old display settings
        /// </summary>
        public void RestoreDisplaySettings()
        {
            ChangeDisplaySettings(ref _oldDevmode, 1);
        }
        #endregion
    }
}