#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class DBRecord
  {
    private int _siteId;
    private RecordType _type;

    public RecordType Type => this._type;

    public int SiteId => this._siteId;

    public DBRecord(RecordType type, int siteID)
    {
      this._type = type;
      this._siteId = siteID;
    }
  }
}
