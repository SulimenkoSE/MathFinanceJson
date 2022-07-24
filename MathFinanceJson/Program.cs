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
            List<MathDbKript> bts_usd_R_400MA = await DeserializeJson<List<MathDbKript>>("C:/2021/VS2022/MathFinanceJson/MathFinanceJson/Json_File/PowerLaw_python.json");
            //List<MathDbKript> bts_usd_R_400MA = await DeserializeJson<List<MathDbKript>>("C:/2021/VS2022/MathFinanceJson/MathFinanceJson/Json_File/Normalization_python.json");


            //Создаем файл по данным из файла указаны только данные по торгам
            //List<KriptDbContext> bts_usd = await DeserializeJson<List<KriptDbContext>>("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc-usd_python.json");
            List<KriptDbContext> bts_usd = await DeserializeJson<List<KriptDbContext>>("C:/2021/VS2022/MathFinanceJson/MathFinanceJson/Json_File/BTC_USD_phyton_START.json");

            //double[] adjValue = new double[500];

            //Занимаемся рассчетами исходя из полученных данных из файла btc-usd_python.json
            //Расчитанные данные записываем в файл btc_usd_sharp_01.json
            foreach (var index in bts_usd)
            {
                int RowNewJson = index.Index - 949;
                //Скопируем в рассчетный файл данные которые используем в расчетах
                // Это значения по закрытию дня 
                bts_usd_R.Add(new MathDbKript
                (
                    //Изменяем индекс так ка в расчетах этот индекс начинается с 949
                    //Смотри файл btc-usd_python_0.json
                    index.Index,
                    index.Date,
                    index.AdjClose

                ));
            }
            //Подключаем клас для проведения расчета по даннім полученнім из файла 
            SequenceCalculations sc = new SequenceCalculations();
            foreach (var index in bts_usd_R)
            {
                int RowNewJson = index.Index - 949;
                sc.Algoritm(bts_usd_R, bts_usd_R_400MA, RowNewJson);
                if (RowNewJson == 10) break;
            }
            sc.TestSequenceCalculations(data: bts_usd_R, data_0: bts_usd_R_400MA);
            

            //  Запишем данные в файл
            //await SerializeJson<List<MathDbKript>>("H:/Projects/MathFinanceJson/MathFinanceJson/MathFinanceJson/Json_File/btc_usd_sharp_00.json", bts_usd_R);
            await SerializeJson<List<MathDbKript>>("C:/2021/VS2022/MathFinanceJson/MathFinanceJson/Json_File/btc_usd_sharp_02.json", bts_usd_R);
        }
    }
}