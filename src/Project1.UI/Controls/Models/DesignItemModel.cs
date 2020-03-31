using Project1.UI.Cores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private double Width_;
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
        private double Height_;
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
        private Brush Background_ = Brushes.Black;
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
        private Brush BorderColor_ = Brushes.Transparent;
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
        private double Opacity_ = 1;
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
        private double FontSize_ = 12;
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
        private Brush TextColor_ = Brushes.Black;
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
        private bool IsFontBold_ = false;
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
        private string Text_;
        /// <summary>
        /// 文字
        /// </summary>
        public string Text
        {
            get { return Text_; }
            set
            {
                Text_ = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region 文字对齐方式
        /// <summary>
        /// 文本元素支持的文字对齐方式
        /// </summary>
        public ObservableCollection<DesignTextAlignment> TextAlignmentList { get; set; }
        private DesignTextAlignment TextAlignment_;
        /// <summary>
        /// 文字对齐方式
        /// </summary>
        public DesignTextAlignment TextAlignment
        {
            get { return TextAlignment_; }
            set
            {
                TextAlignment_ = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 文字对齐方式
        /// </summary>
        public TextAlignment TextALignment
        {
            get
            {
                if (TextAlignment != null)
                {
                    switch (TextAlignment.Value)
                    {
                        case 0:

                            return System.Windows.TextAlignment.Left;
                        case 1:

                            return System.Windows.TextAlignment.Center;
                        case 2:

                            return System.Windows.TextAlignment.Right;
                    }
                }
                return System.Windows.TextAlignment.Left;
            }
        }
        #endregion

        //按钮类型元素属性

        #region 按钮文字
        private string ButtonText_;
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
        private string ButtonStyleName_;
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
        private Style ButtonStyle_;
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

        #region 命令
        private string Command_;
        /// <summary>
        /// 文字
        /// </summary>
        public string Command
        {
            get { return Command_; }
            set
            {
                Command_ = value;
                OnPropertyChanged();
            }
        }
        #endregion
        //图片类型元素属性

        #region 图片
        private ImageSource ImageSource_;
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
        private string Image_ = "";
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
                    //var image = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute));
                    //Image_ = value;
                    //ImageSource = image;
                    //Width = image.Width;
                    //Height = image.Height;
                    Image_ = value;
                    ImageSource = BitmapImager.Load(value);
                    OnPropertyChanged();
                }
                catch
                {
                    Debug.WriteLine(value);
                }
            }
        }
        #endregion

    }
}
