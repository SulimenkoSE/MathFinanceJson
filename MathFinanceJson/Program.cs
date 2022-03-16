using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.Json.Serialization;
using MathJson;
//using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace MathFainanceJson
{
    internal class Program
    {
        /// <summary>
        /// Deserilization Json file
        /// options from //https://shikaku-sh.hatenablog.com/entry/c-sharp-how-to-support-json-nan-in-system-text-json
        /// options from //https://code-maze.com/introduction-system-text-json-examples/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<T> DeserializeJson<T>(string path) where T : new()
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                WriteIndented = true,
                IncludeFields = true
            };

            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (File.Exists(path) && stream.Length > 0)
                {
                    T obj = await JsonSerializer.DeserializeAsync<T>(stream, options);
                    await stream.DisposeAsync();
                    return obj;
                }
                else
                {
                    T obj = new T();
                    await JsonSerializer.SerializeAsync(stream, obj, options);
                    await stream.DisposeAsync();
                    return obj;
                }
            }
        }

        /// <summary>
        /// Serilization Json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task SerializeJson<T>(string path, T data)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                WriteIndented = true,
                IncludeFields = true
            };

            using (FileStream fs_write = new FileStream(path, FileMode.OpenOrCreate))
            {
                // сохранение данных
                await JsonSerializer.SerializeAsync<T>(fs_write, data, options);
                await fs_write.DisposeAsync();
            }
        }


        public static async Task Main(string[] args)
        {
            //Подключаем клас для проведения расчета по даннім полученнім из файла            
            ColumnAverage csd = new ColumnAverage();

            //Создаем новый рассчетный файл данных
            List<MathDbKript> bts_usd_R = new List<MathDbKript>();
            
            //Создаем рассчетный файл данных 4000MA
            // Чтение данных из файла создано для проверки рассчетов
            //List<MathDbKript> bts_usd_R_400MA = await DeserializeJson<List<MathDbKript>>("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc-usd_python_0.json");
            List<MathDbKript> bts_usd_R_400MA = await DeserializeJson<List<MathDbKript>>("C:/2021/VS2022/MathFinanceJson/MathFinanceJson/Json_File/btc-usd_python_0.json");

            //Создаем файл по данным из файла указаны только данные по торгам
            //List<KriptDbContext> bts_usd = await DeserializeJson<List<KriptDbContext>>("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc-usd_python.json");
            List<KriptDbContext> bts_usd = await DeserializeJson<List<KriptDbContext>>("C:/2021/VS2022/MathFinanceJson/MathFinanceJson/Json_File/btc-usd_python.json");

            double[] adjValue = new double[500];

            //Занимаемся рассчетами исходя из полученных данных из файла btc-usd_python.json
            //Расчитанные данные записываем в файл btc_usd_sharp_00.json
            foreach (var index in bts_usd)
            {
                int RowNewJson = index.Index;
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

                //Console.WriteLine($"Index: {RowNewJson + 947}  data_r {bts_usd_R[RowNewJson].Date} data_200  {bts_usd_R_400MA[RowNewJson].Date}");
                Console.WriteLine($"{bts_usd_R[RowNewJson].AdjClose}");
                //adjValue[RowNewJson] = bts_usd_R_400MA[RowNewJson].AdjClose;          

                //400MA
                bts_usd_R[RowNewJson].MA_400 = csd.Avarage(data: bts_usd_R, 400, RowNewJson, FieldsMathDbKript.AdjClose);
                //Выполним проверку расчетногo значения 
                //if (bts_usd_R[RowNewJson].MA_400 != bts_usd_R_400MA[RowNewJson].MA_400) Console.WriteLine($"MA_400 = {bts_usd_R[RowNewJson].MA_400}  а должно быть {bts_usd_R_400MA[RowNewJson].MA_400}");


                //400MArisk
                if (bts_usd_R[RowNewJson].MA_400 != 0 && !double.IsNaN(bts_usd_R[RowNewJson].MA_400))
                {
                    bts_usd_R[RowNewJson].Risk_MA_400 = (double)(bts_usd_R[RowNewJson].AdjClose / bts_usd_R[RowNewJson].MA_400);
                }
                //Выполним проверку расчетногo значения 
                //if (bts_usd_R[RowNewJson].Risk_MA_400 != bts_usd_R_400MA[RowNewJson].Risk_MA_400) Console.WriteLine($"Risk_MA_400 = {bts_usd_R[RowNewJson].Risk_MA_400}");

                //200MA
                bts_usd_R[RowNewJson].MA_200 = csd.Avarage(data: bts_usd_R, 200, RowNewJson, FieldsMathDbKript.AdjClose);
                //Выполним проверку расчетногo значения 
                //if (bts_usd_R[RowNewJson].MA_200 != bts_usd_R_400MA[RowNewJson].MA_200) Console.WriteLine($"MA_200 = {bts_usd_R[RowNewJson].MA_200}  а должно быть {bts_usd_R_400MA[RowNewJson].MA_200}");


                //Mayer
                if (bts_usd_R[RowNewJson].MA_200 != 0 && !double.IsNaN(bts_usd_R[RowNewJson].MA_200))
                {
                    bts_usd_R[RowNewJson].Mayer = (double)(bts_usd_R[RowNewJson].AdjClose / bts_usd_R[RowNewJson].MA_200);
                }
                //Выполним проверку расчетногo значения 
                //if (bts_usd_R[RowNewJson].Mayer != bts_usd_R_400MA[RowNewJson].Mayer) Console.WriteLine($"Mayer = {bts_usd_R[RowNewJson].Mayer}  а должно быть {bts_usd_R_400MA[RowNewJson].MA_200}");

                ////Console.WriteLine($"MA_200 = {bts_usd_R[RowNewJson].MA_200}");

                //btcIssuance
                bts_usd_R[RowNewJson].BtcIssuance = csd.btcIssuance(data: bts_usd_R, RowNewJson);
                //Выполним проверку расчетногo значения 
                //if (bts_usd_R[RowNewJson].BtcIssuance != bts_usd_R_400MA[RowNewJson].BtcIssuance) Console.WriteLine($"BtcIssuance = {bts_usd_R[RowNewJson].BtcIssuance}  а должно быть {bts_usd_R_400MA[RowNewJson].BtcIssuance}");

                //usdIssuance
                bts_usd_R[RowNewJson].UsdIssuance = (double)(bts_usd_R[RowNewJson].BtcIssuance * bts_usd_R[RowNewJson].AdjClose);
                //Выполним проверку расчетногo значения 
                //if (bts_usd_R[RowNewJson].UsdIssuance != bts_usd_R_400MA[RowNewJson].UsdIssuance) Console.WriteLine($"UsdIssuance = {bts_usd_R[RowNewJson].UsdIssuance}  а должно быть {bts_usd_R_400MA[RowNewJson].UsdIssuance}");


                //MAusdIssuance
                bts_usd_R[RowNewJson].MAusdIssuance = csd.Avarage(data: bts_usd_R, 365, RowNewJson, FieldsMathDbKript.UsdIssuance);
                //Выполним проверку расчетногo значения 
                //if (bts_usd_R[RowNewJson].MAusdIssuance != bts_usd_R_400MA[RowNewJson].MAusdIssuance) Console.WriteLine($"MAusdIssuance = {bts_usd_R[RowNewJson].MAusdIssuance}  а должно быть {bts_usd_R_400MA[RowNewJson].MAusdIssuance}");

                //PuellMultiple
                bts_usd_R[RowNewJson].PuellMultiple = (double)(bts_usd_R[RowNewJson].UsdIssuance / bts_usd_R[RowNewJson].MAusdIssuance);

                //365MA 
                bts_usd_R[RowNewJson].MA_365 = csd.Avarage(data: bts_usd_R, 365, RowNewJson, FieldsMathDbKript.AdjClose);
                //Выполним проверку расчетногo значения 
                if (bts_usd_R[RowNewJson].MA_365 != bts_usd_R_400MA[RowNewJson].MA_365) Console.WriteLine($"MA_365 = {bts_usd_R[RowNewJson].MA_365}  а должно быть {bts_usd_R_400MA[RowNewJson].MA_365}");


                //Price_52w 
                if (bts_usd_R[RowNewJson].MA_365 != 0 && !double.IsNaN(bts_usd_R[RowNewJson].MA_365))
                {
                    bts_usd_R[RowNewJson].Price_52w = (double)(bts_usd_R[RowNewJson].AdjClose / bts_usd_R[RowNewJson].MA_365);
                }
                //Console.WriteLine($"Price_52w = {bts_usd_R[RowNewJson].Price_52w}");

                //Return
                bts_usd_R[RowNewJson].Return = csd.ReturnPersant(data: bts_usd_R, RowNewJson);
                //Выполним проверку расчетногo значения 
                //if (bts_usd_R[RowNewJson].Return != bts_usd_R_400MA[RowNewJson].Return) Console.WriteLine($"Return = {bts_usd_R[RowNewJson].Return}  а должно быть {bts_usd_R_400MA[RowNewJson].Return}");


                //365Return%MA-1
                bts_usd_R[RowNewJson].Return_MA_365_1 = csd.Avarage(data: bts_usd_R, 365, RowNewJson, FieldsMathDbKript.Return) - 1;
                //Выполним проверку расчетногo значения 
                if (bts_usd_R[RowNewJson].Return_MA_365_1 != bts_usd_R_400MA[RowNewJson].Return_MA_365_1) Console.WriteLine($"Return_MA_365_1 = {bts_usd_R[RowNewJson].Return_MA_365_1}  а должно быть {bts_usd_R_400MA[RowNewJson].Return_MA_365_1}");

                //365Return%STD
                bts_usd_R[RowNewJson].Return_365_STD = csd.Return_365_STD(data: bts_usd_R, 365, RowNewJson, FieldsMathDbKript.Return);

                //Shape
                // ++++++++++++Проверить данные
                if (bts_usd_R[RowNewJson].Return_365_STD != 0 && !double.IsNaN(bts_usd_R[RowNewJson].Return_365_STD))
                {
                    bts_usd_R[RowNewJson].Sharpe = (double)(bts_usd_R[RowNewJson].Return_MA_365_1 /
                                                            bts_usd_R[RowNewJson].Return_365_STD);
                }
                //else bts_usd_R[RowNewJson].Sharpe = 0;

                //ind
                bts_usd_R[RowNewJson].Ind = RowNewJson + 947;

                if (RowNewJson == 10) break;
            }
            //PowerLaw
            csd.PoweLaw(data: bts_usd_R);
            //произведем нормализацию данных после рассчета
            csd.NormalizationData(data: bts_usd_R);

            //Avg
            csd.AVG(data: bts_usd_R, bts_usd_R_400MA);

            //  Запишем данные в файл
            //await SerializeJson<List<MathDbKript>>("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc_usd_sharp_00.json", bts_usd_R);
            await SerializeJson<List<MathDbKript>>("C:/2021/VS2022/MathFinanceJson/MathFinanceJson/Json_File/btc_usd_sharp_01.json", bts_usd_R);
            //await SerializeJson<double[]>("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/adj.json", adjValue);
            //<List<MathDbKript>>(fs_write, bts_usd_R, options);
            //using (FileStream fs_write = new FileStream("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc_usd_sharp_00.json", FileMode.OpenOrCreate))
            //using (FileStream fs_write = new FileStream("C:/2021/VS2022/MathFinanceJson/MathFinanceJson/Json_File/btc_usd_sharp_00.json", FileMode.OpenOrCreate))
            //{
            //    //https://shikaku-sh.hatenablog.com/entry/c-sharp-how-to-support-json-nan-in-system-text-json
            //    var options = new System.Text.Json.JsonSerializerOptions
            //    {
            //        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            //        WriteIndented = true,
            //    };
            //    // сохранение данных
            //    await JsonSerializer.SerializeAsync<List<MathDbKript>>(fs_write, bts_usd_R, options);

            //    await fs_write.DisposeAsync();
            //}   
        }
    }
}