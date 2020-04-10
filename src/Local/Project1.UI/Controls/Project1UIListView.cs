using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Project1.UI.Controls
{
    public class Project1UIListView : ListView
    {
        public object Selected
        {
            get { return (object)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }
        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(object), typeof(Project1UIListView));

        public Project1UIListView()
        {
            
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            Selected = SelectedItems;

        }
    }
}
