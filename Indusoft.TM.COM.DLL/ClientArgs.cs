using Indusoft.TM.COM.Base;
using System.Net.Sockets;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class ClientArgs
  {
    public Socket _client;
    public ModemType _type;
    public ListenerArgs _listenerArgs;

    public ClientArgs(Socket client, ListenerArgs listenerArgs, ModemType type)
    {
      this._client = client;
      this._listenerArgs = listenerArgs;
      this._type = type;
    }
  }
}
