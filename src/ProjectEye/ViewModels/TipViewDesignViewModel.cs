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
    public class TipViewDesignViewModel : TipViewDesignModel, IViewModel
    {
        private string UIConfigPath;
        public string ScreenName { get; set; }
        public Window WindowInstance { get; set; }

        private readonly ConfigService config;

        public event ViewModelEventHandler ChangedEvent;

        public Command commonCommand { get; set; }
        public Command saveCommand { get; set; }


        public string TipText { get; set; }
        public TipViewDesignViewModel(ConfigService config)
        {

            this.config = config;
            TipText = config.options.Style.TipContent;
            commonCommand = new Command(new Action<object>(commonCommand_action));
            saveCommand = new Command(new Action<object>(saveCommand_action));
            this.PropertyChanged += TipViewDesignViewModel_PropertyChanged;
        }

        private void TipViewDesignViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UIConfigPath = $"UI\\{ScreenName}.json";
            Debug.WriteLine("窗口：" + WindowInstance.ActualWidth + "，屏幕：" + ScreenName);
            if (Container != null)
            {
                try
                {
                    if (FileHelper.Exists(UIConfigPath))
                    {
                        var data = JsonConvert.DeserializeObject<UIDesignModel>(FileHelper.Read(UIConfigPath));
                        Container.SetContainerAttr(data.ContainerAttr);
                        Container.ImportElements(data.Elements);
                    }
                }
                catch
                {
                    MessageBox.Show("无法加载现有UI配置文件", "错误提示");
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
            MessageBox.Show("更新布局需要重新启动软件才会生效", "提示");
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

        public void OnChanged()
        {
            ChangedEvent?.Invoke();
        }
    }
}
