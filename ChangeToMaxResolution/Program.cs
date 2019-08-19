using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeToMaxResolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Press Enter to change the last display device (primary don't count)!");
            Console.ReadLine();

            var enumDisplay = new DisplayDevicesHelper();

            // print display devices
            enumDisplay.Display();

            // change the last display device (primary don't count)
            DisplayDevicesHelper.DISPLAY_DEVICE device;
            if (enumDisplay.GetLastAttached(out device) && !device.StateFlags.HasFlag(DisplayDevicesHelper.DisplayDeviceStateFlags.PrimaryDevice))
            {
                var res = new Resolution();
                var ret = res.ChangeToMaxResolution(device.DeviceName);
                Console.WriteLine(ret.ToString());
            }

            Console.WriteLine("\nPress Enter to quit");
            Console.ReadLine();
        }
    }
}
