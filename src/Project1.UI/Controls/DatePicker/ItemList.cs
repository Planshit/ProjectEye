using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project1.UI.Controls.DatePicker
{

    public class ItemList : Control
    {
        #region 项目
        /// <summary>
        /// 项目
        /// </summary>
        public List<string> Items
        {
            get { return (List<string>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items",
                typeof(List<string>),
                typeof(ItemList),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsChanged)));

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ItemList;
            if (control != null)
            {
                control.RenderData();
            }
        }


        #endregion

        private WrapPanel wrapPanel;
        public ItemList()
        {
            DefaultStyleKey = typeof(ItemList);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            wrapPanel = GetTemplateChild("WrapPanel") as WrapPanel;
            RenderData();
        }

        private void RenderData()
        {
            wrapPanel.Children.Clear();
            foreach (string text in Items)
            {
                CheckButton item = new CheckButton();
                item.Content = text;
                wrapPanel.Children.Add(item);
            }
        }
    }
}
