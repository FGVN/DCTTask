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

            // Initialize and start the data update timer
            dataUpdateTimer = new DispatcherTimer();
            dataUpdateTimer.Interval = TimeSpan.FromSeconds(5); // Update every 5 seconds
            dataUpdateTimer.Tick += DataUpdateTimer_Tick;
            dataUpdateTimer.Start();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initial data load
            await LoadDataAndDisplay();
        }

        private async void DataUpdateTimer_Tick(object sender, EventArgs e)
        {
            // Periodically update data
            await LoadDataAndDisplay();
        }

        private async Task LoadDataAndDisplay()
        {
            cryptoData = await CoinCapParse.GetCoinData(currentPage, itemsPerPage);
            UpdateDataGrid();
        }

        private async void UpdateDataGrid()
        {
            cryptoDataGrid.ItemsSource = await CoinCapParse.GetCoinData(currentPage, itemsPerPage);
        }



        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateDataGrid();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
           // if (currentPage < Math.Ceiling((double)cryptoData.Count / itemsPerPage))
            
                currentPage++;
                UpdateDataGrid();
            
        }


        private void cryptoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes
        }
    }
}
