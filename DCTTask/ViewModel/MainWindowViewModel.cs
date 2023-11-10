using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DCTTask.Services;
using DCTTask.Model;

namespace DCTTask.ViewModel
{
    public class MainWindowViewModel : Window
    {
        private DispatcherTimer dataUpdateTimer;
        private List<CoinCapData> cryptoData;
        public int currentPage = 1;
        private int itemsPerPage = 10; // Number of items to display
        private int maxPages = 10; 
        private static ListView cryptoListView;
        

        private void InitTimer()
        {
            dataUpdateTimer = new DispatcherTimer();
            dataUpdateTimer.Interval = TimeSpan.FromSeconds(0); 
            dataUpdateTimer.Tick += DataUpdateTimer_Tick; 
            dataUpdateTimer.Interval = TimeSpan.FromSeconds(5);// Update every 5 seconds
            dataUpdateTimer.Start();
        }

        private async void DataUpdateTimer_Tick(object sender, EventArgs e)
        {
            await UpdateDataGrid();

            //We do that so the updated data wont overlap with the data needed for the page when they are cahnged
            await LoadDataAndDisplay();
        }

        public async Task InitialLoad(ListView _cryptoListView)
        {
            // Initial data load
            InitTimer();

            cryptoListView = _cryptoListView;
            cryptoData = await CoinCapParse.GetCoinData(currentPage, itemsPerPage, false); 
            cryptoListView.ItemsSource = cryptoData;
            await CoinCapParse.GetCoinData(1, itemsPerPage * maxPages, false);
        }
        public async Task UpdateDataGrid()
        {
            cryptoData = await CoinCapParse.GetCoinData(currentPage, itemsPerPage, true);
            cryptoListView.ItemsSource = cryptoData;
        }
        public async Task LoadDataAndDisplay()
        {
            dataUpdateTimer.Stop();
            cryptoData = await CoinCapParse.GetCoinData(currentPage, itemsPerPage, false); // Use cached data
            cryptoListView.ItemsSource = cryptoData;
            dataUpdateTimer.Start();
        }


        public async Task<CoinCapData> Search(string searchInput)
        {
            CoinCapData foundCoin = null;

            if ((await CoinCapParse.GetCoinNames(false)).Select(x => x.ToLower()).Contains(searchInput))
            {
                foundCoin = await CoinCapParse.GetCoinById(searchInput);
            }
            else if (await CoinCapParse.GetCoinBySymbol(searchInput) != null)
            {
                foundCoin = await CoinCapParse.GetCoinBySymbol(searchInput);
            }

            return foundCoin;
        }
    }
}
