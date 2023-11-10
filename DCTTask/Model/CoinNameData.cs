using System.Collections.Generic;

namespace DCTTask.Model 
{ 
    sealed class CoinNameData
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
    }

    sealed class CoinNameAssets
    {
        public List<CoinNameData> data { get; set; }
    }
}
