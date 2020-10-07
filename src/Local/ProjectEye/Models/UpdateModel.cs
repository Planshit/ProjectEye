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

        private string Tip_ = "Loading...";
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
        //是否显示模态弹窗
        private bool ShowModal_ = false;
        public bool ShowModal
        {
            get
            {
                return ShowModal_;
            }
            set
            {
                ShowModal_ = value;
                OnPropertyChanged();
            }
        }
        //模态弹窗文本
        private string ModalText_ = "";
        public string ModalText
        {
            get
            {
                return ModalText_;
            }
            set
            {
                ModalText_ = value;
                OnPropertyChanged();
            }
        }
    }
}
