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
        public List<int> Items
        {
            get { return (List<int>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items",
                typeof(List<int>),
                typeof(ItemList),
                new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));




        #endregion

        #region 选中值
        /// <summary>
        /// 选中值
        /// </summary>
        public int SelectedValue
        {
            get { return (int)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue",
                typeof(int),
                typeof(ItemList),
                new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));


        #endregion

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ItemList;
            if (control != null)
            {
                if (e.Property == ItemsProperty)
                {
                    control.RenderData();
                }
                if (e.Property == SelectedValueProperty &&
                    e.NewValue != e.OldValue)
                {
                    control.HandleSelectValue();
                }
            }
        }

        #region 事件
        public delegate void SelectedEventHandler(object sender, int value);
        /// <summary>
        /// 当选中项更改时发生
        /// </summary>
        public event SelectedEventHandler OnSelected;
        #endregion

        private WrapPanel wrapPanel;
        private Dictionary<int, CheckButton> buttons;
        public ItemList()
        {
            DefaultStyleKey = typeof(ItemList);
            buttons = new Dictionary<int, CheckButton>();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            wrapPanel = GetTemplateChild("WrapPanel") as WrapPanel;
            RenderData();
            HandleSelectValue();
        }

        private void RenderData()
        {
            if (Items != null && wrapPanel != null)
            {
                wrapPanel.Children.Clear();
                buttons.Clear();
                foreach (var text in Items)
                {
                    CheckButton item = new CheckButton();
                    item.Content = text.ToString();
                    item.MouseDown += (e, c) =>
                    {
                        SelectedValue = text;
                    };
                    buttons.Add(text, item);
                    wrapPanel.Children.Add(item);
                }
            }
        }

        private void HandleSelectValue()
        {
            if (buttons.ContainsKey(SelectedValue))
            {
                OnSelected?.Invoke(this, SelectedValue);
                foreach (var btn in buttons)
                {
                    btn.Value.Checked = btn.Key == SelectedValue;
                }
            }
        }
    }
}
