using DCTTask.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DCTTask.Services
{
    public static class CoinCapApiService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<CoinCapData> GetCoinByIdAsync(string id)
        {
            var apiUrl = $"https://api.coincap.io/v2/assets/{id}";
            var response = await httpClient.GetStringAsync(apiUrl);
            return JsonConvert.DeserializeObject<CoinCapResponse>(response).data;
        }

        public static async Task<List<string>> FetchCoinNamesFromApiAsync()
        {
            var apiUrl = "https://api.coincap.io/v2/assets";
            var response = await httpClient.GetStringAsync(apiUrl);
            var assets = JsonConvert.DeserializeObject<CoinNameAssets>(response);

            return assets.data.Select(x => x.id).ToList();
        }
    }

}
