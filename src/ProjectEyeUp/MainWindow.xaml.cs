using Project1.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectEyeUp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Project1UIWindow
    {
        public DataModel Data { get; set; }

        private string savePath;
        private Updater updater;
        private HttpDownloader downloader;

        public MainWindow(string version,
            string githubUrl,
            string savePath)
        {
            InitializeComponent();
            Data = new DataModel();
            DataContext = Data;
            this.savePath = savePath;
            updater = new Updater("https://api.github.com/repos/planshit/projecteye/releases/latest", "1.0.3");
            updater.RequestCompleteEvent += Updater_RequestCompleteEvent;
            updater.RequestErrorEvent += Updater_RequestErrorEvent;
            Loaded += MainWindow_Loaded;


        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            updater.Request();
            Data.PlayProcess = true;
        }

        private void Updater_RequestErrorEvent(object sender, object value)
        {
            Data.PlayProcess = false;
            MessageBox.Show("无法获取到版本信息，请尝试重启应用程序后再试！", "错误");
        }

        private void Updater_RequestCompleteEvent(object sender, object value)
        {
            bool isCanUpdate = updater.IsCanUpdate();
            Data.PlayProcess = false;
            Data.Tip = isCanUpdate ? "有新版本可以更新！" : "当前已是最新版本！";
            Data.UpVisibility = isCanUpdate ? Visibility.Visible : Visibility.Hidden;

            if (isCanUpdate)
            {
                downloader = new HttpDownloader(updater.Info.DownloadUrl, savePath);
                string preText = updater.Info.IsPre ? "[预览版本]" : "[正式版]";
                Data.VersionInfo = "版本：" + updater.Info.Version + preText + "\r\n\r\n" + updater.Info.Title;
            }
        }
    }
}
