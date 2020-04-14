using Newtonsoft.Json;
using Project1.UI.Controls;
using Project1.UI.Controls.Models;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
using ProjectEye.Models.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEye.ViewModels
{
    public class TipViewDesignViewModel : TipViewDesignModel, IViewModel, IDisposable
    {
        private string UIConfigPath;
        public string ScreenName { get; set; }
        public Window WindowInstance { get; set; }

        private readonly ConfigService config;
        private readonly ThemeService theme;
        private readonly MainService main;

        public event ViewModelEventHandler ChangedEvent;

        public Command commonCommand { get; set; }
        public Command saveCommand { get; set; }


        public string TipText { get; set; }
        public TipViewDesignViewModel(
            ConfigService config,
            ThemeService theme,
            MainService main)
        {

            this.config = config;
            this.theme = theme;
            this.main = main;

            TipText = config.options.Style.TipContent;
            commonCommand = new Command(new Action<object>(commonCommand_action));
            saveCommand = new Command(new Action<object>(saveCommand_action));
            this.PropertyChanged += TipViewDesignViewModel_PropertyChanged;
        }

        private void TipViewDesignViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Container")
            {
                UIConfigPath = $"UI\\{config.options.Style.Theme.ThemeName}_{ScreenName}.json";
                Debug.WriteLine("窗口：" + WindowInstance.ActualWidth + "，屏幕：" + ScreenName + "，界面文件：" + UIConfigPath);
                if (Container != null)
                {
                    try
                    {
                        var data = new UIDesignModel();
                        if (FileHelper.Exists(UIConfigPath))
                        {
                            data = JsonConvert.DeserializeObject<UIDesignModel>(FileHelper.Read(UIConfigPath));

                        }
                        else
                        {
                            data = theme.GetCreateDefaultTipWindowUI(config.options.Style.Theme.ThemeName, ScreenName);
                            FileHelper.Write(UIConfigPath, JsonConvert.SerializeObject(data));
                        }
                        Container.SetContainerAttr(data.ContainerAttr);
                        Container.ImportElements(data.Elements);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Warning(ex.ToString());

                        Modal("无法加载现有UI配置文件，系统尝试创建默认UI失败！请尝试重程序。");
                    }

                }
            }
        }

        private void saveCommand_action(object obj)
        {
            var container = obj as Project1UIDesignContainer;
            var data = new UIDesignModel();
            data.ContainerAttr = container.GetContainerAttr();
            data.Elements = container.GetElements();
            string json = JsonConvert.SerializeObject(data);
            FileHelper.Write(UIConfigPath, json);

            main.CreateTipWindows();
            Modal("布局已更新");
        }

        private void commonCommand_action(object obj)
        {
            switch (obj.ToString())
            {
                case "quit":
                    WindowManager.Close("TipViewDesignWindow");
                    break;

            }
        }

        private void Modal(string msg)
        {
            ModalText = msg;
            ShowModal = true;
        }
        public void OnChanged()
        {
            ChangedEvent?.Invoke();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                UIConfigPath = null;
                ScreenName = null;
                WindowInstance = null;
                ChangedEvent = null;
                commonCommand = null;
                saveCommand = null;
                TipText = null;
                disposedValue = true;
                Debug.WriteLine("Dispose!");
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~TipViewDesignViewModel()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
