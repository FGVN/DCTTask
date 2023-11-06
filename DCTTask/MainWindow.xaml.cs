using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DCTTask
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer dataUpdateTimer;
        private List<CoinCapData> cryptoData;
        private int currentPage = 1;
        private int itemsPerPage = 10; // Change this value as needed

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing; // Handle the Closing event

            // Initialize and start the data update timer
            dataUpdateTimer = new DispatcherTimer();
            dataUpdateTimer.Interval = TimeSpan.FromSeconds(5); // Update every 5 seconds
            dataUpdateTimer.Tick += DataUpdateTimer_Tick;
            dataUpdateTimer.Start();
        }

        private void ShowLoadingOverlay()
        {
            loadingOverlay.Visibility = Visibility.Visible;
        }

        private void HideLoadingOverlay()
        {
            loadingOverlay.Visibility = Visibility.Collapsed;
        }

        private async void DataUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Perform your data update or loading here
            await UpdateDataGrid();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initial data load
            await LoadDataAndDisplay();
            await CoinCapParse.GetCoinData(1, 100, false);
        }
        private async Task UpdateDataGrid()
        {
            cryptoData = await CoinCapParse.GetCoinData(currentPage, itemsPerPage, true);
            cryptoListView.ItemsSource = cryptoData;
        }
        private async Task LoadDataAndDisplay()
        {
            dataUpdateTimer.Stop();
            cryptoData = await CoinCapParse.GetCoinData(currentPage, itemsPerPage, false); // Use cached data
            cryptoListView.ItemsSource = cryptoData;
            dataUpdateTimer.Start();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                // Fetch the next page data
                currentPage--;
                LoadDataAndDisplay();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < 10)
            {
                // Fetch the next page data
                currentPage++;
                LoadDataAndDisplay();
            }
        }

        private void cryptoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop the dataUpdateTimer and release any resources
            dataUpdateTimer.Stop();
        }
    }
}
