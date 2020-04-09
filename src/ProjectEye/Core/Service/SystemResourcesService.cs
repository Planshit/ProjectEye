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
                DisplayName = "默认",
                ThemeName = "Blue",
                ThemeColor = "#4F6BED"
            });
            Themes.Add(new ThemeModel()
            {
                DisplayName = "深色",
                ThemeName = "Dark",
                ThemeColor = "#4F6BED"
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
