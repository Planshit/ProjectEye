using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Diagnostics;
using System.Reflection;

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
        public Command addBreackProcessCommand { get; set; }
        public Command removeBreackProcessCommand { get; set; }
        public Command openWindowCommand { get; set; }

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
            Model.Animations = systemResources.Animations;
            Model.PreAlertActions = systemResources.PreAlertActions;
            Model.Languages = systemResources.Languages;

            string[] version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            Model.Version = version[0] + "." + version[1] + "." + version[2];


            applyCommand = new Command(new Action<object>(applyCommand_action));
            openurlCommand = new Command(new Action<object>(openurlCommand_action));
            inkCommand = new Command(new Action<object>(inkCommand_action));
            soundTestCommand = new Command(new Action<object>(soundTestCommand_actionAsync));
            updateCommand = new Command(new Action<object>(updateCommand_action));
            showWindowCommand = new Command(new Action<object>(showWindowCommand_action));
            addBreackProcessCommand = new Command(new Action<object>(addBreackProcessCommand_action));
            removeBreackProcessCommand = new Command(new Action<object>(removeBreackProcessCommand_action));
            openWindowCommand = new Command(new Action<object>(openWindowCommand_action));
        }

        /// <summary>
        /// 打开窗口命令
        /// </summary>
        /// <param name="obj"></param>
        private void openWindowCommand_action(object obj)
        {
            string window = obj.ToString();
            if (window == "TipViewDesignWindow")
            {
                WindowManager.CreateWindow(window, true, true);
            }
            else
            {
                WindowManager.CreateWindowInScreen(window);
            }
            WindowManager.Show(window);
        }

        /// <summary>
        /// 移除进程命令
        /// </summary>
        /// <param name="obj"></param>
        private void removeBreackProcessCommand_action(object obj)
        {
            Model.Data.Behavior.BreakProgressList.Remove(Model.SelectedItem);
        }

        /// <summary>
        /// 添加跳过进程命令
        /// </summary>
        /// <param name="obj"></param>
        private void addBreackProcessCommand_action(object obj)
        {
            string process = obj.ToString();
            if (process == string.Empty)
            {
                Modal("请输入进程名称");
            }
            else if (Model.Data.Behavior.BreakProgressList.Contains(process))
            {
                Modal("进程已存在，请勿重复添加");
            }
            else
            {
                Model.Data.Behavior.BreakProgressList.Add(process);
            }
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
            string path = "";
            switch (obj.ToString())
            {
                case "2":
                    path = config.options.Tomato.WorkStartSoundPath;
                    break;
                case "3":
                    path = config.options.Tomato.WorkEndSoundPath;
                    break;
                default:
                    path = config.options.General.SoundPath;
                    break;
            }
            if (!string.IsNullOrEmpty(path))
            {
                bool resultTest = sound.Test(path);
                Modal(resultTest ? "自定义音效已被正确加载" : "自定义音效不能被加载");
            }
            else
            {
                sound.Play();
                Modal("当前播放的是默认提示音");
            }
        }
        private void inkCommand_action(object obj)
        {

            string msg = "创建桌面快捷方式失败！";
            if (ShortcutHelper.CreateDesktopShortcut())
            {
                msg = "创建桌面快捷方式成功！";
            }
            //MessageBox.Show(msg, "提示");
            Modal(msg);
        }

        private void openurlCommand_action(object obj)
        {
            Process.Start(new ProcessStartInfo(obj.ToString()));
        }

        private void applyCommand_action(object obj)
        {
            string msg = "更新失败！请尝试重启程序或删除配置文件Config.xml！";
            theme.HandleDarkMode();
            if (config.Save())
            {
                msg = "选项已更新";
                //处理开机启动
                if (!ShortcutHelper.SetStartup(config.options.General.Startup))
                {
                    msg = "选项已更新，但是 “开机启动选项” 可能未生效。";
                }
                //处理休息间隔调整
                if (mainService.SetWarnTime(config.options.General.WarnTime))
                {
                    msg = "选项已更新，提醒计时已重启。";
                }
                //处理主题切换
                theme.SetTheme(config.options.Style.Theme.ThemeName);
            }
            Modal(msg);
        }

        private void Modal(string text)
        {
            Model.ModalText = text;
            Model.ShowModal = true;
        }
    }
}
