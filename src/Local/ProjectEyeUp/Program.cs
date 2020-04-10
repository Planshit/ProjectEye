using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEyeUp
{
    /// <summary>
    /// projecteye升级解压程序
    /// </summary>
    class Program
    {
        /// <summary>
        /// 忽略的文件列表
        /// </summary>
        static readonly string[] IgnoreFiles = { "ProjectEyeUp.exe" };

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("请从选项中进行安装更新，按任意键退出！");
                Console.ReadKey();
                return;
            }
            try
            {
                args[0] = args[0].Replace("\"", "");
                args[1] = args[1].Replace("\"", "");

                Console.WriteLine(args[0]);
                Console.WriteLine(args[1]);

                ExtractZipFile(args[0], args[1]);
                string mainExe = Path.Combine(args[1],
                    "ProjectEye.exe");
                Process.Start(mainExe);
                File.Delete(args[0]);
                Console.WriteLine("程序升级完毕，请按任意键退出！");
                Console.ReadKey();
            }
            catch (Exception ec)
            {
                Console.WriteLine("安装更新包过程发生了一个错误：");
                Console.WriteLine(ec);
                Console.ReadKey();

            }

        }

        static void ExtractZipFile(string zipPath, string extractPath)
        {

            // Ensures that the last character on the extraction path
            // is the directory separator char. 
            // Without this, a malicious zip file could try to traverse outside of the expected
            // extraction path.
            if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                extractPath += Path.DirectorySeparatorChar;

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {

                    if (!IsIgnoreFile(entry.FullName))
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
                        if (!IsDir(destinationPath))
                        {
                            if (File.Exists(destinationPath))
                            {
                                File.Delete(destinationPath);
                            }
                            // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                            // are case-insensitive.
                            Console.WriteLine($"抽取：{destinationPath}");
                            if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                                entry.ExtractToFile(destinationPath);
                        }
                        else
                        {
                            //创建目录
                            Directory.CreateDirectory(destinationPath);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 指示文件是否是忽略的
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static bool IsIgnoreFile(string fileName)
        {
            return (Array.IndexOf(IgnoreFiles, fileName) != -1);
        }
        /// <summary>
        /// 指示路径是否是目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static bool IsDir(string path)
        {
            return (path.Last() == '\\');
        }
    }
}
