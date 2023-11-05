using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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
        public string priceUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string changePercent24Hr { get; set; }
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
   

    public class CoinCapParse
    {
        private static List<string> coinNames { get; set; }
        private static async Task<List<string>> GetCoinNames()
        {
            //Sending http request and getting response from api
            var apiUrl = "https://api.coincap.io/v2/assets";
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(apiUrl);

            //Converting response into the json and returning list of coins
            var assets = JsonSerializer.Deserialize<CoinNameAssets>(response);

            return assets.data.Select(x => x.id).ToList();
        }

        public static async Task<List<CoinCapData>> GetCoinData(int currentPage, int amount)
        {
            var result = new List<CoinCapData>();
            if (coinNames == null)
                coinNames = await GetCoinNames();
            HttpClient httpClient = new HttpClient();

            // Calculate the start and end indices for the current page
            int startIndex = (currentPage - 1) * amount;
            int endIndex = startIndex + amount;

            // Ensure endIndex does not exceed the total number of coinNames
            endIndex = Math.Min(endIndex, coinNames.Count);

            // Fetch data for the coins in the specified range
            var coinsToFetch = coinNames.GetRange(startIndex, endIndex - startIndex);

            foreach (var coin in coinsToFetch)
            {
                var apiUrl = $"https://api.coincap.io/v2/assets/{coin}";
                var response = await httpClient.GetStringAsync(apiUrl);

                CoinCapData coinData = JsonSerializer.Deserialize<CoinCapResponse>(response).data;

                if (coinData != null)
                    result.Add(coinData);
            }
            return result;
        }

    }


}
