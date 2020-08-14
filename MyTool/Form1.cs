using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Web;
using System.Text.RegularExpressions;
using static MyTool.jsonToCclass;
using System.Data.Sql;
using Excel = Microsoft.Office.Interop.Excel;
using ExcelDataReader;
using System.Reflection;
using System.Threading;

namespace MyTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region HttpUitls类
        public class HttpUitls
        {
            public static string Get(string Url)
            {
                //System.GC.Collect();
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Referer = Referer;
                byte[] bytes = Encoding.UTF8.GetBytes(Data);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Stream myResponseStream = request.GetRequestStream();
                myResponseStream.Write(bytes, 0, bytes.Length);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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

        }
        #endregion
        #region json读写示例
        public void jsonreadsample()
        {
            string jsonstr = "{\"Name\" : \"Jack\", \"Age\" : 34, \"Colleagues\" : [{\"Name\" : \"Tom\" , \"Age\":44},{\"Name\" : \"Abel\",\"Age\":29}] }";
            //将json转换为JObject
            JObject jo = JObject.Parse(jsonstr);
            JToken ageToken = jo["Name"]; //获取该员工的姓名
            Console.WriteLine(ageToken.ToString());

            //获取该员工同事所有姓名（读取json数组）
            var names = from staff in jo["Colleagues"].Children() select (string)staff["Name"];
            foreach (var name in names)
                Console.WriteLine(name);
        }
        public void jsonRead()
        {
            //string ip = textBox_IP.Text.Trim();
            //string getIpUrl = "http://apidata.chinaz.com/CallAPI/ip?key=e4727887f69a45b99877d48ab6259c16&ip=" + ip;
            //string getJson = HttpUitls.Get(getIpUrl);
            //richTextBox1.Text = getJson;
            //JObject jo = JObject.Parse(getJson);
            //var result = jo["Result"];
            //var city = result["City"];
            //MessageBox.Show(city.ToString());
        }
        public class App
        {
            public string app_id { get; set; }
            public string app_key { get; set; }

        }
        public void jsonWrite()
        {
        
        string tcUrl = "http://jpebook.ly.com/openapidoc/mock/5db25b80df1ddd0013cafc66";
            App app = new App()
            {
                app_id = "mock",
                app_key = "mock"
            };
            string date = JObject.FromObject(app).ToString();
            Console.WriteLine("date:" + date);
            string toJson = HttpUitls.Post(tcUrl, date, "");
            Console.WriteLine("toJson:" + toJson);
       
    }
        #endregion
        private string getIp(string key, string ip)
        {
            string serviceAddress = "https://apidata.chinaz.com/CallAPI/Ip?" + "key=" + key + "&ip=" + ip;
            Console.WriteLine(serviceAddress);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            
            return retString;

        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            string jsonDate = richTextBox1.Text.Trim();
            string wholeDate = GetTime(jsonDate);
            string MMddDate = GetTime(0,jsonDate);
            richTextBox2.Text = wholeDate;
            richTextBox3.Text = MMddDate;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            string orderNum = textBox_orderNum.Text.Trim();
            string responseLock = string.Empty;
            string response = string.Empty;
            if (richTextBox_encryption.Text.Trim() != "")
            {
                responseLock = richTextBox_encryption.Text.Trim().Replace("\"", ""); //替换前后的双引号""
                string responseDecry = Decompress(responseLock);
                richTextBox_decrypt.Text = responseDecry;
               //txtSaleType.Text = Regex.Match(response, "(?<=\"SaleType\":\")\\w + (?= \")").Value;
            }
            else
            {
                responseLock = MyTool.HttpUitls.getOrderIssue(orderNum);
                JObject responseJsonEncry = JObject.Parse(responseLock);  //string --> JObject
                string responseEncry = responseJsonEncry["result"].ToString();  //取返回的加密Json数据里的result值
                string responseDecry = Decompress(responseEncry);  //JObject-->string  解密result加密字符串,返回Json格式字符串 
                richTextBox_decrypt.Text = responseDecry;
            }
            if(richTextBox_decrypt.Text.Trim() != "")
            {
                response = richTextBox_decrypt.Text.Trim();
                txtPolicyID.Text = Regex.Match(response, "(?<=\"PolicyID\":)\\d+").Value;
                txtSaleType.Text = Regex.Match(response, "(?<=\"SaleType\":\")\\w+(?=\")").Value;
                var flightReg = Regex.Matches(response, "(?<=\"DPort\":\"|\"APort\":\"|\"Flight\":\"|\"SubClass\":\")\\w+(?=\")");
                string flightTime = Regex.Match(response, "(?<=\"TakeOffTime\":\")\\S{28}").Value;
                flightTime = GetTime(0, flightTime);
                textBox_flightInfo.Text = $"{flightReg[1]}{flightReg[2]}  {flightReg[0]}  {flightReg[3]}  {flightTime}";

            }



        }
        private void button3_Click(object sender, EventArgs e)
        {
          

        }
        #region 利用Gzip解压字符串
        /// <summary>
        /// 利用Gzip解压字符串       
        /// </summary>        
        /// <param name=”str”>需要解压的字符串</param>       
        /// <returns>Gzip解压后的字符串</returns>
        public static string Decompress(string str)
        {
            try
            {
                return Encoding.UTF8.GetString(Decompress(Convert.FromBase64String(str)));
            }
            catch (Exception ex)
            {
                //logger.Error(“GzipError:”, ex.ToString());
                MessageBox.Show(ex.ToString());
                Console.WriteLine("解密失败");
            }
            return null;
        }
        
        /// <summary>
        /// 利用Gzip解压字节数组       
        /// </summary>
        private static byte[] Decompress(Byte[] bytes)
        {
            using (MemoryStream tempMs = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    GZipStream Decompress = new GZipStream(ms, CompressionMode.Decompress);
                    Decompress.CopyTo(tempMs);
                    Decompress.Close();
                    return tempMs.ToArray();
                }
            }
        }
        public static string MD5(string str, int code)
        {
            if (code == 16) //16位MD5加密（取32位加密的9~25字符） 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
            }
            if (code == 32) //32位加密 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
            }
            return "00000000000000000000000000000000";
        }


        #endregion
        #region 转时间格式
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public string GetTime(string timeStamp)
        {
            //处理字符串,截取括号内的数字
            var strStamp = Regex.Matches(timeStamp, @"(?<=\()((?<gp>\()|(?<-gp>\))|[^()]+)*(?(gp)(?!))").Cast<Match>().Select(t => t.Value).ToArray()[0].ToString();
            //处理字符串获取+号前面的数字
            var str = Convert.ToInt64(strStamp.Substring(0, strStamp.IndexOf("+")));
            long timeTricks = new DateTime(1970, 1, 1).Ticks + str * 10000 + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours * 3600 * (long)10000000;
            return new DateTime(timeTricks).ToString("yyyy-MM-dd HH:mm:ss");

        }
        public string GetTime(int num, string timeStamp)
        {
            //处理字符串,截取括号内的数字
            var strStamp = Regex.Matches(timeStamp, @"(?<=\()((?<gp>\()|(?<-gp>\))|[^()]+)*(?(gp)(?!))").Cast<Match>().Select(t => t.Value).ToArray()[0].ToString();
            //处理字符串获取+号前面的数字
            var str = Convert.ToInt64(strStamp.Substring(0, strStamp.IndexOf("+")));
            long timeTricks = new DateTime(1970, 1, 1).Ticks + str * 10000 + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours * 3600 * (long)10000000;
            return new DateTime(timeTricks).ToString("MM-dd");

        }
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        #endregion

        public void 获取携程改期数据()
        {
            string[][] arr = new string[5][];

            arr[0] = new string[] { "21440804", "贾秀敏", "KXDSJD", "2480523407" };
            arr[1] = new string[] { "21440805", "蔡少林", "KS4585", "2480504087" };
            arr[2] = new string[] { "21440805", "林伟杰", "KS4585", "2480504088" };
            arr[3] = new string[] { "21440805", "蔡少林1", "KS4585", "2480504089" };
            arr[4] = new string[] { "21440805", "林伟杰1", "KS4585", "2480504090" };
            string[] idArr = new string[5];
            for (int i = 0; i < 5; i++)
            {
                idArr[i] = arr[i][0];
            }
            var disIdArr = idArr.Distinct().ToArray();
            string[] exchangeInfoArr = new string[disIdArr.Length];


            for (int i = 0; i < disIdArr.Length; i++)
            {
                string id = "";
                string pnr = "";
                List<string> name = new List<string>();
                List<string> tkt = new List<string>();
                for (int j = 0; j < idArr.Length; j++)
                {

                    if (disIdArr[i] == arr[j][0])
                    {
                        id = disIdArr[i];
                        name.Add(arr[j][1]);
                        pnr = arr[j][2];
                        tkt.Add(arr[j][3]);
                        exchangeInfoArr[i] = $"{id}|{string.Join(",", name.ToArray())}|{pnr}|{string.Join(",", tkt.ToArray())}";
                    }
                }

            }
            for (int i = 0; i < exchangeInfoArr.Length; i++)
            {
                Console.WriteLine(exchangeInfoArr[i]);
            }
        }
        public void 二维数组测试()
        {
            
            int[][] twoArr = new int[2][];
            twoArr[0] = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            twoArr[1] = new int[] { 1, 1, 1, 1, 1, 1, 1, 1 };
            Console.WriteLine(twoArr.Length);
            Console.WriteLine(twoArr[0].Length);
        }
    
        public void 时间比较()
        {
            var currentTime1 = DateTime.Now.TimeOfDay;
            var timeA = DateTime.Parse("10:00").TimeOfDay;
        }
        public enum SubClass
        {
            F=100, A, J, C, D, I, O, Y, H, B, M, L, K, N, Q, V, T, R, U, Z
        }
        public static Enum GetEnumFromStr<T>(string name)
        {
            return (Enum)Enum.Parse(typeof(T), name);
        }
        public void ACNY()
        {

            string triRes = @"
01 OI ZZG22225 FARE:CNY390.00 CN:CNY50.00 TEXEMPTYQ TOTAL:440.00 OB AC
NY:195.00    RFC:01                                                             
02 OI ZPR200 FARE:CNY470.00 CN:CNY50.00 TEXEMPTYQ TOTAL:520.00 OB ACNY
:275.00  RFC:02  
 ";
            triRes = Regex.Replace(triRes, "\r\n", ""); //去除换行符
            triRes = triRes.Replace("\r", "").Replace("\n", "");
            var ACNY = Regex.Matches(triRes, @"(?<=ACNY:)\d+");
            MatchCollection OB;
            if (triRes.Contains("OB:"))
            {
                OB = Regex.Matches(triRes, @"(?<=OB:CNY)\d+");
            }
            else
            {
                OB = Regex.Matches(triRes, @"(?<=OB)");
            }


            int[] ACNYArr = new int[ACNY.Count];
            int[] OBArr = new int[ACNY.Count];
            int[] SCNYArr = new int[ACNY.Count];
            for (int i = 0; i < ACNY.Count; i++)
            {
                ACNYArr[i] = Convert.ToInt16(ACNY[i].Value);  //改期总价
                if (OB[i].Value == "")
                {
                    OBArr[i] = 0;
                }
                else
                {
                    OBArr[i] = Convert.ToInt16(OB[i].Value);      //改期费         
                }
                SCNYArr[i] = ACNYArr[i] - OBArr[i];     //差价
            }
            #region 找到ACNY里最大值和相应的序号
            int CorrectACNY, CorrectOB, CorrectSCNY;
            CorrectACNY = ACNYArr.Max();
            int RFCindex = -1;
            for (int i = ACNY.Count - 1; i >= 0; i--)
            {
                if (CorrectACNY == ACNYArr[i])
                {
                    RFCindex = i;
                    break;
                }
            }
            CorrectOB = OBArr[RFCindex];
            CorrectSCNY = SCNYArr[RFCindex];
            #endregion

        }
        /// <summary>
        /// 后台获取TKNE票号信息
        /// </summary>
        /// <param name="pnrContent"></param>
        /// <param name="pnrNum"></param>
        /// <returns></returns>
        public static string getTkneInf(string pnrContent, int pnrNum)
        {
            string[] pnrConArr = pnrContent.Split('\n');
            string[] tkneArr = new string[pnrNum];
            string[] pnrTktArr = new string[pnrNum];
            for (int i = 0, j = 0; i < pnrConArr.Length; i++)  //查找编码关联项行
            {
                int tkne = pnrConArr[i].IndexOf("TKNE");
                if (tkne > -1)
                {
                    if (pnrConArr[i].IndexOf("INF731") > -1)  //关联项有婴儿票号
                    {
                        return "PNR HAVE INF";
                    }
                    tkneArr[j] = pnrConArr[i].Trim();
                    int f1 = tkneArr[j].IndexOf("+");   //查找关联项行是否有 + 或者 - , 有的话就删除掉, 一般是最后一位
                    int f2 = tkneArr[j].IndexOf("-");
                    if (f1 > -1 || f2 > -1)
                    {
                        tkneArr[j] = tkneArr[j].Substring(0, tkneArr[j].Length - 1).Trim();
                    }
                    pnrTktArr[j] = tkneArr[j].Substring(tkneArr[j].Length - 18);     //从右取后18位
                    pnrTktArr[j] = pnrTktArr[j].Insert(3, "-");                 //在731后添加横杠-  
                    j++;
                }

            }
            //pnrTktArr = {731-2480412770/1/P2 ,731-2480412769/1/P1}
            // pnrTktArr = { "731-2480412770/1/P2", "731-2480412769/1/P1", "731-2480412771/1/P3" };
            for (int i = 0; i < pnrTktArr.GetLength(0); i++)
            {
                for (int j = 1; j < pnrTktArr.GetLength(0) - i; j++)
                {
                    if (pnrTktArr[j].CompareTo(pnrTktArr[j - 1]) < 0)
                    {
                        string t = pnrTktArr[j];
                        pnrTktArr[j] = pnrTktArr[j - 1];
                        pnrTktArr[j - 1] = t;
                    }
                }
            }
            for (int i = 0; i < pnrTktArr.Length; i++)
            {
                pnrTktArr[i] = pnrTktArr[i].Substring(0, pnrTktArr[i].Length - 5);
                pnrTktArr[i] = pnrTktArr[i].Replace("731-", "");
            }
            string pnrTkt = string.Join(",", pnrTktArr);
            return pnrTkt;
        }
        

       

        private void button5_Click(object sender, EventArgs e)
        {
            string oldjson = richTextBox_oldjson.Text.Trim();
            string[] oldjsonArr = oldjson.Split('#');
            string[] newjsonArr = new string[oldjsonArr.Length];
            for (int i = 0; i < oldjsonArr.Length; i++)
            {
                newjsonArr[i] = Regex.Match(oldjsonArr[i], "(?<=\"detail\":\\[).+(?=]},\"isEntOperator\":false})").Value;
            }
            string newjson = string.Join(",",newjsonArr);
            newjson = "[" + newjson + "]";
            richTextBox_newjson.Text = newjson;



        }
        private string strFileName;
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            OpenFileDialog openFileDialog2 = openFileDialog;
            openFileDialog2.Filter = "Excel files (*.xls)|*.xls|Excel07 files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv";
            openFileDialog2.FilterIndex = 1;
            openFileDialog2.InitialDirectory = "C:\\";
            openFileDialog2.Title = "打开文件";
            openFileDialog2.Multiselect = false;
            openFileDialog2.ReadOnlyChecked = false;
            openFileDialog2.ShowReadOnly = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                strFileName = openFileDialog.FileName;
                txtFile.Text = this.strFileName;
            }
        }
        DataTable DtTable = new DataTable(); //创建空表
        public  void upLoad()
        {
            this.Invoke(new EventHandler(delegate
            {
                btnUpload.Enabled = false;
            }));
            progressBar_upload.Value = 0; //清空进度条
            DtTable.Rows.Clear();
            DtTable.Columns.Clear();
            DataTable originDT = EXCEL.ImportFromExcel(this.txtFile.Text.Trim());
            List<string> OrderIDList = new List<string>();
            List<string> OrderTypeList = new List<string>();
            int UsedRows = originDT.Rows.Count;
            string responseLock = string.Empty;
            JObject responseJsonEncry;
            string responseEncry = string.Empty;
            string responseDecry = string.Empty;
            string orderType = string.Empty;
            this.Invoke(new EventHandler(delegate
            {
                progressBar_upload.Maximum = UsedRows;
            }));
            for (int i = 1; i < UsedRows; i++)
            {
                this.Invoke(new EventHandler(delegate
                {
                    progressBar_upload.Value = i + 1;
                }));
                OrderIDList.Add(originDT.Rows[i][0].ToString());
                responseLock = MyTool.HttpUitls.getOrderIssue(OrderIDList[i-1]);
                responseJsonEncry = JObject.Parse(responseLock);  //string --> JObject
                responseEncry = responseJsonEncry["result"].ToString();  //取返回的加密Json数据里的result值
                responseDecry = Decompress(responseEncry);  //JObject-->string  解密result加密字符串,返回Json格式字符串 
                orderType = Regex.Match(responseDecry, "(?<=\"SaleType\":\")\\w+(?=\")").Value;
                Console.WriteLine(orderType);
                OrderTypeList.Add(orderType);
                Thread.Sleep(300);
            }
            //foreach (var item in OrderTypeList)
            //{
            //    Console.WriteLine(item);
            //}

            //DataColumn newDC = new DataColumn();//创建空列
            //DtTable.Columns.Add(newDC);//空列加进空表
            //DtTable.Columns.Add("订单号", typeof(String));
            //DtTable.Columns.Add("订单来源", typeof(String));
            DataColumn column1 = new DataColumn("订单号", typeof(String));
            DataColumn column2 = new DataColumn("订单来源", typeof(String));
            DtTable.Columns.Add(column1);
            DtTable.Columns.Add(column2);


            DataRow dr = DtTable.NewRow();
            for (int i = 0; i < OrderIDList.Count; i++)
            {
                dr["订单号"] = OrderIDList[i];
                dr["订单来源"] = OrderTypeList[i];
                DtTable.Rows.Add(dr.ItemArray);
            }
           
            this.Invoke(new EventHandler(delegate
            {
                btnUpload.Enabled = true;
            }));




        }
        public void export()
        {
            this.Invoke(new EventHandler(delegate
            {
                btnDownload.Enabled = false;
            }));
            EXCEL.ExportToExcel(DtTable);
            this.Invoke(new EventHandler(delegate
            {
                btnDownload.Enabled = true;
            }));
        }
        static public Thread upLoadThread;
        static public Thread downloadThread;
        private void btnUpload_Click(object sender, EventArgs e)
        {
           
            upLoadThread = new Thread(upLoad);
            upLoadThread.IsBackground = true;
            upLoadThread.SetApartmentState(ApartmentState.STA);
            upLoadThread.Start();
            
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
           
            downloadThread = new Thread(export);
            downloadThread.IsBackground = true;
            downloadThread.SetApartmentState(ApartmentState.STA);
            downloadThread.Start();


        }
    }
}
