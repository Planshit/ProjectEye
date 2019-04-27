using ProjectEye.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectEye.Core.Service
{
    public class ConfigService : IService
    {
        private readonly string configPath;
        private readonly XmlExtensions xmlExtensions;
        public OptionsModel options { get; set; }
        /// <summary>
        /// 配置文件被修改时发生
        /// </summary>
        public event EventHandler Changed;
        public ConfigService()
        {
            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
            xmlExtensions = new XmlExtensions(configPath);
            Init();
        }
        public void Init()
        {
            if (File.Exists(configPath))
            {
                var obj = xmlExtensions.ToModel(typeof(OptionsModel));
                if (obj != null)
                {
                    options = obj as OptionsModel;
                }
            }
            else
            {
                CreateDefaultConfig();
            }
            //每次启动都把不提醒重置
            options.general.noreset = false;
        }
        public bool Save()
        {
            if (options != null)
            {
                Changed?.Invoke(options, null);

                return xmlExtensions.Save(options);
            }
            return false;
        }
        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        private void CreateDefaultConfig()
        {
            options = new OptionsModel();
            options.general = new GeneralModel();
            options.general.data = false;
            options.general.noreset = false;
            options.general.sound = true;
            options.general.startup = false;
            options.general.leavelistener = false;

            xmlExtensions.Save(options);
        }


    }
}
