using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ProjectEyeUp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //MainWindow window = new MainWindow(e.Args[0], e.Args[1], e.Args[2]);
            MainWindow window = new MainWindow("", "", "");
            window.Show();
        }
    }
}
