using Newtonsoft.Json;
using Support_CounterService_WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support_CounterService_WPF.Helpers
{
    class Services
    {
        /// <summary>
        /// Get to Class ClsSetting
        /// Set Retrieve only Class ClsSetting
        /// </summary>
        public ClsSetting Setting
        {
            get
            {
                using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), @"MainSettings.json")))
                {
                    string Json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<ClsSetting>(Json);
                }
            }
            set
            {
                string json = JsonConvert.SerializeObject(value);
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), @"MainSettings.json"), json);
            }
        }

        private List<ClsFileName> GetFiles_ThroughFolder(ClsSetting setting)
        {//Get Files ทั้งหมดที่อยู่ใน Folder and Sub-Folder
            List<ClsFileName> ListfileStructure = new List<ClsFileName>();
            new DirectoryInfo(setting.PathGetPDF).GetFiles("*.pdf", SearchOption.AllDirectories).ToList()
                                   .ForEach(a => { ListfileStructure.Add(new ClsFileName() { Filename = a.Name, FullFileName = a.FullName, FilenameWithOutExt = Path.GetFileNameWithoutExtension(a.Name) }); });
            return ListfileStructure;
        }

        public List<ClsFileName> GetFileFromPath(ClsSetting setting)
        {//ทุกไฟล์ใน Folder and SubFolder ที่เป็นไฟล์ PDF แต่ยังไม่ผ่าน Filters
            return GetFiles_ThroughFolder(setting);
        }
        
        public List<ClsFileName> GetFilesPassedFilter(List<ClsFileName> ListFilenames, ClsSetting setting)
        {//ทุกไฟล์ของ PDF ที่ผ่าน Filter
            ListFilenames = ListFilenames.Where(a => setting.ImportPrefix.Contains(a.FilenameWithOutExt.Substring(4, 1)) || setting.ExportPrefix.Contains(a.FilenameWithOutExt.Substring(4, 1))).ToList();
            return ListFilenames;
        }

        public string FullFilenameToFilename(string FullFilename)
        {
            return Path.GetFileName(FullFilename);
        }
        public string FullFilenameToFilenameWOExt(string FullFilename)
        {
            return Path.GetFileNameWithoutExtension(FullFilename);
        }

        public class ClsFileName
        {
            public string Filename { get; set; }
            public string FullFileName { get; set; }
            public string FilenameWithOutExt { get; set; }
            public string TYPE { get; set; }
            public DateTime? SEND_DATE { get; set; }
        }


        public class ClsError
        {
            public string Filename { get; set; }
            public string FullFileName { get; set; }
        }

        public class CounterFile
        {
            public string DECLARATION_NO { get; set; }
            public DateTime? SEND_DATE { get; set; }
            public string TYPE { get; set; }
            public string CANCEL_S { get; set; }
        }
    }


}
