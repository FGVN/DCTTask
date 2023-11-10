using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DCTTask.Model;

namespace DCTTask.Services
{
    class BinanceParse
    {
        public static async Task<List<CandlestickData>> GetCandlestickData(string symbol, string interval, int limit)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string apiUrl = $"https://api.binance.com/api/v3/klines?symbol={symbol.ToUpper()}USDT&interval={interval}&limit={limit}";

                var response = await httpClient.GetStringAsync(apiUrl);
                JArray rawData = JArray.Parse(response);

                // Create a list to store the parsed CandlestickData objects
                List<CandlestickData> candlestickDataList = new List<CandlestickData>();

                // Loop through the raw data and parse it into CandlestickData objects
                foreach (JArray item in rawData)
                {
                    // Check that the inner array has at least 11 elements before parsing
                    if (item.Count >= 11)
                    {
                        var candlestickData = new CandlestickData
                        {
                            OpenTime = Convert.ToInt64(item[0]),
                            Open = Convert.ToDecimal(item[1]),
                            High = Convert.ToDecimal(item[2]),
                            Low = Convert.ToDecimal(item[3]),
                            Close = Convert.ToDecimal(item[4]),
                            Volume = Convert.ToDecimal(item[5]),
                            CloseTime = Convert.ToInt64(item[6]),
                            QuoteAssetVolume = Convert.ToDecimal(item[7]),
                            NumberOfTrades = Convert.ToInt32(item[8]),
                            TakerBuyBaseAssetVolume = Convert.ToDecimal(item[9]),
                            TakerBuyQuoteAssetVolume = Convert.ToDecimal(item[10]),
                            Unused = item[11].ToString()
                        };

                        candlestickDataList.Add(candlestickData);
                    }
                }
                return candlestickDataList;
            }
        }
    }
}
