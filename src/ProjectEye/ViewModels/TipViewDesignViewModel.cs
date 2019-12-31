using Newtonsoft.Json;
using Project1.UI.Controls;
using Project1.UI.Controls.Models;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using ProjectEye.Models;
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
            if (Container != null)
            {
                Debug.WriteLine(ScreenName);
                List<ElementModel> elements = JsonConvert.DeserializeObject<List<ElementModel>>(FileHelper.Read($"UI\\{ScreenName}.json"));
                Container.ImportElements(elements);
            }
        }

        private void saveCommand_action(object obj)
        {
            var container = obj as Project1UIDesignContainer;
            string json = JsonConvert.SerializeObject(container.GetElements());
            FileHelper.Write($"UI\\{ScreenName}.json", json);
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
