using System;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class DynamicRecord : StatisticsRecord
  {
    public string IpAddress;
    public string ModemId;
    public DynamicTable ModemInfo;

    public DynamicRecord(
      int siteId,
      DateTime connectionTime,
      DateTime recordTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int OutTraffic,
      string ipAddress,
      string modemId,
      DynamicTable info)
      : base(siteId, connectionTime, recordTime, inBatchCount, outBatchCount, inTraffic, OutTraffic, RecordType.Dynamic)
    {
      this.IpAddress = ipAddress;
      this.ModemId = modemId;
      this.ModemInfo = info;
    }
  }
}
