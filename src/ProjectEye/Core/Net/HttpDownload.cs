using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace ProjectEye.Core.Net
{
    /// <summary>
    /// http文件下载
    /// </summary>
    public class HttpDownload
    {
        /// <summary>
        /// 文件网络路径
        /// </summary>
        private string httpUrl;
        private string savePath;
        private Thread thread;
        /// <summary>
        /// 进度更新时发生
        /// </summary>
        public event HttpDownloaderEventHandler ProcessUpdateEvent;
        /// <summary>
        /// 下载完成时发生
        /// </summary>
        public event HttpDownloaderEventHandler CompleteEvent;
        /// <summary>
        /// 发生错误时发生
        /// </summary>
        public event HttpDownloaderEventHandler ErrorEvent;
        /// <summary>
        /// 下载开始时发生
        /// </summary>
        public event HttpDownloaderEventHandler StartEvent;

        public delegate void HttpDownloaderEventHandler(object sender, object value);
        public HttpDownload(string httpUrl, string savePath)
        {
            this.httpUrl = httpUrl;
            this.savePath = savePath;
            thread = new Thread(downloadThread);
            thread.IsBackground = true;

        }

        private void downloadThread()
        {

            try
            {
                Uri URL = new Uri(httpUrl);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.Timeout = 120 * 1000;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();


                long totalBytes = httpWebResponse.ContentLength;
                //更新文件大小
                StartEvent?.Invoke(this, totalBytes);
                Stream st = httpWebResponse.GetResponseStream();
                Stream so = new FileStream(savePath, FileMode.Create);

                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {

                    totalDownloadedByte = osize + totalDownloadedByte;
                    so.Write(by, 0, osize);

                    osize = st.Read(by, 0, (int)by.Length);

                    //进度计算
                    double process = double.Parse(String.Format("{0:F}",
                         ((double)totalDownloadedByte / (double)totalBytes * 100)));
                    ProcessUpdateEvent?.Invoke(this, process);


                }
                //关闭资源
                httpWebResponse.Close();
                so.Close();
                st.Close();
                CompleteEvent?.Invoke(this, null);
            }
            catch (Exception ec)
            {
                //下载发生异常
                ErrorEvent?.Invoke(this, ec.Message);
            }
        }

        public void Start()
        {
            string dir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dir))
            {
                //如果目录不存在则创建
                Directory.CreateDirectory(dir);
            }
            thread.Start();
        }

    }
}
