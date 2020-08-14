using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MyTool
{
    class XML
    {
        //public GetWeb PostXml(string url, Dictionary<string, string> dic)
        //{
        //    System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
        //    System.Net.ServicePointManager.DefaultConnectionLimit = 1000;

        //    GetWeb MyGetWeb; MyGetWeb.result = ""; MyGetWeb.isError = false;

        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    req.Method = "POST";
        //    req.ContentType = "application/x-www-form-urlencoded";
        //    req.ServicePoint.Expect100Continue = false;
        //    req.KeepAlive = false;
        //    #region 添加Post 参数
        //    StringBuilder builder = new StringBuilder();
        //    int i = 0;
        //    foreach (var item in dic)
        //    {
        //        if (i > 0)

        //            builder.Append("&");
        //        builder.AppendFormat("{0}={1}", item.Key, item.Value);
        //        i++;

        //    }
        //    #endregion

        //    byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
        //    req.ContentLength = data.Length;
        //    //using (Stream reqStream = req.GetRequestStream())
        //    //{
        //    //    reqStream.Write(data, 0, data.Length);
        //    //    reqStream.Close();
        //    //}

        //    Stream reqStream = null;
        //    HttpWebResponse resp;
        //    try
        //    {
        //        reqStream = req.GetRequestStream();
        //        reqStream.Write(data, 0, data.Length);
        //        reqStream.Close();
        //        resp = (HttpWebResponse)req.GetResponse();
        //    }
        //    catch (WebException ex)
        //    {
        //        WriteLog("PostXml_ERROR", ex.Message);
        //        MyGetWeb.isError = true;
        //        MyGetWeb.result = ex.Message;

        //        return MyGetWeb;
        //    }
        //    finally
        //    {
        //        if (reqStream != null)
        //        {
        //            ((IDisposable)reqStream).Dispose();
        //        }



        //    }



        //    Stream stream = resp.GetResponseStream();
        //    //获取响应内容
        //    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        //    {
        //        MyGetWeb.result = reader.ReadToEnd();
        //    }
        //    return MyGetWeb;
        //}

        //Dictionary<string, string> dic = new Dictionary<string, string>
        //    {
        //        { "method", "CompositeTicket" },
        //        { "xml", DATA },
        //        { "sign", "9DE57D1533CD84ADDDCCBE3B1D520311" }
        //    };

        //private string bqyOrderList()
        //{

        //    //获取出票列表
        //    string waitTicket = "<WaitTicket><Account>XMN030</Account><Password>RrIWaDOx2ow=</Password><Gettime></Gettime></WaitTicket>";
        //    //Console.WriteLine("BQY订单列表刷新----TIME:" + DateTime.Now);

        //    Dictionary<string, string> dic = new Dictionary<string, string>();

        //    dic.Add("method", "WaitTicket");
        //    dic.Add("xml", waitTicket);
        //    dic.Add("sign", "408C232567142C7A547FDA838BABC006");

        //    GetWeb getList = PostXml("http://www.8000yi.com/Interface/SupplierInterface.aspx", dic);
        //    // Console.WriteLine("getList返回值:" + result);
        //    if (getList.isError)
        //    {
        //        WriteLog("A BQY_List_ERROR", getList.result);
        //        return "A BQY_List_ERROR";
        //    }

        //    XmlDocument doc; XmlNodeList GetMessage;

        //    doc = new XmlDocument();
        //    doc.LoadXml(getList.result);
        //    GetMessage = doc.SelectNodes("/WaitTicket");
            


        //    string isOrder = GetMessage[0].SelectSingleNode("errorMessage").InnerText;
        //    //Console.WriteLine("BQY isOrder:{0}", isOrder);
        //    if (isOrder != "成功")
        //    {
        //        WriteLog("BQY检查列表错误", "返回值:" + isOrder);
        //        notifyIcon1.ShowBalloonTip(2000, "BQY检查列表错误!", "返回值:" + isOrder, ToolTipIcon.Error);
        //        return "NONE";
        //    }

        //    XmlNodeList List = doc.SelectNodes("/WaitTicket/OrderList/Order");

        //    // Console.WriteLine("ListCount:{0}", List.Count);


        //    if (List.Count == 0)
        //    {
        //        //Console.WriteLine("BQY订单列表为空! TIME:" + DateTime.Now);
        //        return "NONE";
        //    }
        //    else
        //    {

        //        return "~OK:" + getList.result;
        //    }


        //}


    }
}
