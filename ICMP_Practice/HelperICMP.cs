using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ICMP_Practice
{
    public class HelperICMP
    {
        EndPoint ep;


        public HelperICMP(EndPoint endPoint)
        {
            ep = endPoint;
        }

        public bool StartPing(string address)
        {
            byte[] data = new byte[1024];
            int recv = 0;
            Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
            ICMP packet = new ICMP();
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(address), 0);
            EndPoint ep = (EndPoint)iep;
            packet.Type = 0x08;
            packet.Code = 0x00;
            packet.Checksum = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 2, 2);
            data = Encoding.ASCII.GetBytes("test packet");
            Buffer.BlockCopy(data, 0, packet.Message, 4, data.Length);
            packet.MessageSize = data.Length + 4;
            int packetsize = packet.MessageSize + 4;

            UInt16 chcksum = packet.getChecksum();
            packet.Checksum = chcksum;
            host.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 50);

            host.SendTo(packet.getBytes(), packetsize, SocketFlags.None, iep);

            try
            {
                data = new byte[1024];
                recv = host.ReceiveFrom(data, ref ep);
                ICMP response = new ICMP(data, recv);

                if (response.Type == 0)
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
