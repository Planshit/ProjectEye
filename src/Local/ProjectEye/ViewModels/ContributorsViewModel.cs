using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace ProjectEye.ViewModels
{
    public class ContributorsViewModel
    {

        public Command openurlCommand { get; set; }

        public ContributorsViewModel()
        {

            openurlCommand = new Command(new Action<object>(openurlCommand_action));
        }

     
        private void openurlCommand_action(object obj)
        {
            Process.Start(new ProcessStartInfo(obj.ToString()));
        }

    }
}
