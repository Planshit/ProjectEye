using ProjectEye.Core.Models.Options;
using System.Collections.Generic;
using System.Windows;

namespace ProjectEye.Core.Service
{
    public class SystemResourcesService : IService
    {
        public List<ThemeModel> Themes { get; set; }
        public List<ComboxModel> PreAlertActions { get; set; }
        public List<AnimationModel> Animations { get; set; }
        public List<ComboxModel> Languages { get; set; }

        public void Init()
        {
            Themes = new List<ThemeModel>();
            PreAlertActions = new List<ComboxModel>();
            Animations = new List<AnimationModel>();
            Languages = new List<ComboxModel>();

            Themes.Add(new ThemeModel()
            {
                DisplayName = $"{Application.Current.Resources["Lang_Default"]}",
                ThemeName = "Blue",
                ThemeColor = "#4F6BED"
            });
            Themes.Add(new ThemeModel()
            {
                DisplayName = $"{Application.Current.Resources["Lang_Dark"]}",
                ThemeName = "Dark",
                ThemeColor = "#4F6BED"
            });

            PreAlertActions.Add(new ComboxModel()
            {
                DisplayName = $"{Application.Current.Resources["Lang_Startthebreak"]}",
                Value = "1"
            });
            PreAlertActions.Add(new ComboxModel()
            {
                DisplayName = $"{Application.Current.Resources["Lang_Skipthebreak"]}",
                Value = "2"
            });
            //预置动画
            Animations.Add(new AnimationModel()
            {
                ID = 0,
                AnimationType = Project1.UI.Controls.Project1UIWindow.AnimationType.None,
                DisplayName = $"{Application.Current.Resources["Lang_None"]}"
            });
            Animations.Add(new AnimationModel()
            {
                ID = 1,
                AnimationType = Project1.UI.Controls.Project1UIWindow.AnimationType.RightBottomScale,
                DisplayName = $"{Application.Current.Resources["Lang_Zoomfrombottom-rightcorner"]}"
            });
            Animations.Add(new AnimationModel()
            {
                ID = 2,
                AnimationType = Project1.UI.Controls.Project1UIWindow.AnimationType.Opacity,
                DisplayName = $"{Application.Current.Resources["Lang_Fadeinandfadeout"]}"
            });
            Animations.Add(new AnimationModel()
            {
                ID = 3,
                AnimationType = Project1.UI.Controls.Project1UIWindow.AnimationType.Cool,
                DisplayName = $"Cool"
            });
            //语言
            Languages.Add(new ComboxModel()
            {
                DisplayName = "中文",
                Value = "zh"
            });
            Languages.Add(new ComboxModel()
            {
                DisplayName = "English",
                Value = "en"
            });
        }
    }
}
