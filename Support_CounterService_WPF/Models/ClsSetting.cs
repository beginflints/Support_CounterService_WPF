using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support_CounterService_WPF.Models
{
    class ClsSetting
    {
        public string PathArchivePDF { get; set; }
        public string PathGetPDF { get; set; }
        public string PathMonthlyReport { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string[] ImportPrefix { get; set; }
        public string[] ExportPrefix { get; set; }
    }
}
