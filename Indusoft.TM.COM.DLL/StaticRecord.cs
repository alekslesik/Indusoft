using System;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class StaticRecord : StatisticsRecord
  {
    public string PortName;
    public PortInfo PortInfo;

    public StaticRecord(
      DateTime connectionTime,
      DateTime recordTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int OutTraffic,
      string portName,
      PortInfo info)
      : base(-1, connectionTime, recordTime, inBatchCount, outBatchCount, inTraffic, OutTraffic, RecordType.Static)
    {
      this.PortName = portName;
      this.PortInfo = info;
    }
  }
}
