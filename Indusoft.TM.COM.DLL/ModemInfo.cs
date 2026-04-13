using Indusoft.TM.COM.Base;
using System.Net.Sockets;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class ModemInfo
  {
    private string _modemID;
    private int _bufferSize;
    public byte[] _buffer;
    private Socket _socket;
    private ATSWPBatchType _batchType;

    public string ModemID
    {
      get => this._modemID;
      set => this._modemID = value;
    }

    public int BufferSize
    {
      get => this._bufferSize;
      set => this._bufferSize = value;
    }

    public Socket Socket
    {
      get => this._socket;
      set => this._socket = value;
    }

    public ATSWPBatchType BatchType
    {
      get => this._batchType;
      set => this._batchType = value;
    }
  }
}
