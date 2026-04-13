using Indusoft.TM.COM.Base;
using System;
using System.Collections;
using System.Net.Sockets;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class DynamicTable
  {
    private Socket _client;
    private string _modemID;
    private ModemType _modemType;
    private bool _flag;
    public byte[] _remain;
    private int _remainSize;
    public byte[] _remainATSWP;
    private int _remainSizeATSWP;
    private DateTime _readTime = DateTime.MinValue;
    private DateTime _writeTime = DateTime.MinValue;
    private Queue _writeQueue = new Queue();
    private DateTime _lastWriteTime = new DateTime();
    private DateTime _connectionTime;
    private int _dynamicId;
    private int _inBatchCount;
    private int _outBatchCount;
    private int _inTraffic;
    private int _outTraffic;

    public DynamicTable(Socket client, string data, ModemType modemType)
    {
      this._client = client;
      this._flag = false;
      this._modemID = data;
      this._remain = (byte[]) null;
      this._remainSize = 0;
      this._connectionTime = DateTime.Now;
      this._modemType = modemType;
    }

    public void Close()
    {
      this._flag = false;
      Utilities.CloseSocket(this._client);
    }

    public Socket Client
    {
      get => this._client;
      set => this._client = value;
    }

    public string ModemID
    {
      get => this._modemID;
      set => this._modemID = value;
    }

    public ModemType ModemType
    {
      get => this._modemType;
      set => this._modemType = value;
    }

    public bool Flag
    {
      get => this._flag;
      set => this._flag = value;
    }

    public int RemainSize
    {
      get => this._remainSize;
      set => this._remainSize = value;
    }

    public int RemainSizeATSWP
    {
      get => this._remainSizeATSWP;
      set => this._remainSizeATSWP = value;
    }

    public DateTime ReadTime
    {
      get => this._readTime;
      set => this._readTime = value;
    }

    public DateTime WriteTime
    {
      get => this._writeTime;
      set => this._writeTime = value;
    }

    public Queue WriteQueue
    {
      get => this._writeQueue;
      set => this._writeQueue = value;
    }

    public DateTime LastWriteTime
    {
      get => this._lastWriteTime;
      set => this._lastWriteTime = value;
    }

    public DateTime ConnectionTime
    {
      get => this._connectionTime;
      set => this._connectionTime = value;
    }

    public int DynamicId
    {
      get => this._dynamicId;
      set => this._dynamicId = value;
    }

    public int InBatchCount
    {
      get => this._inBatchCount;
      set => this._inBatchCount = value;
    }

    public int OutBatchCount
    {
      get => this._outBatchCount;
      set => this._outBatchCount = value;
    }

    public int InTraffic
    {
      get => this._inTraffic;
      set => this._inTraffic = value;
    }

    public int OutTraffic
    {
      get => this._outTraffic;
      set => this._outTraffic = value;
    }
  }
}
