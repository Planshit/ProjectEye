using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectEye.ViewModels
{
    interface IViewModel
    {
        /// <summary>
        /// 所在屏幕
        /// </summary>
        string ScreenName { get; set; }
        /// <summary>
        /// 窗口实例
        /// </summary>
        Window WindowInstance { get; set; }
        event ViewModelEventHandler ChangedEvent;
        void OnChanged();
    }
}
