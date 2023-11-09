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
using System.ComponentModel;

namespace DCTTask
{
    public partial class CoinData : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler BackButtonClicked;
        private DispatcherTimer updateTimer;
        private List<CandlestickData> candleData;

        private CoinCapData _coincapdata;
        public CoinCapData CoinCapData
        {
            get { return _coincapdata; }
            set 
            {
                _coincapdata = value;
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
        public CoinData()
        {
            InitializeComponent();
            // Initialize and start the data update timer
            TimeFrame = "15m";
            InitTimer();
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
            //CoinCapData coinData = DataContext as CoinCapData;
            if (CoinCapData != null)
            {
                CoinCapData updatedData = await CoinCapParse.GetCoinById(CoinCapData.id);
                if (updatedData != null)
                {
                    CoinCapData = updatedData;
                }

                if(isChartInit == false)
                {
                    isChartInit = true;
                    await UpdateChart();
                }

                DataContext = this;
            }
        }

        private async Task UpdateChart()
        {
            try
            {
                candleData = await CoinCapParse.GetCandlestickData(CoinCapData.symbol, TimeFrame, 60);
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Sorry, coin is not supported");
                updateTimer.Stop();
                Visibility = Visibility.Collapsed;
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

        private void TimeframeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)timeframeSelector.SelectedItem;
            TimeFrame = selectedItem.Content.ToString();
            if(CoinCapData != null)
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