using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace ProjectEye.Core
{
    public class Win32APIHelper
    {
        /// <summary>
        /// 获取鼠标坐标
        /// </summary>
        /// <param name="lpPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out Point lpPoint);
    }
}
