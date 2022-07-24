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
            if (col_end == 1) return data[0][fieldsTable];
            else
            {
                int elementov = default;
                double result = double.NaN;
                var legalValues = from value in data
                                  let marker = value[fieldsTable]
                                  where !double.IsNaN(marker)
                                  where !double.IsPositiveInfinity(marker)
                                  select marker;
                // Определяем диапазон выборки
                if (col_end <= period)
                {
                    col_start = 0;
                    elementov = col_end;
                }
                else
                {
                    col_start = col_end - period;
                    elementov = period;
                }
                //Выбираем из набора данных только то что нам нужно с учетом диапазона выборки
                var sampleData = legalValues.Skip(col_start).Take(elementov).ToArray();
                //Опредеяем среднее значение по выборке
                if (sampleData.Count() != 0)
                {
                    result = sampleData.Average();
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

            if (data.Count <= col_end || data.Count == 0)
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

                var returnData = legalValues.Skip(col_end - 1).Take(2).ToArray();

                if (returnData.Count() > 1)
                {
                    result = (double)((returnData[1] * 100 / returnData[0]) - 100);
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
                int elementov = default;
                // Определяем диапазон выборки
                if (col_end <= period)
                {
                    col_start = 0;
                    elementov = col_end;
                }
                else
                {
                    col_start = col_end - period;
                    elementov = period;
                }

                //Считаем среднее значение по выборке 365 значений
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

                    var returnData = legalValues.Skip(col_start).Take(elementov).ToArray();

                    if (returnData.Count() > 0)
                    {
                        result = (double)Math.Sqrt((double)(returnData.Sum() / (returnData.Count() - 1)));
                        //result = (double)Math.Sqrt(returnData.Average());
                        //var ghf = (double)Math.Sqrt((double)(returnData.Sum() / (returnData.Count() - 1)));
                        //Console.WriteLine($"Math.Sqrt = {ghf}");
                    }
                }

                return result;
            }
        }


        public void PoweLaw(List<MathDbKript> data, List<MathDbKript> data_0)
        {
            for (int i = 0; i < data.Count(); i++)
            {
                PowerLawFields(data, i);
                ////Вычисляем xValues
                //double[] xValues = new double[i + 1];
                ////xValues[0] = 0;
                //double[] yValues = new double[i + 1];
                ////yValues[0] = 0;

                //for (int j = 0; j < i + 1; j++)
                //{
                //    //Вычисляем xValues
                //    xValues[j] = Math.Log10(data[j].Ind);
                //    //Вычисляем yValues
                //    yValues[j] = Math.Log10(data[j].AdjClose);

                //}

                //if (i > 0 && i < data.Count())
                //{
                //    //Линейниая регрессия
                //    double rSquared, intercept, slope;
                //    LinearRegression rS = new LinearRegression();
                //    rS.LinearRegressions(xValues, yValues, out rSquared, out intercept, out slope);

                //    var predictedValue = (slope * Math.Log10(data[i].Ind)) + intercept; //посленее значение по оси xValues
                //    //Console.WriteLine($"PredictionValue: {predictedValue}");
                //    data[i - 1].PowerLaw = (double)(Math.Log10(data[i - 1].AdjClose) - predictedValue);
                //    //double result = double.IsNaN(predictedValue) ? 0 : (double)(Math.Log10(data[col_end].AdjClose - predictedValue));
                //    //double result = (double)(Math.Log10(data[i-1].AdjClose) - predictedValue);
                //    //if (double.IsNaN(result)) result = yValues[i];                
                //}

            }
            for (int i = 0; i < data.Count(); i++)
            {
                //Выполним проверку расчетногo значения 
                if (data[i].PowerLaw - data_0[i].PowerLaw > 0.00000001) 
                    Console.WriteLine($"Ind = {data[i].PowerLaw} а должно быть {data_0[i].PowerLaw}");
            }
        }

        public void PowerLawFields(List<MathDbKript> data, int index)
        {
            //Вычисляем xValues
            double[] xValues = new double[index + 1];
            //xValues[0] = 0;
            double[] yValues = new double[index + 1];
            //yValues[0] = 0;

            for (int j = 0; j < index; j++)
            {
                //Вычисляем xValues
                xValues[j] = Math.Log10(data[j].Ind);
                //Вычисляем yValues
                yValues[j] = Math.Log10(data[j].AdjClose);

            }

            if (index > 0 && index < data.Count())
            {
                //Линейниая регрессия
                double rSquared, intercept, slope;
                LinearRegression rS = new LinearRegression();
                rS.LinearRegressions(xValues, yValues, out rSquared, out intercept, out slope);

                var predictedValue = (slope * Math.Log10(data[index].Ind)) + intercept; //посленее значение по оси xValues
                data[index-1].PowerLaw = (double)(Math.Log10(data[index].AdjClose) - predictedValue);               
            }
        }
        public void NormalizationData(List<MathDbKript> data, List<MathDbKript> data_0)
        {

            for (int i = 0; i < data.Count; i++)
            {
                NormalizationDataFields(data, i);
                //foreach (AvgIndicator avgIndicator in Enum.GetValues(typeof(AvgIndicator)))
                //{
                //    if (avgIndicator != AvgIndicator.AVG)
                //        ProcessIndicator(data, i, avgIndicator);
                //}
            }
            //for (int i = 0; i < data.Count; i++)
            //{
            //    foreach (AvgIndicator avgIndicator in Enum.GetValues(typeof(AvgIndicator)))
            //    {
            //        if (avgIndicator != AvgIndicator.AVG)
            //            Выполним проверку расчетногo значения
            //            if (data[i][avgIndicator] - data_0[i][avgIndicator] > 0.00000001) Console.WriteLine($"PuellMultiple = {data[i][avgIndicator]} а должно быть {data_0[i][avgIndicator]}");
            //    }
            //}
        }

        public void NormalizationDataFields(List<MathDbKript> data, int index)
        {
            foreach (AvgIndicator avgIndicator in Enum.GetValues(typeof(AvgIndicator)))
            {
                if (avgIndicator != AvgIndicator.AVG)
                    ProcessIndicator(data, index, avgIndicator);
            }
        }
        public void ProcessIndicator(List<MathDbKript> data, int index, AvgIndicator indicator)
        {
            double sliceMin = default;
            double sliceMax = default;

            var legalValues = from value in data
                              let marker = value[indicator]
                              where !double.IsNaN(marker)
                              where marker != 0
                              where !double.IsPositiveInfinity(marker)
                              select marker;

            var slice = legalValues.Skip(0).Take(index).ToList();
            var element = data[index];
            if (slice.Count() != 0)
            {
                sliceMax = slice.Max();
                sliceMin = slice.Min();
                if (sliceMax - sliceMin != 0) element[indicator] = (double)(element[indicator] - sliceMin) / (sliceMax - sliceMin);
            }
        }
        public void AVG(List<MathDbKript> data, List<MathDbKript> resultAvg)
        {
            double result = default;
            //Считаем среднее знеачене по строке
            for (int index = 0; index < data.Count; index++)
            {
                double X_ = default;
                int rows = default;
                foreach (AvgIndicator _indicator in Enum.GetValues(typeof(AvgIndicator)))
                {
                    if (!double.IsNaN(data[index][_indicator]) &&
                        !double.IsPositiveInfinity(data[index][_indicator]) &&
                        _indicator != AvgIndicator.AVG)
                    {
                        X_ += data[index][_indicator];
                        rows += 1;
                    }
                }
                if (rows != 0) result = X_ / rows;
                data[index].AVG = result * (double)Math.Pow(index, 0.395);
                ProcessIndicator(data, index, AvgIndicator.AVG);
                //Выполним проверку расчетногo значения 
                if (data[index].AVG != resultAvg[index].AVG) Console.WriteLine($"AVG = {data[index].AVG}  а должно быть {resultAvg[index].AVG}");

            }
        }
    }

    public enum AvgIndicator
    {
        PuellMultiple,
        PowerLaw,
        Sharpe,
        Risk_MA_400,
        Mayer,
        Price_52w,
        AVG
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
