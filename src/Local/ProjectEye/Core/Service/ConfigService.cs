using Newtonsoft.Json;
using ProjectEye.Core.Models.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectEye.Core.Service
{
    /// <summary>
    /// 配置文件  Service
    /// </summary>
    public class ConfigService : IService
    {
        private readonly string configPath;
        private readonly XmlExtensions xmlExtensions;
        private readonly SystemResourcesService systemResources;
        //存放文件夹
        private readonly string dir = "Data";
        /// <summary>
        /// 未更改的配置
        /// </summary>
        private OptionsModel oldOptions_;
        public OptionsModel options { get; set; }
        /// <summary>
        /// 配置文件被修改时发生
        /// </summary>
        public event EventHandler Changed;
        public ConfigService(SystemResourcesService systemResources)
        {
            this.systemResources = systemResources;
            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                dir,
                "config.xml");
            xmlExtensions = new XmlExtensions(configPath);
            oldOptions_ = new OptionsModel();
        }
        public void Init()
        {
            if (File.Exists(configPath))
            {
                var obj = xmlExtensions.ToModel(typeof(OptionsModel));
                if (obj != null)
                {
                    options = obj as OptionsModel;
                    SaveOldOptions();
                }
                else
                {
                    CreateDefaultConfig();
                }
            }
            else
            {
                CreateDefaultConfig();
            }
            CheckOptions();
            //每次启动都把不提醒重置
            options.General.Noreset = false;
        }
        public bool Save()
        {
            if (options != null)
            {
                OnChanged();
                SaveOldOptions();
                return xmlExtensions.Save(options);
            }
            return false;
        }

        /// <summary>
        /// 保存旧选项数据
        /// </summary>
        public void SaveOldOptions()
        {
            string optionsStr = JsonConvert.SerializeObject(options);
            oldOptions_ = JsonConvert.DeserializeObject<OptionsModel>(optionsStr);
        }
        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        private void CreateDefaultConfig()
        {
            options = new OptionsModel();
            options.General = new GeneralModel();
            options.General.Data = false;
            options.General.Noreset = false;
            options.General.Sound = true;
            options.General.Startup = false;
            options.General.LeaveListener = true;
            options.General.WarnTime = 20;

            options.Style = new StyleModel();
            options.Style.Theme = systemResources.Themes[0];
            options.Style.TipContent = "您已持续用眼{t}分钟，休息一会吧！请将注意力集中在至少6米远的地方20秒！";
            options.Style.TipWindowAnimation = systemResources.Animations[0];

            options.KeyboardShortcuts = new KeyboardShortcutModel();

            options.Behavior = new BehaviorModel();

            SaveOldOptions();

            xmlExtensions.Save(options);
        }
        private void CheckOptions()
        {
            CheckOptions(options);
            CheckOptions(options.Style);
            CheckOptions(options.Tomato);
            SaveOldOptions();
        }
        private void CheckOptions(object obj)
        {
            System.Reflection.PropertyInfo[] properties = obj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(obj, null);
                if (value == null)
                {
                    //配置项不存在时创建
                    Type[] types = new Type[0];
                    object[] objs = new object[0];

                    ConstructorInfo ctor = item.PropertyType.GetConstructor(types);
                    if (ctor != null)
                    {
                        object instance = ctor.Invoke(objs);
                        item.SetValue(obj, instance);
                    }
                }

                Debug.WriteLine(string.Format("{0}:{1},", name, value));
            }
        }

        public void OnChanged()
        {
            Changed?.Invoke(oldOptions_, null);
        }

    }
}
