using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Essencore
{
    public class BarcodeDetails
    {
        //  public int barcodeid { get; set; }
         public string SyrmaSGSPartno { get; set; }
         public string WorkOrderNo { get; set; }
          public string CustomerPartNo { get; set; }
         public string Bar_Description { get; set; }
        public string ProductNo { get; set; }


        public string Country { get; set; }
        public string DDR { get; set; }
        public string Density { get; set; }
        public string DIMM { get; set; }
        public string DTR { get; set; }
        public string Latency { get; set; }
        public string Model { get; set; }
        public string Rank1 { get; set; }
        public string Voltage { get; set; }

        public string CustomerSerialNo { get; set; }
        public string PCBSerialNo { get; set; }
        //public string DateandTime { get; set; }
        //public string ShiftType { get; set; }
        public int WeekDetails { get; set; }
        //public string Dublicate { get; set; }
        public string capacity { get; set; }

        public string versionno { get; set; }
        public string labeltype { get; set; }

        public string duplicate {  get; set; }

        public string ProviderID { get; set; }
        class UserDetails
        {
            public string emp_id { get; set; }
        }


    }
}
