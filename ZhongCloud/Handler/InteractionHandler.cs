using System;
using System.Collections.Generic;
using System.Text;

namespace ZhongCloud
{
    class InteractionHandler
    {
        private Dictionary<string, List<Action>> DicApi { get; set; }
        public InteractionHandler(Dictionary<string, List<Action>> dicApi) {
            DicApi = dicApi;
        }

        /// <summary>
        /// 获取需要进行交互操作的Action数组
        /// </summary>
        /// <param name="actionIsShow"></param>
        /// <param name="actionIsOper"></param>
        public void GetActionsIsShow(ref List<string> actionIsShow,ref List<string> actionIsOper) {
            if (actionIsShow == null) {
                actionIsShow = new List<string>();
            }
            if (actionIsOper == null)
            {
                actionIsOper = new List<string>();
            }
            int num = 0;
            foreach (var key in DicApi.Keys)
            {
                if (DicApi[key] == null || DicApi[key].Count == 0)
                {
                    continue;
                }
                actionIsShow.Add(key);
                List<Action> actions = DicApi[key];
                actions = actions.FindAll(b => b.IsShow == true);
                if (actions == null && actions.Count == 0)
                {
                    continue;
                }

                foreach (Action action in actions)
                { //遍历需要显示操作的action
                    num ++;
                    actionIsShow.Add(num.ToString() + "." + action.Name + "                " + action.Remark);
                    actionIsOper.Add(key + "|" + action.ActionID);
                }
            }
        }

        /// <summary>
        /// 获取定义好的需要配置的参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParametersNeedConfig(List<Parameter> parameters ) {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (Parameter parameter in parameters)
            {
                if (parameter.IsConfig != true) {
                    continue;
                }
                dic[parameter.Name] = parameter.Name+"("+parameter.Description+")：";

            }
            return dic;
        }

        /// <summary>
        /// 获取单个action
        /// </summary>
        /// <param name="actionStr"></param>
        /// <returns></returns>
        public Action GetAction(string actionStr) {
            Action action= DicApi[actionStr.Split('|')[0]].Find(b => b.ActionID == actionStr.Split('|')[1]);
            return action;
        }

        /// <summary>
        /// 参数合并
        /// </summary>
        /// <param name="dicParameter"></param>
        /// <param name="parameterIsSupplement"></param>
        /// <returns></returns>
        public Dictionary<string, string> MergeParameter(Dictionary<string,string> dicParameter,Dictionary<string,string> parameterIsSupplement) {
            if (parameterIsSupplement == null) {
                return dicParameter;
            }
            foreach (var key in parameterIsSupplement.Keys) {
                dicParameter[key] = parameterIsSupplement[key];
            }
            return dicParameter;
        }

        /// <summary>
        /// 与默认的参数配置合并
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="parameters"></param>
        /// <remarks>即使是Request 是true的，但是他有默认值，故只需要判断isconfig</remarks>
        /// <returns></returns>
        public void MergeParameter(ref Dictionary<string,string> dic,List<Parameter> parameters) {
            if (dic == null) {
                dic = new Dictionary<string, string>();
            }
            foreach (Parameter parameter in parameters) {
                if (dic.ContainsKey(parameter.Name) || string.IsNullOrEmpty(parameter.Default)) {
                    continue;
                }
                dic[parameter.Name] = parameter.Default;
            }
        }

        /// <summary>
        /// 判断是否有需要base64进行编码的参数
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="parameters"></param>
        public void JudgeNeedBase64(ref Dictionary<string, string> dic, List<Parameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.IsBase64 != true)
                {
                    continue;
                }
                if (!dic.ContainsKey(parameter.Name))
                {
                    continue;
                }
                dic[parameter.Name] = StrToBase64(dic[parameter.Name]);
            }

        }


        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        private string StrToBase64(string sourceStr)
        {
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(sourceStr);
            try
            {
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return "";
            }
        }



    }
}
