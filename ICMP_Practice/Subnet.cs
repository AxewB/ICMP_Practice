using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ICMP_Practice
{
    public class Subnet : Device
    {
        public List<Device> devices;

        public Subnet(string ip) : base(ip)
        {
            this.ip = ip;
            devices = new List<Device>();
            ipAddress = new IPEndPoint(IPAddress.Parse(ip), 0);
        }

        public new void StartPing()
        {
            foreach (Device device in devices)
            {
                device.StartPing();
            }
        }
    }
}
