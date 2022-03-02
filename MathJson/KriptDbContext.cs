using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathJson
{
    public class KriptDbContext : IEquatable<KriptDbContext>
    {
        //public Guid SymboId { get; set; }
        public int Index { get; set; }
        public string? Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double AdjClose { get; set; }
        public double Volume { get; set; }
        public KriptDbContext() { }
        public KriptDbContext(int index, string date,  double adjClose,
                double volume)
        {
            this.Index = index;
            this.Date = date;
            this.AdjClose = adjClose;
            this.Volume = volume;
        }
        public KriptDbContext(int index, string date, double open,
                double high, double low,double close, double adjClose,
                double volume)
        {
            this.Index = index; 
            this.Date = date;   
            this.Open = open;
            this.High = high;
            this.Low = low; 
            this.Close = close; 
            this.AdjClose = adjClose;
            this.Volume = volume;
        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            KriptDbContext objAsKriptDbContext = obj as KriptDbContext;
            if (objAsKriptDbContext == null) return false;
            else return Equals(objAsKriptDbContext);
        }
        public override int GetHashCode()
        {
            return Index;
        }
        public bool Equals(KriptDbContext? other)
        {
            if (other == null) return false;
            return (this.Index.Equals(other.Index));
        }
    }
}
