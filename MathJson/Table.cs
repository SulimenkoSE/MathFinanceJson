using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathJson
{
    public class Table
    {
        public Table(List<object[]> data, List<Column> columns)
        {
            Data = data;
            Columns = columns;
        }

        public List<object[]> Data { get; private set; }

        public List<Column> Columns { get; private set; }
    }
}
