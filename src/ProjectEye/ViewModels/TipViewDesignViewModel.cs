using Newtonsoft.Json;
using Project1.UI.Controls;
using ProjectEye.Core;
using ProjectEye.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.ViewModels
{
    public class TipViewDesignViewModel
    {
        private readonly ConfigService config;
        public Command commonCommand { get; set; }
        public Command saveCommand { get; set; }

        public string TipText { get; set; }
        public TipViewDesignViewModel(ConfigService config)
        {
            this.config = config;
            TipText = config.options.Style.TipContent;
            commonCommand = new Command(new Action<object>(commonCommand_action));
            saveCommand = new Command(new Action<object>(saveCommand_action));

        }

        private void saveCommand_action(object obj)
        {
            var container = obj as Project1UIDesignContainer;
            string json = JsonConvert.SerializeObject(container.GetElements());
            FileHelper.Write("UI\\main.json", json);
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
    }
}
