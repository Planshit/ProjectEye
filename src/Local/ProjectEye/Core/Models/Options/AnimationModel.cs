using static Project1.UI.Controls.Project1UIWindow;

namespace ProjectEye.Core.Models.Options
{
    public class AnimationModel
    {
        public int ID { get; set; }
        /// <summary>
        /// 动画名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 动画类型
        /// </summary>
        public AnimationType AnimationType { get; set; }
    }
}
