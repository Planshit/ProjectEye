using ProjectEye.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.Core.Service
{
    public class SystemResourcesService : IService
    {
        public List<ThemeModel> Themes { get; set; }
        public List<ComboxModel> PreAlertActions { get; set; }
        public void Init()
        {
            Themes = new List<ThemeModel>();
            PreAlertActions = new List<ComboxModel>();


            Themes.Add(new ThemeModel()
            {
                DisplayName = "默认（Yellow）",
                ThemeName = "Default",
                ThemeColor = "#ffc83d"
            });
            Themes.Add(new ThemeModel()
            {
                DisplayName = "经典（Green）",
                ThemeName = "Classic",
                ThemeColor = "#00D4A0"
            });
            Themes.Add(new ThemeModel()
            {
                DisplayName = "暗色（Dark）",
                ThemeName = "Dark",
                ThemeColor = "Black"
            });
            Themes.Add(new ThemeModel()
            {
                DisplayName = "黑色（Black）",
                ThemeName = "Black",
                ThemeColor = "#000"
            });

            PreAlertActions.Add(new ComboxModel()
            {
                DisplayName = "进入本次休息",
                Value = "1"
            });
            PreAlertActions.Add(new ComboxModel()
            {
                DisplayName = "跳过本次休息",
                Value = "2"
            });
        }
    }
}
