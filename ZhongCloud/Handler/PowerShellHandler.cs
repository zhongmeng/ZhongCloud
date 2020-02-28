
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace ZhongCloud
{
    public static class PowerShellHandler
    {
        /// <summary>
        /// 调用PowerShell执行命令
        /// </summary>
        /// <param name="scriptText"></param>
        /// <returns></returns>
        public static string RunScript(string scriptText)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                PowerShell ps = PowerShell.Create();
                ps.Runspace = runspace;
                ps.AddScript(scriptText);
                ps.Invoke();
                return "";

            }
        }   
    }
}
