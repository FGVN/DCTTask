namespace DCTTask.Model
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
                        _price = value.Substring(0, count + 4);
                    else
                        _price = value.Substring(0, value.Length);
                }
            }
        }

        private string _volume;
        public string volumeUsd24Hr
        {
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
        public string changePercent24Hr
        {
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
}
