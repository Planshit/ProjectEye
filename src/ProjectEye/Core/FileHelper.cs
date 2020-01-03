using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEye.Core
{
    public class FileHelper
    {
        /// <summary>
        /// 在运行目录写入文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="content">内容</param>
        public static void Write(string path, string contents)
        {
            path = AppDomain.CurrentDomain.BaseDirectory + path;
            Debug.WriteLine(path);
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, contents);
        }
        /// <summary>
        /// 在运行目录中读取一个文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Read(string path)
        {
            path = AppDomain.CurrentDomain.BaseDirectory + path;
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return string.Empty;
        }
        /// <summary>
        /// 在运行目录中判断文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            path = AppDomain.CurrentDomain.BaseDirectory + path;
            return File.Exists(path);

        }
    }
}