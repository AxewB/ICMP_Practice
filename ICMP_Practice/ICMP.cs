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
    /// Класс пакета ICMP
    /// </summary>
    public class ICMP
    {
        public byte type;
        public byte code;
        public UInt16 checksum;
        public UInt16 sequnceId;
        public UInt16 sequnceNumber;
        public byte[] message = new byte[1024];
        public int messageSize;

        public ICMP()
        {

        }

        // Конструктор, создающий icmp пакет из  IP пакета
        // ICMP пакеты инкапсулируются и передаются в IP пакетах
        public ICMP(byte[] ipDiagramm, int ipDiagrammLength)
        {
            // В начале ipDiagramm идет заголовок IP, потому начинаем с 21 байта
            type = ipDiagramm[20];
            code = ipDiagramm[21];
            checksum = BitConverter.ToUInt16(ipDiagramm, 22);
            sequnceId = BitConverter.ToUInt16(ipDiagramm, 24);
            sequnceNumber = BitConverter.ToUInt16(ipDiagramm, 26);
            messageSize = ipDiagrammLength - 28;
            Buffer.BlockCopy(ipDiagramm, 28, message, 0, messageSize);
        }

        // Получить ICMP сообщение в байтах
        public byte[] getBytes()
        {
            byte[] data = new byte[messageSize + 13];
            Buffer.BlockCopy(BitConverter.GetBytes(type), 0, data, 0, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(code), 0, data, 1, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(checksum), 0, data, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(sequnceId), 0, data, 4, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(sequnceNumber), 0, data, 6, 2);
            Buffer.BlockCopy(message, 0, data, 8, messageSize);
            return data;
        }

        /*
        1. Разбиваем заголовок на слова по 16 бит, принимаем значение поля контрольной суммы равным нулю 
        и суммируем полученные 16-битные слова между собой
        2. Если результат сложения превышает по длине 16 бит, разделяем его на на два 16-битных слова, которые складываются между собой
        3. Находим контрольную сумму, как двоичное поразрядное дополнение результата сложения
        */
        public void getChecksum()
        {
            UInt32 checksum = 0;
            byte[] data = getBytes();
            int packetsize = messageSize + 8;
            int index = 0;

            while (index < packetsize)
            {
                checksum += Convert.ToUInt32(BitConverter.ToUInt16(data, index));
                index += 2;
            }
            while (checksum >> 16 != 0)
            {
                checksum = (checksum & 0xffff) + (checksum >> 16);
            }
            this.checksum = (UInt16)(~checksum);
        }
    }
}
