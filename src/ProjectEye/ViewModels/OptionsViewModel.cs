using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEye.ViewModels
{
    public class OptionsViewModel
    {
        public OptionsModel Model { get; set; }
        public Command applyCommand { get; set; }
        public Command openurlCommand { get; set; }
        public Command inkCommand { get; set; }
        public Command soundTestCommand { get; set; }
        public Command updateCommand { get; set; }
        public Command showWindowCommand { get; set; }

        private readonly ConfigService config;
        private readonly MainService mainService;
        private readonly SystemResourcesService systemResources;
        private readonly SoundService sound;
        private readonly ThemeService theme;
        public OptionsViewModel(ConfigService config,
            MainService mainService,
            SystemResourcesService systemResources,
            SoundService sound,
            ThemeService theme)
        {
            this.config = config;
            this.mainService = mainService;
            this.systemResources = systemResources;
            this.sound = sound;
            this.theme = theme;
            Model = new OptionsModel();
            Model.Data = config.options;
            Model.Themes = systemResources.Themes;
            Model.PreAlertActions = systemResources.PreAlertActions;
            string[] version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            Model.Version = version[0] + "." + version[1] + "." + version[2];


            applyCommand = new Command(new Action<object>(applyCommand_action));
            openurlCommand = new Command(new Action<object>(openurlCommand_action));
            inkCommand = new Command(new Action<object>(inkCommand_action));
            soundTestCommand = new Command(new Action<object>(soundTestCommand_actionAsync));
            updateCommand = new Command(new Action<object>(updateCommand_action));
            showWindowCommand = new Command(new Action<object>(showWindowCommand_action));
        }

        private void showWindowCommand_action(object obj)
        {
            WindowManager.CreateWindowInScreen(obj.ToString());
            WindowManager.Show(obj.ToString());
        }

        private void updateCommand_action(object obj)
        {
            WindowManager.CreateWindowInScreen("UpdateWindow");
            WindowManager.Show("UpdateWindow");

            //            string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            //                "Update",
            //                "Download",
            //                "ProjectEye.zip");
            //            string outPath = AppDomain.CurrentDomain.BaseDirectory;
            //#if DEBUG
            //            Model.Version = "1.0.3";
            //#endif


        }

        private void soundTestCommand_actionAsync(object obj)
        {
            if (!string.IsNullOrEmpty(config.options.General.SoundPath))
            {
                bool resultTest = sound.Test(config.options.General.SoundPath);
                if (resultTest)
                {
                    MessageBox.Show("音效已被正确加载！", "提示");
                }
                else
                {
                    MessageBox.Show("音效不能被加载！", "警告");
                }
            }

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
                //处理主题切换
                theme.SetTheme(config.options.Style.Theme.ThemeName);
            }
            MessageBox.Show(msg, "提示");
        }
    }
}
