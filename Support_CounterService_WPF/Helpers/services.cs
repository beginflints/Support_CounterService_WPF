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

        public List<FileInfo> GetFilterFilesInfo(ClsSetting setting)
        {//ทุกไฟล์ของ PDF ที่ผ่าน Filter
            var listFileInfo = new DirectoryInfo(setting.PathGetPDF).GetFiles("*.pdf").ToList();
            return listFileInfo.Where(a => setting.ImportPrefix.Contains(a.Name.Substring(4, 1)) ||
                                           setting.ExportPrefix.Contains(a.Name.Substring(4, 1)))
                                           .ToList();
        }

        public List<FileInfo> GetFilesInfo(ClsSetting setting)
        {//ทุกไฟล์ที่เป็นไฟล์ PDF แต่ยังไม่ผ่าน Filters
            return new DirectoryInfo(setting.PathGetPDF).GetFiles("*.pdf").ToList();
        }

        public List<string> GetFilterFiles(List<string> ListFilenames, ClsSetting setting)
        {
            ListFilenames = ListFilenames.Select(a => Path.GetFileName(a)).ToList();
            return ListFilenames.Where(a => setting.ImportPrefix.Contains(a.Substring(4, 1)) ||
                                            setting.ExportPrefix.Contains(a.Substring(4, 1)))
                                .ToList();
        }

        public List<string> GetAllFiles(List<string> ListFilenames)
        {
            return ListFilenames.Select(a => Path.GetFileName(a)).ToList();
        }
    }


}
