using System;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using DCTTask.Model;

namespace DCTTask.Services
{
    public class CoinCapParse
    {
        public static List<string> coinNames { get; set; }
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
            using (HttpClient httpClient = new HttpClient()) 
            {
                var apiUrl = $"https://api.coincap.io/v2/assets/{id}";
                var response = await httpClient.GetStringAsync(apiUrl);

                return JsonConvert.DeserializeObject<CoinCapResponse>(response).data;
            }
        }

        public static async Task<CoinCapData> GetCoinBySymbol(string symbol)
        {
            // Find the coin data with the given symbol in the cached data

            CoinCapData coinData = cachedData.FirstOrDefault(x => x.symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            if (coinData != null)
            {
                // If the coin data is found, fetch more details using its id
                using (HttpClient httpClient = new HttpClient())
                {
                    string apiUrl = $"https://api.coincap.io/v2/assets/{coinData.id}";
                    var response = await httpClient.GetStringAsync(apiUrl);
                    return JsonConvert.DeserializeObject<CoinCapResponse>(response).data;
                }
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
            using (HttpClient httpClient = new HttpClient())
            {
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
        }

        private static async Task<List<string>> FetchCoinNamesFromAPI()
        {
            // Fetch coin names from the API
            var apiUrl = "https://api.coincap.io/v2/assets";
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(apiUrl);
                var assets = JsonConvert.DeserializeObject<CoinNameAssets>(response);

                return assets.data.Select(x => x.id).ToList();
            }
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
