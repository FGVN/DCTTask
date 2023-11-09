using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DCTTask
{
    /// <summary>
    /// Interaction logic for Converter.xaml
    /// </summary>
    public partial class Converter : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private CoinCapData from;
        private CoinCapData to;
        private double result;
        private double amount;
        public double Amount
        {
            get { return amount; }
            set
            {
                if (amount != value)
                {
                    amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public double Result
        {
            get { return result; }
            set
            {
                if (result != value)
                {
                    result = value;
                    OnPropertyChanged("Result");
                }
            }
        }

        public Converter()
        {
            Result = 0;
            Amount = 0;
            InitializeComponent();
            DataContext = this; // Set the DataContext to the current instance
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the back button click event (e.g., close the UserControl).
            Visibility = Visibility.Collapsed;
        }

        private async void SearchFirstButton_Click(object sender, RoutedEventArgs e)
        {
            from = await Search(searchFirstTextBox.Text.Trim().ToLower());
            if (from != null)
            {
                // Coin is found, set a green background color
                searchFirstTextBox.Background = new SolidColorBrush(Colors.LightGreen);
                CalculateConvertation();
            }
            else
            {
                // Coin is not found, clear the TextBox and reset the background color
                searchFirstTextBox.Clear();
                searchFirstTextBox.Background = new SolidColorBrush(Colors.White);
            }
        }

        private async void SearchSecondButton_Click(object sender, RoutedEventArgs e)
        {
            to = await Search(searchSecondTextBox.Text.Trim().ToLower());
            if (to != null)
            {
                // Coin is found, set a green background color
                searchSecondTextBox.Background = new SolidColorBrush(Colors.LightGreen);
                CalculateConvertation();
            }
            else
            {
                // Coin is not found, clear the TextBox and reset the background color
                searchSecondTextBox.Clear();
                searchSecondTextBox.Background = new SolidColorBrush(Colors.White);
            }
        }

        private async Task<CoinCapData> Search(string searchInput)
        {
            if (!string.IsNullOrWhiteSpace(searchInput))
            {
                // Perform the search using the input (name or ID)
                CoinCapData foundCoin = null;

                // Use async/await to avoid blocking the UI thread
                if (CoinCapParse.GetCoinNames(false).Result.Select(x => x.ToLower()).Contains(searchInput))
                {
                    foundCoin = await CoinCapParse.GetCoinById(searchInput);
                }
                else if (await CoinCapParse.GetCoinBySymbol(searchInput) != null)
                {
                    foundCoin = await CoinCapParse.GetCoinBySymbol(searchInput);
                }

                if (foundCoin != null)
                {
                    return foundCoin;
                }
                else
                {
                    MessageBox.Show("Coin not found.");
                }
            }
            return null;
        }

        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {
            CoinCapData bufcoin = from;
            from = to;
            to = bufcoin;


            double bufamount = Amount;
            Amount = Result;
            Result = bufamount;

            CalculateConvertation();
            if(from != null)
                searchFirstTextBox.Text = from.symbol;
            if(to != null)
                searchSecondTextBox.Text = to.symbol;
        }

        private void AmountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateConvertation();
        }

        private void CalculateConvertation()
        {
            if (from != null && to != null && amountTextBox.Text != "")
            {
                if(!double.TryParse(amountTextBox.Text, out double newAmount))
                {
                    amountTextBox.Clear();
                    MessageBox.Show("Amount should be a number");
                }

                if (newAmount >= 0)
                {
                    Amount = newAmount;
                    Result = (Convert.ToDouble(from.priceUsd) * Amount) / Convert.ToDouble(to.priceUsd);
                }
                else
                {
                    amountTextBox.Clear();
                    MessageBox.Show("Amount should be greater or equal to 0");
                }
            }
        }


        // Notify property changed
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
