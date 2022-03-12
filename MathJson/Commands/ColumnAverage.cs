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
            if (data.Count <= col_end)
            {
                //return double.NaN;
                throw new ArgumentOutOfRangeException($"{col_end} reqvested ven total {data.Count}");
            }
            else
            {
                double result = double.NaN;
                var legalValues = from value in data
                                  let marker = value[fieldsTable]
                                  where !double.IsNaN(marker)
                                  where !double.IsPositiveInfinity(marker)
                                  select marker;
                // Определяем диапазон выборки
                if (col_end >= period) col_start = col_end - period;
                //Выбираем из набора данных только то что нам нужно с учетом диапазона выборки
                var sampleData = legalValues.Skip(col_start).Take(col_end).ToArray();
                //Опредеяем среднее значение по выборке
                if (sampleData.Count() != 0)
                {
                    result = sampleData.Average();//value => value[fieldsTable]
                }
                return result;
            }
        }
        public double btcIssuance(List<MathDbKript> data, int col_end)
        {
            double btc = 0;
            double btcVal = 0;
            double Result = 0;

            btc = Math.Floor(data[col_end].Index / 1458.00);
            btcVal = (double)Math.Pow(2, btc);            
            if (double.IsInfinity(btcVal))
            {
                //return double.NaN;
                throw new ArgumentOutOfRangeException($"Reqvested ven data[col_end].Index >= {data[col_end].Index}");
            }
            Result = 7200 / btcVal;
            return Result;
        }
        
        public double ReturnPersant(List<MathDbKript> data, int col_end)
        {
            double result = double.NaN;
            if (col_end == 0) return result;

            if (data.Count <= col_end || data.Count ==0)
            {
                //return double.NaN;
                throw new ArgumentOutOfRangeException($"{col_end} reqvested ven total {data.Count}");
            }
            else
            {
                result = 0;
                //Выбираем из набора данных только то что нам нужно с учетом диапазона выборки
                var legalValues = from value in data
                                  let marker = value[FieldsMathDbKript.AdjClose]
                                  where !double.IsNaN(marker)
                                  where !double.IsPositiveInfinity(marker)
                                  select marker;

                var returnData = legalValues.Skip(col_end -1).Take(col_end).ToArray();

                if (returnData.Count() > 1 )
                {
                    result = (double)((returnData[returnData.Count()-1] / returnData[returnData.Count() - 1]) - 1) * 100;
                }               
                return result;
            }
        }

        public double Return_365_STD(List<MathDbKript> data, int period, int col_end, FieldsMathDbKript fieldsTable, int col_start = 0)
        {
            double X_ = default;
            double result = double.NaN;

            //Если равны то в знаменателе 0 поэтому выходим
            if (col_end == col_start) return result;

            //А вдруг пустой объект или data.Count <= col_end
            if (data.Count <= col_end || data.Count == 0)
            {
                //return double.NaN;
                throw new ArgumentOutOfRangeException($"{col_end} reqvested ven total {data.Count}");
            }
            else
            {                
                // Определяем диапазон выборки
                if (col_end >= period) col_start = col_end - period;

                //Считаем среднее знеачене по выборке 365 значений
                X_ = Avarage(data: data, 365, col_end, fieldsTable);
                // Вдруг имеем double.NaN
                if (!double.IsNaN(X_))
                {
                    //Выбираем из набора данных только то что нам нужно с учетом диапазона выборки
                    var legalValues = from value in data
                                    let marker = value[fieldsTable]
                                    where !double.IsNaN(marker)
                                    where !double.IsPositiveInfinity(marker)
                                    select (double)Math.Pow((double)(marker - X_), 2);
                
                    var returnData = legalValues.Skip(col_start).Take(col_end).ToArray();
                
                    if (returnData.Count() - col_start > 0)
                    {    
                        result = (double)Math.Sqrt((double)(returnData.Sum() / (returnData.Count() - col_start)));
                    }
                }
                
                return result;
            }
            ////Считаем среднее знеачене по выборке 365 значений
            //X_ = Avarage(data: data, 365, col_end, FieldsMathDbKript.Return);
            ////Вычисляем сумму квадратов
            //for (int i = col_start; i < col_end; i++)            {
            //    // опрделяется ка разница между значением Return и полученным средним значением по всему столбцу 
            //    // в указанном диапазоне
            //    if (!double.IsNaN(data[i].Return)) Y_ += (double)Math.Pow((double)(data[i].Return - X_), 2);
            //}
            //result = (double)Math.Sqrt((double)(Y_ / (col_end - col_start)));
            //return result;
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
            for (int i = 0; i < data.Count; i++)
            {
                ProcessIndicator(data, i, FieldsMathDbKript.PuellMultiple);
                //ProcessIndicator(data, i, FieldsMathDbKript.Price_52w);
                //ProcessIndicator(data, i, FieldsMathDbKript.PowerLaw);
                //ProcessIndicator(data, i, FieldsMathDbKript.Sharpe);
                //ProcessIndicator(data, i, FieldsMathDbKript.Mayer);
                //ProcessIndicator(data, i, FieldsMathDbKript.Risk_MA_400);

            }

            
            //    //Вычисляем Risk_MA_400
            //    if (!double.IsNaN(data[i].Risk_MA_400) && data[i].Risk_MA_400 != 0)
            //    {
            //        if (!double.IsNaN(cummin[5])) cummin[5] = Math.Min(cummin[5], data[i].Risk_MA_400);
            //        else cummin[5] = data[i].Risk_MA_400;

            //        if (!double.IsNaN(cummax[5])) cummax[5] = Math.Max(cummax[5], data[i].Risk_MA_400);
            //        else cummax[5] = data[i].Risk_MA_400;

            //        if ((cummax[5] - cummin[5]) != 0) result = (data[i].Risk_MA_400 - cummin[5]) / (cummax[5] - cummin[5]);
            //        else result = cummin[5];

            //        data[i].Risk_MA_400 = result;
            //    }
            //}
        }
        
        public void ProcessIndicator(IReadOnlyList<MathDbKript> data, int index, FieldsMathDbKript indicator)
        {
            var legalValues = from value in data
                              let marker = value[indicator]
                              where !double.IsNaN(marker)
                              //where marker != 0
                              where marker != double.PositiveInfinity
                              //select value;
                              select new
                              {
                                  value = marker
                              };

            //var element = data[index];

            //var atLeastOne = index + 1;
            var slice = legalValues.Skip(index - 1).Take(index).ToList();
            if (slice.Count() != 0) 
            {
                //var sliceMax = slice.Max();
                //var sliceMax = 0;
                //var sliceMin = slice.Min(value => value[indicator]);
                //Console.WriteLine($"index = {index}, indicator = {indicator}, sliceMin = {sliceMin}, sliceMax = {sliceMax}"); 
            }



            //var diff = (sliceMax - sliceMin);            
            //if (diff != 0)
            //{
            //    element[indicator] = sliceMax / sliceMin;
            //}
            //else

            //{

            //}
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
