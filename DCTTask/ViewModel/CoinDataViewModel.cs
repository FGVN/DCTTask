using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using DCTTask.Services;
using DCTTask.Model;

namespace DCTTask.ViewModel
{
    public class CoinDataViewModel : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler BackButtonClicked;
        public DispatcherTimer updateTimer;
        private List<CandlestickData> candleData; 
        public class MarketData
        {
            public string Exchange { get; set; }
            public string Pair { get; set; }
            public string Price { get; set; }
        }


        private CoinCapData _coin;
        public CoinCapData Coin
        {
            get { return _coin; }
            set 
            {
                _coin = value;
                OnPropertyChanged(nameof(CoinCapData));
            }
        }

        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection)); // Notify UI of changes
            }
        }

        private string[] _labels;
        public string[] Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels)); // Notify UI of changes
            }
        }
        private string _timeFrame;

        public string TimeFrame
        {
            get { return _timeFrame; }
            set
            {
                if (_timeFrame != value)
                {
                    _timeFrame = value;
                    OnPropertyChanged(nameof(TimeFrame));
                }
            }
        }

        private bool isChartInit = false;
        public CoinDataViewModel(CoinCapData coin)
        {
            Coin = coin;
            // Initialize and start the data update timer
            TimeFrame = "15m";
            UpdateData();
            InitTimer();

        }

        public async Task<List<MarketData>> InitMarketData()
        {

            // Create a list of MarketData objects
            List<MarketData> marketDataList = (await MarketParse.GetMarket(Coin.id)).Select(x => new MarketData
            {
                Exchange = x.exchangeId,
                Pair = x.baseSymbol + "/" + x.quoteSymbol,
                Price = x.priceUsd
            }).ToList();

            // Set the ItemsSource of the ListView to the list of MarketData objects
            return marketDataList;
        }

        private async void InitTimer()
        {
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(0); // Update for the first time instantly
            updateTimer.Tick += async (sender, e) => await UpdateData();
            updateTimer.Start(); //And then update every 5 seconds
            updateTimer.Interval = TimeSpan.FromSeconds(5);
        }

        public async Task UpdateData()
        {
            if (Coin != null)
            {
                CoinCapData updatedData = await CoinCapParse.GetCoinById(Coin.id);
                if (updatedData != null)
                {
                    Coin = updatedData;
                }

                if(isChartInit == false)
                {
                    isChartInit = true;
                    await InitMarketData();
                }

                DataContext = this;
            }
        }

        private async Task UpdateChart()
        {
            try
            {
                candleData = await BinanceParse.GetCandlestickData(Coin.symbol, TimeFrame, 60);
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Sorry, coin chart is not supported");
                return;
            }

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
                    return timestamp.ToString("HH:mm\ndd:MM:yyyy");
                }).ToArray();

                DataContext = this;
            
        }

        public void TimeframeSelector_SelectionChanged(ComboBoxItem selectedItem)
        {
            TimeFrame = selectedItem.Content.ToString();
            if(Coin != null)
                UpdateChart();
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Back button click event handler
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            updateTimer.Stop();
            Visibility = Visibility.Collapsed;
        }
    }
}