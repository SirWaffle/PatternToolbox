using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Result
{ 
    public struct ResultStringList
    {
        public List<string>? Values { get; private set; } = null;

        public static implicit operator ResultStringList(string value)
        {
            return new ResultStringList(value);
        }

        public ResultStringList()
        { }

        public ResultStringList(String str)
        {
            AddString(str);
        }

        public void AddString(string str)
        {
            if (Values == null)
                Values = new();
            Values.Add(str);
        }
    }
}
