using Indusoft.TM.COM.Base;
using System.Collections;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class MonitorInfo
  {
    private Queue _monitorQueue = new Queue();
    private bool _mbInUse;

    public Queue MonitorQueue
    {
      get => this._monitorQueue;
      set => this._monitorQueue = value;
    }

    public bool MbInUse
    {
      get => this._mbInUse;
      set
      {
        this._mbInUse = value;
        if (this._mbInUse)
          return;
        this._monitorQueue.Clear();
      }
    }

    public void InsertNewBatch(BatchData data, int maxSize)
    {
      Utilities.InsertIntoQueue(this._monitorQueue, (object) data, maxSize);
    }
  }
}
