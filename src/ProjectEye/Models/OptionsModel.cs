using ProjectEye.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace ProjectEye.Models
{
    public class OptionsModel : UINotifyPropertyChanged
    {
        private ProjectEye.Core.Models.Options.OptionsModel data_;
        public ProjectEye.Core.Models.Options.OptionsModel Data
        {
            get
            {
                return data_;
            }
            set
            {
                data_ = value;
                OnPropertyChanged("Data");
            }
        }

        private string version_;
        public string Version
        {
            get
            {
                return version_;
            }
            set
            {
                version_ = value;
                OnPropertyChanged("Version");
            }
        }

        public string VersionLink
        {
            get
            {
                return "https://github.com/Planshit/ProjectEye/releases/tag/" + Version;
            }

        }
        public string SelectedItem { get; set; }
        public List<ThemeModel> Themes { get; set; }
        public List<ComboxModel> PreAlertActions { get; set; }

        public bool IsPreAlert
        {
            get
            {
                return Data.Style.IsPreAlert;
            }
            set
            {
                Data.Style.IsPreAlert = value;
                OnPropertyChanged();
                OnPropertyChanged("PreAlertConfigVisibility");
            }
        }
        public Visibility PreAlertConfigVisibility
        {
            get
            {

                return Data.Style.IsPreAlert ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsBreakProgressList
        {
            get
            {
                return Data.Behavior.IsBreakProgressList;
            }
            set
            {
                Data.Behavior.IsBreakProgressList = value;
                OnPropertyChanged();
                OnPropertyChanged("PreAlertConfigVisibility");
            }
        }
        public Visibility BreakProgressListVisibility
        {
            get
            {

                return Data.Behavior.IsBreakProgressList ? Visibility.Visible : Visibility.Collapsed;
            }
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
        private string ModalText_;
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
