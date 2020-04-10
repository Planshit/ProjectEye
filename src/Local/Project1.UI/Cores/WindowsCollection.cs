using Project1.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.UI.Cores
{
    public class WindowsCollection
    {
        private static readonly List<Project1UIWindow> _windows = new List<Project1UIWindow>();

        public Project1UIWindow this[int index]
        {
            get
            {
                return _windows[index];
            }
            set
            {
                _windows[index] = value;
            }
        }

        public static void Add(Project1UIWindow _window)
        {

            _windows.Add(_window);

        }

        public static void Remove(Project1UIWindow _window)
        {

            _windows.Remove(_window);

        }
        public static List<Project1UIWindow> ToList()
        {
            return _windows;
        }


    }
}
