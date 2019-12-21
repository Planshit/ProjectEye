using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.UI.Controls.Models
{
    public class DesignItemModel : UINotifyPropertyChanged
    {
        private System.Windows.Visibility ControlPointVisibility_;
        public System.Windows.Visibility ControlPointVisibility
        {
            get { return ControlPointVisibility_; }
            set { ControlPointVisibility_ = value; OnPropertyChanged(); }
        }
    }
}
