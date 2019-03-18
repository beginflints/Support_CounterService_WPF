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

        public List<FileInfo> GetFilesInfoPassedFilter(ClsSetting setting)
        {//ทุกไฟล์ของ PDF ที่ผ่าน Filter

            //var listFileInfo = new DirectoryInfo(setting.PathGetPDF).GetFiles("*.pdf").ToList();
            var listFileInfo = GetFiles_ThroughFolder(setting.PathGetPDF);
            return listFileInfo.Where(a => setting.ImportPrefix.Contains(a.Name.Substring(4, 1)) ||
                                           setting.ExportPrefix.Contains(a.Name.Substring(4, 1)))
                                           .ToList();
        }

        public List<FileInfo> GetFilesInfo(ClsSetting setting)
        {//ทุกไฟล์ใน Folder and SubFolder ที่เป็นไฟล์ PDF แต่ยังไม่ผ่าน Filters
            return GetFiles_ThroughFolder(setting.PathGetPDF);
        }

        public List<string> GetFiles_PassedFilter(List<string> ListFilenames, ClsSetting setting)
        {//เอา ลิสต์ชื่อไฟล์เข้ามา แล้วกรองเอาแต่ที่ตรงกับ Prefix ที่ต้องการค้นหาเท่านั้น
            ListFilenames = ListFilenames.Select(a => Path.GetFileName(a)).ToList();
            return ListFilenames.Where(a => setting.ImportPrefix.Contains(a.Substring(4, 1)) ||
                                            setting.ExportPrefix.Contains(a.Substring(4, 1)))
                                            .ToList();
        }
        
        public List<string> GetFullNamesConvertToFilenames(List<string> ListFilenames)
        {//Input List<FullName> => Output List<Filename>
            return ListFilenames.Select(a => Path.GetFileName(a)).ToList();
        }
        public string GetFullNameConvertToFileName(string FullFileName)
        {
            return Path.GetFileName(FullFileName);
        }

        public List<FileInfo> GetFiles_ThroughFolder(string path)
        {//Get Files ทั้งหมดที่อยู่ใน Folder and Sub-Folder
            return new DirectoryInfo(path).GetFiles("*.pdf", SearchOption.AllDirectories).ToList();
        }

        public List<FileInfo> GetFiles_OnlyTopFolder(string path)
        {//Get Only File on Top Folder
            return new DirectoryInfo(path).GetFiles("*.pdf", SearchOption.TopDirectoryOnly).ToList();
        }

        public class FileStructure
        {
            public string Filename { get; set; }
            public string FullFileName { get; set; }
        }
    }


}
