using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZhongCloud
{
    public class UserInteraction
    {
        private List<string> ActionNeedShow { get; set; }

        public UserInteraction(List<string> actionNeedShow) {
            ActionNeedShow = actionNeedShow;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init() {
            Console.WriteLine("******************************************.net版简易UCloud管理平台ZhongCloud*******************************************");
            ActionInteractionShow();
            // Console.WriteLine("Exit：退出");
        }

        /// <summary>
        /// 清屏
        /// </summary>
        public void Clear() {
            // 清屏专用
            Console.Clear();
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        public void Restart() {
            Clear();
          //  Init();
        }


        /// <summary>
        /// 显示所有可操作的action
        /// </summary>
        public void ActionInteractionShow() {
            foreach (var str in ActionNeedShow)
            {
                Console.WriteLine(str);
            }
        }

        /// <summary>
        /// 参数录入操作
        /// </summary>
        /// <param name="parameterNeedConfig"></param>
        /// <returns></returns>
        public Dictionary<string,string> WriteParameters(Dictionary<string, string> parameterNeedConfig)
        {
            if (parameterNeedConfig == null) {
                return null;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var key in parameterNeedConfig.Keys)
            {
                Console.WriteLine(parameterNeedConfig[key]);
                dic[key] = Console.ReadLine();
            }
            return dic;
        }

        /// <summary>
        /// 遍历所有参数，重新录入
        /// </summary>
        /// <param name="dic"></param>
        public void WriteParameterAgain(ref Dictionary<string,string> dic ) {
            Console.WriteLine("\n");
            Dictionary<string, string> dicNew = new Dictionary<string, string>();
            foreach (var key in dic.Keys) { 
                Console.WriteLine(key+"（现有值："+dic[key]+"）：");
                dicNew[key] = Console.ReadLine();
            }
            foreach (var key in dicNew.Keys ) {
                if (string.IsNullOrWhiteSpace(dicNew[key])) {
                    continue;
                }
                dic[key] = dicNew[key];
            }

        }
        /// <summary>
        /// 获取输入的action 编号
        /// </summary>
        /// <returns></returns>
        public int GetActionNumber() {
            Console.WriteLine("请输入对应编号：");
            string input = Console.ReadLine();
            //解析用户输入的内容
            if (!IsInteger(input)) {
                Console.WriteLine("非法输入！");
                return GetActionNumber();
            }
            return Convert.ToInt32(input);
            }

        /// <summary>
        /// 参数补充，可默认为空
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> SupplementParameters() {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Console.WriteLine("补充参数（默认为空。例:{Name:\"sdf\",LoginMode:\"Password\" }）");
            string inputStr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(inputStr))
            {
                return null;
            }
            try
            {
                Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(inputStr);
                foreach (var key in keyValuePairs.Keys)
                {
                    dic[key] = keyValuePairs[key];
                }
                return dic;
            }
            catch {
                return null;
            }

        }
        /// <summary>
        /// 确认参数
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool NeedTrueParameters(Dictionary<string,string> dic) {
            foreach (var key in dic.Keys)
            {
                Console.WriteLine(key + "：" + dic[key]);
            }
            //重新录入判断
            Console.WriteLine("是否有误（y/任意键）");
            if (Console.ReadKey().KeyChar == 'y')
            {
                Console.WriteLine("\n");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 是否重新录入
        /// </summary>
        /// <returns></returns>
        public bool NeedRestartParameter()
        {
            //重新录入判断
            Console.WriteLine("是否对已有参数修改（y/任意键）");
            if (Console.ReadKey().KeyChar == 'y')
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 是否为正整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsInteger(string value)
        {
            string pattern = @"^[0-9]*[1-9][0-9]*$";
            return Regex.IsMatch(value, pattern);
        }

    }
}
