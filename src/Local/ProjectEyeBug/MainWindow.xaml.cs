using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectEyeBug
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Project1.UI.Controls.Project1UIWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Project1UIButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Planshit/ProjectEye/releases"));
        }

        private void Project1UIButton_Click_1(object sender, RoutedEventArgs e)
        {
            string upexePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "ProjectEye.exe");
            Process.Start(upexePath);
            Close();
        }
    }
}
