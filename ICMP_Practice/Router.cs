using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMP_Practice
{
    public class Router
    {
        public string name { get; set; }
        public List<Subnet> subnets = new List<Subnet>();
    }
}
