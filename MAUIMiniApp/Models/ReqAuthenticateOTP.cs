﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIMiniApp.Models
{
    public class ReqAuthenticateOTP
    {
        public int SysID { get; set; }
        public string Username { get; set; }
        public string OTP { get; set; }
    }
}