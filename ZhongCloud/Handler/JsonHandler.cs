using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZhongCloud
{
    public class JsonHandler
    {
        public JsonHandler()
        {

        }
        public JsonHandler(string templatePath)
        {
            BasePath = templatePath;
        }
        private string BasePath { get; set; }


        /// <summary>
        /// 读取json文件的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public T ReadJson<T>(string filePath)
        {
            using (System.IO.StreamReader file = System.IO.File.OpenText(BasePath+filePath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    T dic = JsonConvert.DeserializeObject<T>(o.ToString());
                    return dic;
                }
            }
        }


        /// <summary>
        /// 将json写入到指定文件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Json</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public bool WriteJson<T>(List<T> list ,string filePath="" ) {
            if (string.IsNullOrWhiteSpace(filePath)) {
                filePath = BasePath + filePath;
            }
            try
            {
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);
                return true;
            } catch (Exception e) {
                return false;
            }
        }


    }
            
}
