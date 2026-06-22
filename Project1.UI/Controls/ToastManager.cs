using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.UI.Controls
{
    public static class ToastManager
    {
        private static Project1UIToast toast_;

        public static void Add(Project1UIToast toast)
        {
            if (toast_ != null)
            {
                toast_.Hide();
            }
            toast_ = toast;
        }

    }
}
