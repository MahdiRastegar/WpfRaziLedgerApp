using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool Suspended = true;
        protected override void OnStartup(StartupEventArgs e)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTU4NUAzMjM3MkUzMTJFMzluT08wbzRnYm4zUlFDOVRzWVpYbUtuSEl0aUhTZmNMYjQxekhrV0NVRnlzPQ==");

            CultureInfo persianCulture = new CultureInfo("fa-IR");
            persianCulture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;

            Thread.CurrentThread.CurrentCulture = persianCulture;
            Thread.CurrentThread.CurrentUICulture = persianCulture;

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setting.txt")))
            {
                WpfRaziLedgerApp.MainWindow.ViewFormLeftRigth = bool.Parse(File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setting.txt")));
            }
            // نمایش اسپلش
            var splash = new SplashWindow();
            splash.Show();

            // زمان‌دهی (مثلاً 2 ثانیه) یا تا آماده شدن فرم اصلی
            Task.Delay(e.Args.Length==0? 2500:0).ContinueWith(_ =>
            {
                splash.Dispatcher.Invoke(() =>
                {
                    splash.Close();

                    if (e.Args.Length == 0)
                    {
                        var win = new winLogin();

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            win.ShowDialog();
#if !DEBUG
                    if (!Suspended)
                    {
                        App.Current.Shutdown();
                        string jsonArg = JsonSerializer.Serialize(WpfRaziLedgerApp.MainWindow.StatusOptions);

                         توجه: چون بعضی کاراکترها ممکنه برای command line مناسب نباشن، پیشنهاد می‌شه encode کنی:
                        string encodedArg = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonArg));

                         Start process with encoded argument
                        Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfRaziLedgerApp.exe"), encodedArg);
                    }
#else
                            WpfRaziLedgerApp.MainWindow.Current.Show();

#endif
                        }));
                    }
                    else
                    {

                        string encodedArg = e.Args[0];

                        // Decode
                        string jsonArg = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedArg));

                        // Deserialize
                        StatusOptions options = JsonSerializer.Deserialize<StatusOptions>(jsonArg);
                        WpfRaziLedgerApp.MainWindow.StatusOptions = options;

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var oldContext = WpfRaziLedgerApp.MainWindow.Current.statusBar.DataContext;
                            WpfRaziLedgerApp.MainWindow.Current.statusBar.DataContext = null;
                            WpfRaziLedgerApp.MainWindow.Current.statusBar.DataContext = oldContext;
                            foreach (var item in ((WpfRaziLedgerApp.MainWindow.Current.statusBar.Items[0] as StatusBarItem).Content as Grid).GetChildsOfType<TextBlock>())
                            {
                                var binding = item.GetBindingExpression(TextBlock.TextProperty);
                                binding?.UpdateTarget();
                            }
                            WpfRaziLedgerApp.MainWindow.Current.LoadUser(options.User.FkUserGroupId);
                            WpfRaziLedgerApp.MainWindow.Current.Show();
                        }));
                    }
                });
            });
            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(e.Exception.Message + e.Exception.InnerException?.Message, "خطای نرم افزار", MessageBoxButton.OK, MessageBoxImage.Error);

            Environment.Exit(0);
        }
        //public static string GetKeyPassword()
        //{
        //    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + "InquiriesModianSystemApp", true);
        //    if (key == null)
        //        key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\" + "InquiriesModianSystemApp");
        //    var x = "";
        //    try
        //    {
        //        x = key.GetValue("Password").ToString();
        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        key.Close();
        //    }
        //    return x;
        //}
        //public static void SetKeyPassword(string str)
        //{
        //    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\" + "InquiriesModianSystemApp", true);
        //    if (key == null)
        //        key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\" + "InquiriesModianSystemApp");

        //    key.SetValue("Password", str);

        //    key.Close();
        //}
    }
}
