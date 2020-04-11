using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 音效Service
    /// 处理休息结束提示音的加载和播放
    /// </summary>
    public class SoundService : IService
    {
        private readonly ConfigService config;
        private SoundPlayer player;
        public SoundService(ConfigService config)
        {
            player = new SoundPlayer();
            player.LoadCompleted += Player_LoadCompleted;
            this.config = config;
        }

        private void Player_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            player.Dispose();
        }

        public void Init()
        {
            string path = null;
            if (!string.IsNullOrEmpty(config.options.General.SoundPath))
            {
                path = config.options.General.SoundPath;
            }
            LoadSound(path);
        }

        #region 播放休息结束音效
        /// <summary>
        /// 播放休息结束音效
        /// </summary>
        public bool Play()
        {
            if (player.IsLoadCompleted)
            {
                try
                {
                    player.Play();
                    return true;
                }
                catch (Exception ec)
                {
                    //播放声音失败，可能是加载了不支持或损坏的文件
                    LogHelper.Warning(ec.ToString());
                    //切换到默认音效
                    LoadSound();
                }
            }
            return false;
        }

        #endregion

        #region 加载指定音效文件
        /// <summary>
        /// 从路径加载音效
        /// </summary>
        /// <param name="file">路径</param>
        /// <param name="resource">指示是否是系统资源</param>
        /// <returns></returns>
        public bool Load(string file, bool resource = true)
        {
            try
            {
                if (resource)
                {
                    Uri soundUri = new Uri(file, UriKind.RelativeOrAbsolute);
                    StreamResourceInfo info = Application.GetResourceStream(soundUri);
                    player.Stream = info.Stream;
                }
                else
                {
                    player.SoundLocation = file;
                }
                player.LoadAsync();
                
                return true;
            }
            catch (Exception ec)
            {
                //Debug.WriteLine(ec);
                LogHelper.Warning(ec.ToString());
                return false;
            }
        }
        /// <summary>
        /// 加载指定音效文件
        /// </summary>
        /// <param name="path">音效文件路径，为空时加载默认音效</param>
        public void LoadSound(string path = null)
        {
            bool isDefault = false;
            if (string.IsNullOrEmpty(path))
            {
                isDefault = true;
                path = "/ProjectEye;component/Resources/relentless.wav";
            }
            bool loadResult = Load(path, isDefault);
            //加载音效失败
            if (!loadResult && !isDefault)
            {
                //加载自定义音效失败
                //尝试加载默认音效
                LoadSound();
            }
        }
        #endregion

        #region 测试外部音效
        /// <summary>
        /// 测试外部音效
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool Test(string file)
        {

            if (Load(file, false))
            {
                if (Play())
                {
                    return true;
                }
            }
            return false;

        }
        #endregion
    }
}
