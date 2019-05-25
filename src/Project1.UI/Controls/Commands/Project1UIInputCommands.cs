using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Project1.UI.Controls.Commands
{
    public static class Project1UIInputCommands
    {
        public static RoutedUICommand ClearTextCommand { get; } = new RoutedUICommand("", "ClearTextCommand", typeof(Project1UIInputCommands));
        public static RoutedUICommand CommonOpenFileDialog { get; } = new RoutedUICommand("", "CommonOpenFileDialog", typeof(Project1UIInputCommands));



    }
}
