using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Linq;

namespace DCTTask
{
    public partial class CoinData : UserControl
    {
        public event EventHandler BackButtonClicked;
        private DispatcherTimer updateTimer;
        private string[] _labels;
        public SeriesCollection SeriesCollection { get; set; }
        public CoinCapData CoinCapData { get; set; }
        private List<CandlestickData> candleData;
        public CoinData()
        {
            InitializeComponent();
            // Initialize and start the data update timer
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(5); // Update every 5 seconds (adjust as needed)
            updateTimer.Tick += async (sender, e) => await UpdateData();
            updateTimer.Start();

        }

        public string[] Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
            }
        }
        public async Task UpdateData()
        {
            //CoinCapData coinData = DataContext as CoinCapData;
            if (CoinCapData != null)
            {
                candleData = await CoinCapParse.GetCandlestickData(CoinCapData.symbol, "15m", 60);

                var ohlc = candleData.Select(data =>
                    new OhlcPoint((double)data.Open, (double)data.High, (double)data.Low, (double)data.Close));
                SeriesCollection = new SeriesCollection
                {
                    new CandleSeries
                    {
                        Values = new ChartValues<OhlcPoint>(ohlc)
                    }
                };

                // Generate labels based on the candle data opentime (assuming it's a Unix timestamp)
                Labels = candleData.Select(data =>
                {
                    DateTime timestamp = DateTimeOffset.FromUnixTimeMilliseconds(data.OpenTime).LocalDateTime;
                    return timestamp.ToString("HH:mm");
                }).ToArray();



                CoinCapData updatedData = await CoinCapParse.GetCoinById(CoinCapData.id);
                if (updatedData != null)
                {
                    CoinCapData = updatedData;
                }


                DataContext = this;
            }
        }

        // Back button click event handler
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}