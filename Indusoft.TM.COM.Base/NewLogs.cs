using System;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  [Serializable]
  public struct NewLogs
  {
    private int _logId;
    private int _siteID;
    private string _COMPort;
    private DateTime _time;
    private string _message;
    private int _statusId;

    public int LogId => this._logId;

    public int SiteID => this._siteID;

    public string COMPort => this._COMPort;

    public DateTime Time => this._time;

    public string Message => this._message;

    public int StatusId => this._statusId;

    public NewLogs(
      int logId,
      int siteID,
      string COMPort_,
      DateTime time,
      string message,
      int statusId)
    {
      this._logId = logId;
      this._siteID = siteID;
      this._COMPort = COMPort_;
      this._message = message;
      this._statusId = statusId;
      this._time = time;
    }
  }
}
