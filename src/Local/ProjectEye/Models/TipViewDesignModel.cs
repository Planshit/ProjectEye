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
                OnPropertyChanged("Container");
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
        private string ModalText_ = "设置已更新";
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
