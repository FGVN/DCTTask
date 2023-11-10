using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DCTTask.Model;

namespace DCTTask.Services
{

    public static class CoinCapParse
    {
        public static List<CoinCapData> CachedData { get; set; }

        public static string CoinNamesCacheFile = "coinNames.json";
        private static string CoinDataCacheFile = "coinData.json";

        public static async Task<List<CoinCapData>> GetCoinDataAsync(int currentPage, int amount, bool refreshData)
        {
            if (CachedData == null)
            {
                CachedData = CoinCacheManager.LoadCachedData(CoinDataCacheFile);
            }

            if (CachedData == new List<CoinCapData>())
            {
                CachedData = await FetchDataFromApiAsync(1, 100);
            }

            int startIndex = (currentPage - 1) * amount;
            int endIndex = Math.Min(startIndex + amount, CachedData.Count);

            if (refreshData || CachedData.Count < currentPage * amount)
            {
                var dataToFetch = await FetchDataFromApiAsync(currentPage, amount);

                if (startIndex < CachedData.Count)
                {
                    CachedData.RemoveRange(startIndex, endIndex - startIndex);
                    CachedData.InsertRange(startIndex, dataToFetch);
                }
                else
                {
                    CachedData.AddRange(dataToFetch);
                }

                CoinCacheManager.CacheData(CoinDataCacheFile, CachedData);
            }

            var dataSubset = CachedData.GetRange(startIndex, endIndex - startIndex);
            return dataSubset;
        }

        private static async Task<List<CoinCapData>> FetchDataFromApiAsync(int currentPage, int amount)
        {
            var coinNames = await CoinCapApiClient.GetCoinNamesAsync(true);

            List<CoinCapData> fetchData = new List<CoinCapData>();

            if (coinNames.Count >= currentPage * amount)
            {
                var coinsToFetch = coinNames.GetRange((currentPage - 1) * amount, amount);

                foreach (var coin in coinsToFetch)
                {
                    fetchData.Add(await CoinCapApiService.GetCoinByIdAsync(coin));
                }

                CoinCacheManager.CacheData(CoinDataCacheFile, fetchData);
            }

            return fetchData;
        }
    }
}
