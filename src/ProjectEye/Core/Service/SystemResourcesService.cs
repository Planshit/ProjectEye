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

        public void Init()
        {
            Themes = new List<ThemeModel>();
            Themes.Add(new ThemeModel()
            {
                DisplayName = "默认",
                ThemeName = "Default",
                ThemeColor = "#ffc83d"
            });
            Themes.Add(new ThemeModel()
            {
                DisplayName="深色",
                ThemeName = "Dark",
                ThemeColor = "Black"
            });
        }
    }
}
