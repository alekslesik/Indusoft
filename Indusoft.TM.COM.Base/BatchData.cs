using Indusoft.TM.COM.Base.Properties;
using System;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  [Serializable]
  public struct BatchData
  {
    private string _unitSiteId;
    private string _direction;
    private string _time;
    private string _siteId;
    private string _batch;

    public string UnitSiteId => this._unitSiteId;

    public string Direction => this._direction;

    public string Time => this._time;

    public string SiteId => this._siteId;

    public string Batch => this._batch;

    public BatchData(
      int unitSiteId,
      Indusoft.TM.COM.Base.Direction direction,
      DateTime time,
      int siteId,
      string batch)
    {
      this._unitSiteId = unitSiteId.ToString();
      this._direction = direction.ToString();
      this._time = string.Format("{0},{1:D3}", (object) time.ToString(), (object) time.Millisecond);
      this._siteId = siteId.ToString();
      if (direction == Indusoft.TM.COM.Base.Direction.отправка)
      {
        this._direction = Resources.Send + this._siteId;
      }
      else
      {
        switch (siteId)
        {
          case -3:
            this._direction = Resources.CommandAnswer;
            break;
          case -2:
            this._direction = Resources.Command;
            break;
          case -1:
            this._direction = Resources.NoReceiver;
            break;
          default:
            this._direction = Resources.Receive + this._siteId;
            break;
        }
      }
      this._batch = batch;
    }

    public BatchData(
      int unitSiteId,
      StringCommandType direction,
      DateTime time,
      int siteId,
      string batch)
    {
      this._unitSiteId = unitSiteId.ToString();
      this._direction = Utils.GetStringCommandType(direction);
      this._time = string.Format("{0},{1:D3}", (object) time.ToString(), (object) time.Millisecond);
      this._siteId = siteId.ToString();
      this._batch = batch;
    }
  }
}
