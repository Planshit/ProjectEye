using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectEye.Core
{
    public class XmlExtensions
    {
        private readonly string file;
        public XmlExtensions(string file)
        {
            this.file = file;
        }
        public object ToModel(Type type)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                XmlReader xmlReader = XmlReader.Create(file);
                if (serializer.CanDeserialize(xmlReader))
                {
                    var des = serializer.Deserialize(xmlReader);
                    xmlReader.Dispose();
                    return des;
                }
                else
                {
                    xmlReader.Dispose();
                    return null;
                }
            }
            catch (Exception e)
            {
                LogHelper.Warning(e.ToString());
                return null;
            }

        }
        public bool Save(object data)
        {
            try
            {
                string dir = Path.GetDirectoryName(file);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                TextWriter writer = new StreamWriter(file);
                serializer.Serialize(writer, data);
                writer.Close();
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Warning(e.ToString());
                return false;
            }
        }
    }
}
