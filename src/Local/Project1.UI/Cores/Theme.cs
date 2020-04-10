using Project1.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Project1.UI.Cores
{
    public class Theme
    {

        //主题颜色键名
        private readonly string ThemeColorKey = "ThemeColor";
        //主题窗口样式键名
        private readonly string ThemeWindowStyleKey = "WindowStyle";
        private readonly Collection<ResourceDictionary> MergedDictionaries;


        public Theme()
        {
            MergedDictionaries = Application.Current.Resources.MergedDictionaries;
        }

        #region private
        private void UpdateThemeColor()
        {

            var themeColorDict = MergedDictionaries.Where(m => m.Contains(ThemeColorKey)).FirstOrDefault();
            if (themeColorDict != null)
            {
                MergedDictionaries.Remove(themeColorDict);
                MergedDictionaries.Add(GetResourceDictionary($"{UIDefaultSetting.DefaultThemeFullPath}/Config.xaml"));
            }
        }
        private void UpdateTheme()
        {
            var windowStyle = Application.Current.Resources[ThemeWindowStyleKey];
            if (windowStyle != null)
            {
                foreach (Project1UIWindow w in WindowsCollection.ToList())
                {

                    Style _windowStyle = windowStyle as Style;
                    if (w.Style != _windowStyle)
                    {
                        w.Style = _windowStyle;
                    }
                }
            }
            SetThemeColor(UIDefaultSetting.DefaultThemeColor);
        }
        private ResourceDictionary GetResourceDictionary(string uri)
        {
            try
            {
                return new ResourceDictionary { Source = new Uri(uri, UriKind.RelativeOrAbsolute) };
            }
            catch
            {
                Debug.WriteLine("获取资源字典时失败");
                return null;
            }
        }
        #endregion

        #region public
        /// <summary>
        /// 应用uidefaultsetting的主题配置
        /// </summary>
        public void ApplyTheme()
        {
            SetTheme(UIDefaultSetting.DefaultThemeName);
        }
        public void SetThemeColor(string color)
        {
            try
            {
                UIDefaultSetting.DefaultThemeColor = color;
                Application.Current.Resources[ThemeColorKey] = (Color)ColorConverter.ConvertFromString(color);
                UpdateThemeColor();
            }
            catch (Exception e)
            {
                Debug.WriteLine("设置主题颜色时发生错误。" + e.Message);
            }
        }

        public void SetTheme(string name, string path = null)
        {
            try
            {
                UIDefaultSetting.DefaultThemeName = name;

                

                var configDict = GetResourceDictionary($"{UIDefaultSetting.DefaultThemeFullPath}/Config.xaml");
                var controlDict = GetResourceDictionary($"{UIDefaultSetting.DefaultThemeFullPath}/Style.xaml");
                var themename = Application.Current.Resources["ThemeName"];

                Debug.WriteLine("themeName:" + themename + "/" + name);
                if (themename != null && themename.ToString() != name || themename == null)
                {
                    MergedDictionaries.Clear();
                    MergedDictionaries.Add(configDict);
                    MergedDictionaries.Add(controlDict);
                    Debug.WriteLine("已加载主题：" + name);
                }
                UIDefaultSetting.DefaultThemeColor = ((Color)configDict[ThemeColorKey]).ToString();
                //Properties.Settings.Default.Save();
                Debug.WriteLine("themeColor:" + UIDefaultSetting.DefaultThemeColor);
                UpdateTheme();
            }
            catch (Exception e)
            {
                Debug.WriteLine("加载主题时发生错误。" + e.Message);
            }
        }

        #endregion
    }
}
