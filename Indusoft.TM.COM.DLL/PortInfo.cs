using Indusoft.TM.COM.Base;
using System;
using System.IO.Ports;
using System.Threading;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class PortInfo
  {
    public const int BufferSize = 8192;
    private int _portId;
    private int _creatorSiteId;
    private SerialPort _port;
    public byte[] Remain;
    private int _remainSize;
    private Thread _thread;
    private int _size = 8192;
    public byte[] Buffer = new byte[8192];
    private MonitorInfo _monitorInfo = new MonitorInfo();
    private int _inBatchCount;
    private int _outBatchCount;
    private int _inTraffic;
    private int _outTraffic;
    private DateTime _connectionTime;
    private int _staticId;

    public PortInfo(SerialPort sp, int siteId, int portId)
    {
      this._port = sp;
      this._creatorSiteId = siteId;
      this.Remain = (byte[]) null;
      this._remainSize = 0;
      this._connectionTime = DateTime.Now;
      this._portId = portId;
    }

    public void Close()
    {
      try
      {
        if (this._port.IsOpen)
          this._port.Close();
        this._monitorInfo.MbInUse = false;
        if (this._thread == null)
          return;
        this._thread.Abort();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("PortInfoCloseError: " + (object) ex));
      }
    }

    public int PortId
    {
      get => this._portId;
      set => this._portId = value;
    }

    public int CreatorSiteId
    {
      get => this._creatorSiteId;
      set => this._creatorSiteId = value;
    }

    public SerialPort Port
    {
      get => this._port;
      set => this._port = value;
    }

    public int RemainSize
    {
      get => this._remainSize;
      set => this._remainSize = value;
    }

    public Thread Thread
    {
      get => this._thread;
      set => this._thread = value;
    }

    public int Size
    {
      get => this._size;
      set => this._size = value;
    }

    public MonitorInfo MonitorInfo
    {
      get => this._monitorInfo;
      set => this._monitorInfo = value;
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

    public DateTime ConnectionTime
    {
      get => this._connectionTime;
      set => this._connectionTime = value;
    }

    public int StaticId
    {
      get => this._staticId;
      set => this._staticId = value;
    }
  }
}
