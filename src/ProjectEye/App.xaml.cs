using ProjectEye.Core;
using ProjectEye.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ProjectEye
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private readonly Main main;
        private readonly Tray tray;
        public App()
        {

            //创建提示窗口
            var tipWindow = WindowManager.CreateWindow("TipWindow");
            tipWindow.DataContext = new TipViewModel();
           
            main = new Main();
            tray = new Tray();
            
            Startup += new StartupEventHandler(startup);
            Exit += new ExitEventHandler(exit);
        }

        private void exit(object sender, ExitEventArgs e)
        {
            tray.Remove();

            main.Stop();
            
        }

        private void startup(object sender, StartupEventArgs e)
        {
            main.Run();
        }
    }
}
