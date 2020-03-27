namespace VovTech
{
    public delegate void NetworkDataReceived(string packetContent);
    public delegate void NetworkDataSent(Packet packet);
    public interface INetworkModule
    {
        NetManager Manager { get; set; }
        void Connect(string ip, int port);
        void SendData(params Packet[] packets);

        event NetworkDataReceived OnPacketRecieved;
        event NetworkDataSent OnPacketSent;
    }
}