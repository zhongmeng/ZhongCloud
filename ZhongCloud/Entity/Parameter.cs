using System;
using System.Collections.Generic;
using System.Text;

namespace ZhongCloud
{
    public class Parameter 
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public string Default { get; set; }
        public bool IsConfig { get; set; }

        public bool IsBase64 { get; set; }

    }
}
