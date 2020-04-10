
using ProjectEye.Core;
using ProjectEye.Core.Net;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEye.ViewModels
{
    public class UpdateViewModel : UpdateModel
    {
        /// <summary>
        /// 更新包下载存放路径
        /// </summary>
        private readonly string savePath;
        /// <summary>
        /// 当前程序版本号
        /// </summary>
        private readonly string version;
        /// <summary>
        /// github api地址
        /// </summary>
        private readonly string githubUrl;
        /// <summary>
        /// 更新包解压目录
        /// </summary>
        private readonly string outPath;

        private GithubRelease githubRelease;
        private HttpDownload downloader;
        public Command openurlCommand { get; set; }
        public Command updateCommand { get; set; }
        public Command installCommand { get; set; }


        private readonly MainService main;
        private readonly TrayService tray;
        public UpdateViewModel(MainService main,
            TrayService tray)
        {
            this.main = main;
            this.tray = tray;

            savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Update",
                "update.zip");
            string[] versionArray = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
            version = versionArray[0] + "." + versionArray[1] + "." + versionArray[2];
#if DEBUG
            version = "1.0.3";
#endif
            githubUrl = "https://api.github.com/repos/planshit/projecteye/releases/latest";
            outPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

            openurlCommand = new Command(new Action<object>(openurlCommand_action));
            updateCommand = new Command(new Action<object>(updateCommand_action));
            installCommand = new Command(new Action<object>(installCommand_action));
            githubRelease = new GithubRelease(githubUrl, version);
            githubRelease.RequestCompleteEvent += Updater_RequestCompleteEvent;
            githubRelease.RequestErrorEvent += Updater_RequestErrorEvent;


            githubRelease.GetRequest();
            PlayProcess = true;
        }

        private void installCommand_action(object obj)
        {
            if (File.Exists(savePath))
            {
                string updateExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "ProjectEyeUp.exe");
                string[] runArgs = {
                                savePath,
                                outPath
                            };
                tray.Remove();
                main.Exit();
                Application.Current.Shutdown();

                bool runResult = ProcessHelper.Run(updateExe, runArgs);
                if (!runResult)
                {
                    Modal("无法启动解压程序，请手动覆盖更新。");
                    UpVisibility = Visibility.Visible;
                }
            }
            else
            {
                Modal("更新包似乎被外部删除，请尝试恢复或重试。");

                UpVisibility = Visibility.Visible;

            }
        }
        private void Modal(string text)
        {
            ModalText = text;
            ShowModal = true;
        }
        private void updateCommand_action(object obj)
        {
            downloader = new HttpDownload(githubRelease.Info.DownloadUrl, savePath);
            downloader.ProcessUpdateEvent += Downloader_ProcessUpdateEvent;
            downloader.CompleteEvent += Downloader_CompleteEvent;
            downloader.ErrorEvent += Downloader_ErrorEvent; ;

            downloader.Start();
            Tip = "正在请求下载资源...";
            PlayProcess = true;
            UpVisibility = Visibility.Collapsed;
        }

        private void Downloader_ErrorEvent(object sender, object value)
        {
            Tip = $"下载时发生异常，{value}";
            UpVisibility = Visibility.Visible;
        }

        private void Downloader_CompleteEvent(object sender, object value)
        {
            Tip = $"更新包下载已完成！";

            InstallVisibility = Visibility.Visible;
        }

        private void Downloader_ProcessUpdateEvent(object sender, object value)
        {
            Tip = $"正在下载更新包 {value} %";
        }
        private void openurlCommand_action(object obj)
        {
            string url = obj.ToString();
            if (!string.IsNullOrEmpty(url))
            {
                Process.Start(new ProcessStartInfo(url));
            }
        }

        private void Updater_RequestErrorEvent(object sender, object value)
        {
            PlayProcess = false;
            Tip = "无法获取版本信息，请重试或检查网络";
            Modal("无法获取到版本信息，请尝试重启应用程序后再试！");
        }

        private void Updater_RequestCompleteEvent(object sender, object value)
        {
            bool isCanUpdate = githubRelease.IsCanUpdate();
            PlayProcess = false;
            Tip = isCanUpdate ? "有新版本可以更新！" : "当前已是最新版本！";
            UpVisibility = isCanUpdate ? Visibility.Visible : Visibility.Hidden;

            if (isCanUpdate)
            {

                //string preText = githubRelease.Info.IsPre ? " [预览版本]" : " [正式版]";
                VersionInfo = "版本号：" + githubRelease.Info.Version + "\r\n\r\n" + githubRelease.Info.Title;
                VersionUrl = githubRelease.Info.HtmlUrl;
                OpenUrlVisibility = Visibility.Visible;
            }
        }

    }
}
