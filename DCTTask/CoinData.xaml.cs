using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DCTTask
{
    public partial class CoinData : UserControl
    {
        public event EventHandler BackButtonClicked;
        private DispatcherTimer updateTimer;
        public CoinData()
        {
            InitializeComponent();
            // Initialize and start the data update timer
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(5); // Update every 5 seconds (adjust as needed)
            updateTimer.Tick += async (sender, e) => await UpdateData();
            updateTimer.Start();
        }

        public async Task UpdateData()
        {
            CoinCapData coinData = DataContext as CoinCapData;
            if (coinData != null)
            {
                CoinCapData updatedData = await CoinCapParse.GetCoinById(coinData.id);
                if (updatedData != null)
                {
                    DataContext = updatedData;
                }
            }
        }

        // Back button click event handler
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
