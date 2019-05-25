using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace ProjectEye.ViewModels
{
    public class OptionsViewModel
    {
        public OptionsModel Model { get; set; }
        public Command applyCommand { get; set; }
        public Command openurlCommand { get; set; }
        public Command inkCommand { get; set; }

        private readonly ConfigService config;
        private readonly MainService mainService;
        private readonly SystemResourcesService systemResources;

        public OptionsViewModel(ConfigService config, 
            MainService mainService,
            SystemResourcesService systemResources)
        {
            this.config = config;
            this.mainService = mainService;
            this.systemResources = systemResources;
            Model = new OptionsModel();
            Model.Data = config.options;
            Model.Themes = systemResources.Themes;
            string[] version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            Model.Version = version[0] + "." + version[1] + "." + version[2];


            applyCommand = new Command(new Action<object>(applyCommand_action));
            openurlCommand = new Command(new Action<object>(openurlCommand_action));
            inkCommand = new Command(new Action<object>(inkCommand_action));
        }

        private void inkCommand_action(object obj)
        {
            string msg = "创建桌面快捷方式失败！";
            if (ShortcutHelper.CreateDesktopShortcut())
            {
                msg = "创建桌面快捷方式成功！";
            }
            MessageBox.Show(msg, "提示");

        }

        private void openurlCommand_action(object obj)
        {
            Process.Start(new ProcessStartInfo(obj.ToString()));
        }

        private void applyCommand_action(object obj)
        {
            string msg = "更新失败！请尝试重启程序或删除配置文件Config.xml！";
            if (config.Save())
            {
                msg = "选项已成功更新";
                //处理开机启动
                if (!ShortcutHelper.SetStartup(config.options.General.Startup))
                {
                    msg = "选项已成功更新。但是开机启动选项可能未生效。";
                }
                //处理离开监听开关
                if (config.options.General.LeaveListener)
                {
                    mainService.OpenLeaveListener();
                }
                else
                {
                    mainService.CloseLeaveListener();
                }
                //处理休息间隔调整
                mainService.SetWarnTime(config.options.General.WarnTime);
            }
            MessageBox.Show(msg, "提示");
        }
    }
}
