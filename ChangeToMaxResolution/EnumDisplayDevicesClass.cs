using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChangeToMaxResolution
{
    public class DisplayDevicesHelper
    {
        [DllImport("user32.dll")]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        public delegate bool DisplaypPredicate(uint index, DISPLAY_DEVICE device);

        public void ForEachDisplay(DisplaypPredicate onEach)
        {
            DISPLAY_DEVICE d = new DISPLAY_DEVICE();
            d.cb = Marshal.SizeOf(d);
            try
            {
                for (uint id = 0; EnumDisplayDevices(null, id, ref d, 0); id++)
                {
                    if (onEach(id, d)) break;
                    d.cb = Marshal.SizeOf(d);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}", ex.ToString()));
            }
        }

        public void Display()
        {
            ForEachDisplay(
                delegate (uint id, DISPLAY_DEVICE d)
                {
                    Console.WriteLine(
                            String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}",
                                     id,
                                     d.DeviceName,
                                     d.DeviceString,
                                     d.StateFlags,
                                     d.DeviceID,
                                     d.DeviceKey,
                                     d.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop) ? "isAttached" : "notAttached"
                                     )
                                     );
                    d.cb = Marshal.SizeOf(d);
                    EnumDisplayDevices(d.DeviceName, 0, ref d, 0);
                    Console.WriteLine(
                        String.Format("{0}, {1}",
                                 d.DeviceName,
                                 d.DeviceString
                                 )
                                 );
                    Console.WriteLine("------------");
                    return false;
                }
                );
        }

        public bool GetLastAttached(out DISPLAY_DEVICE lastOne)
        {
            bool found = false;
            DISPLAY_DEVICE _last = new DISPLAY_DEVICE();
            ForEachDisplay(
                delegate (uint id, DISPLAY_DEVICE d)
                {
                    if (d.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                    {
                        found = true;
                        _last = d;
                    }
                    return false;
                });
            lastOne = _last;
            return found;
        }
    }
}