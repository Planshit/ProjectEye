using ProjectEye.Core.Enums;
using ProjectEye.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Media;
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
        private Dictionary<SoundType, SoundPlayer> players;
        public SoundService(ConfigService config)
        {
            players = new Dictionary<SoundType, SoundPlayer>();
            //player.LoadCompleted += Player_LoadCompleted;
            this.config = config;
        }

        private void Player_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            (sender as SoundPlayer).Dispose();
        }

        public void Init()
        {
            players.Add(SoundType.RestOverSound, new SoundPlayer());
            players.Add(SoundType.TomatoWorkStartSound, new SoundPlayer());
            players.Add(SoundType.TomatoWorkEndSound, new SoundPlayer());
            players.Add(SoundType.Other, new SoundPlayer());

            players[SoundType.RestOverSound].LoadCompleted += Player_LoadCompleted;
            players[SoundType.TomatoWorkStartSound].LoadCompleted += Player_LoadCompleted;
            players[SoundType.TomatoWorkEndSound].LoadCompleted += Player_LoadCompleted;
            players[SoundType.Other].LoadCompleted += Player_LoadCompleted;

            LoadConfigSound();

            config.Changed += Config_Changed;
        }

        private void Config_Changed(object sender, EventArgs e)
        {
            var oldOptions = sender as OptionsModel;
            if (oldOptions.General.SoundPath != config.options.General.SoundPath ||
                oldOptions.Tomato.WorkStartSoundPath != config.options.Tomato.WorkStartSoundPath ||
                oldOptions.Tomato.WorkEndSoundPath != config.options.Tomato.WorkEndSoundPath)
            {
                LoadConfigSound();
            }
        }

        /// <summary>
        /// 加载用户配置的音效
        /// </summary>
        private void LoadConfigSound()
        {
            string restOverPath = null, tomatoWorkStartPath = null, tomatoWorkEndPath = null;
            if (!string.IsNullOrEmpty(config.options.General.SoundPath))
            {
                restOverPath = config.options.General.SoundPath;
            }
            if (!string.IsNullOrEmpty(config.options.Tomato.WorkStartSoundPath))
            {
                tomatoWorkStartPath = config.options.Tomato.WorkStartSoundPath;
            }
            if (!string.IsNullOrEmpty(config.options.Tomato.WorkEndSoundPath))
            {
                tomatoWorkEndPath = config.options.Tomato.WorkEndSoundPath;
            }
            //加载休息结束提示音
            LoadSound(SoundType.RestOverSound, restOverPath);
            //加载番茄时钟工作开始提示音
            LoadSound(SoundType.TomatoWorkStartSound, tomatoWorkStartPath);
            //加载番茄时钟工作结束提示音
            LoadSound(SoundType.TomatoWorkEndSound, tomatoWorkEndPath);
        }
        #region 播放音效
        /// <summary>
        /// 播放音效,默认休息结束音效
        /// </summary>
        public bool Play(SoundType soundType = SoundType.RestOverSound)
        {
            var player = players[soundType];
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
        public bool Load(SoundType soundType, string file, bool resource = true)
        {
            try
            {
                var player = players[soundType];
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
        public void LoadSound(SoundType soundType = SoundType.RestOverSound, string path = null)
        {
            bool isDefault = false;
            if (string.IsNullOrEmpty(path))
            {
                isDefault = true;
                path = "/ProjectEye;component/Resources/relentless.wav";
            }
            bool loadResult = Load(soundType, path, isDefault);
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

            if (Load(SoundType.Other, file, false))
            {
                if (Play(SoundType.Other))
                {
                    return true;
                }
            }
            return false;

        }
        #endregion
    }
}
