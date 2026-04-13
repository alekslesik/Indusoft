using System;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  [Serializable]
  public struct UpdatedModems
  {
    private int _id;
    private bool _mbConnect;
    private DateTime _time;
    private ModemUpdateFlag _flag;

    public int Id => this._id;

    public bool MbConnect => this._mbConnect;

    public DateTime Time => this._time;

    public ModemUpdateFlag Flag => this._flag;

    public UpdatedModems(int id, bool mbConnect, DateTime time, ModemUpdateFlag flag)
    {
      this._id = id;
      this._mbConnect = mbConnect;
      this._time = time;
      this._flag = flag;
    }
  }
}
