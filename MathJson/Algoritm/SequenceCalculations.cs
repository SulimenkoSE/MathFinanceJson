using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathJson
{
    public class SequenceCalculations
    {
        public void Algoritm(List<MathDbKript> data, List<MathDbKript> data_0, int index)
        {
            //Подключаем клас для проведения расчета по даннім полученнім из файла            
            ColumnAverage csd = new ColumnAverage();
            //400MA
            data[index].MA_400 = csd.Avarage(data: data, 400, index, FieldsMathDbKript.AdjClose);

            //400MArisk
            if (data[index].MA_400 != 0 && !double.IsNaN(data[index].MA_400))
            {
                data[index].Risk_MA_400 = (double)(data[index].AdjClose / data[index].MA_400);
            }
            //TestSequenceCalculationsFields(data, data_0, index, FieldsMathDbKript.Risk_MA_400);

            //200MA
            data[index].MA_200 = csd.Avarage(data: data, 200, index, FieldsMathDbKript.AdjClose);

            //Mayer
            if (data[index].MA_200 != 0 && !double.IsNaN(data[index].MA_200))
            {
                data[index].Mayer = (double)(data[index].AdjClose / data[index].MA_200);
            }

            //btcIssuance
            data[index].BtcIssuance = csd.btcIssuance(data: data, index);
            //Выполним проверку расчетногo значения 
            //if (data[index].BtcIssuance != data_400MA[index].BtcIssuance) Console.WriteLine($"BtcIssuance = {data[index].BtcIssuance}  а должно быть {data_400MA[index].BtcIssuance}");

            //usdIssuance
            data[index].UsdIssuance = (double)(data[index].BtcIssuance * data[index].AdjClose);

            //MAusdIssuance
            data[index].MAusdIssuance = csd.Avarage(data: data, 365, index, FieldsMathDbKript.UsdIssuance);

            //PuellMultiple
            data[index].PuellMultiple = (double)(data[index].UsdIssuance / data[index].MAusdIssuance);

            //365MA 
            data[index].MA_365 = csd.Avarage(data: data, 365, index, FieldsMathDbKript.AdjClose);

            //Price_52w 
            if (data[index].MA_365 != 0 && !double.IsNaN(data[index].MA_365))
            {
                data[index].Price_52w = (double)(data[index].AdjClose / data[index].MA_365);
            }

            //Return
            data[index].Return = csd.ReturnPersant(data: data, index);

            //365Return%MA-1
            if (index < data.Count - 1) data[index + 1].Return_MA_365_1 = csd.Avarage(data: data, 365, index, FieldsMathDbKript.Return) - 1;

            //365Return%STD
            if (index < data.Count - 1) data[index + 1].Return_365_STD = csd.Return_365_STD(data: data, 365, index, FieldsMathDbKript.Return);

            //Shape
            // ++++++++++++Проверить данные
            if (data[index].Return_365_STD != 0 && !double.IsNaN(data[index].Return_365_STD))
            {
                data[index].Sharpe = (double)(data[index].Return_MA_365_1 / data[index].Return_365_STD);
            }

            //ind
            data[index].Ind = index + 947;

            //PowerLaw
            csd.PowerLawFields(data: data, index);

            //произведем нормализацию данных после рассчета
            //csd.NormalizationDataFields(data: data, index);
        }
        public void TestSequenceCalculations(List<MathDbKript> data, List<MathDbKript> data_0)
        {
            foreach (FieldsMathDbKript indicator in Enum.GetValues(typeof(FieldsMathDbKript)))
            {
                for (int i = 0; i < data.Count; i++)
                {
                    TestSequenceCalculationsFields(data, data_0, i, indicator);
                }
            }
        }
        public void TestSequenceCalculationsFields(List<MathDbKript> data, List<MathDbKript> data_0, int index, FieldsMathDbKript indicator)
        {
            if (indicator != FieldsMathDbKript.AVG &&
                        indicator != FieldsMathDbKript.Date &&
                        indicator != FieldsMathDbKript.Ind &&
                        indicator != FieldsMathDbKript.Index)
            {
                //Выполним проверку расчетногo значения
                if (data[index][indicator] - data_0[index][indicator] > 0.00000001)
                    Console.WriteLine($"Index = {index} >>> {indicator.ToString()}  = {data[index][indicator]} а должно быть {data_0[index][indicator]}");
            }
        }
    }
}
