﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMP_Practice
{
    public class IPInfo
    {
        public string ip { get; set; }
        public bool status { get; set; }
        public IPInfo(string ip, bool status)
        {
            this.ip = ip;
            this.status = status;
        }
    }
}
