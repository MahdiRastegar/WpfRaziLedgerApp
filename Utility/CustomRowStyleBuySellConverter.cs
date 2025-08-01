using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using WpfRaziLedgerApp;

namespace WpfRaziLedgerApp
{
    public class CustomRowStyleBuySellConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            if (parameter?.ToString() == "Foreground")
            {
                // بخش رنگ متن
            }
            else
            {
                var detail = value as BuyRemittanceDetail;
                if (detail == null)
                    return DependencyProperty.UnsetValue;

                if (detail.SellOrBuy == "خرید")
                {
                    // طیف آبی ملایم
                    var linearGradientBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0.5, 0),
                        EndPoint = new Point(0.5, 1)
                    };
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF90CAF9"), 0));  // آبی خیلی ملایم
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFBBDEFB"), 0.5)); // آبی کم‌رنگ‌تر
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFE3F2FD"), 1));   // آبی خیلی روشن
                    return linearGradientBrush;
                }
                else if (detail.SellOrBuy == "فروش")
                {
                    // طیف صورتی ملایم
                    var linearGradientBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0.5, 0),
                        EndPoint = new Point(0.5, 1)
                    };
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFF8BBD0"), 0));  // صورتی خیلی ملایم
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFCE4EC"), 0.5)); // صورتی خیلی کم‌رنگ
                    linearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFF0F4"), 1));   // تقریباً سفید با ته‌صورتی
                    return linearGradientBrush;
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
