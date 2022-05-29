using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMP_Practice
{
    /// <summary>
    /// Класс роутеров
    /// </summary>
    public class Router
    {
        public string name { get; set; }
        public List<Subnet> subnets = new List<Subnet>();
    }
}
