using System;
using System.Net;
using System.Net.Sockets;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  [Serializable]
  public class TCPIPClient
  {
    public IPEndPoint Point;
    public TcpClient TcpIP;
    public int SiteID;

    public TCPIPClient(int id) => this.SiteID = id;

    public void Connect(string key)
    {
      if (this.Point == null)
      {
        string[] separator = new string[1]{ ":" };
        string[] strArray = key.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        this.Point = new IPEndPoint(IPAddress.Parse(strArray[0]), strArray.Length > 0 ? int.Parse(strArray[1]) : 0);
      }
      this.TcpIP = new TcpClient();
      this.TcpIP.LingerState = new LingerOption(true, 0);
      this.TcpIP.ReceiveBufferSize = 16384;
      this.TcpIP.SendBufferSize = 16384;
      this.TcpIP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, (object) new LingerOption(true, 10));
      this.TcpIP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
      this.TcpIP.Connect(this.Point);
    }
  }
}
