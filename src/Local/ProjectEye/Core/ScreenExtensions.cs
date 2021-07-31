using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace ProjectEye.Core
{
    public static class ScreenExtensions
    {
        public struct Dpi
        {
            public uint x { get; set; }
            public uint y { get; set; }

        }
        public static Dpi GetDpi(this System.Windows.Forms.Screen screen, DpiType dpiType)
        {
            Dpi dpi = new Dpi();
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);

            Win32APIHelper.RtlGetVersion(out Win32APIHelper.OsVersionInfo osVersionInfo);
            if (osVersionInfo.MajorVersion != 10)
            {
                Matrix m =
PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
                dpi.x = (uint)m.M11 * 96; // notice it's divided by 96 already
                dpi.y = (uint)m.M22 * 96; // notice it's divided by 96 already

            }
            else
            {
                uint dpiX, dpiY;
                GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
                dpi.x = dpiX;
                dpi.y = dpiY;
            }
            return dpi;

        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }
}
