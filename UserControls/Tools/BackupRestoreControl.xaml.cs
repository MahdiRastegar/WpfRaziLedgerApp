using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfRaziLedgerApp.Interfaces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for BackupRestoreControl.xaml
    /// </summary>
    public partial class BackupRestoreControl : UserControl, ITabForm
    {
        private string connectionString;
        //private string connectionString = "Server=.;Database=master;Integrated Security=True;";

        public BackupRestoreControl()
        {
            InitializeComponent();
            var path = System.IO.Path.Combine(Directory.GetDirectoryRoot(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory)), "wpfrazydbBackup");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            connectionString = System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cs.txt"));
        }

        // انتخاب مسیر فولدر بکاپ
        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            try
            {
                dialog.InitialDirectory = System.IO.Path.Combine(Directory.GetDirectoryRoot(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory)), "wpfrazydbBackup");
            }
            catch { }
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBackupFolder.Text = dialog.SelectedPath;
            }
        }
        string path2 = "";
        // دکمه پشتیبان‌گیری
        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dbName = "wpfrazydb";
                string folderPath = txtBackupFolder.Text.Trim();

                if (string.IsNullOrEmpty(dbName))
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("لطفا نام دیتابیس را وارد کنید.");
                    return;
                }
                if (string.IsNullOrEmpty(folderPath))
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("لطفا مسیر پوشه بکاپ را انتخاب کنید.");
                    return;
                }

                string fileName = $"{dbName}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.bak";
                string filePath = System.IO.Path.Combine(folderPath, fileName);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = $"BACKUP DATABASE [{dbName}] TO DISK='{filePath}'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                }

                Xceed.Wpf.Toolkit.MessageBox.Show("پشتیبان‌گیری با موفقیت انجام شد.");
                path2 = folderPath;
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("خطا: " + ex.Message);
            }
        }

        // انتخاب فایل بکاپ برای بازیابی
        private void btnBrowseRestore_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Backup Files (*.bak)|*.bak";
            if (path2 != "")
                openFile.InitialDirectory = path2;
            else
                openFile.InitialDirectory = System.IO.Path.Combine(Directory.GetDirectoryRoot(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory)), "wpfrazydbBackup");
            path2 = "";
            if (openFile.ShowDialog() == true)
            {
                txtRestoreFile.Text = openFile.FileName;
            }
        }

        // دکمه بازیابی
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            string restoreFile = txtRestoreFile.Text.Trim();

            if (string.IsNullOrEmpty(restoreFile))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("لطفا فایل بکاپ را انتخاب کنید.");
                return;
            }
            MainWindow.Current.Effect = new BlurEffect() { Radius = 5 };
            if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید عملیات بازیابی صورت گیرد و از برنامه خارج شوید، دوباره وارد شوید؟", "بازیابی اطلاعات", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Process process = new Process();
                process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = restoreFile;
                process.Start();
                Environment.Exit(0);
            }
            MainWindow.Current.Effect = null;
        }

        public bool CloseForm()
        {
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پشتیبان");
            MainWindow.Current.tabcontrol.Items.Remove(item);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Dispose();
            }));
            return true;
        }

        private void Dispose()
        {
            if (DataContext == null)
                return;
            DataContext = null;
            GC.Collect();
        }

        public void SetNull()
        {
            throw new NotImplementedException();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtBackupFolder.Text = System.IO.Path.Combine(Directory.GetDirectoryRoot(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory)), "wpfrazydbBackup");
            }
            catch { }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}
