using System;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  [Serializable]
  public class NewModems
  {
    private int _id;
    private int _siteID;
    private string _modemID;
    private bool _mbAdd;

    public int Id
    {
      get => this._id;
      set => this._id = value;
    }

    public int SiteID
    {
      get => this._siteID;
      set => this._siteID = value;
    }

    public string ModemID
    {
      get => this._modemID;
      set => this._modemID = value;
    }

    public bool MbAdd
    {
      get => this._mbAdd;
      set => this._mbAdd = value;
    }

    public NewModems(int id, int siteID, string modemID)
    {
      this._id = id;
      this._siteID = siteID;
      this._modemID = modemID;
      this._mbAdd = false;
    }
  }
}
