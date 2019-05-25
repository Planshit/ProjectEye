using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    [XmlRootAttribute("theme")]
    public class ThemeModel
    {
        public string DisplayName { get; set; } = "默认";

        public string ThemeName { get; set; } = "Default";
        public string ThemeColor { get; set; } = "#ffc83d";

    }
}
