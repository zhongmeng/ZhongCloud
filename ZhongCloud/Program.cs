using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace ZhongCloud
{
    class Program
    {
        static Dictionary<string, List<Action>> DicApi = new Dictionary<string, List<Action>>();
        static Dictionary<string, string> DicConfig = new Dictionary<string, string>();
        static List<string> ActionIsOper = new List<string>();
        static List<string> ActionIsShow = new List<string>();

        static void Main(string[] args)
        {
            #region 为实现本人要求，额外添加的简单开发代码,可移除
            string eip_ip = "";
            string eip_id = "";
            #endregion

            //获取当前项目的路径
            //读取Json配置
            JsonHandler jsonHandler = new JsonHandler(AppDomain.CurrentDomain.BaseDirectory.ToString());
            DicApi =jsonHandler.ReadJson<Dictionary<string,List<Action>>>("UCloudApi.json");
            if (DicApi == null || DicApi.Count == 0) {
                Console.WriteLine("配置读取错误！");
            }
            DicConfig = jsonHandler.ReadJson<Dictionary<string, string>>("UCloudConfig.json");
            //解析json配置
            InteractionHandler interactionHandler = new InteractionHandler(DicApi);
            //获取Action配置
            interactionHandler.GetActionsIsShow(ref ActionIsShow, ref ActionIsOper);
            UserInteraction userInteraction = new UserInteraction(ActionIsShow);
            //开始进行操作
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();
            UcloudHandler ucloudHandler = new UcloudHandler(DicConfig["PublicKey"], DicConfig["PrivateKey"]);
            HttpHandler httpHandler = new HttpHandler();
            while (1==1) {
                userInteraction.Init();
                int number = userInteraction.GetActionNumber();
                Action action = interactionHandler.GetAction(ActionIsOper[number-1]);

                #region 为实现本人要求，额外添加的简单开发代码,可移除
                if (action.ActionID == "UpdateVpnAddress" ) {
                    //eip_id = "eip-22f2jmmd";
                    //eip_ip = "152.32.225.145";
                    if (string.IsNullOrWhiteSpace(eip_ip) || string.IsNullOrWhiteSpace(eip_id) ) {
                        Console.WriteLine("未获取到弹性ip");
                        continue;
                    }
                    //不管有没有 执行绑定弹性ip操作，我这里都要执行一遍
                    Action actionBindIp = interactionHandler.GetAction("UNet|BindEIP");
                    Dictionary<string, string> dicParameterBindIp = new Dictionary<string, string>();
                    interactionHandler.MergeParameter(ref dicParameterBindIp, actionBindIp.Parameter);
                    dicParameterBindIp["EIPId"] = eip_id;
                    dicParameterBindIp["Action"] = "BindEIP";
                    dicParameterBindIp["Signature"] = ucloudHandler.GetSignature(actionBindIp.ActionID, dicParameterBindIp);
                    var respose=httpHandler.HttpRequestAsync(ucloudHandler.BuildUrl(dicParameterBindIp, DicConfig["Url"]));
                    Console.WriteLine(respose.Result);
                    //直接调用powershell尚未实现，目前折中的办法是新建一个文件，手动点击
                    //sde = PowerShellHandler.RunScript("Set-VpnConnection -Name 'Ucloud' -ServerAddress '12.12.12.132'");
                    string filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "UCloud_Vpn.ps1";
                    //判断文件是否存在
                    File.WriteAllText(filePath, "Set-VpnConnection -Name 'Ucloud' -ServerAddress '"+ eip_ip + "'");
                    continue;

                }
                #endregion
                //获取需要配置的参数
                Dictionary<string, string> dicParameterNeedConfig = interactionHandler.GetParametersNeedConfig(action.Parameter);
                //参数配置
                Dictionary<string, string> dicParametersAlreadyConfig = userInteraction.WriteParameters(dicParameterNeedConfig);
                Dictionary<string, string> dicParameterNeedSupplement = userInteraction.SupplementParameters();
                //参数处理
                dicParameters = interactionHandler.MergeParameter(dicParametersAlreadyConfig, dicParameterNeedSupplement);
                interactionHandler.MergeParameter(ref dicParameters, action.Parameter);
                //参数校验，核对
                while (1 == 1)
                {
                    //确认参数
                    if (userInteraction.NeedTrueParameters(dicParameters))
                    {
                        break;
                    }
                    if (userInteraction.NeedRestartParameter())
                    {
                        userInteraction.Restart(); //需要重置
                        dicParameters = null; // 为空之后，底下的参数校验通不过即会重置
                        break;
                    }
                    userInteraction.WriteParameterAgain(ref dicParameters);
                }
                //执行请求前处理
                if (dicParameters == null || dicParameters.Count == 0)
                {
                    Console.WriteLine("参数配置错误！");
                    continue;
                }
                //生成密钥
                // string sdfew = "{\"Action\":\"CreateUHostInstance\",\"CPU\":\"1\",\"ChargeType\":\"Dynamic\",\"DiskSpace\":\"20\",\"ImageId\":\"uimage-hkt3ycdi\",\"LoginMode\":\"Password\",\"Memory\":\"2048\",\"Name\":\"Host01\",\"Password\":\"Y1dGNmVITjNNREE9\",\"PublicKey\":\"3OwXRdhBdy01cuL1wgBUvrbgrpYZw_BKeuHvya4O\",\"Quantity\":\"1\",\"Region\":\"hk\",\"Zone\":\"hk-02\"}";
                //Dictionary<string, string> dfe = JsonConvert.DeserializeObject<Dictionary<string, string>>(sdfew);
                interactionHandler.JudgeNeedBase64(ref dicParameters,action.Parameter);
                dicParameters["Signature"] = ucloudHandler.GetSignature(action.ActionID,dicParameters);
                string url = ucloudHandler.BuildUrl(dicParameters, DicConfig["Url"]);
                Console.WriteLine(url);
                Console.WriteLine("是否确定执行（任意键/n）");
                if(Console.ReadKey().KeyChar == 'n'){
                    userInteraction.Restart();
                    continue;
                }
                //执行结果
                var repose=httpHandler.HttpRequestAsync(url);
                Console.WriteLine(repose.Result);
                JObject o = (JObject)JsonConvert.DeserializeObject<JObject>(repose.Result);
                if (!o.ContainsKey("Action"))
                {
                    Console.WriteLine("执行失败！");
                    continue;
                }
                Console.WriteLine("执行成功！");
                #region 为实现本人要求，额外添加的简单开发代码,可移除
                if (action.ActionID == "AllocateEIP")
                {
                    //string sde = "{\"Action\":\"AllocateEIPResponse\",\"EIPSet\":[{\"EIPAddr\":[{\"IP\":\"128.1.134.54\",\"OperatorName\":\"International\"}],\"EIPId\":\"eip-xbigzv5y\"}],\"Request_uuid\":\"617cdb2e-6a0d-4263-b8a8-7ca737398bf2\",\"RetCode\":0}";
                    eip_ip = (string)o["EIPSet"][0]["EIPAddr"][0]["IP"];
                    eip_id = (string)o["EIPSet"][0]["EIPId"];
                }
                #endregion



            }
        }

    }
}
