using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project1.UI.Controls
{
    /// <summary>
    /// Fabric icon
    /// </summary>
    public class TimeBox : Control
    {
        #region 时

        public int Hour
        {
            get { return (int)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }
        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register("Hour",
                typeof(int),
                typeof(TimeBox),
                new PropertyMetadata(7, new PropertyChangedCallback(OnChanged)));



        #endregion

        #region 分
        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }
        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register("Minutes",
                typeof(int),
                typeof(TimeBox),
                new PropertyMetadata(24, new PropertyChangedCallback(OnChanged)));


        #endregion

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timebox = d as TimeBox;
            if (timebox != null)
            {
                timebox.HandleValue();
            }
        }

        private ComboBox hourBox, minutesBox;
        public TimeBox()
        {
            DefaultStyleKey = typeof(TimeBox);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            hourBox = GetTemplateChild("HourBox") as ComboBox;
            minutesBox = GetTemplateChild("MinutesBox") as ComboBox;
            if (hourBox != null)
            {
                for (int i = 0; i < 24; i++)
                {
                    var item = new ComboBoxItem();
                    item.Content = i.ToString().PadLeft(2, '0');
                    item.Tag = i;
                    hourBox.Items.Add(item);
                }

            }
            if (minutesBox != null)
            {
                for (int i = 0; i < 60; i++)
                {
                    var item = new ComboBoxItem();
                    item.Content = i.ToString().PadLeft(2, '0');
                    item.Tag = i;
                    minutesBox.Items.Add(item);
                }
            }
            HandleValue();
            hourBox.SelectionChanged += (e, c) =>
            {
                var item = hourBox.SelectedItem as ComboBoxItem;
                Hour = (int)item.Tag;
            };
            minutesBox.SelectionChanged += (e, c) =>
            {
                var item = minutesBox.SelectedItem as ComboBoxItem;
                Minutes = (int)item.Tag;
            };
        }

        private void HandleValue()
        {
            if (hourBox != null)
            {
                var selectedItem = hourBox.Items
                    .Cast<ComboBoxItem>()
                    .Where(e => e.Tag.ToString() == Hour.ToString())
                    .FirstOrDefault();
                if (hourBox.SelectedItem != selectedItem)
                {
                    hourBox.SelectedItem = selectedItem;
                }
            }
            if (minutesBox != null)
            {
                var selectedItem = minutesBox.Items
                    .Cast<ComboBoxItem>()
                    .Where(e => e.Tag.ToString() == Minutes.ToString())
                    .FirstOrDefault();
                if (minutesBox.SelectedItem != selectedItem)
                {
                    minutesBox.SelectedItem = selectedItem;
                }
            }
        }
    }
}
