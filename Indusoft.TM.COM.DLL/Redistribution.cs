#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class Redistribution
  {
    private int _beginSiteId;
    private int _endSiteId;
    private int _linkId;
    private int _portId;

    public Redistribution(int beginSiteId, int endSiteId, int linkId, int portId)
    {
      this._beginSiteId = beginSiteId;
      this._endSiteId = endSiteId;
      this._linkId = linkId;
      this._portId = portId;
    }

    public int BeginSiteId
    {
      get => this._beginSiteId;
      set => this._beginSiteId = value;
    }

    public int EndSiteId
    {
      get => this._endSiteId;
      set => this._endSiteId = value;
    }

    public int LinkId
    {
      get => this._linkId;
      set => this._linkId = value;
    }

    public int PortId
    {
      get => this._portId;
      set => this._portId = value;
    }
  }
}
