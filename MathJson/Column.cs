using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathJson
{
    public class Column
    {
        public Column(string name, string type)
        {
            Name = name;
            Type = type;
        }
                public string Name { get; private set; }

        public string Type { get; private set; }
    }
}
