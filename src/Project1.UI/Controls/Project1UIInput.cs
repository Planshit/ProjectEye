
using Project1.UI.Controls.Commands;
using Project1.UI.Controls.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Project1.UI.Controls
{
    public class Project1UIInput : TextBox
    {
        /// <summary>
        /// 文件选择缓存（处理popup弹出后自动重置的bug）
        /// </summary>
        //private string fileNameCache = string.Empty;
        /// <summary>
        /// 文件扩展名，仅在Type为文件选择对话框时有效
        /// </summary>
        public string ExtNames { get; set; }
        /// <summary>
        /// 输入框圆角半径
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Project1UIInput));

        /// <summary>
        /// 图标
        /// </summary>
        public Project1UIIconType Icon
        {
            get { return (Project1UIIconType)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Project1UIIconType), typeof(Project1UIInput), new PropertyMetadata(Project1UIIconType.Null));

        /// <summary>
        /// 图标大小
        /// </summary>
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(Project1UIInput));

        /// <summary>
        /// 图标对齐方式 只支持LEFT/RIGHT
        /// </summary>
        public HorizontalAlignment IconAlignment
        {
            get { return (HorizontalAlignment)GetValue(IconAlignmentProperty); }
            set { SetValue(IconAlignmentProperty, value); }
        }
        public static readonly DependencyProperty IconAlignmentProperty =
            DependencyProperty.Register("IconAlignment", typeof(HorizontalAlignment), typeof(Project1UIInput));


        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(Project1UIInput));
        /// <summary>
        /// 输入框类别
        /// </summary>
        public Project1UIInputType Type
        {
            get { return (Project1UIInputType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(Project1UIInputType), typeof(Project1UIInput));
        /// <summary>
        /// 内容更改时滚动条自动滚动到最底部
        /// </summary>
        public bool IsAutoScrollToEnd
        {
            get { return (bool)GetValue(IsAutoScrollToEndProperty); }
            set { SetValue(IsAutoScrollToEndProperty, value); }
        }

        public static readonly DependencyProperty IsAutoScrollToEndProperty =
            DependencyProperty.Register("IsAutoScrollToEnd", typeof(bool), typeof(Project1UIInput));

        #region 禁止输入模式
        public bool IsDisabledInput
        {
            get { return (bool)GetValue(IsDisabledInputProperty); }
            set { SetValue(IsDisabledInputProperty, value); }
        }

        public static readonly DependencyProperty IsDisabledInputProperty =
            DependencyProperty.Register("IsDisabledInput", typeof(bool), typeof(Project1UIInput), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDisabledInputPropertyChanged)));

        private static void OnIsDisabledInputPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = (d as Project1UIInput);
            if ((bool)e.NewValue)
            {
                sender.IsReadOnly = true;
            }
        }
        #endregion
        public Project1UIInput()
        {
            this.DefaultStyleKey = typeof(Project1UIInput);
            this.CommandBindings.Add(new CommandBinding(Project1UIInputCommands.ClearTextCommand, OnClearTextCommand));
            this.CommandBindings.Add(new CommandBinding(Project1UIInputCommands.CommonOpenFileDialog, OnCommonOpenFileDialog));
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (IsAutoScrollToEnd)
            {
                ScrollToEnd();
            }
        }

        private void OnCommonOpenFileDialog(object sender, ExecutedRoutedEventArgs e)
        {
            //打开文件夹对话框
            //using (var dialog = new CommonOpenFileDialog())
            //{
            //    if (Type == Project1UIInputType.FolderSelect)
            //    {
            //        dialog.IsFolderPicker = true;
            //    }

            //    CommonFileDialogResult result = dialog.ShowDialog();
            //    if (result == CommonFileDialogResult.Ok)
            //    {
            //        Text = dialog.FileName;
            //        Focus();
            //        SelectionStart = Text.Length;
            //    }
            //}
            if (Type == Project1UIInputType.FileSelect)
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

                openFileDialog.CheckPathExists = true;
                openFileDialog.Filter = ExtNames;
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (IsDisabledInput)
                    {
                        Placeholder = openFileDialog.FileName;
                    }
                    else
                    {
                        Text = openFileDialog.FileName;
                        Focus();
                        SelectionStart = Text.Length;
                    }
                    
                }
            }
            if (Type == Project1UIInputType.FolderSelect)
            {
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();  //选择文件夹


                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Text = folderBrowserDialog.SelectedPath;
                    Focus();
                    SelectionStart = Text.Length;
                }
            }
            /*
             待添加 文件保存对话框 - 2018年12月17日
             */
        }

        private void OnClearTextCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Text = string.Empty;
        }
    }
}
