using System.Xml.Serialization;

namespace ProjectEye.Core.Models.Options
{
    [XmlRootAttribute("ComboxModel")]
    public class ComboxModel
    {
        public string DisplayName { get; set; } = "请选择";

        public string Value { get; set; } = "0";
    }
}
