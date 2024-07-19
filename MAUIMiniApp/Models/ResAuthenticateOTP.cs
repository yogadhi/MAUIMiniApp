using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIMiniApp.Models
{
    public class ResAuthenticateOTP
    {
        public bool data { get; set; }
        public int MsgCode { get; set; }
        public string MsgDescription { get; set; }
        public string Datetime { get; set; }
        public string TransactionLogID { get; set; }
    }
}
