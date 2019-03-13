using Microsoft.Win32;
using Newtonsoft.Json;
using Support_CounterService_WPF.Helpers;
using Support_CounterService_WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UploadApp;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;

namespace Support_CounterService_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<string> listAllFileName = new List<string>();
        ClsSetting setting = new ClsSetting();
        Services Services = new Services();
        List<FileInfo> ListAllFileInfo = new List<FileInfo>();
        List<string> ListDECSNOUnion = new List<string>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckDEBUG();

            //Get Json to Class
            setting = Services.Setting;
        }
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenSettingDialog();
        }

        void OpenSettingDialog()
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void BtnRefreshSetting_Selected(object sender, RoutedEventArgs e)
        {
            //Get Json to Class
            setting = Services.Setting;
            MessageBox.Show("Refresh Done!.");
        }

        private void BtnArchivePDF_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(setting.PathArchivePDF))
            {//ถ้ามีโฟเดอร์ PathArchivePDF
                if (Check_DefaultPDF.IsChecked.Value == true)
                {//ถ้าเลือก Default Get Path
                    if (!Directory.Exists(setting.PathGetPDF))
                    {//ถ้าไม่มี Path Get PDF จะเปิด FolderBrowser Dialog ขึ้นมาเพื่อเก็บ path
                        System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                        folderBrowserDialog.Description = "Choose folder GetPDF, It'll save path.";
                        if (folderBrowserDialog.ShowDialog().ToString() == "OK")
                        {
                            setting.PathGetPDF = folderBrowserDialog.SelectedPath;
                            Services.Setting = setting;
                        }
                        else return;
                    }
                    //List files All.
                    ListAllFileInfo = Services.GetFilesInfo(setting);
                    //List file passed filter.
                    var ListFilesInfo = Services.GetFilterFilesInfo(setting);

                    var ListError = ListAllFileInfo.Select(a => a.Name).Except(ListFilesInfo.Select(a => a.Name)).ToList();
                    //
                    //ส่วน โปรเซส ที่จะเหลือแต่ไฟล์ที่ Matching ไม่เจอ Decsno
                    //Filenames.Exact(oim_new.CT_DEC.Where(a=>Filenames.Contains(a.DECSNO)).Select(a=>a.DECSNO).ToList());
                    //listFileName.Name.Exact(oim_new.CT_DEC.Where(a=>listFileName.Name.Contains(a.DECSNO)).Select(a=>a.DECSNO).ToList());
                    //
                    using (oim_newEntities oim_New = new oim_newEntities())
                    {

                    }

                    //แสดง list ชื่อไฟล์ที่ดึงมาแล้วผ่าน filter
                    ShowList(ListError);
                }
                else if (Check_DefaultPDF.IsChecked.Value == false)
                {//ถ้า ไม่เลือก Default Get path จะเปิด Opendialog
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Multiselect = true,
                        Filter = "Portable Document Format (*.pdf)|*.pdf",
                        FilterIndex = 1,
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };
                    if (openFileDialog.ShowDialog().Value == true)
                    {
                        //Full Path
                        listAllFileName = openFileDialog.FileNames.ToList();
                        //All FileNames
                        listAllFileName = Services.GetAllFiles(listAllFileName);

                        //อ่านไฟล์ PDF ทั้งหมดแล้ว filter แยกออกมาจากตัวที่ไม่ใช่เลขที่ใบขน
                        var listFileName = Services.GetFilterFiles(listAllFileName, setting);

                        var ListErrorFileName = listAllFileName.Except(listFileName).ToList();
                        //Display Only Error file Can't Get
                        ShowList(ListErrorFileName);
                    }
                }
                else return;
            }
            else
            {
                OpenSettingDialog();
            }

            void ShowList(List<string> list)
            {
                if (list.Count > 0)
                {
                    ListView_ListFileName.ItemsSource = list;
                    ListView_ListFileName.Visibility = Header_PDFFiles.Visibility = Visibility.Visible;
                    ListView_ListFileName.Height = 150;
                }
                else
                {
                    ListView_ListFileName.Visibility = Header_PDFFiles.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnDoc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckDEBUG()
        {
#if DEBUG
            //MessageBox.Show("Debug Mode Start");
#else
            UploadAppClass app = new UploadAppClass();
            app.PathTarget = @"\\172.19.102.43\data_i\Support_Counter_C#2017";
            app.ApplicationExecutedFile = System.IO.Path.GetFileName(Application.ExecutablePath);

            if (app.UploadApp_Version())
            {
                MessageBox.Show("Upload Successful.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
#endif
        }

        private void DateChange()
        {
            if (StartDate.SelectedDate == null || EndDate.SelectedDate == null)
            {
                return;
            }
            DateTime Enddate = EndDate.SelectedDate.Value.AddDays(1);

            using (oim_newEntities oim_New = new oim_newEntities())
            using (EXPORTEntities eXPORT = new EXPORTEntities())
            using (EXPORT_AIREntities eXPORT_AIR = new EXPORT_AIREntities())
            using (EXPORT_XBORDEREntities eXPORT_XBORDER = new EXPORT_XBORDEREntities())
            {
                var Listoim_newDECSNO = oim_New.CT_DEC.Where(a => a.SIGN_BY == "S" && a.DEC_CANCEL != "A" && a.DATE_T >= StartDate.SelectedDate && a.DATE_T < Enddate).Select(a => a.DECSNO).ToList();
                var ListExportDECSNO = eXPORT.HDECs.Where(a => a.SIGN_DEC_BY == "S" && a.CANCEL_S != "A" && a.SEND_DATE >= StartDate.SelectedDate && a.SEND_DATE < Enddate).Select(a => a.DECLARATION_NO).ToList();
                var ListExportAirDECSNO = eXPORT_AIR.HDECs.Where(a => a.SIGN_DEC_BY == "S" && a.CANCEL_S != "A" && a.SEND_DATE >= StartDate.SelectedDate && a.SEND_DATE < Enddate).Select(a => a.DECLARATION_NO).ToList();
                var ListExportXBorderDECSNO = eXPORT_XBORDER.HDECs.Where(a => a.SIGN_DEC_BY == "S" && a.CANCEL_S != "A" && a.SEND_DATE >= StartDate.SelectedDate && a.SEND_DATE < Enddate).Select(a => a.DECLARATION_NO).ToList();

                //ListDECSNOUnion = Listoim_newDECSNO.Union(ListExportDECSNO).Union(ListExportAirDECSNO).Union(ListExportXBorderDECSNO).ToList();
                //GridMain.ItemsSource = ListDECSNOUnion;
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            DateChange();
        }
    }
}
