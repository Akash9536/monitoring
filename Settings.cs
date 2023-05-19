using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace flx_web
{
    public class Settings
    {
        public List<string> SynthCustomers { get; set; }
        public string SynthDataFile { get; set; }
        public string ReleaseDataFile { get; set; }

        public int SynthTimeBlockInterval { get; set; }
        public int KibanaTimeBlockInterval { get; set; }
        public int KibanaTimeBlockLag { get; set; }

        public string KibanaDataFile { get; set; }
        public string AlertDataFile { get; set; }

        public List<string> KibanaCustomers { get; set; }
        public List<string> KibanaRequests { get; set; }
        public string A3OA_Requests { get; set; }
        public string AA_Requests { get; set; }
        public string CM_Requests { get; set; }
        public string CMD_Requests { get; set; }
        public string EK_Requests { get; set; }
        public string EKD_Requests { get; set; }
        public string HA_Requests { get; set; }
        public string LHG_Requests { get; set; }
        public string QF_Requests { get; set; }
        public string UAD_Requests { get; set; }
        public string WS_Requests { get; set; }

        public string APIBaseUrl { get; set; }
    }
}
