using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathJson
{
    public class ColumnAverage
    {
        public double Avarage(List<MathDbKript> data, int period,  int col_end, int col_start = 0)
        {
            double result = default;
            
            if (col_end >= period) col_start = col_end - period;
            else period = col_end;

            if(col_end == 0) return 0;

            for (int i = col_start; i < col_end; i++)
            {
                result += data[i].AdjClose;
            }
            return result / period;
        }
        public double btcIssuance(List<MathDbKript> data,  int col_end)
        {
            double btc = 0;
            double btcVal = 0;
            double Result = 0;

            btc = Math.Floor(data[col_end].Index / 1458.00);
            btcVal = (double)Math.Pow(2,btc);
            Result = 7200 / btcVal;

            return Result;
        }
        public double usdIssuance(List<MathDbKript> data, int period, int col_end, int col_start = 0)
        {
            double result = default;

            if (col_end >= period) col_start = col_end - period;
            else period = col_end;

            if (col_end == 0) return 0;
            //if (col_end == 1) period = 1;

            for (int i = col_start; i < col_end; i++)
            {
                result += data[i].UsdIssuance;
            }
            return result / period;
        }

        public double ReturnPersant(List<MathDbKript> data, int col_end)
        {
            double result = default;
            if (col_end == 0) return double.NaN;
            result = (double)((data[col_end].AdjClose/ data[col_end-1].AdjClose)-1)*100;
            return result;
        }

        public double Return_365_MA_1(List<MathDbKript> data, int col_end, int col_start = 0)
        {
            double X_ = default;
            double result = default;
            int nan_Values = 0;

            if (col_end == 0) return double.NaN;
            if (col_end >= 365) col_start = col_end - 365;
            //Считаем среднее знеачене по выборке 365 значений
            for (int i = col_start; i < col_end + 1; i++)
            {
                if (double.IsNaN(data[i].Return)) nan_Values += 1;
                else X_ += data[i].Return;
            }
            if (col_end - col_start - nan_Values == 0) return double.NaN;    
            else result = (X_ / (1 + col_end - col_start - nan_Values)) - 1;
            return result;
        }

        public double Return_365_STD(List<MathDbKript> data, int col_end, int col_start = 0)
        {
            double X_ = default;
            double Y_ = default;
            double result = default;

            if (col_end == 0) return 0;
            if (col_end >= 365) col_start = col_end - 365;
            //Считаем среднее знеачене по выборке 365 значений
            for (int i = col_start; i < col_end; i++)
            {
                X_ += data[i].Return;
            }
            X_ = (double)(X_ / col_end);

            for (int i = col_start; i < col_end; i++)
            {
                Y_ += (double)Math.Pow((double)(data[i].Return - X_),2);
            }
            result = (double)Math.Pow((double)(Y_ / col_end -1), 0.5);

            return result;
        }


        public double PoweLaw(List<MathDbKript> data, int col_end)
        {
            
            //Вычисляем xValues
            double[] xValues = new double[col_end + 1];
            //xValues[0] = 0;
            double[] yValues = new double[col_end + 1];
            //yValues[0] = 0;

            for (int i = 0; i < col_end + 1; i++)
            {               
                    //Вычисляем xValues
                    xValues[i] = Math.Log10(data[i].Ind);
                    //Вычисляем yValues
                    yValues[i] = Math.Log10(data[i].AdjClose);                
            }
                       

            //Линейниая регрессия
            double rSquared, intercept, slope;
            LinearRegression rS = new LinearRegression(); 
            rS.LinearRegressions(xValues, yValues, out rSquared, out intercept, out slope);
            
            var predictedValue = (slope * Math.Log10(data[col_end].Ind)) + intercept; //посленее значение по оси xValues
            //Console.WriteLine($"PredictionValue: {predictedValue}");

            //double result = double.IsNaN(predictedValue) ? 0 : (double)(Math.Log10(data[col_end].AdjClose - predictedValue));
            double result = (double)(Math.Log10(data[col_end].AdjClose) - predictedValue);
            return result;
        }
        public void NormalizationData(List<MathDbKript> data)
        {
            double result = default;
            double[] cummin = {0, 0, 0, 0, 0, 0};
            double[] cummax = { 0, 0, 0, 0, 0, 0 };
      
            for (int i = 0; i < data.Count; i++)
            {
                //Вычисляем PuellMultiple
                if (cummin[0] == 0 || cummin[0] == Double.PositiveInfinity)
                {
                    if (data[i].PuellMultiple != Double.PositiveInfinity)
                    {
                        cummin[0] = data[i].PuellMultiple;
                    }
                }

                if (cummax[0] == 0 || cummin[0] == Double.PositiveInfinity)
                {
                    if (data[i].PuellMultiple != Double.PositiveInfinity)
                    {
                        cummax[0] = data[i].PuellMultiple;
                    }
                }

                cummin[0] = Math.Min(cummin[0], data[i].PuellMultiple);
                cummax[0] = Math.Max(cummax[0], data[i].PuellMultiple);
                if ((cummin[0] + cummax[0]) != 0) result = (data[i].PuellMultiple - cummin[0]) / (cummin[0] + cummax[0]);
                else result = 0;
                //If NaN
                if (Double.IsNaN(result)) data[i].PuellMultiple = 0;
                else data[i].PuellMultiple = result;

                //Вычисляем Price_52w
                if (cummin[1] == 0 || cummin[1] == Double.PositiveInfinity)
                {
                    if (data[i].Price_52w != Double.PositiveInfinity)
                    {
                        cummin[1] = data[i].Price_52w;
                    }
                }

                if (cummax[1] == 0 || cummin[1] == Double.PositiveInfinity)
                {
                    if (data[i].Price_52w != Double.PositiveInfinity)
                    {
                        cummax[1] = data[i].Price_52w;
                    }
                }
                cummin[1] = Math.Min(cummin[1], data[i].Price_52w);
                cummax[1] = Math.Max(cummax[1], data[i].Price_52w);
                if ((cummin[1] + cummax[1]) != 0) result = (data[i].Price_52w - cummin[1]) / (cummin[1] + cummax[1]);
                else result = 0;
                //If NaN
                if (Double.IsNaN(result)) data[i].Price_52w = 0;
                else data[i].Price_52w = result;

                //Вычисляем PowerLaw
                if (cummin[2] == 0 || cummin[2] == Double.PositiveInfinity)
                {
                    if (data[i].PowerLaw != Double.PositiveInfinity)
                    {
                        cummin[2] = data[i].PowerLaw;
                    }
                }

                if (cummax[2] == 0 || cummin[2] == Double.PositiveInfinity)
                {
                    if (data[i].PowerLaw != Double.PositiveInfinity)
                    {
                        cummax[2] = data[i].PowerLaw;
                    }
                }
                cummin[2] = Math.Min(cummin[2], data[i].PowerLaw);
                cummax[2] = Math.Max(cummax[2], data[i].PowerLaw);
                if ((cummin[2] + cummax[2]) != 0) result = (data[i].PowerLaw - cummin[2]) / (cummin[2] + cummax[2]);
                else result = 0;
                //If NaN
                if (Double.IsNaN(result)) data[i].PowerLaw = 0;
                else data[i].PowerLaw = result;

                //Вычисляем Sharpe
                if (cummin[3] == 0 || cummin[3] == Double.PositiveInfinity)
                {
                    if (data[i].Sharpe != Double.PositiveInfinity)
                    {
                        cummin[3] = data[i].Sharpe;
                    }
                }

                if (cummax[3] == 0 || cummin[3] == Double.PositiveInfinity)
                {
                    if (data[i].Sharpe != Double.PositiveInfinity)
                    {
                        cummax[3] = data[i].Sharpe;
                    }
                }
                cummin[3] = Math.Min(cummin[3], data[i].Sharpe);
                cummax[3] = Math.Max(cummax[3], data[i].Sharpe);
                if ((cummin[3] + cummax[3]) != 0) result = (data[i].Sharpe - cummin[3]) / (cummin[3] + cummax[3]);
                else result = 0;
                //If NaN
                if (Double.IsNaN(result)) data[i].Sharpe = 0;
                else data[i].Sharpe = result;

                //Вычисляем Mayer
                if (cummin[4] == 0 || cummin[4] == Double.PositiveInfinity)
                {
                    if (data[i].Mayer != Double.PositiveInfinity)
                    {
                        cummin[4] = data[i].Mayer;
                    }
                }

                if (cummax[4] == 0 || cummin[4] == Double.PositiveInfinity)
                {
                    if (data[i].Mayer != Double.PositiveInfinity)
                    {
                        cummax[4] = data[i].Mayer;
                    }
                }
                cummin[4] = Math.Min(cummin[4], data[i].Mayer);
                cummax[4] = Math.Max(cummax[4], data[i].Mayer);
                if ((cummin[4] + cummax[4]) != 0) result = (data[i].Mayer - cummin[4]) / (cummin[4] + cummax[4]);
                else result = 0;
                //If NaN
                if (Double.IsNaN(result)) data[i].Mayer = 0;
                else data[i].Mayer = result;
                               

                //Вычисляем Risk_MA_400
                if (cummin[5] == 0 || cummin[5] == Double.PositiveInfinity)
                {
                    if (data[i].Risk_MA_400 != Double.PositiveInfinity)
                    {
                        cummin[5] = data[i].Risk_MA_400;
                    }
                }

                if (cummax[5] == 0 || cummin[5] == Double.PositiveInfinity)
                {
                    if (data[i].Risk_MA_400 != Double.PositiveInfinity)
                    {
                        cummax[5] = data[i].Risk_MA_400;
                    }
                }
                cummin[5] = Math.Min(cummin[5], data[i].Risk_MA_400);
                cummax[5] = Math.Max(cummax[5], data[i].Risk_MA_400);
                if((cummin[5] + cummax[5]) != 0) result = (data[i].Risk_MA_400 - cummin[5]) / (cummin[5] + cummax[5]);
                else result = 0;
                //If NaN
                if (Double.IsNaN(result)) data[i].Risk_MA_400 = 0;
                else data[i].Risk_MA_400 = result;
            }
        }
    }
    
}
