using System;
using System.Windows;
using System.Windows.Controls;
using DCTTask.ViewModel;
using DCTTask.Model;

namespace DCTTask.View
{
    /// <summary>
    /// Interaction logic for CoinData.xaml
    /// </summary>
    partial class CoinData : UserControl
    {
        private CoinDataViewModel viewModel;
        public CoinData(CoinCapData coin)
        {
            viewModel = new CoinDataViewModel(coin);
            DataContext = viewModel;

            InitializeComponent();

            InitMarket();

            Chart.AnimationsSpeed = TimeSpan.FromSeconds(0);

        }

        private async void InitMarket()
        {
            marketsListView.ItemsSource = await viewModel.GetMarketData();
        }
        // Back button click event handler
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.updateTimer.Stop();
            viewModel.updateTimer = null;
            Visibility = Visibility.Collapsed;
        }

        private void TimeframeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)timeframeSelector.SelectedItem;
            viewModel.TimeframeSelector_SelectionChanged(selectedItem);
        }
    }
}