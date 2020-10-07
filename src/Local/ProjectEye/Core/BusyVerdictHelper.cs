using System.Diagnostics;

namespace ProjectEye.Core
{
    /// <summary>
    /// 繁忙判断帮助类
    /// 用于判断用户的繁忙状态
    /// </summary>
    public class BusyVerdictHelper
    {
        private readonly static string[] MusicApps = {
        "cloudmusic",""};
        /// <summary>
        /// 指示用户当前是否是在看视频
        /// </summary>
        /// <returns></returns>
        public static bool IsWatchingVideo()
        {
            Process[] processes = Process.GetProcesses();
            //processes.Where(m => m.ProcessName)

            return false;
        }


    }
}
