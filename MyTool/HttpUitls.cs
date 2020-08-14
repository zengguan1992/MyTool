using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyTool
{
    class HttpUitls
    {
        #region HttpUitls类
       
            public static string Get(string Url)
            {
                System.GC.Collect();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url); //创建Web访问对象
                request.Proxy = null;
                request.KeepAlive = false;
                request.Method = "GET";
                request.ContentType = "application/json; charset=UTF-8";
                request.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();  //通过Web访问对象获取响应内容
                Stream myResponseStream = response.GetResponseStream(); //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
                string retString = myStreamReader.ReadToEnd();  //利用StreamReader就可以从响应内容从头读到尾

                myStreamReader.Close();
                myResponseStream.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }

                return retString;
            }

            public static string Post(string Url, string Data, string Referer)
            {
                System.GC.Collect();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Referer = Referer;
                byte[] bytes = Encoding.UTF8.GetBytes(Data);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Stream myResponseStream = request.GetRequestStream();
                myResponseStream.Write(bytes, 0, bytes.Length);
                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (Exception ex)
                {
                    response = null;
                    MessageBox.Show(ex.ToString());
                }

                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                myResponseStream.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                return retString;
            }



        
        #endregion
        public static string getOrderIssue(string orderNum)
        {
            string user = Config.GetConfig("CtripUser");
            string password = Config.GetConfig("CtripPassword");
            string url = Config.GetConfig("IssueBillInfoUrl");
            string data = $"{{\"UserName\":\"{user}\",\"UserPassword\":\"{password}\",\"RequestBody\":{{\"IssueBillID\":{orderNum}}}}}";
            //string postData = JsonConvert.SerializeObject(data);
            string response = HttpUitls.Post(url, data, "");
            return response;
        }

     
    }
}
