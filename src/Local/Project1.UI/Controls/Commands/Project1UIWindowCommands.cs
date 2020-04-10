using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Project1.UI.Controls.Commands
{
    public static class Project1UIWindowCommands
    {
        public static RoutedUICommand MinimizeWindowCommand { get; } = new RoutedUICommand("", "MinimizeWindowCommand", typeof(Project1UIWindowCommands));
        public static RoutedUICommand RestoreWindowCommand { get; } = new RoutedUICommand("", "RestoreWindowCommand", typeof(Project1UIWindowCommands));
        public static RoutedUICommand MaximizeWindowCommand { get; } = new RoutedUICommand("", "MaximizeWindowCommand", typeof(Project1UIWindowCommands));
        public static RoutedUICommand CloseWindowCommand { get; } = new RoutedUICommand("", "CloseWindowCommand", typeof(Project1UIWindowCommands));

        public static RoutedUICommand LogoButtonClickCommand { get; } = new RoutedUICommand("", "LogoButtonClickCommand", typeof(Project1UIWindowCommands));


    }
}
