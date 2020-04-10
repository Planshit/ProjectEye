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
            //Init();
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
            options.General = new GeneralModel();
            options.General.Data = false;
            options.General.Noreset = false;
            options.General.Sound = true;
            options.General.Startup = false;
            options.General.LeaveListener = true;
            options.General.WarnTime = 20;

            options.Style = new StyleModel();
            options.Style.Theme = systemResources.Themes[1];
            options.Style.TipContent = "您已持续用眼{t}分钟，休息一会吧！请将注意力集中在至少6米远的地方20秒！";

            options.KeyboardShortcuts = new KeyboardShortcutModel();

            options.Behavior = new BehaviorModel();
            xmlExtensions.Save(options);
        }
        private void CheckOptions()
        {
            System.Reflection.PropertyInfo[] properties = options.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(options, null);
                if (value == null)
                {
                    //配置项不存在时创建

                    //var constructorInfoObj = item.PropertyType.GetConstructors()[0];
                    //var constructorParameters = constructorInfoObj.GetParameters();
                    //int constructorParametersLength = constructorParameters.Length;
                    Type[] types = new Type[0];
                    object[] objs = new object[0];
                    //for (int i = 0; i < constructorParametersLength; i++)
                    //{
                    //    string typeFullName = constructorParameters[i].ParameterType.FullName;
                    //    Type t = Type.GetType(typeFullName);
                    //    types[i] = t;
                    //}
                    ConstructorInfo ctor = item.PropertyType.GetConstructor(types);
                    object instance = ctor.Invoke(objs);
                    item.SetValue(options, instance);
                }
                Debug.WriteLine(string.Format("{0}:{1},", name, value));

                //if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                //{
                //    tStr += string.Format("{0}:{1},", name, value);
                //}
                //else
                //{
                //    getProperties(value);
                //}
            }
        }

    }
}
