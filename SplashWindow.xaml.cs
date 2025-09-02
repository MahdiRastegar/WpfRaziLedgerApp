using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        private DispatcherTimer timer;
        private int dotCount = 0;
        public SplashWindow()
        {
            InitializeComponent();

            Loaded += SplashWindow_Loaded; // وقتی پنجره آماده شد
        }

        private void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // اول یه بار فوری متن رو تغییر بده
            UpdateLoadingText();

            // بعد Timer رو برای ادامه انیمیشن روشن کن
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(750);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateLoadingText();
        }


        private void UpdateLoadingText()
        {
            dotCount = (dotCount + 1) % 4;
            if (dotCount == 0)
                dotCount = 1;
            string dots = string.Join(" ", Enumerable.Repeat(".", dotCount));
            LoadingText.Text = (dotCount > 0 ? " " + dots : "")+ " در حال اتصال";
        }
    }
}
