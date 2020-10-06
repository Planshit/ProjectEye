
using ProjectEye.Core;
using ProjectEye.Core.Net;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        /// <summary>
        /// 升级程序路径（新目录）
        /// </summary>
        private readonly string upexePath;

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
            upexePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Update",
                "ProjectEyeUp.exe");
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

                //检查升级程序文件
                if (!File.Exists(updateExe) && !File.Exists(upexePath))
                {
                    Modal("升级程序已被删除，请前往/Update/目录手动解压更新。");
                    UpVisibility = Visibility.Visible;
                }

                //将升级程序文件复制到更新目录（处理升级程序自身无法被覆盖更新的问题
                if (File.Exists(updateExe))
                {
                    if (File.Exists(upexePath))
                    {
                        File.Delete(upexePath);
                    }
                    File.Copy(updateExe, upexePath);
                }

                //配置启动参数
                string[] runArgs = {
                                savePath,
                                outPath
                            };

                //退出主程序
                tray.Remove();
                main.Exit();
                Application.Current.Shutdown();

                //启动升级程序
                bool runResult = ProcessHelper.Run(upexePath, runArgs);
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
            Tip = $"{Application.Current.Resources["Lang_Requestingdownloadsources"]}...";
            PlayProcess = true;
            UpVisibility = Visibility.Collapsed;
        }

        private void Downloader_ErrorEvent(object sender, object value)
        {
            Tip = $"{Application.Current.Resources["Lang_Erroroccurredduringdownload"]}，{value}";
            UpVisibility = Visibility.Visible;
        }

        private void Downloader_CompleteEvent(object sender, object value)
        {
            Tip = $"{Application.Current.Resources["Lang_Updatepackagehasbeendownloaded"]}!";

            InstallVisibility = Visibility.Visible;
        }

        private void Downloader_ProcessUpdateEvent(object sender, object value)
        {
            Tip = $"{Application.Current.Resources["Lang_Downloadingupdatepackage"]} {value} %";
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
            Tip = $"{Application.Current.Resources["Lang_Updategeterror"]}";
            Modal($"{Application.Current.Resources["Lang_Updategeterror"]}");
        }

        private void Updater_RequestCompleteEvent(object sender, object value)
        {
            bool isCanUpdate = githubRelease.IsCanUpdate();
            PlayProcess = false;
            Tip = isCanUpdate ? $"{Application.Current.Resources["Lang_Updateisavailable"]}" : $"{Application.Current.Resources["Lang_Noupdateisavailable"]}";
            UpVisibility = isCanUpdate ? Visibility.Visible : Visibility.Hidden;

            if (isCanUpdate)
            {

                //string preText = githubRelease.Info.IsPre ? " [预览版本]" : " [正式版]";
                VersionInfo = $"{Application.Current.Resources["Lang_Version"]}: " + githubRelease.Info.Version + "\r\n\r\n" + githubRelease.Info.Title;
                VersionUrl = githubRelease.Info.HtmlUrl;
                OpenUrlVisibility = Visibility.Visible;
            }
        }

    }
}
