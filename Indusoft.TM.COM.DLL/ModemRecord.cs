using Indusoft.TM.COM.Base;
using System;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class ModemRecord : DBRecord
  {
    public int Id;
    public string ModemId;
    public bool MbConnected;
    public DateTime Time;
    public ModemUpdateFlag Flag;
    public string OldModemId;

    public ModemRecord(
      int id,
      string modemId,
      bool mbConnected,
      DateTime time,
      ModemUpdateFlag flag,
      string oldModemId,
      int siteId)
      : base(RecordType.ModemConnection, siteId)
    {
      this.Id = id;
      this.ModemId = modemId;
      this.MbConnected = mbConnected;
      this.Time = time;
      this.Flag = flag;
      this.OldModemId = oldModemId;
    }
  }
}
