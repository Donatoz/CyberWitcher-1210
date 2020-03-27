using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;

namespace VovTech
{
    public class UdpModule : MonoBehaviour, INetworkModule
    {
        /// <summary>
        /// UDP Client state (not so neccessary, just content-holder)
        /// </summary>
        public struct UdpState
        {
            /// <summary>
            /// Client (socket).
            /// </summary>
            public UdpClient Client;
            /// <summary>
            /// IPv4 + port.
            /// </summary>
            public IPEndPoint Address;

            public UdpState(UdpClient client, IPEndPoint address)
            {
                Client = client;
                Address = address;
            }
        }
        /// <summary>
        /// Network manager.
        /// </summary>
        public NetManager Manager { get; set; }
        /// <summary>
        /// Data-sending socket.
        /// </summary>
        private UdpClient sendingClient;
        /// <summary>
        /// Data-receiving socket.
        /// </summary>
        private UdpClient receivingClient;
        /// <summary>
        /// Address to listen/send.
        /// </summary>
        private IPEndPoint address;
        private IPEndPoint receiveAddress;

        /// <summary>
        /// Invoked when some packet is being sent to the server.
        /// </summary>
        public event NetworkDataReceived OnPacketRecieved;
        /// <summary>
        /// Invoked when some packet is being received from the server.
        /// </summary>
        public event NetworkDataSent OnPacketSent;

        /// <summary>
        /// Start listening/sending data
        /// </summary>
        /// <param name="ip">Server IPv4</param>
        /// <param name="port">Server port</param>
        public void Connect(string ip, int port)
        {
            // Get IP-address by DNS
            IPAddress[] addresses = Dns.GetHostAddresses(Manager.IpAddress);
            // Convert it to end-point
            address = new IPEndPoint(addresses[0], Manager.Port);
            receiveAddress = new IPEndPoint(IPAddress.Any, Manager.Port);
            // Initialize sending client
            sendingClient = new UdpClient();
            // Initialize receiving client with end-point to automaticly listen for packets
            receivingClient = new UdpClient(new IPEndPoint(IPAddress.Any, Manager.Port - 1));
            // Bind socket to server
            sendingClient.Connect(address);
            // Begin listening for packets (async)
            //receivingClient.BeginReceive(new AsyncCallback(ReceiveCallback), receivingClient);
        }

        private void Update()
        {
            Read();
        }

        private void Read()
        {
            if(receivingClient.Available > 0 && receivingClient != null)
            {
                byte[] receiveBytes = receivingClient.Receive(ref receiveAddress);
                // Decode packet
                string decodedString = Encoding.ASCII.GetString(receiveBytes);
                OnPacketRecieved?.Invoke(decodedString);
            }
        }

        /// <summary>
        /// Async callback for data receiving
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            
            // Invokes when packet is received
            // End receiving packet
            byte[] receiveBytes = receivingClient.EndReceive(asyncResult, ref address);
            // Decode packet
            string decodedString = Encoding.ASCII.GetString(receiveBytes);
            Debug.Log(decodedString);
            OnPacketRecieved?.Invoke(decodedString);
            // Recursion
            receivingClient.BeginReceive(new AsyncCallback(ReceiveCallback), receivingClient);
        }

        /// <summary>
        /// Send N amount of packets to end-point
        /// </summary>
        /// <param name="packets"></param>
        public void SendData(params Packet[] packets)
        {
            try
            {
                if (sendingClient != null)
                {
                    for (int i = 0; i < packets.Length; i++)
                    {
                        // Begin sending packet
                        sendingClient.BeginSend
                        (
                            packets[i].ToArray(), // Bytes
                            packets[i].Length(), // Packet length
                            new AsyncCallback(SendCallback), // Callback
                            packets[i] // State (to invoke event)
                        );
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        /// <summary>
        /// Async callback for packet sending
        /// </summary>
        /// <param name="asyncResult"></param>
        private void SendCallback(IAsyncResult asyncResult)
        {
            // End packet sending to end-point
            sendingClient.EndSend(asyncResult);
            OnPacketSent?.Invoke((Packet)asyncResult.AsyncState);
        }
        /// <summary>
        /// On level or application close, untie all bindings.
        /// </summary>
        private void OnDestroy()
        {
            sendingClient?.Close();
            receivingClient?.Close();
        }
    }
}