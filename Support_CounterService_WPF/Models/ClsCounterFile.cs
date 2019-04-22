using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support_CounterService_WPF.Models
{
    class ClsCounterFile
    {
        public string REFNO { get; set; }
        public string REFNOXML { get; set; }
        public string DECLARATION_NO { get; set; }
        public DateTime? SEND_DATE { get; set; }
        public string TYPE { get; set; }
        public string CANCEL_S { get; set; }
        public bool IsHavePDF { get; set; }
        public string Username { get; set; }
    }
}
