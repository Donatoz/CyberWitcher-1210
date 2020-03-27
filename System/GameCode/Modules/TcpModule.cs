using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

namespace VovTech
{
    public class TcpModule : Module, INetworkModule
    {
        public NetManager Manager { get; set; }
        private TcpClient client;
        private NetworkStream stream;
        private byte[] recieveBuffer;
        private bool connected;

        public event NetworkDataReceived OnPacketRecieved;
        public event NetworkDataSent OnPacketSent;

        public void Connect(string ip, int port)
        {
            client = new TcpClient(AddressFamily.InterNetwork)
            {
                ReceiveBufferSize = Manager.RecievePacketSize,
                SendBufferSize = Manager.RecievePacketSize
            };
            recieveBuffer = new byte[Manager.RecievePacketSize];
            IPAddress[] addresses = Dns.GetHostAddresses(ip);
            client.BeginConnect(addresses[0], port, ConnectCallback, client);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            Debug.Log("Preparing connection...");
            client.EndConnect(result);
            connected = true;
            stream = client.GetStream();
            Debug.Log("Connected");
            stream.BeginRead(recieveBuffer, 0, recieveBuffer.Length, RecieveCallback, null);
        }

        private void RecieveCallback(IAsyncResult result)
        {
            int length = stream.EndRead(result);
            if (length <= 0) return;
            byte[] data = new byte[length];
            Array.Copy(recieveBuffer, data, length);
            int encodedMessage = BitConverter.ToInt32(data, 0);
            string decoded = Encoding.ASCII.GetString(data);
            OnPacketRecieved?.Invoke(decoded);
            stream.BeginRead(recieveBuffer, 0, Manager.RecievePacketSize, RecieveCallback, null);
        }

        public void SendData(params Packet[] packets)
        {
            try
            {
                if (client != null)
                {
                    for (int i = 0; i < packets.Length; i++)
                    {
                        stream.BeginWrite(packets[i].ToArray(), 0, packets[i].Length(), 
                            new AsyncCallback(SendCallback), packets[i]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                connected = false;
            }
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            stream.EndWrite(asyncResult);
            OnPacketSent?.Invoke((Packet)asyncResult.AsyncState);
        }

        private void OnDestroy()
        {
            client?.Close();
        }
    }
}