using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Project1.UI.Controls.Models
{
    public class ContainerModel : UINotifyPropertyChanged
    {
        private double Opacity_;
        public double Opacity
        {
            get
            {
                return Opacity_;
            }
            set
            {
                Opacity_ = value;
                OnPropertyChanged();
            }
        }
        private Brush Background_;
        public Brush Background
        {
            get
            {

                return Background_;
            }
            set
            {
                Background_ = value;
                OnPropertyChanged();
            }
        }
    }
}
