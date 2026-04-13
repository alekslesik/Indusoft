using System;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class StatisticsRecord : DBRecord
  {
    public DateTime ConnectionTime;
    public DateTime RecordTime;
    public int InBatchCount;
    public int OutBatchCount;
    public int InTraffic;
    public int OutTraffic;

    public StatisticsRecord(
      int siteId,
      DateTime connectionTime,
      DateTime recordTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int outTraffic,
      RecordType type)
      : base(type, siteId)
    {
      this.ConnectionTime = connectionTime;
      this.RecordTime = recordTime;
      this.InBatchCount = inBatchCount;
      this.OutBatchCount = outBatchCount;
      this.InTraffic = inTraffic;
      this.OutTraffic = outTraffic;
    }
  }
}
