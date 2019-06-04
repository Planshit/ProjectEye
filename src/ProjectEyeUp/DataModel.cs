using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEyeUp
{
    public class DataModel : UINotifyPropertyChanged
    {
        private bool PlayProcess_ = false;
        public bool PlayProcess
        {
            get { return PlayProcess_; }
            set { PlayProcess_ = value; OnPropertyChanged(); }
        }

        private Visibility UpVisibility_ = Visibility.Hidden;

        public Visibility UpVisibility
        {
            get { return UpVisibility_; }
            set { UpVisibility_ = value; OnPropertyChanged(); }
        }

        private string Tip_ = "正在获取版本信息";
        public string Tip
        {
            get { return Tip_; }
            set { Tip_ = value; OnPropertyChanged("Tip"); }
        }

        private string VersionInfo_ = "";
        public string VersionInfo
        {
            get { return VersionInfo_; }
            set { VersionInfo_ = value; OnPropertyChanged("VersionInfo"); }
        }

        private string VersionUrl_ = "";
        public string VersionUrl
        {
            get { return VersionUrl_; }
            set { VersionUrl_ = value; OnPropertyChanged("VersionUrl"); }
        }
    }
}
