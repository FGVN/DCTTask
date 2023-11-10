using DCTTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCTTask.Services
{

    public static class CoinCapApiClient
    {
        private static List<string> coinNames { get; set; }

        public static async Task<List<string>> GetCoinNamesAsync(bool refreshData)
        {
            if (coinNames == null || !coinNames.Any() || refreshData)
            {
                coinNames = await FetchAndCacheCoinNamesAsync();
            }

            return coinNames;
        }

        private static async Task<CoinCapData> GetCoinBySymbolAsync(string symbol, List<CoinCapData> cachedData)
        {
            CoinCapData coinData = cachedData.FirstOrDefault(x => x.symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            if (coinData != null)
            {
                return await CoinCapApiService.GetCoinByIdAsync(coinData.id);
            }

            return null;
        }

        public static async Task<CoinCapData> SearchByNameOrSymbolAsync(string searchInput)
        {
            CoinCapData foundCoin = null;

            var coinNames = await GetCoinNamesAsync(false);

            if (coinNames.Where(x => string.Compare(x, searchInput, StringComparison.OrdinalIgnoreCase) == 0).Any())
            {
                foundCoin = await GetCoinBySymbolAsync(searchInput, CoinCapParse.CachedData);
            }
            else if (await GetCoinBySymbolAsync(searchInput, CoinCapParse.CachedData) != null)
            {
                foundCoin = await GetCoinBySymbolAsync(searchInput, CoinCapParse.CachedData);
            }

            return foundCoin;
        }

        private static async Task<List<string>> FetchAndCacheCoinNamesAsync()
        {
            List<string> names = await CoinCapApiService.FetchCoinNamesFromApiAsync();
            CoinCacheManager.CacheData(CoinCapParse.CoinNamesCacheFile, names);
            return names;
        }
    }
}
