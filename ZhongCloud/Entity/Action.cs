using System;
using System.Collections.Generic;
using System.Text;

namespace ZhongCloud
{
    public class Action
    {
        public string Name { get; set; }
        public string ActionID { get; set; }
        public string Remark { get; set; }
        public bool IsShow { get; set; }
        public List<Parameter> Parameter { get; set; }

    }
}
