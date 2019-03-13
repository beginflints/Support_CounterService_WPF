using Newtonsoft.Json;
using Support_CounterService_WPF.Helpers;
using Support_CounterService_WPF.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Support_CounterService_WPF
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            CultureInfo cd = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            cd.DateTimeFormat.ShortDatePattern = "dd/MMMM/yyyy";

            Thread.CurrentThread.CurrentCulture = cd;
        }

        ClsSetting setting = new ClsSetting();
        Services services = new Services();

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            //Get Json to Class
            setting = services.Setting;

            TxtPathArchivePDF.Text = setting.PathArchivePDF;
            TxtPathGetPDF.Text = setting.PathGetPDF;
            TxtPathMonthlyReport.Text = setting.PathMonthlyReport;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            setting.PathArchivePDF = TxtPathArchivePDF.Text;
            setting.PathGetPDF = TxtPathGetPDF.Text;
            setting.PathMonthlyReport = TxtPathMonthlyReport.Text;
            setting.ReleaseDate = DateTime.Now;

            //Set Class to Json
            services.Setting = setting;
        }

        private void Opend_MonthlyReport_Click(object sender, RoutedEventArgs e)
        {
            OpenDialog(sender);
        }

        private void Opend_GETPDF_Click(object sender, RoutedEventArgs e)
        {
            OpenDialog(sender);
        }

        private void Opend_ArchivePDF_Click(object sender, RoutedEventArgs e)
        {
            OpenDialog(sender);
        }

        void OpenDialog(object sender)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog().ToString() == "OK")
            {
                if (sender == Opend_ArchivePDF)
                {
                    TxtPathArchivePDF.Text = folderBrowserDialog.SelectedPath;
                }
                else if (sender == Opend_GETPDF)
                {
                    TxtPathGetPDF.Text = folderBrowserDialog.SelectedPath;
                }
                else
                {
                    TxtPathMonthlyReport.Text = folderBrowserDialog.SelectedPath;
                }
            }

        }

        private void Create_MonthlyFolder_Click(object sender, RoutedEventArgs e)
        {
            CreateFolder(sender);
        }

        private void Create_GETPDF_Click(object sender, RoutedEventArgs e)
        {
            CreateFolder(sender);
        }

        private void Create_ArchivePDF_Click(object sender, RoutedEventArgs e)
        {
            CreateFolder(sender);
        }

        void CreateFolder(object sender)
        {
            if (sender == Create_ArchivePDF)
            {
                Directory.CreateDirectory(TxtPathArchivePDF.Text);
                MessageBox.Show("Create Directory successful.");
            }
            else if (sender == Create_GETPDF)
            {
                Directory.CreateDirectory(TxtPathGetPDF.Text);
                MessageBox.Show("Create Directory successful.");
            }
            else
            {
                Directory.CreateDirectory(TxtPathMonthlyReport.Text);
                MessageBox.Show("Create Directory successful.");
            }
        }
    }
}
