using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTool
{
    
    class TMCsign
    {

        //System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
        //    return data;


        /**

* 签名.

*

* @param param 请求参数.

* @param secret 安全码.

* @return 签名.

* @throws IOException 异常.

*/

        public static string sign(Dictionary<string, string> param, string secret)
        {

            // 第一步：检查参数是否已经排序

            //string[] keys = param.Keys.ToArray();

            List<string> keys = param.Keys.ToList();

            keys.Sort();

            // 第二步：把所有参数名和参数值串在一起

            StringBuilder query = new StringBuilder();

            query.Append(secret);

            foreach (string key in keys)
            {

                string value = param[key];

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {

                    query.Append(key).Append("=").Append(value).Append("&");

                }

            }

            //query.deleteCharAt(query.length() - 1);
            query.Remove(query.Length - 1, 1);
            query.Append(secret);



            // 第三步：去空格及特殊字符（\r\n\t）后使用MD5加密

            string queryStr = query.ToString().Replace(" ", "").Replace("\\r", "").Replace("\\n", "").Replace("\\t", "");



            byte[] bytes = encryptMD5(queryStr);


            // 第四步：把二进制转化为大写的十六进制(正确签名应该为32大写字符串)

            return byte2hex(bytes);

        }

        /**
        * MD5.
        *
        * @param data
        * 数据.
        * @return MD5.
        * @throws IOException
        * .
*/
        public static byte[] encryptMD5(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            return data;
        }


        /**
        * 二进制转化为大写的十六进制
        *
        * @param bytes
        * 二进制数据.
        * @return 大写的十六进制.
*/
        public static string byte2hex(byte[] bytes)
        {
            StringBuilder sign = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                //string hex = Integer.toHexString(bytes[i] & 0xFF);
                string hex = (bytes[i] & 0xFF).ToString("x");
                if (hex.Length == 1)
                {
                    sign.Append("0");
                }

                sign.Append(hex.ToUpper());
            }

            return sign.ToString();
        }




    }

}
