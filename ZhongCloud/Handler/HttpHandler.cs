using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Flurl;
using Flurl.Http;

namespace ZhongCloud
{
    public class HttpHandler
    {
        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<string> HttpRequestAsync(string url = "")
        {
            var responseString =await  url.GetStringAsync();
            return responseString;

        }

        /// <summary>
        /// 组装请求参数
        /// </summary>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        private string BuildQuery(IDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return "";
            }

            StringBuilder postData = new StringBuilder();
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))
                {
                    postData.Append("&");
                    postData.Append(name);
                    postData.Append("=");
                    postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                }
            }
            return postData.ToString();
        }


    }
}
