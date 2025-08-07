using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfRaziLedgerApp
{
    public class RowIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridRowHeaderCell rowHeaderCell)
            {
                // دریافت ردیف مرتبط با این HeaderCell
                var row = rowHeaderCell.RowIndex;

                // پیدا کردن اندیس ردیف فعلی در لیست ردیف‌های اصلی
                int rowIndex = row + 0; // شماره‌گذاری از 1 شروع شود
                if (rowHeaderCell.DataContext.GetType().Name.Contains("Detail"))
                {
                    return rowIndex;
                }

                if (rowHeaderCell.DataContext.GetType().Name.Contains("Header"))
                {
                    return (row+1)/2;
                }
                
                //
                return "";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
