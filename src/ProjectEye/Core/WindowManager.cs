using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ProjectEye.Core
{
    /// <summary>
    /// 窗口管理
    /// </summary>
    public class WindowManager
    {
        private static IList<Window> windowList;
        private static string nameSpace = "ProjectEye.Views";
        /// <summary>
        /// 创建一个窗口
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <returns>成功返回窗口实例，失败返回NULL</returns>
        public static Window CreateWindow(string name)
        {
            if (windowList == null)
            {
                windowList = new List<Window>();
            }
            Type type = Type.GetType(nameSpace + "." + name);
            Window objWindow = (Window)type.Assembly.CreateInstance(type.FullName);
            objWindow.Uid = name;
            objWindow.Closed += new EventHandler(objWindow_Close);
            windowList.Add(objWindow);
            return objWindow;
        }
        /// <summary>
        /// 通过窗口类名获取一个已经创建的窗口实例
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <returns>成功返回窗口实例，失败返回NULL</returns>
        public static Window Get(string name)
        {
            var window = windowList.Where(m => m.Uid == name);
            if (window.Count() > 0)
            {
                return window.Single();
            }
            return null;
        }


        private static void objWindow_Close(object sender, EventArgs e)
        {
            windowList.Remove((Window)sender);
        }


    }
}
