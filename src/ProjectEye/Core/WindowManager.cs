using ProjectEye.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static ServiceCollection serviceCollection { get; set; }
        static WindowManager()
        {
            windowList = new List<Window>();
        }

        /// <summary>
        /// 创建一个窗口
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <returns>成功返回窗口实例，失败返回NULL</returns>
        public static Window CreateWindow(string name)
        {
            Type type = Type.GetType(nameSpace + "." + name);
            Window objWindow = (Window)type.Assembly.CreateInstance(type.FullName);
            objWindow.Uid = name;
            objWindow.DataContext = InvokeViewModel(name);
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
        /// <summary>
        /// 获取窗口实例，如果没有创建则会创建
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Window GetCreateWindow(string name)
        {
            var window = Get(name);
            if (window == null)
            {
                window = CreateWindow(name);
            }
            return window;
        }
        private static void objWindow_Close(object sender, EventArgs e)
        {
            windowList.Remove((Window)sender);
        }

        private static object InvokeViewModel(string name)
        {
            string nameSpace = "ProjectEye.ViewModels";
            string viewModelName = name.Replace("Window", "ViewModel");
            Type type = Type.GetType(nameSpace + "." + viewModelName);
            var constructorInfoObj = type.GetConstructors()[0];
            var constructorParameters = constructorInfoObj.GetParameters();
            int constructorParametersLength = constructorParameters.Length;
            Type[] types = new Type[constructorParametersLength];
            object[] objs = new object[constructorParametersLength];
            for (int i = 0; i < constructorParametersLength; i++)
            {
                string typeFullName = constructorParameters[i].ParameterType.FullName;
                Type t = Type.GetType(typeFullName);
                types[i] = t;

                objs[i] = serviceCollection.GetInstance(typeFullName);

            }
            ConstructorInfo ctor = type.GetConstructor(types);
            object instance = ctor.Invoke(objs);
            return instance;
        }
    }
}
