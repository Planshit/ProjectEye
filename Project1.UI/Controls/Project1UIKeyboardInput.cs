
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
    public class Project1UIKeyboardInput : Control
    {
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(Project1UIKeyboardInput), new PropertyMetadata(""));

        public string KeyText
        {
            get { return (string)GetValue(KeyTextProperty); }
            set { SetValue(KeyTextProperty, value); }
        }

        public static readonly DependencyProperty KeyTextProperty =
            DependencyProperty.Register("KeyText", typeof(string), typeof(Project1UIKeyboardInput), new PropertyMetadata(""));
        public Project1UIKeyboardInput()
        {
            this.DefaultStyleKey = typeof(Project1UIKeyboardInput);
            this.CommandBindings.Add(new CommandBinding(Project1UIInputCommands.ClearTextCommand, OnClearTextCommand));

        }

        private void OnClearTextCommand(object sender, ExecutedRoutedEventArgs e)
        {
            KeyText = string.Empty;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            string key;
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key != Key.LeftCtrl)
            {
                key = "CTRL + " + e.Key.ToString();
            }
            else
            {
                key = e.Key.ToString();
            }
            KeyText = key;
        }

    }
}
