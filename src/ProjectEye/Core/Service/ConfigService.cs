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
        private XmlDocument xmlDocument;
        public ConfigService()
        {
            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
            xmlDocument = new XmlDocument();
            Init();
        }
        public void Init()
        {
            if (File.Exists(configPath))
            {
                xmlDocument.Load(configPath);
            }
            else
            {
                CreateDefaultConfig();
                xmlDocument.Load(configPath);
            }
            //XmlSerializer
        }
        public string Get(string name)
        {

            var nodes = name.Split('.');

            XmlElement element = xmlDocument.DocumentElement;
            foreach (var nodeName in nodes)
            {
                element = element[nodeName];
            }
            return element.InnerText;
        }
        /// <summary>
        /// 创建默认配置文件
        /// </summary>
        private void CreateDefaultConfig()
        {
            XmlElement rootElement = xmlDocument.CreateElement("config");

            XmlElement generalElement = xmlDocument.CreateElement("general");

            //开机启动
            XmlElement startupElement = xmlDocument.CreateElement("startup");
            startupElement.InnerText = "true";

            //不休息
            XmlElement noresetElement = xmlDocument.CreateElement("noreset");
            noresetElement.InnerText = "false";

            //统计数据
            XmlElement dataElement = xmlDocument.CreateElement("data");
            dataElement.InnerText = "false";

            //提示音
            XmlElement soundElement = xmlDocument.CreateElement("sound");
            soundElement.InnerText = "true";

            generalElement.AppendChild(startupElement);
            generalElement.AppendChild(noresetElement);
            generalElement.AppendChild(dataElement);
            generalElement.AppendChild(soundElement);

            rootElement.AppendChild(generalElement);
            xmlDocument.AppendChild(rootElement);
            xmlDocument.Save(configPath);
        }
    }
}
