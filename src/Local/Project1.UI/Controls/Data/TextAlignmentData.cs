using Project1.UI.Controls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.UI.Controls.Data
{
    public class TextAlignmentData
    {
        private List<DesignTextAlignment> data;
        public TextAlignmentData()
        {
            data = new List<DesignTextAlignment>();
            data.Add(new DesignTextAlignment()
            {

                DisplayName = "左",
                Value = 0
            });
            data.Add(new DesignTextAlignment()
            {

                DisplayName = "中",
                Value = 1
            });
            data.Add(new DesignTextAlignment()
            {

                DisplayName = "右",
                Value = 2
            });
        }
        public DesignTextAlignment this[int index]
        {
            get
            {
                var result = data.Find(m => m.Value == index);
                if (result != null)
                {
                    return result;
                }
                return data[0];
            }
        }
        public List<DesignTextAlignment> ToList()
        {
            return data;
        }
    }
}
