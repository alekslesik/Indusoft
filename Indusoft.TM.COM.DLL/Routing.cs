#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class Routing
  {
    private string _modemID;
    private int _siteID;
    private int _id;
    private bool _mbConnect;
    private MonitorInfo _monitorInfo = new MonitorInfo();
    private MonitorInfo _commandInfo = new MonitorInfo();

    public string ModemID
    {
      get => this._modemID;
      set => this._modemID = value;
    }

    public int SiteID
    {
      get => this._siteID;
      set => this._siteID = value;
    }

    public int Id
    {
      get => this._id;
      set => this._id = value;
    }

    public bool MbConnect
    {
      get => this._mbConnect;
      set => this._mbConnect = value;
    }

    public MonitorInfo MonitorInfo
    {
      get => this._monitorInfo;
      set => this._monitorInfo = value;
    }

    public MonitorInfo CommandInfo
    {
      get => this._commandInfo;
      set => this._commandInfo = value;
    }
  }
}
