using Indusoft.TM.COM.Base;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class ListenerArgs
  {
    public IPAddress _address;
    public int _portNumber;
    public string _serverID;
    public ConfigType _type;
    public int _configurationId;
    public Thread _thread;
    public TcpListener _listener;

    public ListenerArgs(
      IPAddress address,
      int portNumber,
      string serverID,
      ConfigType type,
      int configurationId)
    {
      this._address = address;
      this._portNumber = portNumber;
      this._serverID = serverID;
      this._type = type;
      this._configurationId = configurationId;
    }
  }
}
