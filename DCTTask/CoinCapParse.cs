using System;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace DCTTask
{
    public class CoinCapResponse
    {
        public CoinCapData data { get; set; }
        public double timestamp { get; set; }
    }

    public class CoinCapData
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }

        private string _price { get; set; }

        public string priceUsd
        {
            get
            {
                return _price;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value[0] != '0')
                    _price = value.Substring(0, value.IndexOf('.') + 3);
                else
                {
                    int count = 2;
                    while (count < value.Length && (!char.IsDigit(value[count]) || value[count] == '0'))
                    {
                        count++;
                    }
                    if (count + 4 < value.Length)
                        _price = value.Substring(0, count+4);
                    else
                        _price = value.Substring(0, value.Length);
                }
            }
        }

        private string _volume;
        public string volumeUsd24Hr { 
            get 
            { 
                return _volume;
            }  
            set 
            {
                _volume = value.Substring(0, value.IndexOf('.') + 3); 
            } 
        }


        private string _percentChange { get; set; }
        public string changePercent24Hr {
            get
            {
                return _percentChange;
            } 
            set
            {
                _percentChange = value.Substring(0, value.IndexOf('.') + 3);
            }
        }
    }

    sealed class CoinNameAssets
    {
        public List<CoinNameData> data { get; set; }
    }

    sealed class CoinNameData
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
    }

    public class CandlestickData
    {
        public long OpenTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public long CloseTime { get; set; }
        public decimal QuoteAssetVolume { get; set; }
        public int NumberOfTrades { get; set; }
        public decimal TakerBuyBaseAssetVolume { get; set; }
        public decimal TakerBuyQuoteAssetVolume { get; set; }
        public string Unused { get; set; }
    }

    public class CoinCapParse
    {
        private static List<string> coinNames { get; set; }
        private static List<CoinCapData> cachedData { get; set; }
        private static string CacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CacheDirectory");

        // Define cache file names for coin names and coin data
        private static string CoinNamesCacheFile = "coinNames.json";
        private static string CoinDataCacheFile = "coinData.json";

        public static async Task<List<string>> GetCoinNames(bool refreshData)
        {
            // Check if you have the coin names in cache or if you want to refresh data
            if (coinNames == null || !coinNames.Any() || refreshData)
            {
                // Fetch the coin names from the API
                coinNames = await FetchAndCacheCoinNames(CoinNamesCacheFile);
            }

            return coinNames;
        }

        public static async Task<CoinCapData> GetCoinById(string id)
        {
            HttpClient httpClient = new HttpClient();
            var apiUrl = $"https://api.coincap.io/v2/assets/{id}";
            var response = await httpClient.GetStringAsync(apiUrl);

            return JsonConvert.DeserializeObject<CoinCapResponse>(response).data;
        }

        public static async Task<CoinCapData> GetCoinBySymbol(string symbol)
        {
            // Find the coin data with the given symbol in the cached data

            CoinCapData coinData = cachedData.FirstOrDefault(x => x.symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            if (coinData != null)
            {
                // If the coin data is found, fetch more details using its id
                HttpClient httpClient = new HttpClient();
                string apiUrl = $"https://api.coincap.io/v2/assets/{coinData.id}";
                var response = await httpClient.GetStringAsync(apiUrl);
                return JsonConvert.DeserializeObject<CoinCapResponse>(response).data;
            }

            // Return null if the coin data with the given symbol is not found
            return null;
        }

        public static async Task<List<CoinCapData>> GetCoinData(int currentPage, int amount, bool refreshData)
        {
            // Initialize cached data if not already done   
            if (cachedData == null)
            {
                cachedData = LoadCachedData(CoinDataCacheFile);
            }
            if(cachedData == new List<CoinCapData>())
            {
                cachedData = await FetchDataFromAPI(1, 100);
            }


            int startIndex = (currentPage - 1) * amount;
            int endIndex = Math.Min(startIndex + amount, cachedData.Count);

            // Check if you have the data needed in the cache or if you want to refresh data
            if (refreshData || cachedData.Count < currentPage * amount)
            {
                // Fetch the required data
                var dataToFetch = await FetchDataFromAPI(currentPage, amount);

                // Update the cache with the new data

                // If startIndex is within the range of the cached data, update it
                if (startIndex < cachedData.Count)
                {
                    cachedData.RemoveRange(startIndex, endIndex - startIndex);
                    cachedData.InsertRange(startIndex, dataToFetch);
                }
                else
                {
                    // If startIndex is beyond the range, just add the new data
                    cachedData.AddRange(dataToFetch);
                }

                // Cache the updated data
                CacheData(CoinDataCacheFile, cachedData);
            }

            // Return the requested subset of data from the cache
            var dataSubset = cachedData.GetRange(startIndex, endIndex - startIndex);
            return dataSubset;
        }

        public static async Task<List<CandlestickData>> GetCandlestickData(string symbol, string interval, int limit)
        {
            HttpClient httpClient = new HttpClient();
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
        
        private static async Task<List<string>> FetchAndCacheCoinNames(string cacheFileName)
        {
            // Fetch coin names from the API
            List<string> names = await FetchCoinNamesFromAPI();

            // Cache the data
            CacheData(cacheFileName, names);

            return names;
        }

        private static async Task<List<CoinCapData>> FetchDataFromAPI(int currentPage, int amount)
        {
            // Fetch coin names first
            if (coinNames == null || !coinNames.Any())
            {
                coinNames = await GetCoinNames(true);
            }

            // Fetch data from the API based on currentPage and amount
            List<CoinCapData> fetchData = new List<CoinCapData>();
            HttpClient httpClient = new HttpClient();

            // Make sure the coinNames list has enough items
            if (coinNames.Count >= currentPage * amount)
            {
                var coinsToFetch = coinNames.GetRange((currentPage - 1) * amount, amount);

                foreach (var coin in coinsToFetch)
                {
                    var apiUrl = $"https://api.coincap.io/v2/assets/{coin}";
                    var response = await httpClient.GetStringAsync(apiUrl);

                    CoinCapData coinData = JsonConvert.DeserializeObject<CoinCapResponse>(response).data;

                    if (coinData != null)
                        fetchData.Add(coinData);
                }

                // Cache the fetched data
                CacheData(CoinDataCacheFile, fetchData);
            }

            // Return the fetched data
            return fetchData;
        }

        private static async Task<List<string>> FetchCoinNamesFromAPI()
        {
            // Fetch coin names from the API
            var apiUrl = "https://api.coincap.io/v2/assets";
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(apiUrl);
            var assets = JsonConvert.DeserializeObject<CoinNameAssets>(response);

            return assets.data.Select(x => x.id).ToList();
        }

        private static List<CoinCapData> LoadCachedData(string fileName)
        {
            string filePath = Path.Combine(CacheDirectory, fileName);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<CoinCapData>>(json);
            }

            return new List<CoinCapData>();
        }

        private static void CacheData<T>(string fileName, T data)
        {
            // Create the cache directory if it doesn't exist
            if (!Directory.Exists(CacheDirectory))
            {
                Directory.CreateDirectory(CacheDirectory);
            }

            string filePath = Path.Combine(CacheDirectory, fileName);
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, json);
        }
    }
}
