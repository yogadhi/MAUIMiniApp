using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIMiniApp.Models
{
    public class ResGetQRCode
    {
        public Data data { get; set; }
        public int MsgCode { get; set; }
        public string MsgDescription { get; set; }
        public string Datetime { get; set; }
        public string TransactionLogID { get; set; }
    }

    public class Data
    {
        public string Based64QRImg { get; set; }
        public string EncodedSecretKey { get; set; }
        public string SecretKey { get; set; }
    }
}
