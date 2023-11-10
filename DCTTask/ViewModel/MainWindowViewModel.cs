using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using DCTTask.Model;
using DCTTask.Services;

namespace DCTTask.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherTimer dataUpdateTimer;
        private List<CoinCapData> cryptoData;
        private int currentPage;
        private int itemsPerPage = 10; // Number of items to display
        private int maxPages = 10;
        private ListView cryptoListView;

        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public MainWindowViewModel()
        {
            CurrentPage = 1;
            InitTimer();
        }

        private async void InitTimer()
        {
            dataUpdateTimer = new DispatcherTimer();
            dataUpdateTimer.Interval = TimeSpan.FromSeconds(5); // Update every 5 seconds
            dataUpdateTimer.Tick += async (sender, e) =>
            {
                await UpdateDataGrid();
                await LoadDataAndDisplay();
            };
            dataUpdateTimer.Start();
        }

        private async void DataUpdateTimer_Tick(object sender, EventArgs e)
        {
            await UpdateDataGrid();
            await LoadDataAndDisplay();
        }

        public async Task InitialLoad(ListView _cryptoListView)
        {
            cryptoListView = _cryptoListView;
            cryptoData = await CoinCapParse.GetCoinDataAsync(CurrentPage, itemsPerPage, false);
            cryptoListView.ItemsSource = cryptoData;
            await CoinCapParse.GetCoinDataAsync(1, itemsPerPage * maxPages, false);
        }

        public async Task UpdateDataGrid()
        {
            await CoinCapParse.GetCoinDataAsync(CurrentPage, itemsPerPage, true);
            cryptoListView.ItemsSource = cryptoData;
        }

        public async Task LoadDataAndDisplay()
        {
            cryptoData = await CoinCapParse.GetCoinDataAsync(CurrentPage, itemsPerPage, false);
            cryptoListView.ItemsSource = cryptoData;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
