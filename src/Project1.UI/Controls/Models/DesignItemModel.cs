using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        //通用属性
        #region 宽
        public double Width_;
        /// <summary>
        /// 宽
        /// </summary>
        public double Width
        {
            get { return Width_; }
            set
            {
                Width_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 高
        public double Height_;
        /// <summary>
        /// 高
        /// </summary>
        public double Height
        {
            get { return Height_; }
            set
            {
                Height_ = value;
                OnPropertyChanged();
            }
        }
        #endregion


        #region 背景色
        public Brush Background_ = Brushes.Black;
        /// <summary>
        /// 背景色
        /// </summary>
        public Brush Background
        {
            get { return Background_; }
            set
            {
                Background_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 边宽色
        public Brush BorderColor_ = Brushes.Transparent;
        /// <summary>
        /// 边框色
        /// </summary>
        public Brush BorderColor
        {
            get { return BorderColor_; }
            set
            {
                BorderColor_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 透明度
        public double Opacity_ = 1;
        /// <summary>
        /// 透明度（0~1）
        /// </summary>
        public double Opacity
        {
            get { return Opacity_; }
            set
            {
                Opacity_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 文字大小
        public double FontSize_ = 12;
        /// <summary>
        /// 文字大小
        /// </summary>
        public double FontSize
        {
            get { return FontSize_; }
            set
            {
                FontSize_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 文字色
        public Brush TextColor_ = Brushes.Black;
        /// <summary>
        /// 边框色
        /// </summary>
        public Brush TextColor
        {
            get { return TextColor_; }
            set
            {
                TextColor_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 文字宽度
        /// <summary>
        /// 边框色
        /// </summary>
        public FontWeight FontWeight
        {
            get { return IsFontBold ? FontWeights.Bold : FontWeights.Normal; }
        }
        #endregion

        #region 是否加粗
        public bool IsFontBold_ = false;
        /// <summary>
        /// 文字大小
        /// </summary>
        public bool IsFontBold
        {
            get { return IsFontBold_; }
            set
            {
                IsFontBold_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        //文本类型元素属性

        #region 文字
        public string Text_;
        /// <summary>
        /// 文字
        /// </summary>
        public string Text
        {
            get { return Text_; }
            set
            {
                Text_ = value;
                Debug.WriteLine("更新绑定:" + value);

                OnPropertyChanged();
            }
        }
        #endregion

        //按钮类型元素属性

        #region 按钮文字
        public string ButtonText_;
        /// <summary>
        /// 按钮文字
        /// </summary>
        public string ButtonText
        {
            get { return ButtonText_; }
            set
            {
                ButtonText_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 按钮样式名
        public string ButtonStyleName_;
        /// <summary>
        /// 按钮样式名
        /// </summary>
        public string ButtonStyleName
        {
            get { return ButtonStyleName_; }
            set
            {
                ButtonStyleName_ = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region 按钮样式
        public Style ButtonStyle_;
        /// <summary>
        /// 按钮样式名
        /// </summary>
        public Style ButtonStyle
        {
            get { return ButtonStyle_; }
            set
            {
                ButtonStyle_ = value;
                OnPropertyChanged();
            }
        }
        #endregion
        //图片类型元素属性

        #region 图片
        public ImageSource ImageSource_;
        public ImageSource ImageSource
        {
            get
            {
                return ImageSource_;
            }
            set
            {
                ImageSource_ = value;
            }
        }
        public string Image_ = "";
        /// <summary>
        /// 图片路径
        /// </summary>
        public string Image
        {
            get { return Image_; }
            set
            {
                try
                {
                    var image = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute));
                    Image_ = value;
                    Width = image.Width;
                    Height = image.Height;
                    ImageSource = image;
                    OnPropertyChanged();
                }
                catch
                {

                }

                Debug.WriteLine("更新绑定:" + value);

            }
        }
        #endregion

    }
}
