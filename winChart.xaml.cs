using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfRaziLedgerApp.Reports.Charts;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winChart.xaml
    /// </summary>
    public partial class winChart : Window
    {
        public ObservableCollection<CommodityTotalTonnage> CommodityTotalTonnages {  get; set; } 
        public ObservableCollection<CityTotalTonnage> CityTotalTonnages {  get; set; } 
        public ObservableCollection<MonthlyTotalSale> MonthlyTotalSales {  get; set; } 
        public ObservableCollection<MonthlyTotalTonnage> MonthlyTonnageTotals {  get; set; } 
        public winChart()
        {
            InitializeComponent();
            cmbType.SelectedIndex = 0;
        }

        private void cmbType_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbType.SelectedIndex)
            {
                case 0:
                    chart1.Visibility = Visibility.Visible;
                    chart2.Visibility = Visibility.Collapsed;
                    chart3.Visibility = Visibility.Collapsed;
                    chart4.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    chart1.Visibility = Visibility.Collapsed;
                    chart2.Visibility = Visibility.Visible;
                    chart3.Visibility = Visibility.Collapsed;
                    chart4.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    chart1.Visibility = Visibility.Collapsed;
                    chart2.Visibility = Visibility.Collapsed;
                    chart3.Visibility = Visibility.Visible;
                    chart4.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    chart1.Visibility = Visibility.Collapsed;
                    chart2.Visibility = Visibility.Collapsed;
                    chart4.Visibility = Visibility.Visible;
                    chart3.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }
    }
}
