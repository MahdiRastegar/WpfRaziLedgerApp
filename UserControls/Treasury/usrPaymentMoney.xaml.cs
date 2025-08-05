﻿using Mahdi.PersianDateControls;
using Microsoft.EntityFrameworkCore;
using PersianCalendarWPF;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.TextInputLayout;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using static ClosedXML.Excel.XLPredefinedFormat;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winCol.xaml
    /// </summary>
    public partial class usrPaymentMoney : UserControl, ITabForm, ITabEdidGrid, IDisposable
    {
        public bool DataGridIsFocused
        {
            get
            {
                return DataGridFocused;
            }
        }
        PaymentMoneyViewModel acDocumentViewModel;
        ObservableCollection<Bank> Banks = new ObservableCollection<Bank>();
        List<Mu> mus1 = new List<Mu>();
        List<Mu> mus2 = new List<Mu>();
        public usrPaymentMoney()
        {
            temp_paymentMoney_Details = new ObservableCollection<PaymentMoneyDetail>();
            PaymentMoneyHeaders = new ObservableCollection<PaymentMoneyHeader>();
            InitializeComponent();
            acDocumentViewModel = Resources["viewmodel"] as PaymentMoneyViewModel;
            acDocumentViewModel.paymentMoney_Details.CollectionChanged += AcDocument_Details_CollectionChanged;
            txbCalender.Text = pcw1.SelectedDate.ToString();
        }

        public void Dispose()
        {
            if (acDocumentViewModel == null)
                return;
            mus1.Clear();
            mus2.Clear();
            PaymentMoneyHeaders.Clear();
            paymentMoney_Details.Clear();
            datagridSearch.Dispose();
            dataPager.Dispose();
            DataContext = null;
            acDocumentViewModel.paymentMoney_Details.CollectionChanged -= AcDocument_Details_CollectionChanged;
            acDocumentViewModel = null;
            GC.Collect();
        }

        Brush brush = null;
        public ObservableCollection<PaymentMoneyDetail> temp_paymentMoney_Details { get; set; }
        public ObservableCollection<PaymentMoneyDetail> paymentMoney_Details { get; set; }
        public ObservableCollection<PaymentMoneyHeader> PaymentMoneyHeaders { get; set; }
        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                if ((sender as TextBox).Name == "txtDescription")
                {
                    datagrid.Focus();
                    Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(10);
                        keybd_event(VK_Down, 0, 0, UIntPtr.Zero); // فشار دادن کلید
                        Thread.Sleep(50); // تاخیر برای شبیه‌سازی فشار دادن
                        keybd_event(VK_Down, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // آزاد کردن کلید 
                        var g = datagrid.GetChildOfType<ComboBoxAdv>();
                        var gridCell = g.GetParentOfType<GridCell>();
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(gridCell.ColumnBase.RowIndex, gridCell.ColumnBase.ColumnIndex));
                        await Task.Delay(10);
                        g.Focus();
                        g.IsDropDownOpen = true;
                    }), DispatcherPriority.Render);
                }
                else
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (sender as TextBox).MoveFocus(request);
                }
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (btnConfirm.IsFocused)
                    {
                        btnConfirm_Click(null, null);
                    }
                }));
                return;
            }
            if ((sender as TextBox).Name != "txtDescription")
                e.Handled = !IsTextAllowed(e.Text);
        }
        private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        bool AddedMode = true;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using var db = new wpfrazydbContext();
            mus1.Clear();
            mus2.Clear();
            Banks = acDocumentViewModel.Banks;
            Banks.Clear();
            db.Banks.ToList().ForEach(t => Banks.Add(t));

            var moeins = db.Moeins.Include("FkCol").ToList();
            var preferentials = db.Preferentials.Include("FkGroup").ToList();
            foreach (var item in moeins)
            {
                AccountSearchClass accountSearchClass = new AccountSearchClass();
                accountSearchClass.Id = item.Id;
                accountSearchClass.Moein = item.MoeinCode.ToString();
                accountSearchClass.MoeinName = item.MoeinName;
                accountSearchClass.ColMoein = $"{item.FkCol.ColCode}{item.MoeinCode}";
                mus1.Add(new Mu()
                {
                    Id = item.Id,
                    Value = $"{item.FkCol.ColCode}",
                    Name = $"{item.FkCol.ColName}",
                    AdditionalEntity = accountSearchClass
                });
            }
            foreach (var item in preferentials)
            {
                mus2.Add(new Mu()
                {
                    Id = item.Id,
                    Name = $"{item.PreferentialName}",
                    Value = $"{item.PreferentialCode}",
                    Name2 = item.FkGroup.GroupName
                });
            }

            if (AddedMode)
            {
                paymentMoney_Details = acDocumentViewModel.paymentMoney_Details;
                //AcDocument_Details.Clear();
                var y = db.PaymentMoneyHeaders.OrderByDescending(k => k.ReceiptNumber).FirstOrDefault();
                if (y == null)
                {
                    txtSerial.Text = "1";
                }
                else
                {
                    txtSerial.Text = (y.ReceiptNumber + 1).ToString();
                }
            }
            else
            {
                paymentMoney_Details = acDocumentViewModel.paymentMoney_Details;
                paymentMoney_Details.Clear();
                //AcDocument_Details.Clear();
                var h = db.PaymentMoneyDetails.Where(u => u.FkHeaderId == id).ToList();
                h.ForEach(u => paymentMoney_Details.Add(u));
                RefreshDataGridForSetPersianNumber();
            }
            dataPager.Source = null;
            dataPager.Source = PaymentMoneyHeaders;
            datagrid.SearchHelper.AllowFiltering = true;
            datagridSearch.SearchHelper.AllowFiltering = true;
            FirstLevelNestedGrid.SearchHelper.AllowFiltering = true;
            isCancel = true;
            txtMoein.Focus();
        }

        private static void SetAccountName(wpfrazydbContext db, PaymentMoneyDetail item2)
        {/*
            var strings = item2.AcCode.Split('-');
            var moein = int.Parse(strings[0]);
            var tafzil = int.Parse(strings[2]);
            item2.AccountName = $"{db.Preferentials.First(i => i.PreferentialCode == tafzil).PreferentialName}-{db.Moeins.First(p => p.MoeinCode == moein).MoeinName}";*/
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            bool haserror = false;
            haserror = GetError();

            if (haserror)
                return;
            using var db = new wpfrazydbContext();
            //var c = int.Parse(cmbType.Text);
            //var col = db.tGroup.FirstOrDefault(g => g.GroupCode == c);
            //if (col == null)
            //{
            //    Sf_txtDoumentType.ErrorText = "این کد گروه وجود ندارد";
            //    Sf_txtDoumentType.HasError = true;
            //    return;
            //}
            //var PaymentMoneyDetail = db.PaymentMoneyDetails.Find(id);

            //var nPaymentMoney_Detail = db.PaymentMoneyDetails.FirstOrDefault(g => g.FkGroupId == col.Id && g.PaymentMoney_DetailName == txtNoDocumen.Text);
            //if (PaymentMoneyDetail?.Id != nPaymentMoney_Detail?.Id && nPaymentMoney_Detail != null)
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show("این نام تفضیلی و کد گروه از قبل وجود داشته است!");
            //    return;
            //}

            PaymentMoneyHeader e_addHeader = null;
            PaymentMoneyHeader header = null;
            var xy = db.PaymentMoneyHeaders.OrderByDescending(k => k.ReceiptNumber).FirstOrDefault();
            var serial = "1";
            if (xy != null)
            {
                var yb = db.PaymentMoneyHeaders.OrderByDescending(k => k.ReceiptNumber).FirstOrDefault();
                serial = (xy.ReceiptNumber + 1).ToString();
            }
            List<Thread> threads = new List<Thread>();
            if (id == Guid.Empty)
            {
                e_addHeader = new PaymentMoneyHeader()
                {
                    Id = Guid.NewGuid(),
                    Date = pcw1.SelectedDate.ToDateTime(),
                    ReceiptNumber = int.Parse(serial),
                    Description = txtDescription.Text,
                    FkMoeinId = (txtMoein.Tag as Mu).Id,
                    FkPreferentialId = (txtPreferential.Tag as Mu).Id
                };
                DbSet<PaymentMoneyDetail> details = null;
                int index = 0;
                foreach (var item in paymentMoney_Details)
                {
                    index++;
                    var en = new PaymentMoneyDetail()
                    {
                        FkMoeinId = item.FkMoein.Id,
                        FkPreferentialId = item.FkPreferential.Id,
                        FkHeader = e_addHeader,
                        FkBank = item.FkBankNavigation?.Id,
                        BranchName = item.BranchName,
                        Date = item.Date,
                        Number = item.Number,
                        Price = item.Price,
                        MoneyType = item.MoneyType,
                        Registered = item.Registered,
                        SayadiNumber = item.SayadiNumber,
                        Indexer = index,
                        Id = Guid.NewGuid()
                    };
                    db.PaymentMoneyDetails.Add(en);
                    if (item.MoneyType == 1)
                    {
                        db.CheckPaymentEvents.Add(new CheckPaymentEvent()
                        {
                            Id = Guid.NewGuid(),
                            FkDetai = en,
                            FkChEvent = db.ChEvents.First(u => u.ChEventCode == 6),
                            FkMoeinId = item.FkMoein.Id,
                            FkPreferentialId = item.FkPreferential.Id,
                            EventDate = pcw1.SelectedDate.ToDateTime(),
                        });
                    }
                }
                db.PaymentMoneyHeaders.Add(e_addHeader);
                if (LoadedFill)
                    PaymentMoneyHeaders.Add(e_addHeader);
                e_addHeader.FkMoein = db.Moeins.Include("FkCol").First(g=>g.Id== (txtMoein.Tag as Mu).Id);
                e_addHeader.FkPreferential = db.Preferentials.Find((txtPreferential.Tag as Mu).Id);
                //سند حسابداری
                try
                {
                    var documentType = db.DocumentTypes.Where(y => y.Name == "خزانه داری").First();
                    var yx = db.AcDocumentHeaders.OrderByDescending(k => k.Serial).FirstOrDefault();
                    string serial2 = "1", NoDoument = "1";
                    if (yx != null)
                    {
                        serial2 = (yx.Serial + 1).ToString();
                        NoDoument = (yx.NoDoument + 1).ToString();
                    }
                    var e_addHeader2 = new AcDocumentHeader()
                    {
                        Id = Guid.NewGuid(),
                        Date = pcw1.SelectedDate.ToDateTime(),
                        NoDoument = long.Parse(NoDoument),
                        Serial = long.Parse(serial2),
                        FkDocumentType = documentType
                    };
                    var col = db.CodeSettings.FirstOrDefault(t => t.Name == "ColCodeCheckPayment");
                    var mo = db.CodeSettings.FirstOrDefault(t => t.Name == "MoeinCodeCheckPayment");
                    var moein = db.Moeins.Find(mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == (col.Value + mo.Value).ToString()).Id);                    

                    DbSet<AcDocumentDetail> details2 = null;
                    int index2 = 0;                    
                    foreach (var item in paymentMoney_Details)
                    {
                        string part2 = null;
                        if (moein.Id == item.FkMoein.Id)//اسناد پرداختنی
                        {
                            part2 = $"صدور چک شماره {item.Number} سررسید {item.Date?.ToPersianDateString()} در وجه {e_addHeader.FkPreferential.PreferentialName} طی رسید {e_addHeader.ReceiptNumber} بابت {e_addHeader.Description}";
                        }
                        else if (db.Moeins.FirstOrDefault(y => y.MoeinName == "حساب های پرداختنی تجاری").Id == item.FkMoein.Id || db.Moeins.FirstOrDefault(y => y.MoeinName == "حسابهای پرداختنی تجاری").Id == item.FkMoein.Id)
                        {
                            part2 = $"پرداخت چک شماره {item.Number} سررسید {item.Date?.ToPersianDateString()} طی رسید {e_addHeader.ReceiptNumber} بابت {e_addHeader.Description}";
                        }
                        index2++;
                        var parts = new List<string?>
                            {
                                $"شماره رسید : {serial}" ,
                                item.GetMoneyType.Split('-')[1],
                                item.Date?.ToPersianDateString(),
                                item.Number == ""||item.Number==null ? null :
                                    $"شماره : {item.Number}",
                                item.FkBankNavigation?.Name,
                                item.BranchName,
                                item.SayadiNumber,
                                //item.Registered == null ? null :
                                //    (item.Registered == true ? "ثبت شده" : "ثبت نشده")
                            };

                        var en = new AcDocumentDetail()
                        {
                            FkMoein = e_addHeader.FkMoein,
                            FkPreferential = e_addHeader.FkPreferential,
                            FkAcDocHeader = e_addHeader2,
                            Debtor = item.Price,
                            Creditor = 0,
                            Description =part2??string.Join(",", parts.Where(s => !string.IsNullOrWhiteSpace(s))),
                            Indexer = index2,
                            //AccountName = item.AccountName,
                            Id = Guid.NewGuid()
                        };
                        
                        db.AcDocumentDetails.Add(en);
                    }
                    foreach (var item in paymentMoney_Details)
                    {
                        string part2 = null;
                        if (moein.Id == item.FkMoein.Id)//اسناد پرداختنی
                        {
                            part2 = $"صدور چک شماره {item.Number} سررسید {item.Date?.ToPersianDateString()} در وجه {e_addHeader.FkPreferential.PreferentialName} طی رسید {e_addHeader.ReceiptNumber} بابت {e_addHeader.Description}";
                        }
                        else if (db.Moeins.FirstOrDefault(y => y.MoeinName == "حساب های پرداختنی تجاری").Id == item.FkMoein.Id || db.Moeins.FirstOrDefault(y => y.MoeinName == "حسابهای پرداختنی تجاری").Id == item.FkMoein.Id)
                        {
                            part2 = $"پرداخت چک شماره {item.Number} سررسید {item.Date?.ToPersianDateString()} طی رسید {e_addHeader.ReceiptNumber} بابت {e_addHeader.Description}";
                        }
                        index2++;
                        var parts = new List<string?>
                            {
                                $"شماره رسید : {serial}" ,
                                $"نام حساب : {e_addHeader.FkPreferential.PreferentialName}" ,
                                item.GetMoneyType.Split('-')[1],
                                item.Date?.ToPersianDateString(),
                                item.Number == ""||item.Number==null ? null :
                                    $"شماره : {item.Number}",
                                item.FkBankNavigation?.Name,
                                item.BranchName,
                                item.SayadiNumber,
                                item.Registered == null ? null :
                                    (item.Registered == true ? "ثبت شده" : "ثبت نشده")
                            };

                        var en = new AcDocumentDetail()
                        {
                            FkMoeinId = item.FkMoein.Id,
                            FkPreferentialId = item.FkPreferential.Id,
                            FkAcDocHeader = e_addHeader2,
                            Debtor = 0,
                            Creditor = item.Price,
                            Description = part2 ?? string.Join(",", parts.Where(s => !string.IsNullOrWhiteSpace(s))),
                            Indexer = index2,
                            //AccountName = item.AccountName,
                            Id = Guid.NewGuid()
                        };
                        threads.Add( new Thread(() =>
                        {
                            en.FkMoein = item.FkMoein;
                            en.FkPreferential = item.FkPreferential;
                        }));
                        db.AcDocumentDetails.Add(en);
                    }
                    db.AcDocumentHeaders.Add(e_addHeader2);
                    e_addHeader.FkAcDocumentNavigation = e_addHeader2;

                    foreach (var item in MainWindow.Current.tabcontrol.Items)
                    {
                        if (item is TabItemExt tabItemExt)
                        {
                            if (tabItemExt.Header.ToString() == "سند حسابداری")
                            {
                                if (tabItemExt.Content is usrAccountDocument usrAccountDocument)
                                {
                                    if (usrAccountDocument.LoadedFill)
                                        usrAccountDocument.AcDocumentHeaders.Add(e_addHeader2);
                                }
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"خطا در ایجاد سند حسابداری");
                }
            }
            else
            {
                var h = db.PaymentMoneyDetails.Where(v => v.FkHeaderId == id);
                header = PaymentMoneyHeaders.First(u => u.Id == id);
                foreach (var item in h)
                {
                    if (item.MoneyType == 1 && (!(paymentMoney_Details.FirstOrDefault(s => s.Id == item.Id) is PaymentMoneyDetail paymentMoney) || paymentMoney.MoneyType != 1))
                    {
                        var checkPaymentEvents = db.CheckPaymentEvents.Where(d => d.FkDetaiId == item.Id);
                        var checkPayment = checkPaymentEvents.OrderByDescending(k => k.Indexer).FirstOrDefault();
                        if (checkPayment != null)
                        {
                            //var entry = db.Entry(checkPayment);
                            //entry.Reference(a => a.ChEvent).Load();
                            if (checkPayment.FkChEvent.ChEventCode == 6)
                            {
                                foreach (var check in checkPaymentEvents)
                                {
                                    db.CheckPaymentEvents.Remove(check);
                                }
                            }
                        }
                    }
                    db.PaymentMoneyDetails.Remove(item);
                    header.PaymentMoneyDetails.Remove(header.PaymentMoneyDetails.First(x => x.Id == item.Id));
                }
                var e_Edidet = db.PaymentMoneyHeaders.Find(id);
                e_Edidet.ReceiptNumber = header.ReceiptNumber = int.Parse(txtSerial.Text);
                e_Edidet.Description = header.Description = txtDescription.Text;
                e_Edidet.FkMoeinId = (txtMoein.Tag as Mu).Id;
                header.FkMoein = db.Moeins.Include("FkCol").First(g => g.Id == ((txtMoein.Tag as Mu).Id));
                e_Edidet.FkPreferentialId = (txtPreferential.Tag as Mu).Id;
                header.FkPreferential = db.Preferentials.Find((txtPreferential.Tag as Mu).Id);
                e_Edidet.Date = header.Date = pcw1.SelectedDate.ToDateTime();
                int index = 0;
                foreach (var item in paymentMoney_Details)
                {
                    index++;
                    var en = new PaymentMoneyDetail()
                    {
                        FkHeaderId = header.Id,
                        FkMoeinId = item.FkMoein.Id,
                        FkPreferentialId = item.FkPreferential.Id,
                        FkHeader = e_addHeader,
                        FkBank = item.FkBankNavigation?.Id,
                        BranchName = item.BranchName,
                        Date = item.Date,
                        Number = item.Number,
                        Price = item.Price,
                        MoneyType = item.MoneyType,
                        Registered = item.Registered,
                        SayadiNumber = item.SayadiNumber,
                        Indexer = index,
                        Id = Guid.NewGuid()
                    };
                    bool Enter = false;
                    if (item.Id != Guid.Empty && item.MoneyType == 1 && db.PaymentMoneyDetails.Find(item.Id) is PaymentMoneyDetail detail && detail.MoneyType == 1 &&
                    ExtensionMethods.CompareObjects(item, detail) is List<string> fff && !fff.Contains("FkMoeinId") && !fff.Contains("FkPreferentialId"))
                    {
                        db.CheckPaymentEvents.Where(y => y.FkDetaiId == item.Id).ForEach(u =>
                        {
                            u.FkDetaiId = en.Id;
                        });
                        Enter = true;
                    }
                    if (!Enter && item.MoneyType == 1)
                    {
                        db.CheckPaymentEvents.Add(new CheckPaymentEvent()
                        {
                            Id = Guid.NewGuid(),
                            FkDetai = en,
                            FkChEvent = db.ChEvents.First(u => u.ChEventCode == 6),
                            FkMoeinId = item.FkMoein.Id,
                            FkPreferentialId = item.FkPreferential.Id,
                            EventDate = pcw1.SelectedDate.ToDateTime(),
                        });
                    }
                    db.PaymentMoneyDetails.Add(en);
                    header.PaymentMoneyDetails.Add(en);
                }

                //ویرایش سند حسابداری
                try
                {
                    if (db.AcDocumentHeaders.Find(e_Edidet.FkAcDocument) is AcDocumentHeader ac)
                    {
                        ac.Date = pcw1.SelectedDate.ToDateTime();
                        int index2 = 0;
                        foreach (var item in db.AcDocumentDetails.Where(u => u.FkAcDocHeaderId == ac.Id))
                        {
                            db.AcDocumentDetails.Remove(item);
                        }
                        var list = new List<AcDocumentDetail>();
                        var col = db.CodeSettings.FirstOrDefault(t => t.Name == "ColCodeCheckPayment");
                        var mo = db.CodeSettings.FirstOrDefault(t => t.Name == "MoeinCodeCheckPayment");
                        var moein = db.Moeins.Find(mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == (col.Value + mo.Value).ToString()).Id);
                        foreach (var item in paymentMoney_Details)
                        {
                            string part2 = null;
                            if (moein.Id == item.FkMoein.Id)//اسناد پرداختنی
                            {
                                part2 = $"صدور چک شماره {item.Number} سررسید {item.Date?.ToPersianDateString()} در وجه {e_Edidet.FkPreferential.PreferentialName} طی رسید {e_Edidet.ReceiptNumber} بابت {e_Edidet.Description}";
                            }
                            else if (db.Moeins.FirstOrDefault(y => y.MoeinName == "حساب های پرداختنی تجاری").Id == item.FkMoein.Id || db.Moeins.FirstOrDefault(y => y.MoeinName == "حسابهای پرداختنی تجاری").Id == item.FkMoein.Id)
                            {
                                part2 = $"پرداخت چک شماره {item.Number} سررسید {item.Date?.ToPersianDateString()} طی رسید {e_Edidet.ReceiptNumber} بابت {e_Edidet.Description}";
                            }
                            index2++;
                            var parts = new List<string?>
                            {
                                $"شماره رسید : {serial}" ,
                                item.GetMoneyType.Split('-')[1],
                                item.Date?.ToPersianDateString(),
                                item.Number == ""||item.Number==null ? null :
                                    $"شماره : {item.Number}",
                                item.FkBankNavigation?.Name,
                                item.BranchName,
                                item.SayadiNumber,
                                item.Registered == null ? null :
                                    (item.Registered == true ? "ثبت شده" : "ثبت نشده")
                            };

                            var en = new AcDocumentDetail()
                            {
                                FkMoein = e_Edidet.FkMoein,
                                FkPreferential = e_Edidet.FkPreferential,
                                FkAcDocHeader = ac,
                                Debtor = item.Price,
                                Creditor = 0,
                                Description = part2 ?? string.Join(",", parts.Where(s => !string.IsNullOrWhiteSpace(s))),
                                Indexer = index2,
                                //AccountName = item.AccountName,
                                Id = Guid.NewGuid()
                            };
                            db.AcDocumentDetails.Add(en);
                            list.Add(en);
                        }
                        foreach (var item in paymentMoney_Details)
                        {
                            string part2 = null;
                            if (moein.Id == item.FkMoein.Id)//اسناد پرداختنی
                            {
                                part2 = $"صدور چک شماره {item.Number} سررسید {item.Date?.ToPersianDateString()} در وجه {e_Edidet.FkPreferential.PreferentialName} طی رسید {e_Edidet.ReceiptNumber} بابت {e_Edidet.Description}";
                            }
                            else if (db.Moeins.FirstOrDefault(y => y.MoeinName == "حساب های پرداختنی تجاری").Id == item.FkMoein.Id || db.Moeins.FirstOrDefault(y => y.MoeinName == "حسابهای پرداختنی تجاری").Id == item.FkMoein.Id)
                            {
                                part2 = $"پرداخت چک شماره {item.Number} سررسید {item.Date?.ToPersianDateString()} طی رسید {e_Edidet.ReceiptNumber} بابت {e_Edidet.Description}";
                            }
                            index2++;
                            var parts = new List<string?>
                            {
                                $"شماره رسید : {serial}" ,
                                $"نام حساب : {e_Edidet.FkPreferential.PreferentialName}" ,
                                item.GetMoneyType.Split('-')[1],
                                item.Date?.ToPersianDateString(),
                                item.Number == ""||item.Number==null ? null :
                                    $"شماره : {item.Number}",
                                item.FkBankNavigation?.Name,
                                item.BranchName,
                                item.SayadiNumber,
                                item.Registered == null ? null :
                                    (item.Registered == true ? "ثبت شده" : "ثبت نشده")
                            };

                            var en = new AcDocumentDetail()
                            {
                                FkMoeinId = item.FkMoein.Id,
                                FkPreferentialId = item.FkPreferential.Id,
                                FkAcDocHeader = ac,
                                Debtor = 0,
                                Creditor = item.Price,
                                Description =part2?? string.Join(",", parts.Where(s => !string.IsNullOrWhiteSpace(s))),
                                Indexer = index2,
                                //AccountName = item.AccountName,
                                Id = Guid.NewGuid()
                            };
                            threads.Add( new Thread(() =>
                            {
                                en.FkMoein = item.FkMoein;
                                en.FkPreferential = item.FkPreferential;
                            }));
                            db.AcDocumentDetails.Add(en);
                            list.Add(en);
                        }

                        foreach (var item in MainWindow.Current.tabcontrol.Items)
                        {
                            if (item is TabItemExt tabItemExt)
                            {
                                if (tabItemExt.Header.ToString() == "سند حسابداری")
                                {
                                    if (tabItemExt.Content is usrAccountDocument usrAccountDocument)
                                    {
                                        if (usrAccountDocument.LoadedFill)
                                        {
                                            var r = usrAccountDocument.AcDocumentHeaders.FirstOrDefault(a => a.Id == ac.Id);
                                            if (r != null)
                                            {
                                                //var kk = usrAccountDocument.AcDocumentHeaders.IndexOf(r);
                                                r.Date = ac.Date;
                                                r.Serial = ac.Serial;
                                                r.NoDoument = ac.NoDoument;
                                                r.RefreshSumColumns();
                                                r.AcDocumentDetails = list;
                                                r.CheckPaymentEvents = ac.CheckPaymentEvents;
                                                r.CheckRecieveEvents = ac.CheckRecieveEvents;
                                                r.PaymentMoneyHeaders = ac.PaymentMoneyHeaders;
                                                r.RecieveMoneyHeaders = ac.RecieveMoneyHeaders;
                                                usrAccountDocument.datagridSearch.View.Refresh();
                                                //usrAccountDocument.AcDocumentHeaders.Remove(r);
                                                //usrAccountDocument.AcDocumentHeaders.Insert(kk, r);
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطا در ویرایش سند حسابداری");
                }
            }
            if (!db.SafeSaveChanges())
            {
                RefreshHeader();
                paymentMoney_Details.Clear();
                header.PaymentMoneyDetails.ToList().ForEach(t => paymentMoney_Details.Add(t));

                StateLoadView = true;
                datagrid.View.Refresh();
                datagrid.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await Task.Delay(50);
                    StateLoadView = false;
                }), DispatcherPriority.Render);
                return;
            }
            //ادامه سند حسابداری
            foreach (var item in threads)
            {
                item.Start();
                item.Join();
            }
            if (header != null)
            {
                int i = 0;
                foreach (var item in header.PaymentMoneyDetails)
                {
                    item.FkMoein = paymentMoney_Details[i].FkMoein;
                    item.FkPreferential = paymentMoney_Details[i].FkPreferential;
                    item.FkBankNavigation = paymentMoney_Details[i].FkBankNavigation;
                    i++;
                }
            }
            if (e_addHeader != null)
            {
                int i = 0;
                foreach (var item in e_addHeader.PaymentMoneyDetails)
                {
                    item.FkMoein = paymentMoney_Details[i].FkMoein;
                    item.FkPreferential = paymentMoney_Details[i].FkPreferential;
                    item.FkBankNavigation = paymentMoney_Details[i].FkBankNavigation;
                    i++;
                }
            }
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            datagrid.SearchHelper.ClearSearch();
            if (paymentMoney_Details.Count > 0)
            {
                datagrid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    paymentMoney_Details.Clear();
                }));
                RefreshDataGridForSetPersianNumber();
            }
            //datagrid.TableSummaryRows.Clear();
            SearchTermTextBox.Text = "";
            if (id == Guid.Empty)
            {
                this.gifImage.Visibility = Visibility.Visible;
                var gifImage = new BitmapImage(new Uri("pack://application:,,,/Images/AddDataLarge.gif"));
                XamlAnimatedGif.AnimationBehavior.SetSourceUri(this.gifImage, gifImage.UriSource);
                var th = new Thread(() =>
                {
                    Thread.Sleep(2570);
                    Dispatcher.Invoke(() =>
                    {
                        searchImage.Visibility = Visibility.Visible;
                        this.gifImage.Visibility = Visibility.Collapsed;
                    });
                });
                th.Start();
                searchImage.Visibility = Visibility.Collapsed;
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات اضافه شد.", "ثبت");
                searchImage.Visibility = Visibility.Visible;
                this.gifImage.Visibility = Visibility.Collapsed;
                txtSerial.Text = (long.Parse(serial) + 1).ToString();
                txtMoein.Text = string.Empty;
                txtPreferential.Text = string.Empty;
                txtDescription.Text = string.Empty;
                txbMoein.Text = string.Empty;
                txbPreferential.Text = string.Empty;

                txtMoein.Focus();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات ویرایش شد.", "ویرایش");
                btnCancel_Click(null, null);
            }

            isCancel = true;
            id = Guid.Empty;
        }
        Guid id = Guid.Empty;
        private bool GetError()
        {
            var haserror = false;
            datagrid.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#FF808080"));
            if (txtMoein.Text.Trim() == "")
            {
                Sf_txtMoein.HasError = true;
                haserror = true;
            }
            else
            {
                Sf_txtMoein.HasError = false;
                Sf_txtMoein.ErrorText = "";
            }
            if (txtPreferential.Text.Trim() == "")
            {
                Sf_txtPreferential.HasError = true;
                haserror = true;
            }
            else
            {
                Sf_txtPreferential.HasError = false;
                Sf_txtPreferential.ErrorText = "";
            }
            if (paymentMoney_Details.Count == 0)//AcDocument_Details.Any(g => !viewModel.AllCommodities.Any(y => y.CommodityCode == g.CommodityCode)))
            {
                datagrid.BorderBrush = Brushes.Red;
                haserror = true;
            }
            //else if (paymentMoney_Details.Any(t => t.Price == 0 || t.ColeMoein == "" || t.ColeMoein == null || t.PreferentialCode == "" || t.PreferentialCode == null) ||
            //    paymentMoney_Details.Any(t => t.MoneyType == 1 && (t.Date == null || t.Bank == null || t.Number == null||t.Number=="")))
            else if (paymentMoney_Details.Any(t => t.Error != string.Empty))
            //else if (datagrid.GetChildsByName<Border>("PART_InValidCellBorder").Any(g => g.Visibility == Visibility.Visible))
            {
                datagrid.BorderBrush = Brushes.Red;
                haserror = true;
            }
            return haserror;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!forceClose && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این فرم خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }


        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var gr = brush as LinearGradientBrush;
            if (gr != null)
            {
                var gr2 = new LinearGradientBrush();
                foreach (var item in gr.GradientStops)
                {
                    gr2.GradientStops.Add(new GradientStop(item.Color, item.Offset));
                }
                for (var i = 1; i < gr2.GradientStops.Count; i++)
                {
                    gr2.GradientStops[i].Color = ColorToBrushConverter.GetLightOfColor(gr.GradientStops[i].Color, .15f);
                }
                gr2.EndPoint = gr.EndPoint;
                gr2.StartPoint = gr.StartPoint;
                border.Background = gr2;
            }
            else
            {
                border.Background = new SolidColorBrush(ColorToBrushConverter.GetLightOfColor((brush as SolidColorBrush).Color, .15f));
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void border_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = brush;
        }

        private void border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border.IsMouseOver)
            {
                Border_MouseEnter(sender, e);
            }
            else
            {
                border.Background = brush;
            }
        }

        private void btnExcelPattern_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelPattern", "Commodity.xlsx"));
        }

        private void txtProductID_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.F1)
            {
                Border_MouseDown("id",null);
            }*/
        }

        private void txtMu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                Border_MouseDown("mu", null);
            }
        }
        // تعریف توابع مورد نیاز از user32.dll
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // کلیدهای مجازی
        const byte VK_F2 = 0x71; // کد کلید F2
        const byte VK_Down = 0x28; // کد مجازی برای کلید جهت پایین
        const uint KEYEVENTF_KEYUP = 0x0002; // نشان دهنده آزاد کردن کلید
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);
        private void datagrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                if (datagrid.SelectionController.CurrentCellManager?.CurrentCell?.ColumnIndex == 1)
                {
                    dynamic y = null;
                    var element = (datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell)
                            .Content as FrameworkElement;
                    y = element.DataContext;
                    if (element is TextBlock)
                    {
                        if (sender == null)
                            return;
                        if (datagrid.SelectedIndex == -1)
                        {
                            if (y == null)
                            {
                                bool d = datagrid.GetGridModel().AddNewRowController.AddNew();
                            }
                            y = element.DataContext;
                        }
                        var cell = datagrid.SelectionController.CurrentCellManager.CurrentCell.Element;
                        //var screenPosition = cell.PointToScreen(new System.Windows.Point(0, 0));
                        //SetCursorPos((int)screenPosition.X - 30, (int)screenPosition.Y + 15);
                        //LeftDoubleClick();
                        // شبیه‌سازی فشار دادن کلید F2
                        keybd_event(VK_F2, 0, 0, UIntPtr.Zero); // فشار دادن کلید
                        Thread.Sleep(50); // تاخیر برای شبیه‌سازی فشار دادن
                        keybd_event(VK_F2, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // آزاد کردن کلید
                        var th = new Thread(() =>
                        {
                            Thread.Sleep(10);
                            Dispatcher.Invoke(() =>
                            datagrid_PreviewKeyDown(null, e));
                        });
                        th.Start();
                        return;
                    }
                    ShowSearchMoein(y);
                    datagrid.IsHitTestVisible = false;
                }
                else if (datagrid.SelectionController.CurrentCellManager?.CurrentCell?.ColumnIndex == 2)
                {
                    dynamic y = null;
                    var element = (datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell)
                            .Content as FrameworkElement;
                    y = element.DataContext;
                    if (datagrid.SelectedIndex == -1 || element is TextBlock)
                    {
                        if (y == null || y.PreferentialCode != null)
                        {
                            if (sender == null)
                                return;
                            var cell = datagrid.SelectionController.CurrentCellManager.CurrentCell.Element;
                            keybd_event(VK_F2, 0, 0, UIntPtr.Zero); // فشار دادن کلید
                            Thread.Sleep(50); // تاخیر برای شبیه‌سازی فشار دادن
                            keybd_event(VK_F2, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // آزاد کردن کلید
                            var th = new Thread(() =>
                            {
                                Thread.Sleep(10);
                                Dispatcher.Invoke(() =>
                                datagrid_PreviewKeyDown(null, e));
                            });
                            th.Start();
                            return;
                        }
                    }                    
                    winSearch win = ShowSearchPreferential(y);
                    if (y.MoneyType == 1)
                    {
                        win.datagrid.ItemsSource = (win.datagrid.ItemsSource as ObservableCollection<Mu>).Where(u => u.Name2.Trim() == "بانک ها"|| u.Name2.Trim() == "بانکها");
                    }
                    datagrid.IsHitTestVisible = false;
                }
                else if (datagrid.SelectionController.CurrentCellManager?.CurrentCell?.ColumnIndex == 5)
                {
                    dynamic y = null;
                    var element = (datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell)
                            .Content as FrameworkElement;
                    y = element.DataContext;
                    if (datagrid.SelectedIndex == -1 || element is TextBlock)
                    {
                        if (y == null)
                        {
                            if (sender == null)
                                return;
                            var cell = datagrid.SelectionController.CurrentCellManager.CurrentCell.Element;
                            keybd_event(VK_F2, 0, 0, UIntPtr.Zero); // فشار دادن کلید
                            Thread.Sleep(50); // تاخیر برای شبیه‌سازی فشار دادن
                            keybd_event(VK_F2, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // آزاد کردن کلید
                            var th = new Thread(() =>
                            {
                                Thread.Sleep(10);
                                Dispatcher.Invoke(() =>
                                datagrid_PreviewKeyDown(null, e));
                            });
                            th.Start();
                            return;
                        }
                        else if (y.MoneyType != 1)
                            return;
                    }
                    (MyPopupS.Parent as Grid).Children.Remove(MyPopupS);
                    ParentDataGrid.Children.Add(MyPopupS);
                    MyPopupS.Visibility = Visibility.Visible;
                    MyPopupS.IsOpen = true;
                    datagrid.IsHitTestVisible = false;
                }
            }
        }
        int tempSelectedIndex = -1;
        public void SetEnterToNextCell(RowColumnIndex? rowColumn = null)
        {
            //datagrid.CurrentCellEndEdit -= datagrid_CurrentCellEndEdit;
            var dataGrid = datagrid;

            // پیدا کردن سطر و ستون فعلی
            var currentCell = datagrid.SelectionController.CurrentCellManager?.CurrentCell;
            if (currentCell != null || rowColumn != null)
            {
                int currentRowIndex = rowColumn == null ? currentCell.RowIndex : rowColumn.Value.RowIndex;
                int currentColumnIndex = rowColumn == null ? currentCell.ColumnIndex : rowColumn.Value.ColumnIndex;
                if (currentColumnIndex > 2)
                    datagrid.CurrentCellEndEdit -= datagrid_CurrentCellEndEdit;

                // افزایش اندیس ستون
                currentColumnIndex++;

                // اگر به انتهای ستون‌ها رسیدیم، به سطر بعد بروید
                if (currentColumnIndex >= dataGrid.Columns.Count)
                {
                    currentColumnIndex = 0; // به اولین ستون برگردید
                    currentRowIndex++; // به سطر بعد بروید
                }

                // اگر به انتهای سطرها رسیدیم، به اولین سطر برگردید
                if (currentRowIndex >= paymentMoney_Details.Count + 2)
                {
                    currentRowIndex = 0; // به اولین سطر برگردید
                }

                //Updates the PressedRowColumnIndex value in the GridBaseSelectionController.
                try
                {
                    if (currentColumnIndex == 3)
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex, currentColumnIndex + 1));
                    else if (tempSelectedIndex != -1 || dataGrid.SelectedIndex != -1)
                    {
                        var PaymentMoneyDetail = paymentMoney_Details[dataGrid.SelectedIndex == -1 ? tempSelectedIndex : dataGrid.SelectedIndex];
                        switch (PaymentMoneyDetail.MoneyType)
                        {
                            case 0:
                            case 2:
                                if (currentColumnIndex == 5)
                                    (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex + 1, currentColumnIndex = 0));
                                else
                                    (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex, currentColumnIndex));
                                break;
                            case 1:
                                (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex, currentColumnIndex));
                                break;
                            case 3:
                                if (currentColumnIndex == 5)
                                    (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex, currentColumnIndex + 1));
                                else if (currentColumnIndex == 7)
                                    (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex + 1, currentColumnIndex = 0));
                                else
                                    (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex, currentColumnIndex));
                                break;
                        }
                    }
                    else
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex, currentColumnIndex));
                }
                catch { }
                if (currentColumnIndex == 0)
                {
                    datagrid.Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(50);
                        var comboBoxAdvs = datagrid.GetChildsOfType<ComboBoxAdv>();
                        ComboBoxAdv comboBoxAdv = null;
                        int i = 0;
                        foreach (var item in comboBoxAdvs)
                        {
                            var gridCell = item.GetParentOfType<GridCell>();
                            if (gridCell == datagrid.SelectionController.CurrentCellManager?.CurrentCell.Element)
                            {
                                comboBoxAdv = comboBoxAdvs[i];
                                break;
                            }
                            i++;
                        }
                        if (comboBoxAdv != null)
                        {
                            await Task.Delay(10);
                            comboBoxAdv.Focus();
                            comboBoxAdv.IsDropDownOpen = true;
                        }
                    }), DispatcherPriority.Render);
                }
                datagrid.CurrentCellEndEdit += datagrid_CurrentCellEndEdit;
                dataGrid.Focus();
            }
        }
        private void datagrid_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            isCancel = false;
            CalDebCre();

            if (window == null && datagrid.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex) is PaymentMoneyDetail acDocument_Detail)
            {
                if ((CurrentCellText ?? "") != "")
                {
                    if (e.RowColumnIndex.ColumnIndex == 1)
                    {
                        using var db = new wpfrazydbContext();
                        var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == CurrentCellText);
                        if (mu == null)
                        {

                        }
                        else
                        {
                            var moein = db.Moeins.Include("FkCol").First(g => g.Id == (mu.AdditionalEntity as AccountSearchClass).Id);
                            acDocument_Detail.FkMoein = moein;
                        }
                    }
                    else if (e.RowColumnIndex.ColumnIndex == 2)
                    {
                        using var db = new wpfrazydbContext();
                        var mu = (datagrid.SelectedItem as PaymentMoneyDetail).MoneyType == 1 ? mus2.Find(t => (t.Name2.Trim() == "بانک ها" || t.Name2.Trim() == "بانکها") && t.Value == CurrentCellText)
                            : mus2.Find(t => t.Value == CurrentCellText);
                        if (mu == null)
                        {

                        }
                        else
                        {
                            var preferential = db.Preferentials.Find(mu.Id);
                            acDocument_Detail.FkPreferential = preferential;
                        }
                    }
                }
            }
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                var th = new Thread(() =>
                {
                    StateLoadView = true;
                    Thread.Sleep(30);
                    Dispatcher.Invoke(new Action(() =>
                    SetEnterToNextCell(this.CurrentRowColumnIndex)));
                    Thread.Sleep(30);
                    StateLoadView = false;
                });
                th.Start();
            }
            else
            {
                StateLoadView = true;
                datagrid.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await Task.Delay(50);
                    StateLoadView = false;
                }), DispatcherPriority.Render);
            }
        }

        private void btnTransferOfExcel_Click(object sender, RoutedEventArgs e)
        {

        }
        bool forceClose = false;
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseForm();
            }
        }

        bool isCancel = true;
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (morefields.Visibility == Visibility.Visible)
            {
                morefields.Visibility = Visibility.Collapsed;
                column1.Width = new GridLength(170);
                column2.Width = new GridLength(170);
            }
            if (AddedMode && isCancel)
            {
                return;
            }
            if (searchImage.ToolTip.ToString() == "جستجو" && sender != null && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این عملیات انصراف دهید؟", "انصراف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }
            searchImage.Visibility = Visibility.Visible;
            searchGrid.Visibility = Visibility.Collapsed;
            searchImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Data.png"));
            searchImage.ToolTip = "جستجو";
            GStop1.Color = new Color()
            {
                R = 244,
                G = 248,
                B = 255,
                A = 255
            };
            searchImage.Opacity = 1;
            using var db = new wpfrazydbContext();
            if (!AddedMode)
            {
                if (id != Guid.Empty)
                {
                    var e_Edidet = db.PaymentMoneyHeaders.
                         Include(d => d.FkPreferential)
                        .Include(d => d.FkMoein)
                        .ThenInclude(d => d.FkCol)
                        .Include(h => h.PaymentMoneyDetails)
                        .ThenInclude(d => d.FkPreferential)
                        .Include(h => h.PaymentMoneyDetails)
                        .ThenInclude(d => d.FkMoein)
                        .ThenInclude(d => d.FkCol)
                        .Include(h => h.PaymentMoneyDetails)
                        .ThenInclude(d => d.FkBankNavigation)
                        .First(g => g.Id == id);
                    var header = PaymentMoneyHeaders.FirstOrDefault(o => o.Id == id);
                    header.PaymentMoneyDetails.Clear();
                    foreach (var item in e_Edidet.PaymentMoneyDetails)
                    {
                        header.PaymentMoneyDetails.Add(item);
                        SetAccountName(db, item);
                    }
                }
                AddedMode = true;
                column1.Width = new GridLength(170);
                column2.Width = new GridLength(225);
                datagrid.AllowEditing = datagrid.AllowDeleting = true;
                datagrid.AddNewRowPosition = Syncfusion.UI.Xaml.Grid.AddNewRowPosition.Bottom;
            }
            datagrid.Visibility = Visibility.Visible;
            datagridSearch.Visibility = Visibility.Collapsed;
            //gridSetting.Visibility = 
                gridConfirm.Visibility = Visibility.Visible;
            Sf_txtMoein.HasError = Sf_txtPreferential.HasError = false;
            datagrid.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#FF808080"));
            txtMoein.Text = string.Empty;
            txtPreferential.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txbMoein.Text = string.Empty;
            txbPreferential.Text = string.Empty;
            //txtCodeAcDocument_Detail.Text = (en.AcDocument_DetailCode + 1).ToString();

            txtMoein.Focus();
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            //datagrid.TableSummaryRows.Clear();
            datagrid.SearchHelper.ClearSearch();
            testsearch.Text = "جستجو...";
            SearchTermTextBox.Text = "";
            dataPager.Visibility = Visibility.Collapsed;
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            txtSerial.Text = "";
            datagrid.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#FF808080"));
            if (paymentMoney_Details.Count > 0)
            {
                datagrid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    paymentMoney_Details.Clear();
                }));
                RefreshDataGridForSetPersianNumber();
            }

            var y = db.PaymentMoneyHeaders.OrderByDescending(k => k.ReceiptNumber).FirstOrDefault();
            if (y == null)
            {
                txtSerial.Text = "1";
            }
            else
            {
                var yb = db.PaymentMoneyHeaders.OrderByDescending(k => k.ReceiptNumber).FirstOrDefault();
                txtSerial.Text = (y.ReceiptNumber + 1).ToString();
            }
            isCancel = true;
            id = Guid.Empty;
        }

        private void datagrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            return;
            if (datagrid.SelectedItem != null && !AddedMode)
            {
                gridDelete.Visibility = Visibility.Visible;
                /*var acDocument_Detail = datagrid.SelectedItem as AcDocument_Detail;
                id = acDocument_Detail.Id;
                cmbType.TextChanged -= txtDoumentType_TextChanged;
                cmbType.Text = acDocument_Detail.tGroup.GroupCode.ToString();
                cmbType.TextChanged += txtDoumentType_TextChanged;
                txtSerial.Text = acDocument_Detail.tGroup.GroupName;
                txtNoDocumen.Text = acDocument_Detail.AcDocument_DetailName;
                gridDelete.Visibility = Visibility.Visible;
                borderEdit.Visibility = Visibility.Visible;
                cmbType.IsReadOnly = true;
                isCancel = true;
                GetError();*/
            }
            else
                gridDelete.Visibility = Visibility.Collapsed;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (id == Guid.Empty)
                return;
            if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید این اطلاعات پاک شود؟", "حذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }
            using var db = new wpfrazydbContext();
            foreach (var item in db.PaymentMoneyDetails.Where(u=>u.FkHeaderId==id))
            {
                if (item.MoneyType == 1 && (!(paymentMoney_Details.FirstOrDefault(s => s.Id == item.Id) is PaymentMoneyDetail paymentMoney) || paymentMoney.MoneyType != 1))
                {
                    var checkPaymentEvents = db.CheckPaymentEvents.Where(d => d.FkDetaiId == item.Id);
                    var checkPayment = checkPaymentEvents.OrderByDescending(k => k.Indexer).FirstOrDefault();
                    if (checkPayment != null)
                    {
                        //var entry = db.Entry(checkPayment);
                        //entry.Reference(a => a.ChEvent).Load();
                        if (checkPayment.FkChEvent.ChEventCode == 6)
                        {
                            foreach (var check in checkPaymentEvents)
                            {
                                db.CheckPaymentEvents.Remove(check);
                            }
                        }
                    }
                }
                db.PaymentMoneyDetails.Remove(item);
            }
            //حذف سند حسابداری
            var paymentMoneyHeader = db.PaymentMoneyHeaders.Find(id);
            if (paymentMoneyHeader.FkAcDocument is Guid acDocument)
            {
                foreach (var item in db.AcDocumentDetails.Where(u => u.FkAcDocHeaderId == acDocument))
                {
                    db.AcDocumentDetails.Remove(item);
                }
                db.AcDocumentHeaders.Remove(db.AcDocumentHeaders.Find(acDocument));
                foreach (var item in MainWindow.Current.tabcontrol.Items)
                {
                    if (item is TabItemExt tabItemExt)
                    {
                        if (tabItemExt.Header.ToString() == "سند حسابداری")
                        {
                            if (tabItemExt.Content is usrAccountDocument usrAccountDocument)
                            {
                                if (usrAccountDocument.LoadedFill)
                                {
                                    usrAccountDocument.AcDocumentHeaders.Remove(usrAccountDocument.AcDocumentHeaders.First(y => y.Id == acDocument));
                                }
                            }
                            break;
                        }
                    }
                }
            }
            db.PaymentMoneyHeaders.Remove(paymentMoneyHeader);
            if (!db.SafeSaveChanges())  return;
            try
            {
                PaymentMoneyHeaders.Remove(PaymentMoneyHeaders.First(f => f.Id == id));
            }
            catch
            {

            }
            id = Guid.Empty;
            //btnCancel_Click(null, null);
        }

        private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTermTextBox.Text.Trim() == string.Empty)
            {
                if (FirstLevelNestedGrid.SearchHelper.SearchText.Trim() != "")
                {

                }
                else
                    return;
            }
            try
            {
                if (datagrid.Visibility == Visibility.Visible)
                {
                    datagrid.SelectedIndex = -1;
                    datagrid.SearchHelper.Search(SearchTermTextBox.Text);
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    if (InputLanguageManager.Current.CurrentInputLanguage.Name != "fa-IR")
                    {
                        decimal ds = 0;
                        if (decimal.TryParse(SearchTermTextBox.Text.Trim().Replace(",", ""), out ds) && ds >= 0)
                        {

                            int temp = SearchTermTextBox.SelectionStart;
                            SearchTermTextBox.TextChanged -= SearchTermTextBox_TextChanged;
                            SearchTermTextBox.Text = string.Format("{0:#,###}", ds);
                            if (SearchTermTextBox.SelectionStart != temp)
                                SearchTermTextBox.SelectionStart = temp + 1;
                            SearchTermTextBox.TextChanged += SearchTermTextBox_TextChanged;
                        }
                    }
                    datagridSearch.SelectedIndex = -1;
                    if (SearchTermTextBox.Text.Trim() == "")
                    {
                        dataPager.Visibility = Visibility.Visible;
                        datagridSearch.SearchHelper.ClearSearch();
                        FirstLevelNestedGrid.SearchHelper.ClearSearch();
                        var g = dataPager.Source;
                        dataPager.Source = null;
                        dataPager.Source = g;
                    }
                    else
                    {
                        //dataPager.Visibility = Visibility.Collapsed;
                        datagridSearch.SearchHelper.Search("");
                        FirstLevelNestedGrid.SearchHelper.Search(SearchTermTextBox.Text);
                        SetHide_EmptyDetails();
                        //datagridSearch.View.Refresh();

                        //var h2 = FirstLevelNestedGrid.SearchHelper.GetSearchRecords();
                        //var h1 = datagridSearch.SearchHelper.GetSearchRecords();

                        /*foreach (AcDocument_Header item in datagridSearch.DetailsViewDefinition)
                        {
                            if(item.AcDocument_Detail.Count!=0)
                            {

                            }
                            else
                            {

                            }
                        }*/
                        //datagridSearch.SearchHelper.Search(SearchTermTextBox.Text);
                    }
                }
                if (SearchTermTextBox.Text == "")
                    RefreshDataGridForSetPersianNumber();
            }
            catch (Exception ex)
            {
            }
            Mouse.OverrideCursor = null;
        }

        private void SetHide_EmptyDetails()
        {
            if (SearchTermTextBox.Text == "")
                return;
            int ir = 0;
            var list = new List<int>();
            foreach (var item in datagridSearch.View?.Records)
            {
                var tt = item.Data as PaymentMoneyHeader;
                if (!tt.PaymentMoneyDetails.Any(i =>
                i.Name.ToLower().Contains(SearchTermTextBox.Text.ToLower()) ||
                i.Price2?.ToString().ToLower().Contains(SearchTermTextBox.Text.ToLower()) == true ||
                i.ColeMoein.ToLower().Contains(SearchTermTextBox.Text.ToLower()) ||
                i.PreferentialCode.ToLower().Contains(SearchTermTextBox.Text.ToLower()) ||
                i.BranchName?.ToString().ToLower().Contains(SearchTermTextBox.Text.ToLower()) == true ||
                i.Number?.ToString().ToLower().Contains(SearchTermTextBox.Text.ToLower()) == true ||
                i.GetMoneyType?.ToString().ToLower().Contains(SearchTermTextBox.Text.ToLower()) == true ||
                i.FkBankNavigation?.Name?.ToString().ToLower().Contains(SearchTermTextBox.Text.ToLower()) == true))
                {
                    //datagridSearch.View.Records.Remove(item);
                    list.Add(ir);
                }
                else
                {
                    item.IsExpanded = true;
                    //this.datagrid.ExpandDetailsViewAt(this.datagrid.ResolveToRecordIndex(ir));
                }
                ir++;
            }
            var l = 0;
            for (var i = 0; i < list.Count; i++)
            {
                datagridSearch.View.Records.RemoveAt(list[i] - l);
                l++;
            }

            datagridSearch.ExpandAllDetailsView();

        }

        private void txtNoDocumen_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void TxtCodeAcDocument_Detail_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }
        public static Window window;
        private void txtDoumentType_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void txtDoumentType_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void DataPager_PageIndexChanging(object sender, Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangingEventArgs e)
        {
            var ex = datagrid.View.FilterPredicates;

            using var db = new wpfrazydbContext();
            //db.AcDocument_Detail.Where(ex)
            var count = db.PaymentMoneyDetails.Count();
            var F = db.PaymentMoneyDetails.OrderBy(d => d.Id).Skip(10 * e.NewPageIndex).Take(10).ToList();
            int j = 0;
            for (int i = 10 * e.NewPageIndex; i < 10 * (e.NewPageIndex + 1) && i < count; i++)
            {
                paymentMoney_Details[i] = F[j];
                j++;
            }
        }
        public bool CloseForm()
        {
            if (!isCancel && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این فرم خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return false;
            }
            forceClose = true;
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(u => u.Header == "پرداخت وجه");
            MainWindow.Current.tabcontrol.Items.Remove(item);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Dispose();
            }));
            return true;
        }

        private void ClearSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            ClearSearch.Opacity = 1;
        }

        private void ClearSearch_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearSearch.Opacity = .65;
        }

        private void ClearSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchTermTextBox.Clear();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            datagridSearch.AllowFiltering = !datagridSearch.AllowFiltering;
            FirstLevelNestedGrid.AllowFiltering = !FirstLevelNestedGrid.AllowFiltering;
            if (!datagridSearch.AllowFiltering)
                datagridSearch.ClearFilters();
            if (!FirstLevelNestedGrid.AllowFiltering)
                FirstLevelNestedGrid.ClearFilters();
        }

        public void SetNull()
        {
            if (window != null)
            {
                if ((window as winSearch).ParentTextBox is PaymentMoneyDetail)
                {
                    var y = (window as winSearch).ParentTextBox as PaymentMoneyDetail;
                    //((datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell).Content as FrameworkElement).DataContext = null;
                    //((datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell).Content as FrameworkElement).DataContext = y;
                    var detail = y;
                    var v = datagrid.SelectionController.CurrentCellManager.CurrentCell;
                    if ((window as winSearch)?.MuText != null)
                    {
                        datagrid.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //MMM
                            var th = new Thread(() =>
                            {
                                Thread.Sleep(100);
                                Dispatcher.Invoke(() =>
                                {
                                    var i = 1;
                                    if (v.ColumnIndex == 2)
                                        i++;
                                    if (datagrid.SelectedIndex == -1)
                                    {
                                        datagrid.GetAddNewRowController().CommitAddNew();
                                        datagrid.View.Refresh();
                                        datagrid.SelectedIndex = datagrid.GetLastRowIndex() - 1;
                                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(v.RowIndex - 1, v.ColumnIndex + i));
                                    }
                                    else
                                    {
                                        datagrid.View.Refresh();
                                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(v.RowIndex, v.ColumnIndex + i));
                                    }
                                    //MMM
                                    datagrid.IsHitTestVisible = true;
                                });
                            });
                            th.Start();
                            //datagrid.SelectCells(datagrid.GetRecordAtRowIndex(datagrid.SelectedIndex-1), datagrid.Columns[1], datagrid.GetRecordAtRowIndex(datagrid.SelectedIndex), datagrid.Columns[2]);
                        }));
                    }
                }
                else if ((window as winSearch).ParentTextBox is TextBox textBox && textBox.Tag.ToString() != "True")
                {
                    if (textBox.Name == "txtMoein")
                    {
                        txbMoein.Text = ((textBox.Tag as Mu).AdditionalEntity as AccountSearchClass).MoeinName;
                    }
                    else
                    {
                        txbPreferential.Text = (textBox.Tag as Mu).Name;
                    }
                    datagrid.Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(100);
                        TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                        request.Wrapped = true;
                        textBox.MoveFocus(request);
                    }), DispatcherPriority.Background);
                }
            }
            window = null;
        }

        private void pcw1_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            txbCalender.Text = pcw1.SelectedDate.ToString();

        }

        private void Pcw1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void datagrid_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            return;
            if (e.NewObject is PaymentMoneyDetail PaymentMoneyDetail && PaymentMoneyDetail.MoneyType == 0 && PaymentMoneyDetail.FkMoein == null)
            {

            }
            /*
            var h = acDocumentViewModel.acDocument_Details.FirstOrDefault(q => q.AcCode == ctext);
            if (h != null)
            {
                (e.NewObject as UtililtyCommodity).CommodityId = h.ID;
                (e.NewObject as UtililtyCommodity).CommodityCode = ctext;
                (e.NewObject as UtililtyCommodity).Discount = 0;
                (e.NewObject as UtililtyCommodity).Tax = h.Tax;
            }*/
        }

        private void searchImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (morefields.Visibility == Visibility.Visible)
            {
                morefields.Visibility = Visibility.Collapsed;
                column1.Width = new GridLength(170);
                column2.Width = new GridLength(170);
            }
            if (searchImage.Opacity != .6)
            {
                if (!AddedMode && (searchImage.Source as BitmapImage).UriSource.AbsoluteUri.Contains("dataedit.png"))
                {
                    if (datagridSearch.SelectedItem == null)
                        return;
                    searchImage.Visibility = Visibility.Visible;
                    searchImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Data.png"));
                    searchImage.ToolTip = "جستجو";
                    GStop1.Color = new Color()
                    {
                        R = 244,
                        G = 248,
                        B = 255,
                        A = 255
                    };
                    searchImage.Opacity = 1;
                    gridDelete.Visibility = Visibility.Collapsed;
                    searchGrid.Visibility = Visibility.Collapsed;
                    paymentMoney_Details.Clear();
                    temp_paymentMoney_Details.Clear();
                    var header = datagridSearch.SelectedItem as PaymentMoneyHeader;
                    id = header.Id;
                    //header.PaymentMoneyDetails.ForEach(t => temp_paymentMoney_Details.Add(t.DeepClone()));
                    header.PaymentMoneyDetails.ForEach(t => paymentMoney_Details.Add(t));
                    //cmbType.SelectedItem = (cmbType.ItemsSource as List<DocumentType>).First(u => u.Id == header.DocumentType.Id);
                    pcw1.SelectedDate = new PersianCalendarWPF.PersianDate(header.Date);
                    txbCalender.Text = pcw1.SelectedDate.ToString();
                    Sf_txtMoein.HasError = Sf_txtPreferential.HasError = false;
                    datagrid.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#FF808080"));

                    txtSerial.Text = header.ReceiptNumber.ToString();
                    txtMoein.Text = $"{header.FkMoein.FkCol.ColCode}{header.FkMoein.MoeinCode}";
                    txbMoein.Text = header.FkMoein.MoeinName;
                    txtMoein.Tag = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == txtMoein.Text);
                    txtPreferential.Text = header.FkPreferential.PreferentialCode.ToString();
                    txbPreferential.Text = header.FkPreferential.PreferentialName;
                    txtPreferential.Tag = mus2.Find(t => t.Value == txtPreferential.Text);
                    txtDescription.Text = header.Description;

                    datagrid.AllowEditing = datagrid.AllowDeleting = true;
                    datagrid.AddNewRowPosition = Syncfusion.UI.Xaml.Grid.AddNewRowPosition.Bottom;
                    datagrid.Visibility = Visibility.Visible;
                    dataPager.Visibility = Visibility.Collapsed;
                    testsearch.Text = "جستجو...";
                    try
                    {
                        datagrid.SearchHelper.ClearSearch();
                    }
                    catch { }
                    SearchTermTextBox.TextChanged -= SearchTermTextBox_TextChanged;
                    SearchTermTextBox.Text = "";
                    SearchTermTextBox.TextChanged += SearchTermTextBox_TextChanged;
                    datagridSearch.Visibility = Visibility.Collapsed;
                    //gridSetting.Visibility = 
                        gridConfirm.Visibility = Visibility.Visible;
                    column1.Width = new GridLength(170);
                    column2.Width = new GridLength(225);
                    borderEdit.Visibility = Visibility.Visible;
                    RefreshDataGridForSetPersianNumber();
                    datagrid.SelectedIndex = paymentMoney_Details.Count - 1;
                    isCancel = true;
                    StateLoadView = true;
                    datagrid.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                new Action(async () =>
                {
                    await Task.Delay(200);
                    StateLoadView = false;
                    datagrid.Focus();
                }));
                }
                else
                {
                    if (!isCancel)
                    {
                        if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این عملیات انصراف دهید؟", "انصراف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                        {
                            return;
                        }
                        isCancel = true;
                    }
                    FillHeaders();
                    if (!AddedMode)
                    {
                        RefreshHeader();
                    }
                    datagridSearch.ClearFilters();
                    datagridSearch.SortColumnDescriptions.Clear();
                    datagridSearch.SortColumnDescriptions.Add(new SortColumnDescription()
                    {
                        ColumnName = "ReceiptNumber",
                        SortDirection = System.ComponentModel.ListSortDirection.Descending
                    });
                    datagridSearch.SearchHelper.ClearSearch();
                    FirstLevelNestedGrid.SearchHelper.ClearSearch();
                    SearchTermTextBox.Text = "";
                    datagridSearch.SelectedItem = null;
                    var t = dataPager.Source;
                    dataPager.Source = null;
                    borderEdit.Visibility = Visibility.Collapsed;
                    gridDelete.Visibility = Visibility.Visible;
                    searchGrid.Visibility = Visibility.Visible;
                    datagrid.Visibility = Visibility.Collapsed;
                    datagridSearch.Visibility = Visibility.Visible;
                    dataPager.Visibility = Visibility.Visible;
                    testsearch.Text = "جستجو در جزئیات...";
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        datagridSearch.Visibility = Visibility.Collapsed;
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            datagridSearch.Visibility = Visibility.Visible;
                            dataPager.Source = t;
                        }), DispatcherPriority.Render);
                    }), DispatcherPriority.Render);
                    gridSetting.Visibility = gridConfirm.Visibility = Visibility.Collapsed;
                    if ((t as ObservableCollection<PaymentMoneyHeader>).Count == 0)
                        searchImage.Opacity = .6;
                    searchImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/dataedit.png"));
                    searchImage.ToolTip = "ویرایش";
                    GStop1.Color = new Color()
                    {
                        R = 209,
                        G = 226,
                        B = 255,
                        A = 240
                    };
                    column1.Width = new GridLength(0);
                    column2.Width = new GridLength(0);
                    datagrid.AllowEditing = datagrid.AllowDeleting = false;
                    datagrid.AddNewRowPosition = Syncfusion.UI.Xaml.Grid.AddNewRowPosition.None;
                    AddedMode = false;
                    datagrid.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
                new Action(() =>
                {
                    SetHide_EmptyDetails();
                }));
                    datagrid.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render,
                new Action(async () =>
                {
                    await Task.Delay(50);
                    datagridSearch.SelectedIndex = 0;
                }));
                }
            }
        }
        private void RefreshHeader()
        {
            using var db = new wpfrazydbContext();
            var e_Edidet = db.PaymentMoneyHeaders.Include(d => d.FkPreferential)
                .Include(d => d.FkMoein)
                .ThenInclude(d => d.FkCol)
                .Include(h => h.PaymentMoneyDetails)
                .ThenInclude(d => d.FkPreferential)
                .Include(h => h.PaymentMoneyDetails)
                .ThenInclude(d => d.FkMoein)
                .ThenInclude(d => d.FkCol)
                .Include(h => h.PaymentMoneyDetails)
                .ThenInclude(d => d.FkBankNavigation)
                .First(g => g.Id == id);
            var header = PaymentMoneyHeaders.FirstOrDefault(o => o.Id == id);
            header.PaymentMoneyDetails.Clear();
            e_Edidet.PaymentMoneyDetails = e_Edidet.PaymentMoneyDetails
              .OrderBy(d => d.Indexer)
              .ToList();
            foreach (var item in e_Edidet.PaymentMoneyDetails)
            {
                header.PaymentMoneyDetails.Add(item);
                SetAccountName(db, item);
            }
        }
        bool LoadedFill = false;
        private void FillHeaders()
        {
            if (!LoadedFill)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using var db = new wpfrazydbContext();
                var documents = db.PaymentMoneyHeaders
                    .Include(d => d.FkPreferential)
                    .Include(d => d.FkMoein)
                    .ThenInclude(d => d.FkCol)
                    .Include(h => h.PaymentMoneyDetails)
                    .ThenInclude(d => d.FkPreferential)
                    .Include(h => h.PaymentMoneyDetails)
                    .ThenInclude(d => d.FkMoein)
                    .ThenInclude(d => d.FkCol)
                    .Include(h => h.PaymentMoneyDetails)
                    .ThenInclude(d => d.FkBankNavigation)
                    .AsNoTracking()
                    .ToList();
                foreach (var item in documents)
                {
                    item.PaymentMoneyDetails = item.PaymentMoneyDetails
                        .OrderBy(d => d.Indexer)
                        .ToList();
                    /*foreach (var item2 in item.PaymentMoneyDetail)
                    {
                        SetAccountName(db, item2);
                    }*/
                    PaymentMoneyHeaders.Add(item);
                }
                LoadedFill = true;
                Mouse.OverrideCursor = null;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Wait;
                PaymentMoneyHeaders.ForEach(y => y.PaymentMoneyDetails = y.PaymentMoneyDetails
                   .OrderBy(d => d.Indexer)
                   .ToList());
                Mouse.OverrideCursor = null;
            }
        }

        private void RefreshDataGridForSetPersianNumber()
        {
            var wy = datagrid.Template;
            var uy = datagrid.ItemsSource;
            datagrid.Template = null;
            datagrid.ItemsSource = null;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                datagrid.Template = wy;
                datagrid.ItemsSource = uy;
            }), DispatcherPriority.Render);
        }

        private void datagridSearch_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (datagridSearch.SelectedItem != null)
            {
                searchImage.Opacity = 1;
                var header = datagridSearch.SelectedItem as PaymentMoneyHeader;
                id = header.Id;
            }
            else if (datagrid.Visibility != Visibility.Visible)
                id = Guid.Empty;
        }

        private void datagrid_RowValidated(object sender, RowValidatedEventArgs e)
        {
            var detail = e.RowData as PaymentMoneyDetail;
            if (datagrid.SelectedIndex != -1 && detail.MoneyType == 0 && detail.FkMoein == null)
            {
                paymentMoney_Details.Remove(detail);
                return;
            }
            var currentCell = datagrid.SelectionController.CurrentCellManager?.CurrentCell;
            if (window != null)
                (window as winSearch).ParentTextBox = detail;
            /*if (currentCell?.ColumnIndex == 4 && (detail.Debtor ?? 0) != 0)
                detail.Creditor = null;
            if (currentCell?.ColumnIndex == 5 && (detail.Creditor ?? 0) != 0)
                detail.Debtor = null;*/
        }

        private void AcDocument_Details_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var detail = paymentMoney_Details.LastOrDefault();
            if (detail == null)
                return;
            if ((Keyboard.IsKeyDown(Key.Enter) || datagrid.SelectedIndex != -1 || CurrentRowColumnIndex.ColumnIndex != 0) && detail.MoneyType != 3 && detail.ColeMoein == null && detail.PreferentialCode == null)
            {
                datagrid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    paymentMoney_Details.Remove(detail);
                }));
            }
            datagrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                CalDebCre();
            }));
        }

        private void CalDebCre()
        {
            //if (datagrid.SelectionController.CurrentCellManager?.CurrentCell?.ColumnIndex >= 4)
            //{
            //    var t = datagrid.ItemsSource;
            //    datagrid.ItemsSource = null;
            //    datagrid.ItemsSource = t;
            //}
            datagrid.View?.Refresh();
            return;

            //var c = AcDocument_Details.Sum(y => y.Creditor);
            //var d = AcDocument_Details.Sum(y => y.Debtor);
            //{
            //    datagrid.TableSummaryRows[0].SummaryColumns.Add(new GridSummaryColumn() {Name="hfgh", Format = "{Sum:N0}", MappingName = "Debtor", SummaryType = Syncfusion.Data.SummaryType.DoubleAggregate });

            //}
            //datagrid.TableSummaryRows.Clear();
            //var gridSummaryRow = new Syncfusion.UI.Xaml.Grid.GridSummaryRow();            
            //var Tafazol = AcDocument_Details.Sum(y => y.Debtor) - AcDocument_Details.Sum(y => y.Creditor);
            //if (Tafazol != null)
            //{
            //    var sign = Tafazol.Value >= 0 ? "" : "منفی";
            //    datagrid.TableSummaryRows.Add(new Syncfusion.UI.Xaml.Grid.GridSummaryRow() { Title = $"اختلاف : {string.Format("{0:#,###}", Math.Abs(Tafazol.Value))} {sign}" });
            //}
        }

        private void datagridSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchImage_PreviewMouseDown(null, null);
            }
        }
        private void PART_AdvancedFilterControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        TextBox textBox1, textBox2;
        DatePicker datePicker1, datePicker2;
        private void PART_AdvancedFilterControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var advance = sender as AdvancedFilterControl;
            if (datePicker1 == null)
            {
                var comboBoxes = advance.GetChildsOfType<ComboBox>();
                var combo = comboBoxes[1];
                var grid = combo.Parent as Grid;
                grid.Children[0].Visibility = Visibility.Collapsed;
                grid.Children[1].Visibility = Visibility.Visible;
                (grid.Children[1] as TextBox).IsReadOnly = true;
                textBox1 = grid.Children[1] as TextBox;
                (comboBoxes[3].Parent as Grid).Children[0].Visibility = Visibility.Collapsed;
                (comboBoxes[3].Parent as Grid).Children[1].Visibility = Visibility.Visible;
                textBox2 = (comboBoxes[3].Parent as Grid).Children[1] as TextBox;
                ((comboBoxes[3].Parent as Grid).Children[1] as TextBox).IsReadOnly = true;
                datePicker1 = grid.Children[2] as DatePicker;
                datePicker2 = ((comboBoxes[3].Parent as Grid).Children[2]) as DatePicker;
                (MyPopupS.Parent as Grid).Children.Remove(MyPopupS);
                MyPopupS.Visibility = Visibility.Visible;
                grid.Children.Add(MyPopupS);
                (MyPopupE.Parent as Grid).Children.Remove(MyPopupE);
                MyPopupE.Visibility = Visibility.Visible;
                (comboBoxes[3].Parent as Grid).Children.Add(MyPopupE);
            }
            if (datePicker1?.IsMouseOver == true)
            {
                //grid.Children.RemoveAt(2);
                FieldInfo fieldInfo = typeof(DatePicker).GetField("_popUp", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                (fieldInfo.GetValue(datePicker1) as Popup).IsOpen = false;
                if (textBox1.Text == null || textBox1.Text == "" || persianCalendar.SelectedDate.ToDateTime() == System.DateTime.Today)
                {
                    persianCalendar.SelectedDate = new Mahdi.PersianDate(System.DateTime.Today.AddDays(-1));
                    persianCalendar.SelectedDate = new Mahdi.PersianDate(System.DateTime.Today);
                }
                MyPopupS.IsOpen = true;
            }
            if (datePicker2?.IsMouseOver == true)
            {
                //grid.Children.RemoveAt(2);
                FieldInfo fieldInfo = typeof(DatePicker).GetField("_popUp", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                (fieldInfo.GetValue(datePicker2) as Popup).IsOpen = false;
                if (textBox2.Text == null || textBox2.Text == "" || persianCalendarE.SelectedDate.ToDateTime() == System.DateTime.Today)
                {
                    persianCalendarE.SelectedDate = new Mahdi.PersianDate(System.DateTime.Today.AddDays(-1));
                    persianCalendarE.SelectedDate = new Mahdi.PersianDate(System.DateTime.Today);
                }
                MyPopupE.IsOpen = true;
            }
        }

        GridFilterControl gridFilterControl;
        private void PART_AdvancedFilterControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (textBox1 != null)
            {
                textBox1.Text = string.Empty;
                textBox1.TextChanged += TextBox1_TextChanged;
                textBox2.Text = string.Empty;
                textBox2.TextChanged += TextBox2_TextChanged; ;
            }
            datePicker1 = datePicker2 = null;
            //textBox1 = null;
            //textBox2 = null;
            var advance = sender as AdvancedFilterControl;
            advance.Tag = true;
            FieldInfo fieldInfo = typeof(AdvancedFilterControl).GetField("gridFilterCtrl", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            gridFilterControl = (GridFilterControl)fieldInfo.GetValue(advance);
        }

        private void TextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            var h = textBox2.Text.Split('/');
            textBox2.TextChanged -= TextBox2_TextChanged;
            textBox2.Text = $"{h[2]}/{h[1]}/{h[0]}";
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            var h = textBox1.Text.Split('/');
            textBox1.TextChanged -= TextBox1_TextChanged;
            textBox1.Text = $"{h[2]}/{h[1]}/{h[0]}";
        }
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        // تعریف ثابت‌ها
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;  // برای نگه داشتن کلیک راست
        private const int MOUSEEVENTF_RIGHTUP = 0x10;    // برای رها کردن کلیک راست

        // تابع برای ارسال رویداد موس از کتابخانه user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        static void LeftDoubleClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

            System.Threading.Thread.Sleep(50);

            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        static void RightClick()
        {
            // کلیک راست را نگه دارید
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);

            // کلیک راست را رها کنید
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        private void persianCalendar_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            MyPopupS.IsOpen = false;
            if (datePicker1 == null && datagrid.Visibility == Visibility.Visible)
            {
                (datagrid.SelectedItem as PaymentMoneyDetail).Date = persianCalendar.SelectedDate.ToDateTime();
                datagrid.IsHitTestVisible = true;
                datagrid.View.Refresh();
                return;
            }
            datePicker1.SelectedDate = persianCalendar.SelectedDate.ToDateTime();
            var persian = new System.Globalization.PersianCalendar();
            var h = $"{persian.GetYear(datePicker1.SelectedDate.Value)}/{persian.GetMonth(datePicker1.SelectedDate.Value)}/{persian.GetDayOfMonth(datePicker1.SelectedDate.Value)}";
            ((MyPopupS.Parent as Grid).Children[1] as TextBox).Text = h;
        }

        private void persianCalendarE_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            if (MyPopupE.IsOpen)
                datagrid.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await Task.Delay(80);
                    MyPopupE.IsOpen = false;
                }));
            datePicker2.SelectedDate = persianCalendarE.SelectedDate.ToDateTime();
            var persian = new System.Globalization.PersianCalendar();
            var h = $"{persian.GetYear(datePicker2.SelectedDate.Value)}/{persian.GetMonth(datePicker2.SelectedDate.Value)}/{persian.GetDayOfMonth(datePicker2.SelectedDate.Value)}";
            ((MyPopupE.Parent as Grid).Children[1] as TextBox).Text = h;
        }
        bool rl1, rl2 = false;

        private void persianCalendarE_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rl2 = true;
            RightClick();
        }
        RowColumnIndex CurrentRowColumnIndex;

        private void datagrid_CurrentCellBeginEdit(object sender, CurrentCellBeginEditEventArgs e)
        {
            if (datagrid.SelectedIndex == -1)
            {
                e.Cancel = true;
                Xceed.Wpf.Toolkit.MessageBox.Show("ابتدا باید نوع وجه  را تعیین کنید!");
            }
            else if ((datagrid.SelectionController.CurrentCellManager?.CurrentCell.Element as GridCell)?.DataContext is PaymentMoneyDetail PaymentMoneyDetail)
            {
                if (PaymentMoneyDetail.MoneyType != 3)
                {
                    if (e.RowColumnIndex.ColumnIndex == 2 && PaymentMoneyDetail.MoneyType == 1)
                    {

                    }
                    else
                    {
                        if (e.RowColumnIndex.ColumnIndex == 1 || e.RowColumnIndex.ColumnIndex == 2)
                            e.Cancel = true;
                        else if (PaymentMoneyDetail.MoneyType != 1 && e.RowColumnIndex.ColumnIndex >= 5)
                            e.Cancel = true;
                    }
                }
                else if (e.RowColumnIndex.ColumnIndex == 5 || e.RowColumnIndex.ColumnIndex == 7 || e.RowColumnIndex.ColumnIndex == 8)
                    e.Cancel = true;
            }
            if (SearchTermTextBox.Text != "")
            {
                datagrid.SearchHelper.ClearSearch();
                SearchTermTextBox.Text = "";
            }
            CurrentRowColumnIndex = e.RowColumnIndex;
            CurrentCellText = "";
        }
        string CurrentCellText;
        private void datagrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {
            var ele = (datagrid.SelectionController.CurrentCellManager?.CurrentCell.Element as GridCell).Content;
            if (ele is TextBox textBox)
            {
                if (textBox.Text != "" && e.Record is PaymentMoneyDetail detail && detail.GetType().GetProperty(e.Column.MappingName).GetValue(detail)?.ToString() != textBox.Text && !Keyboard.IsKeyDown(Key.Enter))
                    CurrentCellText = textBox.Text;
            }
            else if (ele is CheckBox check)
            {
                if (e.Record is PaymentMoneyDetail detail && detail.MoneyType != 1 && e.RowColumnIndex.ColumnIndex >= 5)
                {
                    check.IsChecked = null;
                }
                else
                {
                    datagrid.Focus();
                }
            }
        }

        private void datagrid_RowValidating(object sender, RowValidatingEventArgs e)
        {
            if (e.RowData is PaymentMoneyDetail detail)
            {
                var dataColumn = datagrid.SelectionController.CurrentCellManager?.CurrentCell;
                var textBox = (dataColumn.Element as GridCell).Content as TextBox;
                if (textBox == null)
                    return;
                var u = textBox.Text == "" ? CurrentCellText : textBox.Text;
                if (dataColumn.ColumnIndex == 1)
                {
                    var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == u);
                    if (mu == null)
                    {
                        e.IsValid = false;
                        e.ErrorMessages.Add("ColeMoein", "چنین کل و معینی وجود ندارد!");
                    }
                }
                else if (dataColumn.ColumnIndex == 2)
                {
                    var mu = mus2.Find(t => t.Value == u);
                    if (mu == null)
                    {
                        e.IsValid = false;
                        e.ErrorMessages.Add("PreferentialCode", "چنین تفضیلی وجود ندارد!");
                    }
                }
            }
        }

        private void datagridSearch_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var t = datagridSearch.GetChildByName<Grid>("PART_GroupDropAreaGrid");
            if (t == null) return;
            var textBlock = (t.Children[0] as Grid).Children[1] as TextBlock;
            textBlock.Foreground = Brushes.DarkBlue;
            textBlock.FontWeight = FontWeights.Bold;
        }

        private void datagridSearch_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // ارتفاع سطرهای grid را محاسبه کنید (می‌توانید ارتفاع سطر ثابت فرض کنید)
            double rowHeight = 30; // ارتفاع هر سطر (این مقدار ممکن است بسته به طراحی تغییر کند)

            // ارتفاع موجود در grid را محاسبه کنید
            double availableHeight = datagridSearch.ActualHeight;

            // محاسبه تعداد سطرهایی که در صفحه جا می‌شوند
            int visibleRows = (int)(availableHeight / rowHeight);

            // تنظیم PageSize بر اساس تعداد سطرهای محاسبه شده
            if (visibleRows > 0)
            {
                var y = dataPager.PageSize;
                dataPager.PageSize = visibleRows - 4;
                if (dataPager.PageSize != y)
                {
                    var g = dataPager.Source;
                    dataPager.Source = null;
                    dataPager.Source = g;
                    dataPager.Visibility = Visibility.Visible;
                    datagridSearch.SearchHelper.ClearSearch();
                    SearchTermTextBox.Text = "";
                }
            }
        }
        bool StateLoadView = false;
        private void ComboBoxAdv_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (StateLoadView || Keyboard.IsKeyDown(Key.Delete))
                return;
            var comboBoxAdv = sender as Syncfusion.Windows.Tools.Controls.ComboBoxAdv;
            var hg = comboBoxAdv.SelectedIndex;
            switch (comboBoxAdv.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
            var v = datagrid.SelectionController.CurrentCellManager.CurrentCell;
            if (v != null)
                CurrentRowColumnIndex = new RowColumnIndex(v.RowIndex - 1, v.ColumnIndex);
            if (v != null && hg != -1)
            {
                if (datagrid.SelectedIndex == -1)
                {
                    var gridAddNewRowController = datagrid.GetAddNewRowController();
                    try
                    {
                        gridAddNewRowController.AddNew();
                    }
                    catch { }
                    datagrid.GetAddNewRowController().CommitAddNew(true);
                    datagrid.View.Refresh();
                    if (hg == 3)
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(datagrid.GetLastRowIndex(), v.ColumnIndex + 1));
                    else if (hg == 1)
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(datagrid.GetLastRowIndex(), v.ColumnIndex + 2));
                    else
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(datagrid.GetLastRowIndex(), v.ColumnIndex + 4));
                    datagrid.Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(50);
                        var dataColumn = datagrid.SelectionController.CurrentCellManager.CurrentCell;
                        if (dataColumn == null)
                            return;
                        var PaymentMoneyDetail = (dataColumn.Element as GridCell).DataContext as PaymentMoneyDetail;
                        if (PaymentMoneyDetail == null)
                            return;
                        PaymentMoneyDetail.MoneyType = (byte)hg;
                        PaymentMoneyDetail.ClearErrors();
                        using var db = new wpfrazydbContext();
                        var tMoein = db.Moeins.Include("FkCol");
                        if (db.CodeSettings.Any(t => t.Name == "MoeinCodeCheckPayment"))
                        {
                        }
                        else
                        {
                            MessageBox.Show("تنظیمات پیکر بندی را انحام دهید!");
                            return;
                        }
                        switch (PaymentMoneyDetail.MoneyType)
                        {
                            case 0:
                                var moein = int.Parse(db.CodeSettings.First(j => j.Name == "MoeinCodeMoneyPayment").Value);
                                var col = int.Parse(db.CodeSettings.First(j => j.Name == "ColCodeMoneyPayment").Value);
                                var p = int.Parse(db.CodeSettings.First(j => j.Name == "PreferentialCodeMoneyPayment").Value);
                                PaymentMoneyDetail.FkMoein = tMoein.First(d => d.MoeinCode == moein && d.FkCol.ColCode == col);
                                PaymentMoneyDetail.FkPreferential = db.Preferentials.First(d => d.PreferentialCode == p);
                                break;
                            case 1:
                                moein = int.Parse(db.CodeSettings.First(j => j.Name == "MoeinCodeCheckPayment").Value);
                                col = int.Parse(db.CodeSettings.First(j => j.Name == "ColCodeCheckPayment").Value);
                                PaymentMoneyDetail.FkMoein = tMoein.First(d => d.MoeinCode == moein && d.FkCol.ColCode == col);
                                PaymentMoneyDetail.FkPreferential = null;
                                break;
                            case 2:
                                moein = int.Parse(db.CodeSettings.First(j => j.Name == "MoeinCodeDiscountPayment").Value);
                                col = int.Parse(db.CodeSettings.First(j => j.Name == "ColCodeDiscountPayment").Value);
                                p = int.Parse(db.CodeSettings.First(j => j.Name == "PreferentialCodeDiscountPayment").Value);
                                PaymentMoneyDetail.FkMoein = tMoein.First(d => d.MoeinCode == moein && d.FkCol.ColCode == col);
                                PaymentMoneyDetail.FkPreferential = db.Preferentials.First(d => d.PreferentialCode == p);
                                break;
                            case 3:
                                PaymentMoneyDetail.FkMoein = null;
                                PaymentMoneyDetail.FkPreferential = null;
                                break;
                        }
                        datagrid.View.Refresh();
                        datagrid.Focus();
                    }), DispatcherPriority.Render);
                }
                else
                {
                    var PaymentMoneyDetail = (v.Element as GridCell).DataContext as PaymentMoneyDetail;
                    if (PaymentMoneyDetail != comboBoxAdv.DataContext || v.ColumnIndex != 0)
                        return;
                    PaymentMoneyDetail.MoneyType = (byte)hg;
                    PaymentMoneyDetail.ClearErrors();
                    using var db = new wpfrazydbContext();
                    var tMoein = db.Moeins.Include("FkCol");
                    if (db.CodeSettings.Any(t => t.Name == "MoeinCodeCheckPayment"))
                    {
                    }
                    else
                    {
                        MessageBox.Show("تنظیمات پیکر بندی را انحام دهید!");
                        return;
                    }
                    switch (PaymentMoneyDetail.MoneyType)
                    {
                        case 0:
                            var moein = int.Parse(db.CodeSettings.First(j => j.Name == "MoeinCodeMoneyPayment").Value);
                            var col = int.Parse(db.CodeSettings.First(j => j.Name == "ColCodeMoneyPayment").Value);
                            var p = int.Parse(db.CodeSettings.First(j => j.Name == "PreferentialCodeMoneyPayment").Value);
                            PaymentMoneyDetail.FkMoein = tMoein.First(d => d.MoeinCode == moein && d.FkCol.ColCode == col);
                            PaymentMoneyDetail.FkPreferential = db.Preferentials.First(d => d.PreferentialCode == p);
                            break;
                        case 1:
                            moein = int.Parse(db.CodeSettings.First(j => j.Name == "MoeinCodeCheckPayment").Value);
                            col = int.Parse(db.CodeSettings.First(j => j.Name == "ColCodeCheckPayment").Value);
                            PaymentMoneyDetail.FkMoein = tMoein.First(d => d.MoeinCode == moein && d.FkCol.ColCode == col);
                            PaymentMoneyDetail.FkPreferential = null;
                            break;
                        case 2:
                            moein = int.Parse(db.CodeSettings.First(j => j.Name == "MoeinCodeDiscountPayment").Value);
                            col = int.Parse(db.CodeSettings.First(j => j.Name == "ColCodeDiscountPayment").Value);
                            p = int.Parse(db.CodeSettings.First(j => j.Name == "PreferentialCodeDiscountPayment").Value);
                            PaymentMoneyDetail.FkMoein = tMoein.First(d => d.MoeinCode == moein && d.FkCol.ColCode == col);
                            PaymentMoneyDetail.FkPreferential = db.Preferentials.First(d => d.PreferentialCode == p);
                            break;
                        case 3:
                            PaymentMoneyDetail.FkMoein = null;
                            PaymentMoneyDetail.FkPreferential = null;
                            break;
                    }
                    switch (PaymentMoneyDetail.MoneyType)
                    {
                        case 0:
                        case 2:
                            PaymentMoneyDetail.FkBank = null;
                            PaymentMoneyDetail.Date = null;
                            PaymentMoneyDetail.Number = null;
                            break;
                    }
                    StateLoadView = true;
                    if (hg == 3)
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(datagrid.SelectedIndex + 1, v.ColumnIndex + 1));
                    else if (hg == 1)
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(datagrid.SelectedIndex + 1, v.ColumnIndex + 2));
                    else
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(datagrid.SelectedIndex + 1, v.ColumnIndex + 4));
                    datagrid.View.Refresh();
                    datagrid.Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(50);
                        StateLoadView = false;
                        datagrid.Focus();
                    }), DispatcherPriority.Render);
                }
            }
        }

        private void txtDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void txtMoein_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                txtMoein.Tag = true;
                ShowSearchMoein(txtMoein);
            }
        }

        private winSearch ShowSearchMoein(dynamic y, Window owner = null)
        {
            var win = new winSearch(mus1);
            win.Closed += (yf, rs) =>
            {
                datagrid.IsHitTestVisible = true;
            };
            win.Width = 640;
            win.datagrid.Columns[0].HeaderText = "نام";
            win.datagrid.Columns[1].HeaderText = "کل";
            win.datagrid.Columns[0].Width = 255;
            win.datagrid.Columns[1].Width = 100;
            win.datagrid.Columns.MoveTo(0, 1);
            win.datagrid.Columns.Add(new GridTextColumn() { TextAlignment = TextAlignment.Center, HeaderText = "معین", MappingName = "AdditionalEntity.Moein", Width = 100, AllowSorting = true });
            win.datagrid.Columns.Add(new GridTextColumn() { TextAlignment = TextAlignment.Center, HeaderText = "نام", MappingName = "AdditionalEntity.MoeinName", AllowSorting = true, ColumnSizer= GridLengthUnitType.AutoWithLastColumnFill });
            win.datagrid.AllowResizingColumns = true;
            if (owner == null)
                win.Tag = this;
            else
                win.Tag = owner;
            if (owner == null)
                owner = MainWindow.Current;
            win.ParentTextBox = y;
            win.SearchTermTextBox.Text = "";
            win.SearchTermTextBox.Select(1, 0);
            win.Owner = MainWindow.Current;
            window = win;
            win.Show();
            win.Focus();
            return win;
        }

        private void txtPreferential_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                txtPreferential.Tag = true;
                ShowSearchPreferential(txtPreferential);
            }
        }

        private winSearch ShowSearchPreferential(dynamic y, Window owner = null)
        {
            var win = new winSearch(mus2);
            win.Closed += (yf, rs) =>
            {
                datagrid.IsHitTestVisible = true;
            };
            win.datagrid.Columns.Add(new GridTextColumn() { TextAlignment = TextAlignment.Center, HeaderText = "گروه", MappingName = "Name2", Width = 150, AllowSorting = true });
            win.Width = 640;
            if (owner == null)
                win.Tag = this;
            else
                win.Tag = owner;
            if (owner == null)
                owner = MainWindow.Current;
            win.ParentTextBox = y;
            win.SearchTermTextBox.Text = "";
            win.SearchTermTextBox.Select(1, 0);
            win.Owner = MainWindow.Current;
            window = win;
            win.Show();
            win.Focus();
            return win;
        }

        private void datagrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var currentCell = datagrid.SelectionController.CurrentCellManager?.CurrentCell;
            if (currentCell != null)
            {
                if ((currentCell.Element as GridCell)?.IsMouseOver == true && currentCell.ColumnIndex == 7 && ((currentCell.Element as GridCell).Content as ContentControl)?.Content is TextBlock)
                {
                    var PaymentMoneyDetail = (currentCell.Element as GridCell).DataContext as PaymentMoneyDetail;
                    if (PaymentMoneyDetail == null)
                        return;
                    if (!(PaymentMoneyDetail.MoneyType == 0 || PaymentMoneyDetail.MoneyType == 2))
                    {
                        e.Handled = true;
                        keybd_event(VK_F2, 0, 0, UIntPtr.Zero); // فشار دادن کلید
                        Thread.Sleep(50); // تاخیر برای شبیه‌سازی فشار دادن
                        keybd_event(VK_F2, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // آزاد کردن کلید
                    }
                }
                if (currentCell.ColumnIndex == 10)
                {
                    //Dispatcher.BeginInvoke(new Action(async () =>
                    //{
                    //    await Task.Delay(300);
                    //    datagrid.Focus();
                    //}), DispatcherPriority.Render);
                }
            }
        }

        private void ComboBoxAdv_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBoxAdv = sender as Syncfusion.Windows.Tools.Controls.ComboBoxAdv;
            if (comboBoxAdv.DataContext is PaymentMoneyDetail PaymentMoneyDetail)
            {
                comboBoxAdv.SelectedIndex = PaymentMoneyDetail.MoneyType;
            }
        }

        private void ComboBoxAdv_DropDownOpened(object sender, EventArgs e)
        {
            var comboBoxAdv = sender as Syncfusion.Windows.Tools.Controls.ComboBoxAdv;
            if (comboBoxAdv.DataContext == null && comboBoxAdv.SelectedIndex != -1)
            {
                comboBoxAdv.SelectedIndex = -1;
            }
        }
        private void ComboBoxAdv_GotFocus(object sender, RoutedEventArgs e)
        {
            var comboBoxAdv = sender as Syncfusion.Windows.Tools.Controls.ComboBoxAdv;
            if (comboBoxAdv.DataContext == null)
            {
                comboBoxAdv.SelectedIndex = -1;
            }
        }

        private void txtMoein_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMoein.Text == "")
            {
                txbMoein.Text = string.Empty;
                return;
            }
            var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == txtMoein.Text);
            if (mu == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("چنین کل و معینی وجود ندارد!");
                txtMoein.Text = txbMoein.Text = string.Empty;
            }
            else
            {
                txtMoein.Tag = mu;
                txbMoein.Text = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
            }
        }

        private void txtPreferential_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPreferential.Text == "")
            {
                txbPreferential.Text = string.Empty;
                return;
            }
            var mu = mus2.Find(t => t.Value == txtPreferential.Text);
            if (mu == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("چنین تفضیلی وجود ندارد!");
                txtPreferential.Text = txbPreferential.Text = string.Empty;
            }
            else
            {
                txtPreferential.Tag = mu;
                txbPreferential.Text = mu.Name;
            }
        }

        private void ComboBoxAdv_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var comboBoxAdv = sender as ComboBoxAdv;
            switch (e.Key)
            {
                case Key.NumPad1:
                case Key.D1:
                    comboBoxAdv.SelectedIndex = 0;
                    break;
                case Key.NumPad2:
                case Key.D2:
                    comboBoxAdv.SelectedIndex = 1;
                    break;
                case Key.NumPad3:
                case Key.D3:
                    comboBoxAdv.SelectedIndex = 2;
                    break;
                case Key.NumPad4:
                case Key.D4:
                    comboBoxAdv.SelectedIndex = 3;
                    break;
            }
        }

        private void datagrid_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            tempSelectedIndex = datagrid.SelectedIndex;
        }

        private void MyPopupS_Closed(object sender, EventArgs e)
        {
            if (datePicker1 == null && datagrid.Visibility == Visibility.Visible)
            {
                datagrid.IsHitTestVisible = true;
            }
        }

        private void dataPager_PageIndexChanged(object sender, Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangedEventArgs e)
        {
            if (SearchTermTextBox.Text.Trim() != string.Empty)
                datagridSearch.ExpandAllDetailsView();
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            var win = new winSettingCode() { Width = 460 };
            win.grid.Width = 435;
            using var db = new wpfrazydbContext();
            var exist = false;
            if (db.CodeSettings.Any(t => t.Name == "MoeinCodeCheckPayment"))
            {
                exist = true;
            }
            GroupBox groupBox = SettingDefinitionGroupBox(win, db, exist, "نوع وجه چک", "ColCodeCheckPayment", "MoeinCodeCheckPayment", null);
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                groupBox.GetChildOfType<TextBox>().Focus();
            }), DispatcherPriority.Render);
            win.stack.Children.Add(groupBox);
            var groupBox2 = SettingDefinitionGroupBox(win, db, exist, "نوع وجه نقد", "ColCodeMoneyPayment", "MoeinCodeMoneyPayment", "PreferentialCodeMoneyPayment");
            win.stack.Children.Add(groupBox2);

            groupBox2 = SettingDefinitionGroupBox(win, db, exist, "نوع وجه تخفیف", "ColCodeDiscountPayment", "MoeinCodeDiscountPayment", "PreferentialCodeDiscountPayment");
            win.stack.Children.Add(groupBox2);            

            win.ShowDialog();
        }

        private GroupBox SettingDefinitionGroupBox(winSettingCode win, wpfrazydbContext db, bool exist, string name, string str1, string str2, string str3)
        {
            var groupBox = new GroupBox() { Header = name };
            var stackPanel = new DockPanel();
            groupBox.Content = stackPanel;

            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add(str1, exist ? db.CodeSettings.First(i => i.Name == str1).Value : "");
            keyValuePairs.Add(str2, exist ? db.CodeSettings.First(i => i.Name == str2).Value : "");

            var textInputLayout = new SfTextInputLayout() { Tag = keyValuePairs, Hint =(str1== "ColCodeCheckPayment"? "کد کل و معین اسناد پرداختنی "
                : "کد کل و معین "), Width = 175 };
            if (str1 == "ColCodeCheckPayment")
                textInputLayout.Width = 190;
            var textBox = new TextBox() { Text = exist ? keyValuePairs.ElementAt(0).Value + keyValuePairs.ElementAt(1).Value : "", Tag = true };
            textInputLayout.InputView = textBox;
            if (exist)
            {
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == textBox.Text);
                textInputLayout.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
            }
            textBox.PreviewKeyDown += (s1, e1) =>
            {
                if (e1.Key == Key.F1)
                {
                    win.childWindow = ShowSearchMoein(s1, win);
                }
                else if (e1.Key == Key.Enter)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (s1 as TextBox).MoveFocus(request);
                }
            };
            textBox.LostFocus += (s1, e1) =>
            {
                var txt = s1 as TextBox;
                var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                if (txt.Text == "")
                {
                    sfTextInput.HelperText = string.Empty;
                    return;
                }
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == txt.Text);
                if (mu == null)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("چنین کل و معینی وجود ندارد!");
                    sfTextInput.HelperText = txt.Text = string.Empty;
                }
                else
                {
                    txt.Tag = mu;
                    sfTextInput.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
                    keyValuePairs = sfTextInput.Tag as Dictionary<string, string>;
                    keyValuePairs[keyValuePairs.ElementAt(0).Key] = mu.Value;
                    keyValuePairs[keyValuePairs.ElementAt(1).Key] = (mu.AdditionalEntity as AccountSearchClass).Moein;
                }
            };
            stackPanel.Children.Add(textInputLayout);
            if (str3 != null)
            {
                textInputLayout = new SfTextInputLayout() { Tag = str3, Hint = "کد تفضیل", Margin = new Thickness(10, 0, 10, 0) };
                textBox = new TextBox() { Text = exist ? db.CodeSettings.First(i => i.Name == str3).Value : "", Tag = true };
                textInputLayout.InputView = textBox;
                if (exist)
                {
                    var mu = mus2.Find(t => t.Value == textBox.Text);
                    textInputLayout.HelperText = mu.Name;
                }
                textBox.PreviewKeyDown += (s1, e1) =>
                {
                    if (e1.Key == Key.F1)
                    {
                        win.childWindow = ShowSearchPreferential(s1, win);
                    }
                    else if (e1.Key == Key.Enter)
                    {
                        TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                        request.Wrapped = true;
                        (s1 as TextBox).MoveFocus(request);
                    }
                };
                textBox.LostFocus += (s1, e1) =>
                {
                    var txt = s1 as TextBox;
                    var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                    if (txt.Text == "")
                    {
                        sfTextInput.HelperText = string.Empty;
                        return;
                    }
                    var mu = mus2.Find(t => t.Value == txt.Text);
                    if (mu == null)
                    {
                        Xceed.Wpf.Toolkit.MessageBox.Show("چنین تفضیلی وجود ندارد!");
                        sfTextInput.HelperText = txt.Text = string.Empty;
                    }
                    else
                    {
                        txt.Tag = mu;
                        sfTextInput.HelperText = mu.Name;
                    }
                };
                stackPanel.Children.Add(textInputLayout);
            }
            else
            {
                stackPanel.HorizontalAlignment= HorizontalAlignment.Left;
            }
            return groupBox;
        }
        bool DataGridFocused = false;
        private void datagrid_GotFocus(object sender, RoutedEventArgs e)
        {
            DataGridFocused = true;
            if (SystemParameters.PrimaryScreenWidth <= 1500 && morefields.Visibility == Visibility.Collapsed)
            {
                column1.Width = new GridLength(50);
                column2.Width = new GridLength(0);
                morefields.Visibility = Visibility.Visible;
            }
        }

        private void datagrid_LostFocus(object sender, RoutedEventArgs e)
        {
            DataGridFocused = false;
        }

        private void btnMorefields_Click(object sender, RoutedEventArgs e)
        {
            morefields.Visibility = Visibility.Collapsed;
            column1.Width = new GridLength(170);
            column2.Width = new GridLength(170);
        }

        private void persianCalendarE_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!rl2)
                e.Handled = true;
            rl2 = false;
        }

        private void persianCalendar_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!rl1)
                e.Handled = true;
            rl1 = false;
        }

        private void persianCalendar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rl1 = true;
            RightClick();
        }
    }
}
