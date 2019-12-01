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
        public string TipText { get; set; }
        public TipViewDesignViewModel(ConfigService config)
        {
            this.config = config;
            TipText = config.options.Style.TipContent;
            commonCommand = new Command(new Action<object>(commonCommand_action));
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
