using ProjectEye.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.ViewModels
{
    public class TipViewDesignViewModel
    {
        public Command commonCommand { get; set; }
        public TipViewDesignViewModel()
        {
            commonCommand = new Command(new Action<object>(commonCommand_action));
        }

        private void commonCommand_action(object obj)
        {
            switch (obj.ToString())
            {
                case "quit":
                    WindowManager.Close("TipViewDesign");
                    break;
            }
        }
    }
}
