using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace WpfRaziLedgerApp
{
    public class DeleteRowStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00821A")) { Opacity = .17 };
            if (value is AcDocumentDetail acDocumentDetail)
                //if (acDocumentDetail.Moein == null&& acDocumentDetail.Preferential == null)
                //    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00821A")) { Opacity = .17 };
                //else
                {
                    if(acDocumentDetail.Debtor>0)
                    {
                        return new SolidColorBrush(Colors.SkyBlue) { Opacity = .4 };
                    }
                    else if (acDocumentDetail.Creditor > 0)
                    {
                        return new SolidColorBrush(Colors.DarkOliveGreen) { Opacity = .3 };
                    }
                }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
