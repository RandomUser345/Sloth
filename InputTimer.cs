using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Sloth
{
    public static class InputTimer
    {
        public static TimeSpan GetInputIdleTime()
        {
            var plii = new NativeMethods.LastInputInfo();
            plii.cbSize = (UInt32)Marshal.SizeOf(plii);

            if (NativeMethods.GetLastInputInfo(ref plii))
            {
                return TimeSpan.FromMilliseconds(Environment.TickCount64 - plii.dwTime);
            }
            else
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static DateTimeOffset GetLastInputTime()
        {
            return DateTimeOffset.Now.Subtract(GetInputIdleTime());
        }


    }
}