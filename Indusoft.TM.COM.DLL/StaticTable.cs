using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class StaticTable
  {
    public List<Redistribution> PortDataList;
    private int _siteID;
    private Queue _queue = new Queue();
    private DateTime _lastWriteTime = new DateTime();
    private int _localSiteID;

    public StaticTable(int siteID_, int localSiteId)
    {
      this._siteID = siteID_;
      this.PortDataList = new List<Redistribution>();
      this._localSiteID = localSiteId;
    }

    public int SiteID
    {
      get => this._siteID;
      set => this._siteID = value;
    }

    public Queue Queue
    {
      get => this._queue;
      set => this._queue = value;
    }

    public DateTime LastWriteTime
    {
      get => this._lastWriteTime;
      set => this._lastWriteTime = value;
    }

    public int LocalSiteID
    {
      get => this._localSiteID;
      set => this._localSiteID = value;
    }
  }
}
