using Project1.UI.Controls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.Models.UI
{
    public class UIDesignModel
    {
        /// <summary>
        /// 容器属性
        /// </summary>
        public ContainerModel ContainerAttr { get; set; }
        /// <summary>
        /// 元素集合
        /// </summary>
        public List<ElementModel> Elements { get; set; }
    }
}
