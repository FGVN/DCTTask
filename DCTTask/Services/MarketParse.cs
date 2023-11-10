using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DCTTask.Model;

namespace DCTTask.Services
{
    public class MarketParse
    {
        public static async Task<List<MarketData>> GetMarket(string coinName)
        {

            var apiUrl = $"https://api.coincap.io/v2/assets/{coinName}/markets";
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(apiUrl);

                MarketRoot marketData = JsonConvert.DeserializeObject<MarketRoot>(response);

                return marketData.data;
            }
        }
    }
}
