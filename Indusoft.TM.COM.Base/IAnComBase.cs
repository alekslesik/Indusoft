using System;
using System.Collections.Generic;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  public interface IAnComBase
  {
    bool Connect(string machineName);

    void Disonnect();

    void Break(string modemId);

    string DoCommand(
      byte[] data,
      int begin,
      int size,
      string identifier,
      ATSWPBatchType batchType);

    bool GetUseCommand(string modemID, int siteId);

    bool SetUseCommand(string modemID, int siteId, bool mbUse);

    List<BatchData> GetCommandData(string caption, int index);

    void TestConnect(string modemId);

    List<BatchData> GetMonitorData(SendTo to, string caption, int index);

    bool SetUseMonitor(SendTo to, string caption, int index, bool mbUse);

    List<NewLogs> GetLogsData();

    List<UpdatedModems> GetModemsData();

    List<NewModems> GetNewModems();

    List<NewModems> GetModemsQuery();

    void SetDontForConnection(List<NewModems> data);

    void SetConnection(List<NewModems> data);

    ITMCOMDataSet GetDataSet();

    void GetDataFromDB(bool mbWait);

    ITMCOMDataSet SelectConfig();

    bool DeleteAllConfig();

    bool DeleteConfig(int configId);

    int InsertConfig(
      string iPAddress,
      int portNumber,
      string serverID,
      bool mbCurrent,
      int typeId);

    bool UpdateConfig(
      int configId,
      string iPAddress,
      int portNumber,
      string serverID,
      bool mbCurrent,
      int typeId);

    ITMCOMDataSet SelectDynamic();

    bool DeleteDynamic(int dynamicId);

    bool DeleteAllDynamics();

    int InsertDynamic(
      int siteID,
      string modemIP,
      string modemID,
      DateTime connectionTime,
      DateTime releaseTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int outTraffic);

    bool UpdateDynamic(
      int dynamicId,
      int siteID,
      string modemIP,
      string modemID,
      DateTime connectionTime,
      DateTime releaseTime,
      int inBatchCount,
      int outBatchCount,
      int InTraffic,
      int OutTraffic);

    ITMCOMDataSet SelectStatic();

    bool DeleteStatic(int staticId);

    bool DeleteAllStatics();

    int InsertStatic(
      string COMPort,
      DateTime connectionTime,
      DateTime releaseTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int outTraffic);

    bool UpdateStatic(
      int linkID,
      string COMPort,
      DateTime connectionTime,
      DateTime releaseTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int outTraffic);

    ITMCOMDataSet SelectPort();

    bool DeletePort(int portId, string portName);

    bool DeleteAllPorts();

    int InsertPort(string namePort, int baudRate, int parity, int dataBits, int stopBits);

    bool UpdatePort(
      int routingId,
      string portName,
      int baudRate,
      int parity,
      int dataBits,
      int stopBits);

    ITMCOMDataSet SelectRouting();

    bool DeleteRouting(int routingId, int siteID);

    bool DeleteAllRouting();

    int InsertRouting(
      int siteID,
      int linkID,
      int portId,
      int beginRange,
      int endRange,
      int localSiteId);

    bool UpdateRouting(
      int routingId,
      int siteID,
      int linkID,
      int portId,
      int beginRange,
      int endRange,
      int localSiteId);

    ITMCOMDataSet SelectLog();

    bool DeleteLog(int logId);

    bool DeleteAllLogs();

    bool InsertLog(int siteID, string COMPort, string message, int statusId);

    bool UpdateLog(
      int logId,
      int siteID,
      string COMPort,
      string message,
      int statusId,
      DateTime time);

    ITMCOMDataSet SelectModem();

    bool DeleteModem(int id, string modemID, int siteID);

    bool DeleteAllModems();

    int InsertModem(string modemID, int siteID, ConfigType type);

    bool UpdateModem(
      int id,
      string modemID,
      int siteID,
      bool mbConnect,
      DateTime time,
      ModemUpdateFlag flag,
      string oldModemID);
  }
}
