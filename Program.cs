using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput;
using static Sloth.NativeMethods;

namespace Sloth
{
    class Program
    {
        static void Main(string[] args)
        {

            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;

            NativeMethods.ShowWindow(handle, 6);

            uint dwSessionID = NativeMethods.WTSGetActiveConsoleSessionId();
            uint dwBytesReturned = 0;
            int dwFlags = 0;
            IntPtr pInfo = IntPtr.Zero;
            int clicks = 0;
            DateTime start = DateTime.Now;
            var inputSimulator = new InputSimulator();
            while (true)
            {
                Console.Clear();
                var runningFor = DateTime.Now-start;
                Console.WriteLine($"{DateTime.Now.ToString("T")}");
                Console.WriteLine($"{(int)runningFor.TotalMinutes} min. total running");
                Console.WriteLine($"{clicks * 3} min. idle ({(int)(100/runningFor.TotalMinutes*clicks*3)}%)");

                TimeSpan idle = InputTimer.GetInputIdleTime();


                NativeMethods.WTSQuerySessionInformationW(IntPtr.Zero, dwSessionID, WTS_INFO_CLASS.WTSSessionInfoEx, ref pInfo, ref dwBytesReturned);
                var shit = Marshal.PtrToStructure<WTSINFOEXW>(pInfo);

                if (shit.Level == 1)
                {
                    dwFlags = shit.Data.WTSInfoExLevel1.SessionFlags;
                }

                if (dwFlags == 1)
                {
                    if (idle.TotalMinutes >= 3)
                    {

                        inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.SHIFT);
                        idle = InputTimer.GetInputIdleTime();
                        clicks++;
                        Console.WriteLine($"Status: simulated KeyEvent");
                        Thread.Sleep(100);

                    }
                    else
                    {
                        Console.WriteLine($"Status: current idletime {(int)idle.TotalSeconds} sec.");
                    }
                }
                else
                {
                    // 0 = unknown, 2 = locked
                    Console.WriteLine($"Status: System Locked");
                }
                
                Console.WriteLine("=================================");
                Thread.Sleep(60 * 1000);
            }
        }

    }

}
