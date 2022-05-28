using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ICMP_Practice
{
    public class Device
    {
        public EndPoint ipAddress;
        public string ip;
        public bool pinged;

        private HelperICMP helper;


        public Device(string ip)
        {
            this.ip = ip;
            ipAddress = new IPEndPoint(IPAddress.Parse(ip), 0);
            helper = new HelperICMP(ipAddress);
        }

        public bool StartPing()
        {
            for (int i = 0; i < 5; i++)
            {
                pinged = helper.StartPing(ip);
                if (pinged) break;
            }

            return pinged;
        }


        public int[] getStartOctets()
        {
            int[] octets = new int[3];
            string[] octetsStr = ip.Split('.');
            for (int i = 0; i < 3; i++)
            {
                octets[i] = Convert.ToInt32(octetsStr[i]);
            }
            return octets;
        }
    }
}
