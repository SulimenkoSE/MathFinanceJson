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

                //Занимаемся рассчетами
                
                foreach (var index in bts_usd)
                {
                    int RowNewJson = index.Index;
                    //Скопируем в рассчетный файл данные которые используем в расчетах
                    // Это значения по закрытию дня 
                    bts_usd_R.Add(new MathDbKript
                    {
                        //Изменяем индекс так ка в расчетах этот индекс начинается с 949
                        //Смотри файл btc-usd_python_0.json
                        Index = index.Index + 949, 
                        Date = index.Date,  
                        AdjClose = index.AdjClose

                    });

                    //400MA - 400MArisk
                    bts_usd_R[RowNewJson].MA_400 = csd.Avarage(data: bts_usd_R, 400, RowNewJson);
                    if (bts_usd_R[RowNewJson].MA_400 != 0) bts_usd_R[RowNewJson].Risk_MA_400 = (double)(bts_usd_R[RowNewJson].AdjClose / bts_usd_R[RowNewJson].MA_400);
                    else bts_usd_R[RowNewJson].Risk_MA_400 = 0;

                    //200MA - Mayer
                    bts_usd_R[RowNewJson].MA_200 = csd.Avarage(data: bts_usd_R, 200, RowNewJson);
                    if (bts_usd_R[RowNewJson].MA_200 != 0) bts_usd_R[RowNewJson].Mayer = (double)(bts_usd_R[RowNewJson].AdjClose / bts_usd_R[RowNewJson].MA_200);
                    else bts_usd_R[RowNewJson].Mayer = 0;
                    
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
                    bts_usd_R[RowNewJson].Return_MA_365_1 = csd.Return_365_MA_1(data: bts_usd_R, RowNewJson);

                    //365Return%STD
                    bts_usd_R[RowNewJson].Return_365_STD = csd.Return_365_STD(data: bts_usd_R, RowNewJson);
                    
                    //Shape
                    // ++++++++++++Проверить данные
                    if (bts_usd_R[RowNewJson].Return_365_STD != 0) bts_usd_R[RowNewJson].Sharpe = (double)(bts_usd_R[RowNewJson].Return_MA_365_1 / bts_usd_R[RowNewJson].Return_365_STD);
                    else bts_usd_R[RowNewJson].Sharpe = 0;

                    //ind
                    bts_usd_R[RowNewJson].Ind = RowNewJson + 947;

                    //PowerLaw
                    bts_usd_R[RowNewJson].PowerLaw = csd.PoweLaw(data: bts_usd_R, RowNewJson);

                    Console.WriteLine($"Index: {RowNewJson}");
                }

                //произведем нормализацию данных после рассчета
                csd.NormalizationData(data: bts_usd_R);
                
                //Запишем данные в файл 
                using (FileStream fs_write = new FileStream("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc_usd_sharp.json", FileMode.OpenOrCreate))
                {
                    //await System.Text.Json.JsonSerializer.SerializeAsync(fs_write, bts_usd_R);
                    await GetJson(bts_usd_R);
                    await fs_write.DisposeAsync();
                }
                await fs.DisposeAsync();
            }
            
        }
        private static async Task<string> GetJson(object obj)
        {
            string json = string.Empty;
            using (var stream = new MemoryStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj, obj.GetType());
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }
    }

}