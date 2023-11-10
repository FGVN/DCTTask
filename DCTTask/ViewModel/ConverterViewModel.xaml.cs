using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DCTTask.Services;
using DCTTask.Model;

namespace DCTTask.ViewModel
{
    public partial class ConverterViewModel : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CoinCapData from;
        public CoinCapData to;
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


        public void Swap()
        {
            CoinCapData bufcoin = from;
            from = to;
            to = bufcoin;


            double bufamount = Amount;
            Amount = Result;
            Result = bufamount;
        }


        public void CalculateConvertation(string amountString)
        {
            if (from != null && to != null && amountString != "")
            {
                if (!double.TryParse(amountString, out double newAmount))
                {
                    MessageBox.Show("Amount should be a number");
                }
                if (newAmount >= 0)
                {
                    Amount = newAmount;
                    Result = (Convert.ToDouble(from.priceUsd) * Amount) / Convert.ToDouble(to.priceUsd);
                }
                else
                {
                    MessageBox.Show("Amount should be greater or equal to 0");
                }
            }
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
