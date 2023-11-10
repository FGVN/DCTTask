using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using DCTTask.Model;
using DCTTask.ViewModel;
using DCTTask.Services;

namespace DCTTask.View
{
    partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            DataContext = viewModel; 
            viewModel.InitialLoad(cryptoListView);
            Closing += MainWindow_Closing; // Handle the Closing event
            UpdatePageLabel();
        }


        private async void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CurrentPage > 1)
            {
                // Fetch the previous page data
                viewModel.CurrentPage--;
                UpdatePageLabel();
                await viewModel.LoadDataAndDisplay();
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CurrentPage < 10)
            {
                // Fetch the next page data
                viewModel.CurrentPage++;
                UpdatePageLabel();
                await viewModel.LoadDataAndDisplay();
            }
        }

        private void cryptoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cryptoListView.SelectedItem != null)
            {

                // Create an instance of the CoinData user control and set its data context
                CoinData coinDataControl = new CoinData((CoinCapData)cryptoListView.SelectedItem);

                // Set the content of the coinDataContainer to display the CoinData user control
                coinDataContainer.Content = coinDataControl;
                coinDataContainer.Visibility = Visibility.Visible;
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchInput = searchTextBox.Text.Trim().ToLower();
            CoinCapData toSearch = await CoinCapApiClient.SearchByNameOrSymbolAsync(searchInput);

            if (toSearch != null)
            {
                // Display the found coin using the CoinData UserControl
                coinDataContainer.Content = new CoinData(toSearch);
                coinDataContainer.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Coin not found.");
            }

            searchTextBox.Clear();
        }

        private void ConverterButton_Click(object sender, RoutedEventArgs e)
        {
            Converter converter = new Converter();
            converterContainer.Content = converter;
            converterContainer.Visibility = Visibility.Visible;
        }



        private void SwitchToWhiteTheme()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("View/LightTheme.xaml", UriKind.Relative)
            });
        }

        private void SwitchToDarkTheme()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("View/DarkTheme.xaml", UriKind.Relative)
            });
        }

        private void SwitchThemeButton_Click(object sender, RoutedEventArgs e)
        {
            // Check the current theme, and switch to the opposite theme
            if (Application.Current.Resources.MergedDictionaries.Any(d => d.Source.ToString() == "View/LightTheme.xaml"))
            {
                // Remove the WhiteTheme
                var whiteTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.ToString() == "View/LightTheme.xaml");
                if (whiteTheme != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(whiteTheme);
                }
                SwitchToDarkTheme();
            }
            else
            {
                // Remove the DarkTheme
                var darkTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.ToString() == "View/DarkTheme.xaml");
                if (darkTheme != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(darkTheme);
                }
                SwitchToWhiteTheme();
            }
        }

        private void UpdatePageLabel()
        {
            pageLabel.Text = $"Page {viewModel.CurrentPage} of 10"; 
        }


        private void cryptoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection changes
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save data about theme
            if (Application.Current.Resources.MergedDictionaries.Any(d => d.Source.ToString() == "View/LightTheme.xaml"))
            {
                File.WriteAllText(App.themeCacheDirectory, "0");
            }
            else
            {
                File.WriteAllText(App.themeCacheDirectory, "1");
            }
            // Stop the dataUpdateTimer and release any resources
            App.Current.Shutdown();
            Process.GetCurrentProcess().Kill();
        }
    }
}
