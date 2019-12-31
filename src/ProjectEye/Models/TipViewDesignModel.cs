using Project1.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEye.Models
{
    public class TipViewDesignModel : UINotifyPropertyChanged
    {
        private Project1UIDesignContainer Container_;
        public Project1UIDesignContainer Container
        {
            get
            {
                return Container_;
            }
            set
            {
                Container_ = value;
                OnPropertyChanged();
            }
        }
    }
}
