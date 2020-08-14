using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyTool
{
    class API
    {

        /// <summary>
        /// Post Json
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public GetWeb PostUrl(string url, string postData)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

            GetWeb MyGetWeb; MyGetWeb.result = ""; MyGetWeb.isError = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            // req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");//定义gzip压缩页面支持
            req.Method = "POST";
            //req.Timeout = 600000;//设置请求超时时间，单位为毫秒
            req.KeepAlive = false;
            req.ContentType = "application/json";
            req.ServicePoint.Expect100Continue = false;
            req.ServicePoint.ConnectionLimit = int.MaxValue;//定义最大连接数
            byte[] data = Encoding.UTF8.GetBytes(postData);

            req.ContentLength = data.Length;


            Stream reqStream = null;
            HttpWebResponse resp;
            try
            {
                reqStream = req.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
                //WriteLog("PostUrl_ERROR", ex.Message);

                MyGetWeb.isError = true;
                MyGetWeb.result = ex.Message;
                return MyGetWeb;
            }

            Stream stream = resp.GetResponseStream();

            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                MyGetWeb.result = reader.ReadToEnd();
                reader.Close();

            }

            if (resp != null)
            {
                resp.Close();
                resp = null;
            }
            if (req != null)
            {
                req.Abort();
                req = null;
            }
            //Console.WriteLine("\r\n>PostUrl:" + postData);
            //Console.WriteLine(">PostTime:" + DateTime.Now);
            //Console.WriteLine(">PostUrl:" + MyGetWeb.result);
            return MyGetWeb;
        }

        public struct GetWeb
        {
            public bool isError;
            public string result;

        }
        // string ret = "{\"UserName\": \"曾冠\",\"UserPassword\": \"6d29c64bad19f6a8bcd3f3e8d4ad228c\",\"RequestBody\": {\"IssueBillID\": 510255121}";
        ///<summary>
        ///post XML string
        ///</summary>
        public GetWeb PostXml(string url, string postData)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;

            GetWeb MyGetWeb; MyGetWeb.result = ""; MyGetWeb.isError = false;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ServicePoint.Expect100Continue = false;
            req.KeepAlive = false;


            byte[] data = Encoding.UTF8.GetBytes(postData);
            req.ContentLength = data.Length;

            Stream reqStream = null;
            HttpWebResponse resp;
            try
            {
                reqStream = req.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException ex)
            {
               // WriteLog("PostXmlStr", ex.Message);
                MyGetWeb.isError = true;
                MyGetWeb.result = ex.Message;
                return MyGetWeb;
            }
            finally
            {
                if (reqStream != null)
                {
                    ((IDisposable)reqStream).Dispose();
                }



            }



            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                MyGetWeb.result = reader.ReadToEnd();
            }
            return MyGetWeb;
        }




    }
}
   

