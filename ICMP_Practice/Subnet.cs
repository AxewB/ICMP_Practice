using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ICMP_Practice
{
    /// <summary>
    /// Класс подсетей. Наследуется от класс устройств
    /// </summary>
    public class Subnet : Device
    {
        // Список возможных устройств подсети
        public List<Device> devices;

        // Конструктор подсети
        public Subnet(string ip) : base(ip)
        {
            this.ip = ip;
            devices = new List<Device>();
            ipAddress = new IPEndPoint(IPAddress.Parse(ip), 0);
        }

        // Фукнция опроса устройств
        public new void StartPing()
        {
            foreach (Device device in devices)
            {
                device.StartPing();
            }
        }
    }
}
