using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Statistic
{
    [XmlRootAttribute("StatisticList")]
    public class StatisticListModel
    {
        public List<StatisticModel> Data { get; set; }
    }
}
