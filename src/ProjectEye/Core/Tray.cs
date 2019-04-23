using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace ProjectEye.Core
{
    /// <summary>
    /// 管理和显示托盘图标
    /// </summary>
    public class Tray
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.ComponentModel.IContainer components;
        public Tray()
        {
            components = new System.ComponentModel.Container();
            contextMenu = new System.Windows.Forms.ContextMenu();
            //主界面菜单项
            //System.Windows.Forms.MenuItem menuItem_showMainWindow = new System.Windows.Forms.MenuItem();
            //menuItem_showMainWindow.Index = 0;
            //menuItem_showMainWindow.Text = "主界面";
            //menuItem_showMainWindow.Click += new System.EventHandler(menuItem_showMainWindow_Click);
            //退出菜单项
            System.Windows.Forms.MenuItem menuItem_exit = new System.Windows.Forms.MenuItem();
            menuItem_exit.Index = 1;
            menuItem_exit.Text = "退出";
            menuItem_exit.Click += new System.EventHandler(menuItem_exit_Click);

            this.contextMenu.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { menuItem_exit });

          
            notifyIcon = new System.Windows.Forms.NotifyIcon(components);
            Uri iconUri = new Uri("/ProjectEye;component/Resources/sunglasses.ico", UriKind.RelativeOrAbsolute);
            StreamResourceInfo info = Application.GetResourceStream(iconUri);
            notifyIcon.Icon = new Icon(info.Stream);
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Text = "请爱护你的眼睛";
            notifyIcon.Visible = true;
            //notifyIcon.DoubleClick += new System.EventHandler(notifyIcon_DoubleClick);
        }

        /// <summary>
        /// 清除图标
        /// </summary>
        public void Remove()
        {
            notifyIcon.Visible = false;
            if (components != null)
            {
                components.Dispose();
            }
        }

        private void menuItem_exit_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        //private void menuItem_showMainWindow_Click(object sender, EventArgs e)
        //{
        //    var window = WindowManager.CreateWindow("MainWindow");
        //    window.Show();
        //}

        //private void notifyIcon_DoubleClick(object sender, EventArgs e)
        //{
        //    var window = WindowManager.CreateWindow("MainWindow");
        //    window.Show();
        //}
    }
}
