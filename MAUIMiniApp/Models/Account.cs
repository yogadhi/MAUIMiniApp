using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIMiniApp.Models
{
    public class Account
    {
        public string CompanyCode { get; set; }
        [PrimaryKey]
        public string Accode { get; set; }
        public string SecretKey { get; set; }
    }
}
