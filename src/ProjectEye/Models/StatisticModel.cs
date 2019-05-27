using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.Models
{
    public class StatisticModel : UINotifyPropertyChanged
    {
        public double[] Working_;
        public double[] Working
        {
            get
            {
                return Working_;
            }
            set
            {
                Working_ = value;
                OnPropertyChanged("Working");
            }
        }

        public double[] Reset_;
        public double[] Reset {
            get
            {
                return Reset_;
            }
            set
            {
                Reset_ = value;
                OnPropertyChanged("Reset");
            }
        }

        public double[] Skip_;
        public double[] Skip {
            get
            {
                return Skip_;
            }
            set
            {
                Skip_ = value;
                OnPropertyChanged("Skip");
            }
        }
        public string[] Labels_;
        public string[] Labels {
            get
            {
                return Labels_;
            }
            set
            {
                Labels_ = value;
                OnPropertyChanged("Labels");
            }
        }

       
    }
}
