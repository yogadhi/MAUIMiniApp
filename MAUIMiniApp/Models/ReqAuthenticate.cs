using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIMiniApp.Models
{
    public class Authenticate
    {
        public string UserLogin { get; set; }
        public string Password { get; set; }
        public string CompanyCode { get; set; }
        public string Accode { get; set; }
    }

    public class ReqAuthenticate : Authenticate
    {
        public string AccessRight { get; set; } = "1";
    }
}
