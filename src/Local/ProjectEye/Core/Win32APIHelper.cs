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

        #region 窗口类
        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        /// <summary>
        /// 获取窗口类名
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        /// <summary>
        /// 获取当前焦点窗口句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        /// <summary>
        /// 窗口是否最大化
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);
        /// <summary>
        /// 获取窗口位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;                             //最左坐标
            public int Top;                             //最上坐标
            public int Right;                           //最右坐标
            public int Bottom;                        //最下坐标
        }
        /// <summary>
        /// 窗口信息结构
        /// </summary>
        public struct WindowInfo
        {
            /// <summary>
            /// 窗口宽度
            /// </summary>
            public int Width;
            /// <summary>
            /// 窗口高度
            /// </summary>
            public int Height;
            /// <summary>
            /// 窗口标题
            /// </summary>
            public string Title;
            /// <summary>
            /// 窗口类名
            /// </summary>
            public string ClassName;
            /// <summary>
            /// 是否全屏
            /// </summary>
            public bool IsFullScreen;
            /// <summary>
            /// 是否最大化
            /// </summary>
            public bool IsZoomed;
        }
        /// <summary>
        /// 获取当前焦点窗口信息
        /// </summary>
        /// <returns></returns>
        public static WindowInfo GetFocusWindowInfo()
        {
            var result = new WindowInfo();
            //获取当前焦点窗口句柄
            IntPtr intPtr = GetForegroundWindow();
            //获取窗口大小
            RECT rect = new RECT();
            GetWindowRect(intPtr, ref rect);
            result.IsZoomed = IsZoomed(intPtr);
            result.Width = rect.Right - rect.Left;
            result.Height = rect.Bottom - rect.Top;
            //获取窗口标题
            StringBuilder title = new StringBuilder(256);
            GetWindowText(intPtr, title, title.Capacity);
            result.Title = title.ToString();
            //获取窗口类名
            StringBuilder className = new StringBuilder(256);
            GetClassName(intPtr, className, className.Capacity);
            result.ClassName = className.ToString();
            //判断全屏
            result.IsFullScreen = false;
            if (!result.IsZoomed)
            {
                //非最大化状态下计算判断全屏
                result.IsFullScreen = result.Width >= SystemParameters.PrimaryScreenWidth && result.Height >= SystemParameters.PrimaryScreenHeight;
            }
            //浏览器全屏判断
            if (result.ClassName.ToLower().IndexOf("chrome_widgetwin") != -1)
            {
                //chrome浏览器比较特殊，全屏模式不能被识别，需要另外计算
                result.IsFullScreen = result.Width >= SystemParameters.PrimaryScreenWidth && result.Height >= SystemParameters.PrimaryScreenHeight;
                result.IsZoomed = !result.IsFullScreen;
            }
            return result;
        }
        #endregion
    }
}
