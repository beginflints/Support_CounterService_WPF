using Microsoft.Win32;
using Newtonsoft.Json;
using Support_CounterService_WPF.Helpers;
using Support_CounterService_WPF.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

        List<Services.ClsFileName> listFileNames = new List<Services.ClsFileName>();
        List<Services.ClsFileName> listFileNames_PassedFilter = new List<Services.ClsFileName>();
        List<Services.ClsFileName> listFileNames_PassedFileterDB = new List<Services.ClsFileName>();
        List<Services.ClsError> listFileNames_Error = new List<Services.ClsError>();

        Services Services = new Services();
        ClsSetting setting = new ClsSetting();

        List<FileInfo> ListAllFile = new List<FileInfo>();


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
                List<Services.ClsFileName> ListErrorFileNames = new List<Services.ClsFileName>();
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
                    //Get List files PDF From Path.
                    listFileNames = Services.GetFileFromPath(setting);
                    //Get List files Passed Filter.
                    listFileNames_PassedFilter = Services.GetFilesPassedFilter(listFileNames, setting);
                    //Get List file Passed Filter and have records in DB
                    listFileNames_PassedFileterDB = GetCounter(listFileNames_PassedFilter);

                    var li = listFileNames_PassedFileterDB.Select(a => a.FilenameWithOutExt).ToList();

                    //Part Error
                    //var listdecsno = listFileNames.Select(a => new Services.ClsError() { Filename = a.Filename, FullFileName = a.FullFileName }).ToList();
                    //var listdecsno_PassFilter = listFileNames_PassedFilter.Select(a => new Services.ClsError() { Filename = a.Filename, FullFileName = a.FullFileName }).ToList();
                    //var listdecsno_PassedFileterDB = listFileNames_PassedFileterDB.Select(a => new Services.ClsError() { Filename = a.Filename, FullFileName = a.FullFileName }).ToList();

                    listFileNames_Error = listFileNames.Where(a => !li.Contains(a.FilenameWithOutExt)).Select(a => new Services.ClsError() { Filename = a.Filename, FullFileName = a.FullFileName }).ToList();

                    //แสดง list ชื่อไฟล์ที่ดึงมาแล้วผ่าน filter
                    ShowList(listFileNames_Error);

                    MoveFileFromList(listFileNames_PassedFileterDB, setting);
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
                        //Get List files PDF From OpenDialog.
                        listFileNames = openFileDialog.FileNames.Select(a => new Services.ClsFileName()
                        {
                            FullFileName = a,
                            Filename = Services.FullFilenameToFilename(a),
                            FilenameWithOutExt = Services.FullFilenameToFilenameWOExt(a)
                        }).ToList();
                        //Get List files Passed Filter.
                        listFileNames_PassedFilter = Services.GetFilesPassedFilter(listFileNames, setting);
                        //Get List files Passed Filter and have record in DB
                        listFileNames_PassedFileterDB = GetCounter(listFileNames_PassedFilter);

                        var li = listFileNames_PassedFileterDB.Select(a => a.FilenameWithOutExt).ToList();
                        //Part Error
                        //var listdecsno = listFileNames.Select(a => new Services.ClsError() { Filename = a.Filename, FullFileName = a.FullFileName }).ToList();
                        //var listdecsno_PassFilter = listFileNames_PassedFilter.Select(a => new Services.ClsError() { Filename = a.Filename, FullFileName = a.FullFileName }).ToList();
                        //var listdecsno_PassedFileterDB = listFileNames_PassedFileterDB.Select(a => new Services.ClsError() { Filename = a.Filename, FullFileName = a.FullFileName }).ToList();

                        listFileNames_Error = listFileNames.Where(a => !li.Contains(a.FilenameWithOutExt)).Select(a => new Services.ClsError() { Filename = a.Filename, FullFileName = a.FullFileName }).ToList();

                        //แสดง list ชื่อไฟล์ที่ดึงมาแล้วผ่าน filter
                        ShowList(listFileNames_Error);

                        MoveFileFromList(listFileNames_PassedFileterDB, setting);
                    }
                }
                else return;
            }
            else
            {
                OpenSettingDialog();
            }

            void ShowList(List<Services.ClsError> list)
            {
                if (list.Count > 0)
                {
                    ListView_ListFileName.ItemsSource = list.ToList();
                    ListView_ListFileName.Visibility = Visibility.Visible;
                    ListView_ListFileName.Height = 150;
                }
                else
                {
                    ListView_ListFileName.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnDoc_Click(object sender, RoutedEventArgs e)
        {
            //Not yet to use
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            if (StartDate.SelectedDate == null || EndDate.SelectedDate == null)
            {
                return;
            }

            //Get List Counter from DB Via use DateTime
            GridMain.ItemsSource = GetCounterViaDate((DateTime)StartDate.SelectedDate, (DateTime)EndDate.SelectedDate);
        }

        private List<Services.ClsFileName> GetCounter(List<Services.ClsFileName> listFileName)
        {//Get มาทั้งหมดเลย จาก DB ทั้ง Cancel หรือไม่ Cancel

            using (oim_newEntities oim_New = new oim_newEntities())
            using (EXPORTEntities eXPORT = new EXPORTEntities())
            using (EXPORT_AIREntities eXPORT_AIR = new EXPORT_AIREntities())
            using (EXPORT_XBORDEREntities eXPORT_XBORDER = new EXPORT_XBORDEREntities())
            {
                var listfilenameWO = listFileName.Select(b => b.FilenameWithOutExt).ToList();

                //Import
                var List_DECSNO = oim_New.CT_DEC.Where(a => a.SIGN_BY == "S" && listfilenameWO.Contains(a.DECSNO))
                                                      .Select(a => new { DECLARATION_NO = a.DECSNO, SEND_DATE = a.DATE_T }).ToList();

                var list_oimnew = listFileName.Where(a => List_DECSNO.Select(b => b.DECLARATION_NO).Contains(a.FilenameWithOutExt)).Select(a => new Services.ClsFileName()
                {
                    FilenameWithOutExt = a.FilenameWithOutExt,
                    Filename = a.Filename,
                    FullFileName = a.FullFileName,
                    TYPE = "Import",
                    SEND_DATE = List_DECSNO.Where(b => b.DECLARATION_NO == a.FilenameWithOutExt).Select(b => b.SEND_DATE).FirstOrDefault()
                }).ToList();

                //Export
                List_DECSNO = eXPORT.HDEC_N.Where(a => a.SIGN_DEC_BY == "S" && listfilenameWO.Contains(a.DECLARATION_NO))
                                                    .Select(a => new { a.DECLARATION_NO, a.SEND_DATE }).ToList();
                var list_Export = listFileName.Where(a => List_DECSNO.Select(b => b.DECLARATION_NO).Contains(a.FilenameWithOutExt)).Select(a => new Services.ClsFileName()
                {
                    FilenameWithOutExt = a.FilenameWithOutExt,
                    Filename = a.Filename,
                    FullFileName = a.FullFileName,
                    TYPE = "Export",
                    SEND_DATE = List_DECSNO.Where(b => b.DECLARATION_NO == a.FilenameWithOutExt).Select(b => b.SEND_DATE).FirstOrDefault()
                }).ToList();

                //Export Air
                List_DECSNO = eXPORT_AIR.HDEC_A.Where(a => a.SIGN_DEC_BY == "S" && listfilenameWO.Contains(a.DECLARATION_NO))
                                                          .Select(a => new { a.DECLARATION_NO, a.SEND_DATE }).ToList();
                var list_ExportAir = listFileName.Where(a => List_DECSNO.Select(b => b.DECLARATION_NO).Contains(a.FilenameWithOutExt)).Select(a => new Services.ClsFileName()
                {
                    FilenameWithOutExt = a.FilenameWithOutExt,
                    Filename = a.Filename,
                    FullFileName = a.FullFileName,
                    TYPE = "ExportAIR",
                    SEND_DATE = List_DECSNO.Where(b => b.DECLARATION_NO == a.FilenameWithOutExt).Select(b => b.SEND_DATE).FirstOrDefault()
                }).ToList();

                //Export XBorder
                List_DECSNO = eXPORT_XBORDER.HDEC_X.Where(a => a.SIGN_DEC_BY == "S" && listfilenameWO.Contains(a.DECLARATION_NO))
                                                                   .Select(a => new { a.DECLARATION_NO, a.SEND_DATE }).ToList();
                var list_ExportXBorder = listFileName.Where(a => List_DECSNO.Select(b => b.DECLARATION_NO).Contains(a.FilenameWithOutExt)).Select(a => new Services.ClsFileName()
                {
                    FilenameWithOutExt = a.FilenameWithOutExt,
                    Filename = a.Filename,
                    FullFileName = a.FullFileName,
                    TYPE = "ExportXBorder",
                    SEND_DATE = List_DECSNO.Where(b => b.DECLARATION_NO == a.FilenameWithOutExt).Select(b => b.SEND_DATE).FirstOrDefault()
                }).ToList();

                return list_oimnew.Union(list_Export).Union(list_ExportAir).Union(list_ExportXBorder).ToList();
            }
        }

        private List<Services.CounterFile> GetCounterViaDate(DateTime StartDate, DateTime EndDate)
        {//Get มาทั้งหมดเลย จาก DB ทั้ง Cancel หรือไม่ Cancel
            EndDate = EndDate.AddDays(1);
            using (oim_newEntities oim_New = new oim_newEntities())
            using (EXPORTEntities eXPORT = new EXPORTEntities())
            using (EXPORT_AIREntities eXPORT_AIR = new EXPORT_AIREntities())
            using (EXPORT_XBORDEREntities eXPORT_XBORDER = new EXPORT_XBORDEREntities())
            {
                //Import
                var Listoim_newDECSNO = oim_New.CT_DEC.Where(a => a.SIGN_BY == "S" && (a.DATE_T >= StartDate && a.DATE_T < EndDate.Date))
                                                      .Select(a => new Services.CounterFile
                                                      {
                                                          DECLARATION_NO = a.DECSNO,
                                                          SEND_DATE = a.DATE_T,
                                                          TYPE = "Import",
                                                          CANCEL_S = a.DEC_CANCEL.Trim()
                                                      }).ToList();

                //Export
                var ListExportDECSNO = eXPORT.HDEC_N.Where(a => a.SIGN_DEC_BY == "S" && (a.SEND_DATE >= StartDate && a.SEND_DATE < EndDate.Date))
                                                    .Select(a => new Services.CounterFile
                                                    {
                                                        DECLARATION_NO = a.DECLARATION_NO,
                                                        SEND_DATE = a.SEND_DATE,
                                                        TYPE = "Export",
                                                        CANCEL_S = a.CANCEL_S.Trim()
                                                    }).ToList();

                //Export Air
                var ListExportAirDECSNO = eXPORT_AIR.HDEC_A.Where(a => a.SIGN_DEC_BY == "S" && (a.SEND_DATE >= StartDate && a.SEND_DATE < EndDate.Date))
                                                           .Select(a => new Services.CounterFile
                                                           {
                                                               DECLARATION_NO = a.DECLARATION_NO,
                                                               SEND_DATE = a.SEND_DATE,
                                                               TYPE = "ExportAir",
                                                               CANCEL_S = a.CANCEL_S.Trim()
                                                           }).ToList();

                //Export XBorder
                var ListExportXBorderDECSNO = eXPORT_XBORDER.HDEC_X.Where(a => a.SIGN_DEC_BY == "S" && (a.SEND_DATE >= StartDate && a.SEND_DATE < EndDate.Date))
                                                                   .Select(a => new Services.CounterFile
                                                                   {
                                                                       DECLARATION_NO = a.DECLARATION_NO,
                                                                       SEND_DATE = a.SEND_DATE,
                                                                       TYPE = "ExportXBorder",
                                                                       CANCEL_S = a.CANCEL_S.Trim()
                                                                   }).ToList();

                return Listoim_newDECSNO.Union(ListExportDECSNO).Union(ListExportAirDECSNO).Union(ListExportXBorderDECSNO).ToList();
            }
        }

        private void MoveFileFromList(List<Services.ClsFileName> ListFiles, ClsSetting setting)
        {
            var th = new CultureInfo("th-TH");
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("th-TH");
            var YearMonth = ListFiles.Select(a => new { Year = a.SEND_DATE.Value.ToString("yyyy", th), Month = a.SEND_DATE.Value.ToString("MMMM", th) })
                                     .Distinct().ToList();
            //Create Folders
            YearMonth.ForEach(a => { Directory.CreateDirectory(Path.Combine(setting.PathArchivePDF, a.Year, a.Month)); });
            //Move Files
            ListFiles.ForEach(a =>
            {
                var Des = Path.Combine(setting.PathArchivePDF, a.SEND_DATE.Value.ToString("yyyy", th), a.SEND_DATE.Value.ToString("MMMM", th));
                if (File.Exists(Des))
                {
                    File.Copy(a.FullFileName, Des, true);
                    File.Delete(a.FullFileName);
                }
            });
            MessageBox.Show("Move File Done!");
        }

        void CountFilePathGetPDF()
        {
            //if (Directory.Exists())
            //{

            //}
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
    }
}
