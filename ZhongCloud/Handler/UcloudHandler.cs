using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ZhongCloud
{
    public class UcloudHandler
    {

        private string PublicKey { set; get; }
        private string PrivateKey { set; get; }

        public UcloudHandler()
        {
            PublicKey = "";
            PrivateKey = "";
        }
        public UcloudHandler(string publicKey=null,string privateKey=null) {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        /// <summary>
        /// 根据参数获取密钥
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetSignature( string action,Dictionary<string,string> dic) {
            if (dic == null)
            {
                return null;
            }
            dic["PublicKey"] = PublicKey;
            dic["Action"] = action;
            return BuildSignature(ConstructionSignature(dic, PrivateKey));
        }

        /// <summary>
        /// 构造密钥生成字符串
        /// </summary>
        /// <param name="dic">参数</param>
        /// <returns></returns>
        private string ConstructionSignature(Dictionary<string,string> dic,string privateKey) {
            
            StringBuilder sb = new StringBuilder();
            //按照字母顺序 升序排序
            string[] keys = dic.Keys.ToArray();
            Array.Sort(keys,string.CompareOrdinal); //需要以ASCII码从小到大排序
            foreach (var key in keys)
            {
                //首字母大写
                string key1= key.Substring(0, 1).ToUpper() + key.Substring(1);
                sb.Append(key1+ dic[key]);
            }
            sb.Append(privateKey);
            return sb.ToString();
            //  return HttpUtility.UrlEncode(sb.ToString(), Encoding.UTF8); 造成严重bug的不必要的操作
        }

        /// <summary>
        /// 生成Url
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        public string BuildUrl(Dictionary<string, string> dic, string apiUrl)
        {
            StringBuilder url = new StringBuilder();
            url.Append(apiUrl);
            url.Append("?");
            foreach (var key in dic.Keys)
            {
                url.Append(key + "=" + RFC3986Encoder.Encode(dic[key]) + "&");
            }
            return url.ToString().Substring(0, url.Length - 1);
        }


        /// <summary>
        /// 生成密钥
        /// </summary>
        /// <param name="paramStr"></param>
        /// <returns></returns>
        private string  BuildSignature(string paramStr) {
            
            byte[] byte1 = Encoding.UTF8.GetBytes(paramStr);
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            byte[] byte2 = sha.ComputeHash(byte1);
            sha.Clear();
            // 注意， 不能用这个
            //string output123 = Convert.ToBase64String(temp2);// 不能直接转换成base64string
            var output = BitConverter.ToString(byte2);
            //规则：不带 -、全小写
            output = output.Replace("-", "").ToLower();
            return output;
        }
        

       


    }
}
