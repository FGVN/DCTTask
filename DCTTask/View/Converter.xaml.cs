using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DCTTask.Services;
using DCTTask.Model;
using DCTTask.ViewModel;

namespace DCTTask.View
{

    /// <summary>
    /// Interaction logic for Converter.xaml
    /// </summary>
    partial class Converter : UserControl
    {
        private ConverterViewModel viewModel;

        public Converter()
        {
            viewModel = new ConverterViewModel();
            viewModel.Result = 0;
            viewModel.Amount = 0;
            InitializeComponent();
            DataContext = viewModel;
        }

        private async void SearchFirstButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.from = await CoinCapApiClient.SearchByNameOrSymbolAsync(searchFirstTextBox.Text);
            SearchBoxHadnler(searchFirstTextBox, viewModel.from);
        }

        private async void SearchSecondButton_Click(object sender, RoutedEventArgs e)
        {

            viewModel.to = await CoinCapApiClient.SearchByNameOrSymbolAsync(searchSecondTextBox.Text);
            SearchBoxHadnler(searchSecondTextBox, viewModel.to);
        }

        private async void SearchBoxHadnler(TextBox textBox, object toFind)
        {

            if (toFind != null)
            {
                // Coin is found, set a green background color
                textBox.Background = new SolidColorBrush(Colors.LightGreen);
                textBox.Foreground = new SolidColorBrush(Colors.Black);
                textBox.Text = textBox.Text.ToUpper();
                viewModel.CalculateConvertation(amountTextBox.Text);
            }
            else
            {
                // Coin is not found, clear the TextBox and reset the background color
                if(textBox.Text != "")
                {
                    textBox.Background = new SolidColorBrush(Colors.Red);
                    textBox.Foreground = new SolidColorBrush(Colors.Black);
                    MessageBox.Show("Coin not found");
                    textBox.Clear();
                }
            }
        }

        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {

            if (viewModel.from != null && viewModel.to != null)
            {
                viewModel.Swap();

                searchFirstTextBox.Text = viewModel.from.symbol;
                searchSecondTextBox.Text = viewModel.to.symbol;

            }
        }


        private void AmountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            viewModel.CalculateConvertation(amountTextBox.Text);
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the back button click event (e.g., close the UserControl).
            Visibility = Visibility.Collapsed;
        }




    }
}
