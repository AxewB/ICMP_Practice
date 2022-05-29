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
        
        public EndPoint ipAddress;  // Класс IP адреса
        public string ip;           // Строка с IP адресом
        public bool pinged;         // Переменная, отвечающая на наличие устройства

        private HelperICMP helper;

        // Конструктор класса 
        public Device(string ip)
        {
            this.ip = ip;
            ipAddress = new IPEndPoint(IPAddress.Parse(ip), 0);
            helper = new HelperICMP(ipAddress);
        }

        // Функция опроса устройства
        public bool StartPing()
        {
            for (int i = 0; i < 5; i++)
            {
                pinged = helper.StartPing(ip);
                if (pinged) break;
            }

            return pinged;
        }

        // Получение первых трех октетов для сравнения с подсетями
        // для дальнейшего их распределения между подсетями
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
