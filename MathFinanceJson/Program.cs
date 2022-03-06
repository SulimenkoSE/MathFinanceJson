using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.Json.Serialization;
using MathJson;
using Newtonsoft.Json;
using System.Linq;

namespace MathFainanceJson
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            //Подключаем клас для проведения расчета по даннім полученнім из файла            
            ColumnAverage csd = new ColumnAverage();

            //Создаем новый рассчетный файл данных
            List<MathDbKript> bts_usd_R = new List<MathDbKript>();
            
            // Чтение данных из фала
            using (FileStream fs = new FileStream("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc-usd_python.json", FileMode.Open))
            {
                //Создаем файл по данным из файла указаны только данные по торгам
                List<KriptDbContext> bts_usd = await System.Text.Json.JsonSerializer.DeserializeAsync<List<KriptDbContext>>(fs);
                await fs.DisposeAsync();
                //Занимаемся рассчетами

                foreach (var index in bts_usd)
                {
                    int RowNewJson = index.Index;
                    Console.WriteLine($"Index: {RowNewJson}");

                    //Скопируем в рассчетный файл данные которые используем в расчетах
                    // Это значения по закрытию дня 
                    bts_usd_R.Add(new MathDbKript
                    (
                        //Изменяем индекс так ка в расчетах этот индекс начинается с 949
                        //Смотри файл btc-usd_python_0.json
                        index.Index + 949, 
                        index.Date,  
                        index.AdjClose

                    ));

                    //400MA - 400MArisk
                    bts_usd_R[RowNewJson].MA_400 = csd.Avarage(data: bts_usd_R, 400, RowNewJson);
                    if (bts_usd_R[RowNewJson].MA_400 != 0)
                    {
                        bts_usd_R[RowNewJson].Risk_MA_400 = (double)(bts_usd_R[RowNewJson].AdjClose / bts_usd_R[RowNewJson].MA_400);
                    }
                    //Console.WriteLine($"Risk_MA_400 = {bts_usd_R[RowNewJson].Risk_MA_400}");

                    ////200MA - Mayer
                    bts_usd_R[RowNewJson].MA_200 = csd.Avarage(data: bts_usd_R, 200, RowNewJson);
                    if (bts_usd_R[RowNewJson].MA_200 != 0)
                    { 
                        bts_usd_R[RowNewJson].Mayer = (double)(bts_usd_R[RowNewJson].AdjClose / bts_usd_R[RowNewJson].MA_200); 
                    }

                    //btcIssuance
                    bts_usd_R[RowNewJson].BtcIssuance = csd.btcIssuance(data: bts_usd_R, RowNewJson);

                    //usdIssuance
                    bts_usd_R[RowNewJson].UsdIssuance = (double)(bts_usd_R[RowNewJson].BtcIssuance * bts_usd_R[RowNewJson].AdjClose);

                    //MAusdIssuance
                    bts_usd_R[RowNewJson].MAusdIssuance = csd.usdIssuance(data: bts_usd_R, 365, RowNewJson);

                    //PuellMultiple
                    bts_usd_R[RowNewJson].PuellMultiple = (double)(bts_usd_R[RowNewJson].UsdIssuance / bts_usd_R[RowNewJson].MAusdIssuance);

                    //365MA - Price_52w 
                    bts_usd_R[RowNewJson].MA_365 = csd.Avarage(data: bts_usd_R, 365, RowNewJson);

                    //Price52W
                    if (bts_usd_R[RowNewJson].MA_365 != 0) bts_usd_R[RowNewJson].Price_52w = (double)(bts_usd_R[RowNewJson].AdjClose / bts_usd_R[RowNewJson].MA_365);
                    else bts_usd_R[RowNewJson].Price_52w = 0;

                    //Return
                    bts_usd_R[RowNewJson].Return = csd.ReturnPersant(data: bts_usd_R, RowNewJson);

                    //365Return%MA-1
                    bts_usd_R[RowNewJson].Return_MA_365_1 = csd.Return_365_MA_1(data: bts_usd_R, 365, RowNewJson);

                    //365Return%STD
                    bts_usd_R[RowNewJson].Return_365_STD = csd.Return_365_STD(data: bts_usd_R, RowNewJson);

                    //Shape
                    // ++++++++++++Проверить данные
                    if (bts_usd_R[RowNewJson].Return_365_STD != 0)
                    {
                        bts_usd_R[RowNewJson].Sharpe = (double)(bts_usd_R[RowNewJson].Return_MA_365_1 / 
                                                                bts_usd_R[RowNewJson].Return_365_STD);
                    }
                    else bts_usd_R[RowNewJson].Sharpe = 0;

                    //ind
                    bts_usd_R[RowNewJson].Ind = RowNewJson + 947;

                    //PowerLaw
                    bts_usd_R[RowNewJson].PowerLaw = csd.PoweLaw(data: bts_usd_R, RowNewJson);

                    if (RowNewJson == 5) break;
                }

                //произведем нормализацию данных после рассчета
                csd.NormalizationData(data: bts_usd_R);

                //Avg
                csd.AVG(data: bts_usd_R);

                //  Запишем данные в файл                 
                using (FileStream fs_write = new FileStream("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc_usd_sharp_00.json", FileMode.OpenOrCreate))
                {
                    //https://shikaku-sh.hatenablog.com/entry/c-sharp-how-to-support-json-nan-in-system-text-json
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
                        WriteIndented = true,
                    };
                    // сохранение данных
                    await System.Text.Json.JsonSerializer.SerializeAsync<List<MathDbKript>>(fs_write, bts_usd_R, options);
                    
                    await fs_write.DisposeAsync();
                }
            }   
        }
    }
}