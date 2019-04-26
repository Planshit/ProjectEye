using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                    return serializer.Deserialize(xmlReader);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }
        public bool Save(object data)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                TextWriter writer = new StreamWriter(file);
                serializer.Serialize(writer, data);
                writer.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
