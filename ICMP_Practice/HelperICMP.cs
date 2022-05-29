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
    /// Класс для формирования пакетов ICMP
    /// </summary>
    public class HelperICMP
    {
        EndPoint ep;

        public HelperICMP(EndPoint endPoint)
        {
            ep = endPoint;
        }

        public bool StartPing(string address)
        {
            Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            //Устанавливаем тайм-аут для функции ReceiveFrom()
            host.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 200);

            //Создаем icmp пакет и задаем ему нужные параметры
            ICMP packet = new ICMP();
            packet.type = 0x08; //эхо-запрос
            packet.code = 0x00;
            packet.checksum = 0;
            packet.sequnceId = 1;
            packet.sequnceNumber = 1;

            //Создаем передаваемые данные
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes("Test packet");

            //Записываем их в ICMP пакет
            Buffer.BlockCopy(data, 0, packet.message, 0, data.Length);
            packet.messageSize = data.Length;

            packet.getChecksum();

            //Адрес получателя
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(address), 0);
            EndPoint dumpEP = (EndPoint)remoteEP;

            //Отправляем наше сообщение удаленному хосту
            host.SendTo(packet.getBytes(), packet.messageSize + 8, SocketFlags.None, remoteEP);

            try
            {
                //Буфер
                data = new byte[1024];

                //Получаем ответный IP пакет
                int ipDiagrammLength = host.ReceiveFrom(data, ref dumpEP);

                //Из IP пакета извлекаем пакет ICMP
                ICMP response = new ICMP(data, ipDiagrammLength);

                //Проверка кода полученного сообщения, если это эхо-ответ, устройство есть в сети
                if (response.type == 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
    }
}
