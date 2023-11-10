using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            viewModel.from = await viewModel.Search(searchFirstTextBox.Text.Trim().ToLower());

            if (viewModel.from != null)
            {
                // Coin is found, set a green background color
                searchFirstTextBox.Background = new SolidColorBrush(Colors.LightGreen);
                searchFirstTextBox.Foreground = new SolidColorBrush(Colors.Black);
                viewModel.CalculateConvertation(amountTextBox.Text);
            }
            else
            {
                // Coin is not found, clear the TextBox and reset the background color
                searchFirstTextBox.Background = new SolidColorBrush(Colors.Red);
                searchFirstTextBox.Foreground = new SolidColorBrush(Colors.Black);
                searchFirstTextBox.Clear();
            }
        }

        private async void SearchSecondButton_Click(object sender, RoutedEventArgs e)
        {

            viewModel.to = await viewModel.Search(searchSecondTextBox.Text.Trim().ToLower());

            if (viewModel.to != null)
            {
                // Coin is found, set a green background color
                searchSecondTextBox.Background = new SolidColorBrush(Colors.LightGreen);
                searchSecondTextBox.Foreground = new SolidColorBrush(Colors.Black);
                viewModel.CalculateConvertation(amountTextBox.Text);
            }
            else
            {
                // Coin is not found, clear the TextBox and reset the background color
                searchSecondTextBox.Background = new SolidColorBrush(Colors.Red);
                searchSecondTextBox.Foreground = new SolidColorBrush(Colors.Black);
                searchSecondTextBox.Clear();
            }
        }

        private void SwapButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Swap();

            if (viewModel.from != null)
                searchFirstTextBox.Text = viewModel.from.symbol;
            else
                searchFirstTextBox.Clear();

            if (viewModel.to!= null)
                searchSecondTextBox.Text = viewModel.to.symbol;
            else
                searchSecondTextBox.Clear();
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
