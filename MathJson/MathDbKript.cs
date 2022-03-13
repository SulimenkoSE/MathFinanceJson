using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static MathJson.ColumnAverage;

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

        // Constructor for the class.
        public MathDbKript(int index, string date, double adjClose)
        {
            this.Index = index;
            this.Date = date;
            this.AdjClose = adjClose;
            this.MA_400 = double.NaN;
            this.Risk_MA_400 = double.NaN;
            this.MA_200 = double.NaN;
            this.Mayer = double.NaN;
            this.BtcIssuance = double.NaN;
            this.UsdIssuance = double.NaN;
            this.MAusdIssuance = double.NaN;
            this.PuellMultiple = double.NaN;
            this.MA_365 = double.NaN;
            this.Price_52w = double.NaN;
            this.Return = double.NaN;
            this.Return_MA_365_1 = double.NaN;
            this.Return_365_STD = double.NaN;
            this.Sharpe = double.NaN;
            this.PowerLaw = double.NaN;
            this.Ind = 0;
            this.AVG = double.NaN;

        }

        public double this[FieldsMathDbKript indicator]
        { 
        get
            {
                switch (indicator)
                {
                    case FieldsMathDbKript.AdjClose:
                        return this.AdjClose;
                    case FieldsMathDbKript.MA_400:
                        return this.MA_400;
                    case FieldsMathDbKript.Risk_MA_400:
                        return this.Risk_MA_400;
                    case FieldsMathDbKript.MA_200:
                        return this.MA_200;
                    case FieldsMathDbKript.Mayer:
                        return this.Mayer;
                    case FieldsMathDbKript.BtcIssuance:
                        return this.BtcIssuance;
                    case FieldsMathDbKript.UsdIssuance:
                        return this.UsdIssuance;
                    case FieldsMathDbKript.MAusdIssuance:
                        return this.MAusdIssuance;
                    case FieldsMathDbKript.PuellMultiple:
                        return this.PuellMultiple;
                    case FieldsMathDbKript.MA_365:
                        return this.MA_365;
                    case FieldsMathDbKript.Price_52w:
                        return this.Price_52w;                                           
                    case FieldsMathDbKript.Return:
                        return this.Return;
                    case FieldsMathDbKript.Return_MA_365_1:
                        return this.Return_MA_365_1;
                    case FieldsMathDbKript.Return_365_STD:
                        return this.Return_365_STD;
                    case FieldsMathDbKript.Sharpe:
                        return this.Sharpe;
                    case FieldsMathDbKript.PowerLaw:
                        return this.PowerLaw;
                    case FieldsMathDbKript.AVG:
                        return this.AVG;
                    default:
                        throw new NotImplementedException(nameof(FieldsMathDbKript));
                }                    
            }
            set
            {
                switch (indicator)
                {
                    case FieldsMathDbKript.AdjClose:
                        this.AdjClose = value;
                        break;
                    case FieldsMathDbKript.MA_400:
                        this.MA_400 = value;
                        break;
                    case FieldsMathDbKript.Risk_MA_400:
                        this.Risk_MA_400 = value;
                        break;
                    case FieldsMathDbKript.MA_200:
                        this.MA_200 = value;
                        break;
                    case FieldsMathDbKript.Mayer:
                        this.Mayer = value;
                        break;
                    case FieldsMathDbKript.BtcIssuance:
                        this.BtcIssuance = value;
                        break;
                    case FieldsMathDbKript.UsdIssuance:
                        this.UsdIssuance = value;
                        break;
                    case FieldsMathDbKript.MAusdIssuance:
                        this.MAusdIssuance = value;
                        break;
                    case FieldsMathDbKript.PuellMultiple:
                        this.PuellMultiple = value;
                        break;                    
                    case FieldsMathDbKript.MA_365:
                        this.MA_365 = value;
                        break;
                    case FieldsMathDbKript.Price_52w:
                        this.Price_52w = value;
                        break;
                    case FieldsMathDbKript.Return:
                        this.Return = value;
                        break;
                    case FieldsMathDbKript.Return_MA_365_1:
                        this.Return_MA_365_1 = value;
                        break;
                    case FieldsMathDbKript.Return_365_STD:
                        this.Return_365_STD = value;
                        break;
                    case FieldsMathDbKript.Sharpe:
                        this.Sharpe = value;
                        break;
                    case FieldsMathDbKript.PowerLaw:
                        this.PowerLaw = value;
                        break;
                    case FieldsMathDbKript.AVG:
                        this.AVG = value;
                        break;
                    default:
                        throw new NotImplementedException(nameof(FieldsMathDbKript));
                }
            }
        }

        public double this[AvgIndicator indicator]
        {
            get
            {
                switch (indicator)
                {
                    case AvgIndicator.Risk_MA_400:
                        return this.Risk_MA_400;                   
                    case AvgIndicator.Mayer:
                        return this.Mayer;                   
                    case AvgIndicator.PuellMultiple:
                        return this.PuellMultiple;                    
                    case AvgIndicator.Sharpe:
                        return this.Sharpe;
                    case AvgIndicator.PowerLaw:
                        return this.PowerLaw;                    
                    default:
                        throw new NotImplementedException(nameof(AvgIndicator));
                }
            }
            set
            {
                switch (indicator)
                {
                    case AvgIndicator.PuellMultiple:
                        this.PuellMultiple = value;
                        break;
                    case AvgIndicator.PowerLaw:
                        this.PowerLaw = value;
                        break;
                    case AvgIndicator.Sharpe:
                        this.Sharpe = value;
                        break;
                    case AvgIndicator.Risk_MA_400:
                        this.Risk_MA_400 = value;
                        break;
                    case AvgIndicator.Mayer:
                        this.Mayer = value;
                        break;
                    default:
                        throw new NotImplementedException(nameof(FieldsMathDbKript));
                }
            }
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
