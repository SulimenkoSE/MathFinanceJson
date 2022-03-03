using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathJson
{
    public class MathDbKript : IEquatable<MathDbKript>
    {
        //public Guid SymboId { get; set; }
        public int Index { get; set; }
        public string? Date { get; set; }
        public double AdjClose { get; set; }
        public double MA_400 { get; set; }
        public double Risk_MA_400 { get; set; }
        public double MA_200 { get; set; }
        public double Mayer { get; set; }
        public double BtcIssuance { get; set; }
        public double UsdIssuance { get; set; }
        public double MAusdIssuance { get; set; }
        public double PuellMultiple { get; set; }
        public double MA_365 { get; set; }
        public double Price_52w { get; set; }
        public double Return { get; set; }
        public double Return_MA_365_1 { get; set; }
        public double Return_365_STD { get; set; }
        public double Sharpe { get; set; }
        public int Ind { get; set; }
        public double PowerLaw { get; set; }
        public double AVG { get; set; }            
        //public MathDbKript() { }
        public MathDbKript(int index, string date, double adjClose)
        {
            this.Index = index;
            this.Date = date;
            this.AdjClose = adjClose;
            this.MA_400 = double.NaN;
            this.Risk_MA_400 = double.NaN;
            //this.MA_200 = double.NaN;
            //this.Mayer = double.NaN;  
            //this.BtcIssuance = double.NaN;
            //this.UsdIssuance = double.NaN;            
            //this.MAusdIssuance = double.NaN;
            //this.PuellMultiple = double.NaN;
            //this.MA_365 = double.NaN;
            //this.Price_52w = double.NaN;
            //this.Return = double.NaN;
            //this.Return_MA_365_1 = double.NaN;
            //this.Return_365_STD = double.NaN;
            //this.Sharpe = double.NaN;
            //this.PowerLaw = double.NaN;
            //this.Ind = 0;
            //this.AVG = double.NaN;

        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            MathDbKript objAsKriptDbContext = obj as MathDbKript;
            if (objAsKriptDbContext == null) return false;
            else return Equals(objAsKriptDbContext);
        }
        public override int GetHashCode()
        {
            return Index;
        }
        public bool Equals(MathDbKript? other)
        {
            if (other == null) return false;
            return (this.Index.Equals(other.Index));
        }
    }
    
}
