using ProjectEye.Core.Models;
using ProjectEye.Core.Service;
using ProjectEye.ViewModels;
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
        private static IList<WindowModel> windowList;
        private static IList<object> viewModelList;
        public static ServiceCollection serviceCollection { get; set; }
        static WindowManager()
        {
            windowList = new List<WindowModel>();
            viewModelList = new List<object>();

        }

        //window
        #region 创建窗口
        private static Window CreateWindow(string name, string screen, double left = -999999, double top = -999999, double width = -999999, double height = -999999)
        {
            //var selectWindow = GetWindowByScreen(name, screen);
            //if (selectWindow != null)
            //{
            //    return selectWindow;
            //}
            var viewModel = GetCreateViewModel(name);

            Type type = Type.GetType("ProjectEye.Views." + name);
            Window objWindow = (Window)type.Assembly.CreateInstance(type.FullName);
            objWindow.Uid = name;
            objWindow.DataContext = viewModel;
            objWindow.Closed += new EventHandler(window_closed);
            if (left > -999999)
            {
                objWindow.Left = left;
            }
            if (top > -999999)
            {
                objWindow.Top = top;
            }
            if (width > -999999)
            {
                objWindow.Width = width;
            }
            if (height > -999999)
            {
                objWindow.Height = height;
            }

            if (viewModel != null)
            {
                var basicModel = viewModel as IViewModel;
                if (basicModel != null)
                {
                    basicModel.ScreenName = screen.Replace("\\", "");
                    basicModel.WindowInstance = objWindow;
                    basicModel.OnChanged();
                }
            }

            var windowModel = new WindowModel();
            windowModel.window = objWindow;
            windowModel.screen = screen;

            windowList.Add(windowModel);
            return objWindow;
        }



        /// <summary>
        /// 在指定显示器上创建一个window（默认在主显示器）
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <param name="screen">显示器</param>
        /// <returns></returns>
        public static Window CreateWindowInScreen(string name, System.Windows.Forms.Screen screen = null, bool isMaximized = false)
        {

            //var windowModel = GetWindowModel(name, screen.DeviceName);
            //if (windowModel != null)
            //{
            //    //先销毁再创建
            //    windowModel.window.Close();
            //    windowList.Remove(windowModel);
            //}
            //创建

            double left = -999999, top = -999999, width = -999999, height = -999999;
            if (screen == null)
            {
                screen = System.Windows.Forms.Screen.PrimaryScreen;
            }
            if (isMaximized)
            {
                left = screen.Bounds.Left;
                top = screen.Bounds.Top;
                width = screen.Bounds.Width;
                height = screen.Bounds.Height;
            }
            var window = CreateWindow(name,
                screen.DeviceName,
                left,
                top,
                width,
                height);
            return window;
        }
        /// <summary>
        /// 在所有显示器中创建一个窗口
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <param name="isMaximized">是否全屏</param>
        /// <returns></returns>
        public static Window[] CreateWindow(string name, bool isMaximized)
        {
            int screenCount = System.Windows.Forms.Screen.AllScreens.Length;
            var screens = System.Windows.Forms.Screen.AllScreens;
            Window[] windows = new Window[screenCount];

            for (int index = 0; index < screenCount; index++)
            {
                var screen = screens[index];
                double width = -999999;
                double height = -999999;
                if (isMaximized)
                {
                    width = screen.Bounds.Width;
                    height = screen.Bounds.Height;
                }
                var window = CreateWindow(name, screen.DeviceName, screen.Bounds.Left, screen.Bounds.Top, width, height);
                windows[index] = window;

            }
            return windows;

        }
        #endregion

        #region 获取窗口
        /// <summary>
        /// 通过窗口类名获取已经创建的窗口实例
        /// </summary>
        /// <param name="name">窗口类名</param>
        /// <returns>成功返回窗口实例数组，失败返回NULL</returns>
        public static Window[] GetWindows(string name)
        {
            var window = windowList.Where(m => m.window.Uid == name).Select(s => s.window);
            if (window.Count() > 0)
            {
                return window.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 获取窗口实例，如果没有找到则会创建
        /// </summary>
        /// <param name="name"></param>
        /// <returns>成功返回窗口实例数组</returns>
        public static Window[] GetCreateWindow(string name, bool isMaximized)
        {
            var window = GetWindows(name);
            if (window == null)
            {
                window = CreateWindow(name, isMaximized);
            }
            return window;
        }
        /// <summary>
        /// 获取window通过窗口类名+显示器（驱动名称）查找
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="screen"></param>
        /// <returns>成功只会返回window实例</returns>
        public static Window GetWindowByScreen(string windowName, string screen)
        {
            var select = windowList.Where(m => m.window.Uid == windowName
              && m.screen == screen).Select(s => s.window);
            if (select.Count() == 1)
            {
                return select.Single();
            }
            return null;
        }
        /// <summary>
        /// 获取windowmodel
        /// </summary>
        /// <param name="windowName">窗口类名</param>
        /// <param name="screen">显示器</param>
        /// <returns></returns>
        public static WindowModel GetWindowModel(string windowName, string screen)
        {
            var select = windowList.Where(m => m.window.Uid == windowName
              && m.screen == screen);
            if (select.Count() > 0)
            {
                return select.Single();
            }
            return null;
        }
        #endregion

        #region 显示窗口
        public static void Show(string name)
        {
            var screens = System.Windows.Forms.Screen.AllScreens;
            foreach (var screen in screens)
            {
                var window = GetWindowByScreen(name, screen.DeviceName);
                if (window != null)
                {
                    window.Show();
                }
            }
        }
        #endregion

        #region 关闭窗口
        /// <summary>
        /// 关闭窗口（所有显示器）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int Close(string name)
        {
            var windows = GetWindows(name);
            if (windows == null)
            {
                return 0;
            }

            foreach (var window in windows)
            {
                window.Close();

            }
            RemoveViewModel(name);
            RemoveWindow(name);
            return windows.Length;
        }
        #endregion

        #region 隐藏窗口
        public static int Hide(string name)
        {
            var windows = GetWindows(name);
            if (windows == null)
            {
                return 0;
            }

            foreach (var window in windows)
            {
                window.Hide();
            }
            return windows.Length;
        }
        #endregion

        #region 移除窗口实例
        private static void RemoveWindow(string name)
        {
            var select = windowList.Where(m => m.window.Uid == name).ToList();
            foreach (var windowModel in select)
            {
                windowList.Remove(windowModel);
            }
        }
        #endregion

        #region 在所有显示器中刷新一个窗口
        /// <summary>
        /// 在所有显示器中刷新一个窗口，如果在某个显示器中没有实例则会创建。跳过主显示器。
        /// </summary>
        /// <param name="name"></param>
        public static void UpdateAllScreensWindow(string name, bool isMaximized)
        {
            var screens = System.Windows.Forms.Screen.AllScreens;
            var mainScreen = System.Windows.Forms.Screen.PrimaryScreen;
            foreach (var screen in screens)
            {
                //跳过主显示器
                if (mainScreen != screen)
                {
                    var window = GetWindowByScreen(name, screen.DeviceName);
                    if (window != null)
                    {
                        window.Left = screen.Bounds.Left;
                        window.Top = screen.Bounds.Top;
                        window.Width = screen.Bounds.Width;
                        window.Height = screen.Bounds.Height;
                    }
                    else
                    {
                        CreateWindowInScreen(name, screen, isMaximized);
                    }
                }
            }
        }
        #endregion

        //window event
        #region 窗口被关闭event
        private static void window_closed(object sender, EventArgs e)
        {
            var window = sender as Window;
            Close(window.Uid);
        }
        #endregion

        //viewmodel
        #region 创建viewmodel实例
        private static object CreateViewModel(string windowName)
        {
            string nameSpace = "ProjectEye.ViewModels";
            string viewModelName = windowName.Replace("Window", "ViewModel");
            Type type = Type.GetType(nameSpace + "." + viewModelName);
            if (type == null)
            {
                //找不到对应的ViewModel
                return null;
            }
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
            viewModelList.Add(instance);
            return instance;
        }
        #endregion

        #region 获取viewmodel实例
        private static object GetViewModel(string windowName)
        {
            string viewModelName = windowName.Replace("Window", "ViewModel");
            var select = viewModelList.Where(m => m.GetType().Name == viewModelName);
            if (select.Count() > 0)
            {
                return select.Single();
            }
            return null;
        }
        #endregion

        #region 获取viewmodel实例，不存在时创建
        private static object GetCreateViewModel(string windowName)
        {
            var viewModel = GetViewModel(windowName);
            if (viewModel == null)
            {
                viewModel = CreateViewModel(windowName);
            }
            return viewModel;
        }
        #endregion

        #region 移除viewmodel实例
        private static void RemoveViewModel(string windowName)
        {
            var viewModel = GetViewModel(windowName);
            if (viewModel != null)
            {
                viewModelList.Remove(viewModel);
            }
        }
        #endregion


    }
}
