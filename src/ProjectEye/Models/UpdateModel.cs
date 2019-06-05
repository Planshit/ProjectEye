using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEye.Models
{
    public class UpdateModel : UINotifyPropertyChanged
    {
        private bool PlayProcess_ = false;
        public bool PlayProcess
        {
            get { return PlayProcess_; }
            set { PlayProcess_ = value; OnPropertyChanged(); }
        }

        private Visibility UpVisibility_ = Visibility.Collapsed;

        public Visibility UpVisibility
        {
            get { return UpVisibility_; }
            set { UpVisibility_ = value; OnPropertyChanged(); }
        }

        private Visibility OpenUrlVisibility_ = Visibility.Collapsed;

        public Visibility OpenUrlVisibility
        {
            get { return OpenUrlVisibility_; }
            set { OpenUrlVisibility_ = value; OnPropertyChanged(); }
        }
        private Visibility InstallVisibility_ = Visibility.Collapsed;

        public Visibility InstallVisibility
        {
            get { return InstallVisibility_; }
            set { InstallVisibility_ = value; OnPropertyChanged(); }
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
