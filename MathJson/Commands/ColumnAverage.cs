using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathJson
{
    public class ColumnAverage
    {
        public double Avarage(List<MathDbKript> data, int period, int col_end, FieldsMathDbKript fieldsTable, int col_start = 0)
        {
            double result = double.NaN;
            var legalValues = from value in data
                              let marker = value[fieldsTable]
                              where marker != double.NaN
                              where marker != double.PositiveInfinity
                              select value;
            //Полуаем елемент для обновления значения
            var element = data[col_end];
            // Определяем диапазон выборки
            // if (col_end == 0) return double.NaN;
            if (col_end >= period) col_start = col_end - period;
            //Выбираем из набора данных только то что нам нужно с учетом диапазона выборки
            var sampleData = legalValues.Skip(col_start).Take(col_end).ToArray();
            //Опредеяем среднее значение по выборке
            if (sampleData.Count() != 0)
            {
                result = sampleData.Average(value => value[fieldsTable]);

            }
            return result;
        }
        public double btcIssuance(List<MathDbKript> data, int col_end)
        {
            double btc = 0;
            double btcVal = 0;
            double Result = 0;

            btc = Math.Floor(data[col_end].Index / 1458.00);
            btcVal = (double)Math.Pow(2, btc);
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
            result = (double)((data[col_end].AdjClose / data[col_end - 1].AdjClose) - 1) * 100;
            return result;
        }

        public double Return_365_MA_1(List<MathDbKript> data, int period, int col_end, int col_start = 0)
        {
            double X_ = default;
            double result = default;
            int nan_Values = 0;

            if (col_end == 0) return double.NaN;

            if (col_end >= period) col_start = col_end - period;
            //Считаем среднее знеачене по выборке 365 значений
            for (int i = col_start; i < col_end; i++)
            {
                if (double.IsNaN(data[i].Return)) nan_Values += 1;
                else X_ += data[i].Return;
            }
            if (col_end - col_start - nan_Values == 0) return double.NaN;
            else result = X_ / (col_end - col_start - nan_Values) - 1;
            return result;
        }

        public double Return_365_STD(List<MathDbKript> data, int col_end, int col_start = 0)
        {
            double X_ = default;
            double Y_ = default;
            double result = default;
            int nan_Values = 0;

            if (col_end == 0) return 0;
            if (col_end >= 365) col_start = col_end - 365;
            //Считаем среднее знеачене по выборке 365 значений
            for (int i = col_start; i < col_end; i++)
            {
                if (double.IsNaN(data[i].Return)) nan_Values += 1;
                else X_ += data[i].Return;
            }

            if (col_end - col_start - nan_Values == 0) return double.NaN;
            else X_ = X_ / (col_end - col_start - nan_Values);

            //Вычисляем сумму квадратов
            for (int i = col_start; i < col_end; i++)
            {
                // опрделяется ка разница между значением Return и полученным средним значением по всему столбцу 
                // в указанном диапазоне
                if (!double.IsNaN(data[i].Return)) Y_ += (double)Math.Pow((double)(data[i].Return - X_), 2);
            }
            result = (double)Math.Sqrt((double)(Y_ / (col_end - col_start - nan_Values - 1)));

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
            if (double.IsNaN(result)) result = yValues[col_end];
            return result;
        }
        public void NormalizationData(List<MathDbKript> data)
        {
            double result = default;
            double[] cummin = { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
            double[] cummax = { double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
            //indicators = { "PuellMultiple", "Price_52w", "PowerLaw", "Sharpe", "Mayer", "Risk_MA_400" };


            for (int i = 0; i < data.Count; i++)
            {
                ProcessIndicator(data, i, AvgIndicator.Mayer);
                ProcessIndicator(data, i, AvgIndicator.PowerLaw);


                ////Вычисляем PuellMultiple
                if (!double.IsNaN(data[i].Risk_MA_400) && data[i].Risk_MA_400 != 0)
                {
                    if (!double.IsNaN(cummin[0])) cummin[0] = Math.Min(cummin[0], data[i].Risk_MA_400);
                    else cummin[0] = data[i].Risk_MA_400;

                    if (!double.IsNaN(cummax[0])) cummax[0] = Math.Max(cummax[0], data[i].Risk_MA_400);
                    else cummax[0] = data[i].Risk_MA_400;

                    if ((cummax[0] - cummin[0]) != 0) result = (data[i].Risk_MA_400 - cummin[0]) / (cummax[0] - cummin[0]);
                    else result = cummin[0];

                    data[i].Risk_MA_400 = result;
                }

                //Вычисляем Price_52w
                if (!double.IsNaN(data[i].Price_52w) && data[i].Price_52w != 0)
                {
                    if (!double.IsNaN(cummin[1])) cummin[1] = Math.Min(cummin[1], data[i].Price_52w);
                    else cummin[1] = data[i].Price_52w;

                    if (!double.IsNaN(cummax[1])) cummax[1] = Math.Max(cummax[1], data[i].Price_52w);
                    else cummax[1] = data[i].Price_52w;

                    if ((cummax[1] - cummin[1]) != 0) result = (data[i].Price_52w - cummin[1]) / (cummax[1] - cummin[1]);
                    else result = cummin[1];

                    data[i].Price_52w = result;
                }

                ////Вычисляем PowerLaw
                if (!double.IsNaN(data[i].PowerLaw) && data[i].PowerLaw != 0)
                {
                    if (!double.IsNaN(cummin[2])) cummin[2] = Math.Min(cummin[2], data[i].PowerLaw);
                    else cummin[2] = data[i].PowerLaw;

                    if (!double.IsNaN(cummax[2])) cummax[2] = Math.Max(cummax[2], data[i].PowerLaw);
                    else cummax[2] = data[i].PowerLaw;

                    if ((cummax[2] - cummin[2]) != 0) result = (data[i].PowerLaw - cummin[2]) / (cummax[2] - cummin[2]);
                    else result = cummin[2];

                    data[i].PowerLaw = result;
                }

                ////Вычисляем Sharpe
                if (!double.IsNaN(data[i].Sharpe) && data[i].Sharpe != 0)
                {
                    if (!double.IsNaN(cummin[3])) cummin[3] = Math.Min(cummin[3], data[i].Sharpe);
                    else cummin[3] = data[i].Sharpe;

                    if (!double.IsNaN(cummax[3])) cummax[3] = Math.Max(cummax[3], data[i].Sharpe);
                    else cummax[3] = data[i].Sharpe;

                    if ((cummax[3] - cummin[3]) != 0) result = (data[i].Sharpe - cummin[3]) / (cummax[3] - cummin[3]);
                    else result = cummin[3];

                    data[i].Sharpe = result;
                }

                ////Вычисляем Mayer
                if (!double.IsNaN(data[i].Mayer) && data[i].Mayer != 0)
                {
                    if (!double.IsNaN(cummin[4])) cummin[4] = Math.Min(cummin[4], data[i].Mayer);
                    else cummin[4] = data[i].Mayer;

                    if (!double.IsNaN(cummax[4])) cummax[1] = Math.Max(cummax[4], data[i].Mayer);
                    else cummax[4] = data[i].Mayer;

                    if ((cummax[4] - cummin[4]) != 0) result = (data[i].Mayer - cummin[4]) / (cummax[4] - cummin[4]);
                    else result = cummin[4];

                    data[i].Mayer = result;
                }


                //Вычисляем Risk_MA_400
                if (!double.IsNaN(data[i].Risk_MA_400) && data[i].Risk_MA_400 != 0)
                {
                    if (!double.IsNaN(cummin[5])) cummin[5] = Math.Min(cummin[5], data[i].Risk_MA_400);
                    else cummin[5] = data[i].Risk_MA_400;

                    if (!double.IsNaN(cummax[5])) cummax[5] = Math.Max(cummax[5], data[i].Risk_MA_400);
                    else cummax[5] = data[i].Risk_MA_400;

                    if ((cummax[5] - cummin[5]) != 0) result = (data[i].Risk_MA_400 - cummin[5]) / (cummax[5] - cummin[5]);
                    else result = cummin[5];

                    data[i].Risk_MA_400 = result;
                }
            }
        }

        public void AVG(List<MathDbKript> data)
        {
            //string[] indicators = { "PuellMultiple", "PowerLaw", "Sharpe", "Risk_MA_400", "Mayer"};
            AvgIndicator indicator = new AvgIndicator();
            var legalValues = from value in data
                              let marker = value[indicator]
                              where marker != double.NaN
                              //where marker != 0
                              where marker != double.PositiveInfinity
                                  //select value;
                                  //Некотрые столбцы из общего набора столбцов
                              select new
                              {
                                  
                                  puellMultiple = value[AvgIndicator.PuellMultiple],
                                  powerLaw = value[AvgIndicator.PowerLaw],
                                  sharpe = value[AvgIndicator.Sharpe],
                                  risk_MA_400 = value[AvgIndicator.Risk_MA_400],
                                  mayer = value[AvgIndicator.Mayer]
                              };
            
            double result = default;
            

            //Считаем среднее знеачене по строке
            for (int index = 0; index < data.Count; index++)
            {
                double X_ = default;
                var element = data[index].AVG;
                var slice = legalValues.Take(index).ToArray();
                //var average = slice.Average(value =>value[indicator]);
                //int nan_Values = 0;

                //if (!double.IsNaN(data[i].PuellMultiple) && data[i].PuellMultiple != double.PositiveInfinity) 
                //{ 
                //    X_ = data[i].PuellMultiple;
                //    nan_Values += 1;
                //}
                //if (!double.IsNaN(data[i].PowerLaw))
                //{
                //    X_ = X_ + data[i].PowerLaw;
                //    nan_Values += 1;
                //}
                //if (!double.IsNaN(data[i].Sharpe))
                //{
                //    X_ = X_ + data[i].Sharpe;
                //    nan_Values += 1;
                //}                
                //if (!double.IsNaN(data[i].Risk_MA_400))
                //{
                //    X_ = X_ + data[i].Risk_MA_400;
                //    nan_Values += 1;
                //}
                //if (!double.IsNaN(data[i].Mayer))
                //{
                //    X_ = X_ + data[i].Mayer;
                //    nan_Values += 1;
                //}
                //if (nan_Values != 0) data[i].AVG = X_ / nan_Values;

            }
        }


        public void ProcessIndicator(IReadOnlyList<MathDbKript> data, int index, AvgIndicator indicator)
        {
            var legalValues = from value in data
                              let marker = value[indicator]
                              where marker != double.NaN
                              where marker != 0
                              where marker != double.PositiveInfinity
                              select value;
            
            //Некотрые столбцы из общего набора столбцов
            //select new
            //{
            //    mayer = value[AvgIndicator.Mayer],
            //    other = value[AvgIndicator.PuellMultiple]
            //};

            var element = data[index];

            var atLeastOne = index + 1;
            var slice = legalValues.Take(atLeastOne).ToArray(); 
            var sliceMax = slice.Max(value => value[indicator]);
            var sliceMin = slice.Min(value => value[indicator]);


            var diff = (sliceMax - sliceMin);            
            if (diff != 0)
            {
                element[indicator] = sliceMax / sliceMin;
            }
            else

            {
                
            }
        }
    }

    public enum AvgIndicator
    {
        PuellMultiple,
        PowerLaw,
        Sharpe,
        Risk_MA_400,
        Mayer
    }
    public enum FieldsMathDbKript
    {
        Index,
        Date,
        AdjClose,
        MA_400,
        Risk_MA_400,
        MA_200,
        Mayer,
        BtcIssuance,
        UsdIssuance,
        MAusdIssuance,
        PuellMultiple,
        MA_365,
        Price_52w,
        Return,
        Return_MA_365_1,
        Return_365_STD,
        Sharpe,
        PowerLaw,
        Ind,
        AVG
    }
}
