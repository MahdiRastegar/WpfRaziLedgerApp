using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.TextInputLayout;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfRaziLedgerApp.Interfaces;
using WpfRaziLedgerApp.Windows.toolWindows;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winSettingCode.xaml
    /// </summary>
    public partial class winSettingCode : Window, ITabForm
    {
        public winSettingCode()
        {
            InitializeComponent();
            MainWindow.Current.Effect = new BlurEffect() { Radius = 4 };
        }
        private bool AnyIdValue(wpfrazydbContext db,Guid guid)
        {
            if (db.RecieveMoneyDetails.Any(y => y.FkMoeinId == guid) || db.RecieveMoneyDetails.Any(y => y.FkPreferentialId == guid) ||
                db.PaymentMoneyDetails.Any(y => y.FkMoeinId == guid) || db.PaymentMoneyDetails.Any(y => y.FkPreferentialId == guid) ||
                db.CheckRecieveEvents.Any(y => y.FkMoeinId == guid) || db.CheckRecieveEvents.Any(y => y.FkPreferentialId == guid) ||
                db.CheckPaymentEvents.Any(y => y.FkMoeinId == guid) || db.CheckPaymentEvents.Any(y => y.FkPreferentialId == guid))
                return true;
            return false;
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in stack.Children)
            {
                if (item is GroupBox groupBox)
                {
                    foreach (var im in (groupBox.Content as DockPanel).Children)
                    {
                        (im as SfTextInputLayout).HasError = false;
                    }
                }
                else
                    (item as SfTextInputLayout).HasError = false;
            }
            List<SfTextInputLayout> sfTextInputs = new List<SfTextInputLayout>();
            var error = false;
            foreach (var item in stack.Children)
            {
                if (item is GroupBox groupBox)
                {
                    foreach (var im in (groupBox.Content as DockPanel).Children)
                    {
                        sfTextInputs.Add(im as SfTextInputLayout);
                        var textBox = (im as SfTextInputLayout).InputView as System.Windows.Controls.TextBox;
                        if (textBox.Text.Trim() == "")
                        {
                            (im as SfTextInputLayout).HasError = true;
                            error = true;
                        }
                    }
                }
                else
                {
                    sfTextInputs.Add(item as SfTextInputLayout);
                    var textBox = (item as SfTextInputLayout).InputView as System.Windows.Controls.TextBox;
                    if (textBox.Text.Trim() == "")
                    {
                        (item as SfTextInputLayout).HasError = true;
                        error = true;
                    }
                }
            }
            if (error)
                return;
            Mouse.OverrideCursor = Cursors.Wait;
            using var db = new wpfrazydbContext();
            if (sfTextInputs[0].Hint == "درصد مالیات")
            {
                if (MainWindow.Current.TaxPercent == -1)
                    db.Add(new CodeSetting()
                    {
                        Description = "درصد مالیات",
                        Id = Guid.NewGuid(),
                        Name = "TaxPercent",
                        Value = (sfTextInputs[0].InputView as TextBox).Text,
                    });
                else
                    db.CodeSettings.FirstOrDefault(t => t.Name == "TaxPercent").Value = (sfTextInputs[0].InputView as TextBox).Text;
                MainWindow.Current.TaxPercent = int.Parse((sfTextInputs[0].InputView as TextBox).Text);
            }
            else
                foreach (SfTextInputLayout item in sfTextInputs)
                {
                    var textBox = item.InputView as System.Windows.Controls.TextBox;
                    CodeSetting y = null;
                    if (item.Tag is string aff)
                    {
                        y = db.CodeSettings.FirstOrDefault(t => t.Name == aff);
                        if (y != null)
                        {
                            var newvalue = (item.InputView as System.Windows.Controls.TextBox).Text;
                            if (y.Value != newvalue && AnyIdValue(db, y.IdValue))
                            {
                                Mouse.OverrideCursor = null;
                                Xceed.Wpf.Toolkit.MessageBox.Show("بعضی از فیلدهای پیکربندی در وضعیت های مختلف دارای گردش است و باید برای تغییر ابتدا تکلیف آنها مشخص شوند!", "خطای پایگاه داده", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            if ((item.InputView as System.Windows.Controls.TextBox).Tag is Mu mu1)
                                y.IdValue = mu1.Id;
                            y.Value = newvalue;
                        }
                        else
                            db.CodeSettings.Add(new CodeSetting()
                            {
                                Id = Guid.NewGuid(),
                                Name = item.Tag.ToString(),
                                Value = (item.InputView as System.Windows.Controls.TextBox).Text,
                                IdValue = ((item.InputView as System.Windows.Controls.TextBox).Tag as Mu).Id
                            });
                    }
                    else if (item.Tag is Dictionary<string, string> ss)
                    {
                        foreach (var itemv in ss)
                        {

                            y = db.CodeSettings.FirstOrDefault(t => t.Name == itemv.Key);
                            if (y != null)
                            {
                                if (y.Value != itemv.Value && AnyIdValue(db, y.IdValue))
                                {
                                    Mouse.OverrideCursor = null;
                                    Xceed.Wpf.Toolkit.MessageBox.Show("بعضی از فیلدهای پیکربندی در وضعیت های مختلف دارای گردش است و باید برای تغییر ابتدا تکلیف آنها مشخص شوند!", "خطای پایگاه داده", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                if ((item.InputView as System.Windows.Controls.TextBox).Tag is Mu mu1)
                                    y.IdValue = mu1.Id;
                                y.Value = itemv.Value;
                            }
                            else
                                db.CodeSettings.Add(new CodeSetting()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = itemv.Key,
                                    Value = itemv.Value,
                                    IdValue = ((item.InputView as System.Windows.Controls.TextBox).Tag as Mu).Id
                                });
                        }
                    }
                }
            if (!db.SafeSaveChanges())
            {
                Mouse.OverrideCursor = null;
                return;
            }
            Mouse.OverrideCursor = null;
            Xceed.Wpf.Toolkit.MessageBox.Show("تنظیمات با موفقیت ثبت شد");
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                await Task.Delay(0);
                Height=stack.ActualHeight+175;
            }), DispatcherPriority.Render);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.Current.Effect = null;
        }

        public bool CloseForm()
        {
            throw new NotImplementedException();
        }
        public winSearch childWindow = null;
        public void SetNull()
        {
            if(childWindow != null)
            {
                var sfTextInput = (childWindow.ParentTextBox as System.Windows.Controls.TextBox).GetParentOfType<SfTextInputLayout>();
                var mu = (childWindow.ParentTextBox.Tag as Mu);
                if (mu == null)
                {
                    Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(0);
                        Focus();
                    }), DispatcherPriority.Render);
                    return;
                }
                if (mu.AdditionalEntity != null)
                {
                    sfTextInput.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
                    var keyValuePairs = sfTextInput.Tag as Dictionary<string, string>;
                    keyValuePairs[keyValuePairs.ElementAt(0).Key] = mu.Value;
                    keyValuePairs[keyValuePairs.ElementAt(1).Key] = (mu.AdditionalEntity as AccountSearchClass).Moein;
                }
                else
                {
                    sfTextInput.HelperText = mu.Name;                    
                }
            }
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                await Task.Delay(0);
                Focus();
            }), DispatcherPriority.Render);
            childWindow = null;
        }
    }
}
