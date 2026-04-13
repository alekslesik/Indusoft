using Indusoft.TM.COM.Base;
using Indusoft.TM.COM.DLL.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class ITMCOMDLL : MarshalByRefObject, IAnComBase
  {
    private const int _localSiteIDConst = 65534;
    private const int _toolboxSiteIDConst = 32767;
    private const int _maxBufferSize = 10000;
    private static ITMCOMDLL _instance;
    public ITMCOMDataSet _mainDataSet = new ITMCOMDataSet();
    private int _cleanupTime = 60;
    private bool _mbDetailedConnectionLog = true;
    private int _logRecordsCount = 500;
    private int _statisticHoursCount = 2880;
    private int _queueSize = 10000;
    private int _sendVal = 15000;
    private int _recieveVal = 15000;
    private int _sendPortVal = 30000;
    private int _recievePortVal = 30000;
    private int _portSendVal = 15000;
    private int _portRecieveVal = 15000;
    private bool _mbConnectUnknown;
    private bool _mbAskClient;
    private bool _mbAutoCloseDisconnected = true;
    private bool _mbCheckTimeDifference;
    private int _timeDifference = 30000;
    private string _machineName;
    private bool _closeFlag;
    private Hashtable _routingList = new Hashtable();
    private Hashtable _routingListEx = new Hashtable();
    private Hashtable _myTable = new Hashtable();
    private Hashtable _tempList = new Hashtable();
    private Hashtable _forConnection = new Hashtable();
    private Hashtable _dontForConnection = new Hashtable();
    private Hashtable _tempListCOM = new Hashtable();
    private Hashtable _portList = new Hashtable();
    private Hashtable _listeners = new Hashtable();
    private Hashtable _TCPIPCacheList = new Hashtable();
    private System.Collections.Queue _logList = new System.Collections.Queue();
    private System.Collections.Queue _modemList = new System.Collections.Queue();
    private System.Collections.Queue _newModemList = new System.Collections.Queue();
    private Thread _readWriteThread;
    private Thread _statisticsThread;
    private Thread _writePortThread;
    private Thread _cacheThread;
    private System.Collections.Queue _recordsQueue = new System.Collections.Queue();
    private Thread _DBThread;
    private Thread _clientThread;
    private Hashtable _clientList = new Hashtable();

    private void InsertRecord(DBRecord record)
    {
      lock (this._recordsQueue.SyncRoot)
        Utilities.InsertIntoQueue(this._recordsQueue, (object) record, this._queueSize);
    }

    private void ProcessingDBRecords()
    {
      while (!this._closeFlag)
      {
        this.PutAllDBRecords();
        Thread.Sleep(1000);
      }
    }

    public void PutAllDBRecords()
    {
      try
      {
        while (this._recordsQueue.Count > 0)
        {
          DBRecord dbRecord = (DBRecord) null;
          lock (this._recordsQueue.SyncRoot)
            dbRecord = this._recordsQueue.Dequeue() as DBRecord;
          switch (dbRecord)
          {
            case DynamicRecord _:
              DynamicRecord dynamicRecord = dbRecord as DynamicRecord;
              int num1 = this.InsertDynamic(dynamicRecord.SiteId, dynamicRecord.IpAddress, dynamicRecord.ModemId, dynamicRecord.ConnectionTime, dynamicRecord.RecordTime, dynamicRecord.InBatchCount, dynamicRecord.OutBatchCount, dynamicRecord.InTraffic, dynamicRecord.OutTraffic);
              try
              {
                dynamicRecord.ModemInfo.DynamicId = num1;
                continue;
              }
              catch
              {
                continue;
              }
            case StaticRecord _:
              StaticRecord staticRecord = dbRecord as StaticRecord;
              int num2 = this.InsertStatic(staticRecord.PortName, staticRecord.ConnectionTime, staticRecord.RecordTime, staticRecord.InBatchCount, staticRecord.OutBatchCount, staticRecord.InTraffic, staticRecord.OutTraffic);
              try
              {
                staticRecord.PortInfo.StaticId = num2;
                continue;
              }
              catch
              {
                continue;
              }
            case LogRecord _:
              LogRecord logRecord = dbRecord as LogRecord;
              this.InsertLog(logRecord.SiteId, logRecord.COMPort, logRecord.Message, (int) logRecord.Status);
              continue;
            case ModemRecord _:
              ModemRecord modemRecord = dbRecord as ModemRecord;
              this.UpdateModem(modemRecord.Id, modemRecord.ModemId, modemRecord.SiteId, modemRecord.MbConnected, modemRecord.Time, modemRecord.Flag, modemRecord.OldModemId);
              continue;
            case NewModemRecord _:
              NewModemRecord newModemRecord = dbRecord as NewModemRecord;
              this.InsertModem(newModemRecord.ModemId, newModemRecord.SiteId, newModemRecord.ConfigType);
              continue;
            default:
              continue;
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ProcessingDBRecords: " + ex.Message));
      }
    }

    public void COMPorts()
    {
      for (int index = 0; index < this._mainDataSet.Port.Count; ++index)
        this.AddNewPort(this._mainDataSet.Port[index].PortId, this._mainDataSet.Port[index].PortName, this._mainDataSet.Port[index].BaudRate, this._mainDataSet.Port[index].Parity, this._mainDataSet.Port[index].DataBits, this._mainDataSet.Port[index].StopBits);
      for (int index = 0; index < this._mainDataSet.Routing.Count; ++index)
      {
        int localSiteId = !(this._mainDataSet.Routing[index]["LocalSiteID"] is DBNull) ? this._mainDataSet.Routing[index].LocalSiteID : 0;
        this.AddNewRouting(this._mainDataSet.Routing[index].SiteID, this._mainDataSet.Routing[index].LinkID, this._mainDataSet.Routing[index].PortId, this._mainDataSet.Routing[index].BeginRange, this._mainDataSet.Routing[index].EndRange, localSiteId);
      }
    }

    private int GetPortId(StaticTable item, WriteObject obj)
    {
      try
      {
        foreach (Redistribution portData in item.PortDataList)
        {
          if (portData.BeginSiteId == 0 || portData.BeginSiteId <= obj._siteIdFrom && portData.EndSiteId >= obj._siteIdFrom)
            return portData.PortId;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetPortIdError: " + ex.Message));
      }
      return -1;
    }

    private void DataSend()
    {
      while (!this._closeFlag)
      {
        foreach (StaticTable staticTable in (IEnumerable) this._tempListCOM.Values)
        {
          try
          {
            lock (staticTable)
            {
              lock (this._portList.SyncRoot)
              {
                while (staticTable.Queue.Count > 0)
                {
                  WriteObject writeObject = (WriteObject) staticTable.Queue.Dequeue();
                  if (writeObject != null)
                  {
                    PortInfo port = (PortInfo) this._portList[(object) this.GetPortId(staticTable, writeObject)];
                    if (port != null && port.Port.IsOpen)
                    {
                      if (port.Port.BaseStream.CanWrite)
                      {
                        try
                        {
                          port.Port.BaseStream.Write(writeObject._data, writeObject._begin, writeObject._size);
                        }
                        catch (Exception ex1)
                        {
                          EventLogger.Info((object) string.Format("Port.BaseStream.Write: {0}", (object) ex1.Message));
                          try
                          {
                            port.Port.Close();
                          }
                          catch (Exception ex2)
                          {
                            EventLogger.Info((object) string.Format("SerialPort.Port.Close error (DataReceive): {0} - {1}", (object) ex2.Message, (object) port.Port.PortName));
                          }
                        }
                        ++port.OutBatchCount;
                        port.OutTraffic += writeObject._size;
                        staticTable.LastWriteTime = DateTime.Now;
                        Routing routing = (Routing) null;
                        int key = writeObject._siteIdFrom;
                        if (key == 65534)
                          key = staticTable.LocalSiteID;
                        lock (this._routingList.SyncRoot)
                          routing = this._routingList[(object) key] as Routing;
                        if (routing == null)
                        {
                          EventLogger.Info((object) string.Format(Resource.ModemNotFound, (object) writeObject._siteIdFrom.ToString()));
                        }
                        else
                        {
                          BatchData data = new BatchData(writeObject._siteIdFrom, Direction.отправка, DateTime.Now, staticTable.SiteID, Utils.TranslateModemBatch(writeObject._data, writeObject._begin, writeObject._size));
                          this.Save(data, SendTo.Modem, routing.ModemID);
                          data = new BatchData(staticTable.SiteID, Direction.получение, DateTime.Now, writeObject._siteIdFrom, Utils.TranslateModemBatch(writeObject._data, writeObject._begin, writeObject._size));
                          this.Save(data, SendTo.COM, port.Port.PortName);
                        }
                      }
                    }
                  }
                  else
                    break;
                }
                if (staticTable.LastWriteTime.AddSeconds((double) this._cleanupTime) <= DateTime.Now)
                {
                  if (staticTable.Queue.Count > 0)
                    staticTable.Queue.Clear();
                }
              }
            }
          }
          catch (Exception ex)
          {
            if (this._closeFlag)
              return;
            EventLogger.Info((object) string.Format("SerialPortWriteError: {0}", (object) ex.Message));
          }
        }
        Thread.Sleep(100);
      }
    }

    private void DataReceive(object sender)
    {
      if (!(sender is PortInfo portInfo))
        return;
      while (!this._closeFlag)
      {
        if (!portInfo.Port.IsOpen)
        {
          try
          {
            portInfo.Port.DtrEnable = true;
            portInfo.Port.RtsEnable = true;
            portInfo.Port.ReadTimeout = this._portRecieveVal;
            portInfo.Port.WriteTimeout = this._portSendVal;
            portInfo.Port.Open();
            this.InsertRecord((DBRecord) new LogRecord(portInfo.CreatorSiteId, portInfo.Port.PortName, Resource.PortConnection, LogStatus.Diagnostic));
          }
          catch (Exception ex)
          {
            this.InsertRecord((DBRecord) new LogRecord(portInfo.CreatorSiteId, portInfo.Port.PortName, string.Format(Resource.PortOpenningError, (object) ex.Message), LogStatus.Alarm));
            EventLogger.Info((object) ("DataReceiveError - " + string.Format(Resource.PortOpenningErrorEx, (object) ex.Message, (object) portInfo.Port.PortName)));
            Thread.Sleep(60000);
            continue;
          }
        }
        if (portInfo.Port.BaseStream.CanRead)
        {
          if (portInfo.Port.BytesToRead > 0)
          {
            try
            {
              int num;
              while ((num = portInfo.Port.BaseStream.Read(portInfo.Buffer, 0, portInfo.Size)) > 0)
              {
                int size = num + portInfo.RemainSize;
                byte[] receive = new byte[size];
                if (portInfo.RemainSize > 0)
                {
                  for (int index = 0; index < portInfo.RemainSize; ++index)
                    receive[index] = portInfo.Remain[index];
                }
                for (int index = 0; index < num; ++index)
                  receive[portInfo.RemainSize + index] = portInfo.Buffer[index];
                int batchCount = 0;
                portInfo.RemainSize = this.GetBatchs(receive, size, SendTo.Modem, ref portInfo.Remain, ref batchCount, -1, portInfo.Port.PortName);
                portInfo.InTraffic += num;
                portInfo.InBatchCount += batchCount;
              }
            }
            catch (TimeoutException ex)
            {
            }
            catch (Exception ex1)
            {
              if (this._closeFlag)
                break;
              EventLogger.Info((object) string.Format("SerialPortReadError: {0} - {1}", (object) ex1.Message, (object) portInfo.Port.PortName));
              this.InsertRecord((DBRecord) new LogRecord(portInfo.CreatorSiteId, portInfo.Port.PortName, string.Format(Resource.ReadFromPort, (object) ex1.Message), LogStatus.Alarm));
              try
              {
                portInfo.Port.Close();
              }
              catch (Exception ex2)
              {
                EventLogger.Info((object) string.Format("SerialPort.Port.Close error (DataReceive): {0} - {1}", (object) ex2.Message, (object) portInfo.Port.PortName));
              }
            }
          }
        }
        Thread.Sleep(100);
      }
    }

    public void Statistics()
    {
      int num = DateTime.Now.Hour;
      while (!this._closeFlag)
      {
        Thread.Sleep(1000);
        try
        {
          if (num == DateTime.Now.Hour)
          {
            if (DateTime.Now.Minute == 59)
            {
              if (DateTime.Now.Second > 50)
              {
                num = DateTime.Now.Hour >= 23 ? 0 : DateTime.Now.Hour + 1;
                this.WriteStatistics();
              }
            }
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("StatisticsError: " + ex.Message));
        }
      }
    }

    public void WriteStatistics()
    {
      try
      {
        lock (this._myTable.SyncRoot)
        {
          foreach (DynamicTable info in (IEnumerable) this._myTable.Values)
          {
            this.InsertRecord((DBRecord) new DynamicRecord(((Routing) this._routingListEx[(object) info.ModemID]).SiteID, info.ConnectionTime, DateTime.Now, info.InBatchCount, info.OutBatchCount, info.InTraffic, info.OutTraffic, this.GetIPAddress(info.Client), info.ModemID, info));
            info.InBatchCount = 0;
            info.OutBatchCount = 0;
            info.InTraffic = 0;
            info.OutTraffic = 0;
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("WriteStatisticsError(myTable): " + ex.Message));
      }
      try
      {
        lock (this._portList.SyncRoot)
        {
          foreach (PortInfo info in (IEnumerable) this._portList.Values)
          {
            this.InsertRecord((DBRecord) new StaticRecord(info.ConnectionTime, DateTime.Now, info.InBatchCount, info.OutBatchCount, info.InTraffic, info.OutTraffic, info.Port.PortName, info));
            info.InBatchCount = 0;
            info.OutBatchCount = 0;
            info.InTraffic = 0;
            info.OutTraffic = 0;
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("WriteStatisticsError(tempListCOM): " + ex.Message));
      }
    }

    private void AddNewRouting(
      int siteID,
      int linkID,
      int portId,
      int beginRange,
      int endRange,
      int localSiteId)
    {
      try
      {
        lock (this._tempListCOM.SyncRoot)
        {
          StaticTable staticTable = (StaticTable) this._tempListCOM[(object) siteID] ?? new StaticTable(siteID, localSiteId);
          staticTable.LocalSiteID = localSiteId;
          Redistribution redistribution = new Redistribution(beginRange, endRange, linkID, portId);
          staticTable.PortDataList.Add(redistribution);
          this._tempListCOM[(object) siteID] = (object) staticTable;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("AddNewRoutingError: " + ex.Message));
      }
    }

    private void AddNewPort(
      int index,
      string portName,
      int baudRate,
      int parity,
      int dataBits,
      int stopBits)
    {
      try
      {
        lock (this._portList.SyncRoot)
        {
          if ((PortInfo) this._portList[(object) index] != null)
            return;
          SerialPort sp;
          try
          {
            sp = new SerialPort(portName, baudRate, (Parity) parity, dataBits, (StopBits) stopBits);
            sp.ReadTimeout = this._recievePortVal;
            sp.WriteTimeout = this._sendPortVal;
          }
          catch (Exception ex)
          {
            this.InsertRecord((DBRecord) new LogRecord(-1, portName, string.Format(Resource.PortInitError, (object) ex.Message), LogStatus.Alarm));
            EventLogger.Info((object) string.Format("SerialPortReadError: {0} - {1}", (object) string.Format(Resource.PortInitError, (object) ex.Message), (object) portName));
            sp = (SerialPort) null;
          }
          if (sp == null)
            return;
          PortInfo parameter = new PortInfo(sp, -1, index);
          Thread thread = new Thread(new ParameterizedThreadStart(this.DataReceive));
          parameter.Thread = thread;
          this._portList[(object) index] = (object) parameter;
          thread.Start((object) parameter);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("AddNewPortError: " + ex.Message));
      }
    }

    public void RemovePort(int portId, string portName)
    {
      try
      {
        lock (this._portList.SyncRoot)
        {
          PortInfo port = (PortInfo) this._portList[(object) portId];
          if (port == null)
            return;
          port.Close();
          this._portList.Remove((object) portId);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemovePortError: " + ex.Message));
      }
    }

    public void RemoveAllPorts()
    {
      try
      {
        lock (this._portList.SyncRoot)
        {
          foreach (PortInfo port in this._portList)
            port.Close();
          this._portList.Clear();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemovePortError: " + ex.Message));
      }
    }

    public void RemoveRouting(int siteID)
    {
      try
      {
        lock (this._tempListCOM.SyncRoot)
        {
          StaticTable staticTable = (StaticTable) this._tempListCOM[(object) siteID];
          this._tempListCOM.Remove((object) siteID);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveRoutingError: " + ex.Message));
      }
    }

    public void RemoveAllRouting()
    {
      try
      {
        lock (this._tempListCOM.SyncRoot)
          this._tempListCOM.Clear();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveRoutingError: " + ex.Message));
      }
    }

    public string GetPortName(int siteID, int siteIDFrom)
    {
      int key = 0;
      try
      {
        lock (this._tempListCOM.SyncRoot)
        {
          StaticTable staticTable = (StaticTable) this._tempListCOM[(object) siteID];
          if (staticTable == null)
            return "";
          foreach (Redistribution portData in staticTable.PortDataList)
          {
            if (portData.BeginSiteId == 0 || portData.BeginSiteId <= siteIDFrom && portData.EndSiteId >= siteIDFrom)
            {
              key = portData.PortId;
              break;
            }
          }
          if (key == 0)
            return "";
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetPortNameError(tempListCOM): " + ex.Message));
      }
      try
      {
        lock (this._portList.SyncRoot)
        {
          PortInfo port = (PortInfo) this._portList[(object) key];
          if (port != null)
            return port.Port.PortName;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetPortNameError(portList): " + ex.Message));
      }
      return "";
    }

    public override object InitializeLifetimeService()
    {
      ILease lease = (ILease) base.InitializeLifetimeService();
      if (lease != null && lease.CurrentState == LeaseState.Initial)
        lease.InitialLeaseTime = TimeSpan.Zero;
      return (object) lease;
    }

    public static ITMCOMDLL Instance
    {
      get
      {
        lock (typeof (ITMCOMDLL))
        {
          if (ITMCOMDLL._instance == null)
            ITMCOMDLL._instance = new ITMCOMDLL();
          return ITMCOMDLL._instance;
        }
      }
    }

    public bool Connect(string machineName_)
    {
      if (this._machineName == null)
        this._machineName = machineName_;
      return this._machineName == machineName_;
    }

    public void Disonnect()
    {
      this._machineName = (string) null;
      try
      {
        lock (this._newModemList.SyncRoot)
          this._newModemList.Clear();
        lock (this._modemList.SyncRoot)
          this._modemList.Clear();
        lock (this._logList.SyncRoot)
          this._logList.Clear();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("Disonnect function error: {0}", (object) ex.Message));
      }
    }

    private void Save(BatchData data, SendTo sendTo, string name)
    {
      if (name == "")
        return;
      if (sendTo == SendTo.Modem)
      {
        try
        {
          lock (this._routingListEx.SyncRoot)
          {
            if (!(this._routingListEx[(object) name] is Routing routing) || !routing.MonitorInfo.MbInUse)
              return;
            routing.MonitorInfo.InsertNewBatch(data, this._queueSize);
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) string.Format("Save Function Error (SendTo.Modem): {0}", (object) ex.Message));
        }
      }
      else
      {
        try
        {
          lock (this._portList.SyncRoot)
          {
            PortInfo portInfo1 = (PortInfo) null;
            foreach (PortInfo portInfo2 in (IEnumerable) this._portList.Values)
            {
              if (portInfo2.Port.PortName == name)
                portInfo1 = portInfo2;
            }
            if (portInfo1 == null || !portInfo1.MonitorInfo.MbInUse)
              return;
            portInfo1.MonitorInfo.InsertNewBatch(data, this._queueSize);
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) string.Format("Save Function Error (SendTo.COM): {0}", (object) ex.Message));
        }
      }
    }

    public void SendData(
      byte[] data,
      int begin,
      int size,
      SendTo to,
      int siteIdTo,
      int siteIdFrom,
      int linkId,
      int siteId,
      string identifier,
      ATSWPBatchType batchType)
    {
      switch (to)
      {
        case SendTo.COM:
          WriteObject writeObject = new WriteObject(data, begin, size, siteIdFrom);
          try
          {
            int key = siteIdTo;
            if (key == 65534)
              key = (int) short.MaxValue;
            else if (siteId != -1 && siteIdFrom != siteId)
              this.InsertRecord((DBRecord) new LogRecord(siteId, "", string.Format(Resource.DifferentSiteID, (object) siteId, (object) siteIdFrom, (object) identifier), LogStatus.Warning));
            lock (this._tempListCOM.SyncRoot)
            {
              if (!(this._tempListCOM[(object) key] is StaticTable staticTable))
              {
                this.Save(new BatchData(siteIdFrom, Direction.получение, DateTime.Now, -1, Utils.TranslateModemBatch(data, begin, size)), SendTo.Modem, identifier);
                break;
              }
              if (staticTable.Queue.Count == 0)
                staticTable.LastWriteTime = DateTime.Now;
              staticTable.Queue.Enqueue((object) writeObject);
              break;
            }
          }
          catch (Exception ex)
          {
            EventLogger.Info((object) ("SendDataError (SendTo.COM): " + ex.Message));
            break;
          }
        case SendTo.Modem:
          Routing routing = (Routing) null;
          int key1 = 0;
          if (siteIdTo == 65534)
          {
            try
            {
              lock (this._tempListCOM.SyncRoot)
              {
                if (this._tempListCOM[(object) siteIdFrom] is StaticTable staticTable)
                  key1 = staticTable.LocalSiteID;
              }
            }
            catch (Exception ex)
            {
              EventLogger.Info((object) ("SendDataError (SendTo.Modem): " + ex.Message));
            }
          }
          try
          {
            lock (this._routingList.SyncRoot)
              routing = siteIdTo != 65534 ? this._routingList[(object) siteIdTo] as Routing : this._routingList[(object) key1] as Routing;
            if (routing == null)
            {
              this.Save(new BatchData(siteIdFrom, Direction.получение, DateTime.Now, -1, Utils.TranslateModemBatch(data, begin, size)), SendTo.COM, identifier);
              EventLogger.Info((object) string.Format("Получатель не найден: {0}", (object) Utils.TranslateModemBatch(data, begin, size)));
              break;
            }
            ModemInfo modemInfo = new ModemInfo();
            modemInfo.BufferSize = size;
            modemInfo.ModemID = routing.ModemID;
            modemInfo._buffer = new byte[size];
            for (int index = 0; index < modemInfo.BufferSize; ++index)
              modemInfo._buffer[index] = data[index + begin];
            lock (this._myTable.SyncRoot)
            {
              if (!(this._tempList[(object) modemInfo.ModemID] is Socket temp) || !(this._myTable[(object) temp] is DynamicTable dynamicTable))
                break;
              if (dynamicTable.WriteQueue.Count == 0)
                dynamicTable.LastWriteTime = DateTime.Now;
              dynamicTable.WriteQueue.Enqueue((object) modemInfo);
              EventLogger.Info((object) string.Format("Отправлено в очередь для модема: {0}", (object) Utils.TranslateModemBatch(data, begin, size)));
              this._myTable[(object) temp] = (object) dynamicTable;
              break;
            }
          }
          catch (Exception ex)
          {
            EventLogger.Info((object) ("SendDataError (ModemInfo): " + ex.Message));
            break;
          }
      }
    }

    public int GetBatchs(
      byte[] receive,
      int size,
      SendTo to,
      ref byte[] remain,
      ref int batchCount,
      int siteId,
      string identifier)
    {
      return this.GetBatchs(receive, size, to, ref remain, ref batchCount, siteId, identifier, ATSWPBatchType.DataUART1);
    }

    public int GetBatchs(
      byte[] receive,
      int size,
      SendTo to,
      ref byte[] remain,
      ref int batchCount,
      int siteId,
      string identifier,
      ATSWPBatchType batchType)
    {
      try
      {
        int batchs = size;
        int begin = 0;
        bool flag1 = false;
        while (batchs >= 9 && receive.Length >= 9)
        {
          byte[] numArray = new byte[9];
          int num1 = 0;
          bool flag2 = false;
          bool flag3 = true;
          for (int index = 0; index < 9; ++index)
          {
            if (num1 >= batchs)
            {
              flag2 = true;
              break;
            }
            if (!flag3 && receive[begin + num1] == (byte) 10)
            {
              batchs -= num1;
              begin += num1;
              flag1 = true;
              break;
            }
            if (receive[begin + num1] != (byte) 16)
            {
              numArray[index] = receive[begin + num1];
            }
            else
            {
              ++num1;
              if (num1 >= batchs)
              {
                flag2 = true;
                break;
              }
              numArray[index] = receive[begin + num1];
              numArray[index] = (byte) ((uint) numArray[index] ^ (uint) byte.MaxValue);
            }
            flag3 = false;
            ++num1;
          }
          if (flag1)
            flag1 = false;
          else if (!flag2)
          {
            DataConvert dataConvert = (DataConvert) numArray;
            dataConvert.Convert();
            int size1 = 0;
            bool flag4 = false;
            bool flag5 = true;
            while (receive.Length > size1 + begin)
            {
              byte num2 = receive[begin + size1];
              if (!flag5 && num2 == (byte) 10)
              {
                flag1 = true;
                begin += size1;
                if (to == SendTo.Modem)
                {
                  EventLogger.Info((object) "Битый пакет...");
                  break;
                }
                break;
              }
              flag5 = false;
              ++size1;
              if (num2 == (byte) 13 && (size1 == size || receive.Length <= begin + size1 || receive[begin + size1] == (byte) 10))
              {
                flag4 = true;
                break;
              }
            }
            if (flag1)
            {
              flag1 = false;
              batchs -= size1;
            }
            else if (flag4)
            {
              this.SendData(receive, begin, size1, to, (int) dataConvert.sideIdTo, (int) dataConvert.sideIdFrom, (int) dataConvert.linkId, siteId, identifier, batchType);
              ++batchCount;
              batchs -= size1;
              begin += size1;
            }
            else
              break;
          }
          else
            break;
        }
        if (batchs > 0 && batchs < 10000)
        {
          remain = new byte[batchs];
          for (int index = 0; index < batchs; ++index)
            remain[index] = receive[begin + index];
        }
        else
        {
          batchs = 0;
          remain = (byte[]) null;
        }
        return batchs;
      }
      catch (Exception ex)
      {
        batchCount = 0;
        EventLogger.Info((object) ("GetBatchs Error: " + ex.Message));
        return 0;
      }
    }

    public void ParseQueue(
      ReadModemObject state,
      int ReadBytes,
      out int batchCount,
      int siteId,
      string identifier)
    {
      try
      {
        int allSize = 0;
        batchCount = 0;
        if (state.Work.ModemType == ModemType.Legacy)
        {
          byte[] receive = this.RefreshAllBytes(state.Work.RemainSize, state.Work._remain, ReadBytes, state.Buffer, ref allSize);
          state.Work.RemainSize = this.GetBatchs(receive, allSize, SendTo.COM, ref state.Work._remain, ref batchCount, siteId, identifier);
        }
        else
        {
          byte[] receive = this.RefreshAllBytes(state.Work.RemainSizeATSWP, state.Work._remainATSWP, ReadBytes, state.Buffer, ref allSize);
          int remainSize = state.Work.RemainSize;
          state.Work.RemainSizeATSWP = this.GetBatchsATSWP(receive, allSize, SendTo.COM, ref state.Work._remain, ref batchCount, siteId, identifier, ref state.Work._remainATSWP, ref remainSize);
          state.Work.RemainSize = remainSize;
        }
      }
      catch (Exception ex)
      {
        batchCount = 0;
        EventLogger.Info((object) ("ParseQueue Error: " + ex.Message));
      }
    }

    private byte[] RefreshAllBytes(
      int remainSize,
      byte[] remain,
      int extraSize,
      byte[] extra,
      ref int allSize)
    {
      allSize = extraSize + remainSize;
      byte[] numArray = new byte[allSize];
      if (remainSize > 0)
      {
        for (int index = 0; index < remainSize; ++index)
          numArray[index] = remain[index];
      }
      for (int index = 0; index < extraSize; ++index)
        numArray[remainSize + index] = extra[index];
      return numArray;
    }

    public void ReadWrite()
    {
      while (!this._closeFlag)
      {
        ArrayList checkRead = new ArrayList();
        ArrayList checkWrite = new ArrayList();
        ArrayList checkError = new ArrayList();
        Hashtable hashtable = new Hashtable();
        try
        {
          this.ControlModems();
          lock (this._myTable.SyncRoot)
          {
            foreach (DynamicTable dynamicTable in (IEnumerable) this._myTable.Values)
            {
              if (!dynamicTable.Flag)
              {
                checkRead.Add((object) dynamicTable.Client);
                if (dynamicTable.WriteQueue.Count > 0)
                  checkWrite.Add((object) dynamicTable.Client);
              }
              checkError.Add((object) dynamicTable.Client);
              if (dynamicTable.LastWriteTime.AddSeconds((double) this._cleanupTime) <= DateTime.Now && dynamicTable.WriteQueue.Count > 0)
                dynamicTable.WriteQueue.Clear();
            }
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("ReadWriteError (_myTable - fill lists): " + ex.Message));
        }
        if (checkRead.Count == 0)
          checkRead = (ArrayList) null;
        if (checkError.Count == 0)
          checkError = (ArrayList) null;
        if (checkWrite.Count == 0)
          checkWrite = (ArrayList) null;
        if (checkRead == null)
        {
          if (checkError == null)
          {
            if (checkWrite == null)
              goto label_78;
          }
        }
        try
        {
          Socket.Select((IList) checkRead, (IList) checkWrite, (IList) checkError, 1000);
        }
        catch (Exception ex)
        {
          if (this._closeFlag)
            break;
          EventLogger.Info((object) string.Format("ReadWriteError (Socket.Select): {0}", (object) ex));
          Thread.Sleep(100);
          continue;
        }
        Socket socket1 = (Socket) null;
        dynamicTable = (DynamicTable) null;
        if (checkError != null && checkError.Count > 0)
        {
          for (int index = 0; index < checkError.Count; ++index)
          {
            socket1 = (Socket) null;
            dynamicTable = (DynamicTable) null;
            try
            {
              Socket key = checkError[index] as Socket;
              lock (this._myTable.SyncRoot)
              {
                if (this._myTable[(object) key] is DynamicTable dynamicTable)
                {
                  int siteId = 0;
                  lock (this._routingListEx.SyncRoot)
                    siteId = ((Routing) this._routingListEx[(object) dynamicTable.ModemID]).SiteID;
                  this.InsertRecord((DBRecord) new LogRecord(siteId, "", string.Format(Resource.ModemError, (object) dynamicTable.ModemID), LogStatus.Alarm));
                  this.CloseModemConnection(dynamicTable.ModemID);
                }
              }
            }
            catch (Exception ex)
            {
              EventLogger.Info((object) string.Format("ReadWriteError (Error): {0}", (object) ex));
            }
          }
        }
        if (checkWrite != null && checkWrite.Count > 0)
        {
          for (int index = 0; index < checkWrite.Count; ++index)
          {
            if (checkWrite[index] is Socket socket2)
            {
              try
              {
                lock (this._myTable.SyncRoot)
                {
                  if (this._myTable[(object) socket2] is DynamicTable dynamicTable)
                  {
                    if (dynamicTable.WriteQueue.Count != 0)
                    {
                      ModemInfo info = dynamicTable.WriteQueue.Dequeue() as ModemInfo;
                      dynamicTable.LastWriteTime = DateTime.Now;
                      dynamicTable.Flag = true;
                      this._myTable[(object) socket2] = (object) dynamicTable;
                      info.Socket = socket2;
                      if (!ThreadPool.QueueUserWorkItem(new WaitCallback(this.WriteData), (object) new WriteModemInfo(info, dynamicTable.ModemType)))
                        this.SetFalse(socket2);
                    }
                  }
                }
              }
              catch (Exception ex)
              {
                EventLogger.Info((object) ("ReadWriteError (_myTable - write): " + ex.Message));
              }
            }
          }
        }
        if (checkRead != null && checkRead.Count > 0)
        {
          for (int index = 0; index < checkRead.Count; ++index)
          {
            if (checkRead[index] is Socket socket3)
            {
              try
              {
                lock (this._myTable.SyncRoot)
                {
                  if (this._myTable[(object) socket3] is DynamicTable dynamicTable)
                  {
                    if (!dynamicTable.Flag)
                      ((DynamicTable) this._myTable[(object) socket3]).Flag = true;
                    else
                      continue;
                  }
                  else
                    continue;
                }
                if (!ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReadData), (object) new ReadModemObject()
                {
                  Work = dynamicTable
                }))
                  this.SetFalse(socket3);
              }
              catch (Exception ex)
              {
                EventLogger.Info((object) ("ReadWriteError (_myTable - read): " + ex.Message));
              }
            }
          }
        }
label_78:
        Thread.Sleep(100);
      }
    }

    private bool GetSocketConnection(Socket socket, DynamicTable temp)
    {
      if (!this._mbCheckTimeDifference || !socket.Connected)
        return socket.Connected;
      TimeSpan timeSpan = new TimeSpan(0, 0, this._recieveVal / 1000);
      if (DateTime.Now - temp.WriteTime < timeSpan || temp.WriteTime < temp.ReadTime || temp.WriteTime == DateTime.MinValue)
        return socket.Connected;
      timeSpan = new TimeSpan(0, 0, this._timeDifference / 1000);
      return temp.WriteTime - temp.ReadTime < timeSpan;
    }

    private void ControlModems()
    {
      if (!this._mbAutoCloseDisconnected)
        return;
      ArrayList arrayList = new ArrayList();
      lock (this._myTable.SyncRoot)
      {
        foreach (Socket key in (IEnumerable) this._myTable.Keys)
        {
          DynamicTable temp = (DynamicTable) this._myTable[(object) key];
          if (!temp.Flag && !this.GetSocketConnection(key, temp))
            arrayList.Add((object) temp);
        }
      }
      foreach (DynamicTable dynamicTable in arrayList)
        this.CloseModemConnection(dynamicTable.ModemID);
    }

    private void SetTimeForControlConnection(Socket socket, bool mbWrite)
    {
      lock (this._myTable.SyncRoot)
      {
        DynamicTable dynamicTable = (DynamicTable) this._myTable[(object) socket];
        if (mbWrite)
        {
          dynamicTable.WriteTime = DateTime.Now;
          if (!(dynamicTable.ReadTime == DateTime.MinValue))
            return;
          dynamicTable.ReadTime = DateTime.Now;
        }
        else
          dynamicTable.ReadTime = DateTime.Now;
      }
    }

    private void WriteData(object obj)
    {
      if (this._closeFlag)
        return;
      ModemInfo modemInfo = (ModemInfo) null;
      Routing routing = (Routing) null;
      try
      {
        if (!(obj is WriteModemInfo writeModemInfo))
          return;
        modemInfo = writeModemInfo.Info;
        int num = 0;
        byte[] numArray = modemInfo._buffer;
        if (writeModemInfo.Type == ModemType.ATSWP)
          numArray = this.CorrectBatchContent(numArray, writeModemInfo.Info.BatchType);
        string str = string.Empty;
        try
        {
          str = Utils.TranslateModemBatch(numArray, 0, numArray.Length);
          EventLogger.Info((object) string.Format("Отправляется модему {1}: {0}", (object) str, (object) modemInfo.ModemID));
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) string.Format("Ошибка записи лога до отправки модему: {0}", (object) ex.Message));
        }
        while ((num += modemInfo.Socket.Send(numArray, 0, numArray.Length, SocketFlags.None)) != 0)
        {
          if (num >= modemInfo.BufferSize)
            break;
        }
        try
        {
          EventLogger.Info((object) string.Format("Отправлен пакет для модема {1}: {0}", (object) str, (object) modemInfo.ModemID));
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) string.Format("Ошибка записи лога после отправки модему: {0}", (object) ex.Message));
        }
        this.SetTimeForControlConnection(modemInfo.Socket, true);
        this.SetDynamicSt(modemInfo.Socket, 1, numArray.Length, true);
        routing = this.GetRoutingItem(modemInfo.ModemID);
        if (routing == null)
          return;
        int siteIdTo = routing.SiteID;
        int siteIdFrom = -2;
        if (!this.IsCommand(modemInfo.BatchType))
          this.GetConvertData(modemInfo._buffer, 0, out siteIdFrom, out siteIdTo);
        string batch = Utils.TranslateModemBatch(modemInfo._buffer, 0, modemInfo.BufferSize);
        BatchData data = new BatchData(siteIdTo, Direction.получение, DateTime.Now, siteIdFrom, batch);
        this.Save(data, SendTo.Modem, routing.ModemID);
        EventLogger.Info((object) string.Format("Отправлено в окно для модема {1}: {0}", (object) batch, (object) modemInfo.ModemID));
        if (this.IsCommand(modemInfo.BatchType))
          return;
        data = new BatchData(siteIdFrom, Direction.отправка, DateTime.Now, siteIdTo, batch);
        this.Save(data, SendTo.COM, this.GetPortName(siteIdFrom, siteIdTo));
        EventLogger.Info((object) string.Format("Отправлено в окно для порта {1}: {0}", (object) batch, (object) this.GetPortName(siteIdFrom, siteIdTo)));
      }
      catch (Exception ex)
      {
        if (this._closeFlag)
          return;
        EventLogger.Info((object) string.Format("WriteDataError: {0}", (object) ex.Message));
        if (modemInfo == null)
          return;
        if (routing != null)
          this.InsertRecord((DBRecord) new LogRecord(routing.SiteID, "", string.Format(Resource.WriteToModemError, (object) ex.Message), LogStatus.Warning));
        this.CloseModemConnection(modemInfo.ModemID);
        modemInfo = (ModemInfo) null;
      }
      finally
      {
        if (modemInfo != null)
          this.SetFalse(modemInfo.Socket);
      }
    }

    private void ReadData(object obj)
    {
      if (this._closeFlag)
        return;
      state = (ReadModemObject) null;
      Routing routing = (Routing) null;
      try
      {
        if (!(obj is ReadModemObject state))
          return;
        lock (this._routingListEx.SyncRoot)
          routing = this._routingListEx[(object) state.Work.ModemID] as Routing;
        int num = state.Work.Client.Receive(state.Buffer);
        if (num > 0)
        {
          this.SetTimeForControlConnection(state.Work.Client, false);
          int siteId = -1;
          if (routing != null)
            siteId = routing.SiteID;
          state.Work.InTraffic += num;
          int batchCount;
          this.ParseQueue(state, num, out batchCount, siteId, state.Work.ModemID);
          this.SetDynamicSt(state.Work.Client, batchCount, num, false);
          Routing routingItem = this.GetRoutingItem(state.Work.ModemID);
          if (routingItem == null)
            return;
          this.UpdateModem(routingItem.Id, routingItem.ModemID, routingItem.SiteID, routingItem.MbConnect, DateTime.Now, ModemUpdateFlag.LastBatch, (string) null);
        }
        else
        {
          this.CloseModemConnection(state.Work.ModemID);
          state = (ReadModemObject) null;
        }
      }
      catch (Exception ex)
      {
        if (this._closeFlag)
          return;
        EventLogger.Info((object) string.Format("ReadData exception: {0}", (object) ex.Message));
        if (state == null)
          return;
        this.InsertRecord((DBRecord) new LogRecord(routing.SiteID, "", string.Format(Resource.ReadFromModemError, (object) ex.Message), LogStatus.Warning));
        this.CloseModemConnection(state.Work.ModemID);
        state = (ReadModemObject) null;
      }
      finally
      {
        if (state != null)
          this.SetFalse(state.Work.Client);
      }
    }

    private void CloseModemConnection(string modemID)
    {
      try
      {
        lock (this._myTable.SyncRoot)
        {
          if (!(this._tempList[(object) modemID] is Socket temp) || this._myTable[(object) temp] == null)
            return;
          DynamicTable info = this._myTable[(object) temp] as DynamicTable;
          this.InsertRecord((DBRecord) new DynamicRecord(((Routing) this._routingListEx[(object) info.ModemID]).SiteID, info.ConnectionTime, DateTime.Now, info.InBatchCount, info.OutBatchCount, info.InTraffic, info.OutTraffic, this.GetIPAddress(info.Client), info.ModemID, info));
          try
          {
            lock (this._routingListEx.SyncRoot)
            {
              ((Routing) this._routingListEx[(object) info.ModemID]).MbConnect = false;
              this.UpdateModem(((Routing) this._routingListEx[(object) info.ModemID]).Id, info.ModemID, ((Routing) this._routingListEx[(object) info.ModemID]).SiteID, ((Routing) this._routingListEx[(object) info.ModemID]).MbConnect, DateTime.Now, ModemUpdateFlag.Relcase, (string) null);
            }
          }
          catch (Exception ex)
          {
            EventLogger.Info((object) ("CloseModemConnectionError (_routingListEx): " + ex.Message));
          }
          info.Close();
          this._myTable.Remove((object) temp);
          this._tempList.Remove((object) modemID);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("CloseModemConnectionError : " + ex.Message));
      }
    }

    private void StartTCPClientThread()
    {
      try
      {
        if (this._clientThread != null)
          return;
        this._clientThread = new Thread(new ThreadStart(this.TCPModuleMain));
        this._clientThread.Start();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("StartTCPClientThread: {0} - {1}", (object) ex.Message, (object) ex.StackTrace));
      }
    }

    private void TCPModuleMain()
    {
      while (!this._closeFlag)
      {
        lock (this._clientList.SyncRoot)
        {
          foreach (object key in (IEnumerable) this._clientList.Keys)
          {
            try
            {
              TCPIPClient client = this._clientList[key] as TCPIPClient;
              if (client.TcpIP != null)
              {
                if (client.TcpIP.Connected)
                  continue;
              }
              string str = key as string;
              client.Connect(str);
              Routing routingItem = this.GetRoutingItem(str);
              EventLogger.Info((object) string.Format("Подключение {0} прошло успешно", (object) str));
              this.InsertRecord((DBRecord) new LogRecord(client.SiteID, "", Resource.TCPIPConnection, LogStatus.Diagnostic));
              routingItem.MbConnect = true;
              this.UpdateModem(routingItem.Id, routingItem.ModemID, routingItem.SiteID, routingItem.MbConnect, DateTime.Now, ModemUpdateFlag.Connection, (string) null);
              this.InsertRecord((DBRecord) new ModemRecord(routingItem.Id, routingItem.ModemID, routingItem.MbConnect, DateTime.Now, ModemUpdateFlag.Connection, (string) null, routingItem.SiteID));
              DynamicTable dynamicTable = new DynamicTable(client.TcpIP.Client, str, ModemType.Legacy);
              lock (this._myTable.SyncRoot)
              {
                this._tempList[(object) str] = (object) dynamicTable.Client;
                this._myTable[(object) dynamicTable.Client] = (object) dynamicTable;
              }
            }
            catch (Exception ex)
            {
              EventLogger.Info((object) string.Format("TCPModuleMainException ({1}): {0}", (object) ex.Message, key));
              Thread.Sleep(100);
            }
          }
        }
        Thread.Sleep(1000);
      }
    }

    private void AddClientTCPIP(string modemId, int siteId, int id)
    {
      lock (this._clientList.SyncRoot)
        this._clientList[(object) modemId] = (object) new TCPIPClient(siteId);
      this.AddModem(modemId, siteId, id);
    }

    private void DeleteClientTCPIP(string modemId)
    {
      lock (this._clientList.SyncRoot)
      {
        if (!this._clientList.ContainsKey((object) modemId))
          return;
        this._clientList.Remove((object) modemId);
      }
    }

    private void UpdateClientTCPIP(string oldModemId, string modemId, int siteId)
    {
      lock (this._clientList.SyncRoot)
      {
        if (!this._clientList.ContainsKey((object) oldModemId) || string.Equals(oldModemId, modemId))
          return;
        this._clientList.Remove((object) oldModemId);
        this._clientList[(object) modemId] = (object) new TCPIPClient(siteId);
      }
    }

    private void SaveCommand(byte[] data, int siteIdTo, int siteIdFrom, ATSWPBatchType batchType)
    {
      if (!this.IsCommand(batchType))
        return;
      try
      {
        lock (this._routingList.SyncRoot)
        {
          if (!(this._routingList[(object) siteIdFrom] is Routing routing) || !routing.CommandInfo.MbInUse)
            return;
          BatchData data1 = new BatchData(siteIdFrom, StringCommandType.Answer, DateTime.Now, siteIdTo, Utils.TranslateModemBatch(data, 0, data.Length - 1));
          routing.CommandInfo.InsertNewBatch(data1, this._queueSize);
          routing.MonitorInfo.InsertNewBatch(data1, this._queueSize);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("SaveCommandError: {0}", (object) ex.Message));
      }
    }

    private int GetBatchsATSWP(
      byte[] receive,
      int size,
      SendTo to,
      ref byte[] remain,
      ref int batchCount,
      int siteId,
      string identifier,
      ref byte[] remainATSWP,
      ref int remainSize)
    {
      int batchsAtswp = size;
      try
      {
        int index1 = 0;
        List<byte> byteList = new List<byte>();
        while (batchsAtswp >= 7 && receive.Length >= 7)
        {
          if (index1 != receive.Length)
          {
            try
            {
              if (receive[index1] != (byte) 192)
              {
                ++index1;
                continue;
              }
            }
            catch (Exception ex)
            {
              EventLogger.Info((object) ("GetBatchsATSWP Error (receive[begin] != 192): " + ex.Message));
              throw ex;
            }
            int num1 = 6;
            ATSWPBatchType batchType;
            try
            {
              batchType = (ATSWPBatchType) receive[index1 + 1];
              if (batchType == ATSWPBatchType.Config)
                num1 = 4;
            }
            catch
            {
              index1 += 2;
              continue;
            }
            index1 += num1;
            int num2 = 0;
            bool flag = false;
            try
            {
              while (receive.Length > num2 + index1)
              {
                byte num3 = receive[index1 + num2];
                ++num2;
                byteList.Add(num3);
                if (num3 == (byte) 193)
                {
                  flag = true;
                  break;
                }
              }
            }
            catch (Exception ex)
            {
              EventLogger.Info((object) ("GetBatchsATSWP Error (while (receive.Length > (batchSize + begin))): " + ex.Message));
              throw ex;
            }
            if (flag)
            {
              byte[] numArray1 = new byte[byteList.Count];
              byteList.CopyTo(numArray1);
              try
              {
                for (int index2 = num1 - 1; index2 >= 0; --index2)
                  byteList.Insert(0, receive[index1 - num1 + index2]);
              }
              catch (Exception ex)
              {
                EventLogger.Info((object) ("GetBatchsATSWP Error (newBatch.Insert(...)): " + ex.Message));
                throw ex;
              }
              byte[] array = new byte[byteList.Count];
              byteList.CopyTo(array);
              byteList.Clear();
              byte[] numArray2;
              try
              {
                numArray2 = this.ClearByteStaffing(numArray1);
              }
              catch (Exception ex)
              {
                EventLogger.Info((object) ("GetBatchsATSWP Error (ClearByteStaffing(...)): " + ex.Message));
                throw ex;
              }
              int allSize = 0;
              try
              {
                if (this.IsCommand(batchType))
                {
                  this.SaveCommand(numArray2, -3, siteId, batchType);
                }
                else
                {
                  byte[] receive1 = this.RefreshAllBytes(remainSize, remain, numArray2.Length - 1, numArray2, ref allSize);
                  remainSize = this.GetBatchs(receive1, allSize, to, ref remain, ref batchCount, siteId, identifier, batchType);
                }
              }
              catch (Exception ex)
              {
                EventLogger.Info((object) ("GetBatchsATSWP Error (RefreshAllBytes;GetBatchs): " + ex.Message));
                throw ex;
              }
              index1 += num2;
              batchsAtswp = receive.Length - index1;
            }
            else
              break;
          }
          else
            break;
        }
        if (batchsAtswp > 0 && batchsAtswp < 10000)
        {
          int index3 = 0;
          try
          {
            remainATSWP = new byte[batchsAtswp];
            for (index3 = 0; index3 < batchsAtswp; ++index3)
              remainATSWP[index3] = receive[index1 + index3];
          }
          catch (Exception ex)
          {
            EventLogger.Info((object) string.Format("GetBatchsATSWP Error (remainATSWP->begin={0};returnSize={1};recieve.Length={2};i={4}): {3}", (object) index1, (object) batchsAtswp, (object) receive.LongLength, (object) ex.Message, (object) index3));
            throw ex;
          }
        }
        else
        {
          batchsAtswp = 0;
          remainATSWP = (byte[]) null;
        }
      }
      catch (Exception ex)
      {
        batchCount = 0;
        EventLogger.Info((object) ("GetBatchsATSWP Error: " + ex.Message));
        batchsAtswp = 0;
        remainATSWP = (byte[]) null;
      }
      return batchsAtswp;
    }

    private byte[] CorrectBatchContent(byte[] oldBatch, ATSWPBatchType batchType)
    {
      List<byte> byteList = new List<byte>();
      byteList.Add((byte) 192);
      byteList.Add((byte) batchType);
      byteList.AddRange((IEnumerable<byte>) DataConvert.UShortToBytesVV((ushort) oldBatch.Length));
      bool flag = !this.IsCommand(batchType);
      if (flag)
      {
        byteList.Add((byte) 0);
        byteList.Add((byte) 0);
      }
      foreach (byte num in oldBatch)
      {
        if (flag && (num == (byte) 192 || num == (byte) 193 || num == (byte) 195))
        {
          byteList.Add((byte) 195);
          byteList.Add((byte) ((uint) num | 32U));
        }
        else
          byteList.Add(num);
      }
      byteList.Add((byte) 193);
      byte[] bytesVv = DataConvert.UShortToBytesVV((ushort) byteList.Count);
      if (flag)
      {
        byteList[4] = bytesVv[0];
        byteList[5] = bytesVv[1];
      }
      else
      {
        byteList[2] = bytesVv[0];
        byteList[3] = bytesVv[1];
      }
      byte[] array = new byte[byteList.Count];
      byteList.CopyTo(array);
      return array;
    }

    private byte[] ClearByteStaffing(byte[] oldBatch)
    {
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < oldBatch.Length; ++index)
      {
        byte num1 = oldBatch[index];
        if (num1 == (byte) 195)
        {
          ++index;
          byte num2 = oldBatch[index];
          byteList.Add((byte) ((uint) num2 & 223U));
        }
        else
          byteList.Add(num1);
      }
      byte[] array = new byte[byteList.Count];
      byteList.CopyTo(array);
      return array;
    }

    private bool IsCommand(ATSWPBatchType batchType)
    {
      return batchType < ATSWPBatchType.DataUART1 || batchType > ATSWPBatchType.DataUSB;
    }

    private int GetModemSiteID(string modemID)
    {
      int result = -1;
      if (int.TryParse(modemID, out result))
      {
        lock (this._routingList.SyncRoot)
        {
          if (this._routingList.ContainsKey((object) result))
          {
            result = -1;
            this.InsertRecord((DBRecord) new LogRecord(result, "", string.Format(Resource.DifferentModemID, (object) modemID), LogStatus.Warning));
          }
        }
      }
      if (result == 0)
        result = -1;
      return result;
    }

    private Routing GetRoutingItem(string modemID)
    {
      lock (this._routingListEx.SyncRoot)
        return this._routingListEx[(object) modemID] as Routing;
    }

    private Routing GetModemInfo(string modemID)
    {
      Routing routingItem = this.GetRoutingItem(modemID);
      try
      {
        int num = -1;
        if (routingItem == null)
          num = this.GetModemSiteID(modemID);
        if (routingItem == null)
        {
          if (this._mbConnectUnknown)
          {
            lock (this._dontForConnection.SyncRoot)
            {
              if (this._dontForConnection[(object) modemID] != null)
              {
                this.InsertRecord((DBRecord) new LogRecord(-1, "", string.Format(Resource.IllegalModem, (object) modemID), LogStatus.Diagnostic));
                return (Routing) null;
              }
            }
            if (num != -1)
            {
              if (this._mbAskClient)
              {
                lock (this._forConnection.SyncRoot)
                  this._forConnection[(object) modemID] = (object) new NewModems(num, num, modemID);
              }
              else
              {
                this.InsertModemInDataBase(modemID, num);
                routingItem = this.GetRoutingItem(modemID);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetModemInfoError (_dontForConnection): " + ex.Message));
      }
      return routingItem;
    }

    private void InsertModemInDataBase(string modemID, int siteID)
    {
      try
      {
        this.UpdateNewModems(this.InsertModem(modemID, siteID, ConfigType.Modem), siteID, modemID);
        List<NewModems> newModemsList = new List<NewModems>();
        lock (this._forConnection.SyncRoot)
          this._forConnection.Remove((object) modemID);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("InsertModemInDataBaseError: " + ex.Message));
      }
    }

    public int GetConvertData(byte[] buffer, int begin, out int siteIdFrom, out int siteIdTo)
    {
      byte[] numArray = new byte[9];
      int convertData = 0;
      try
      {
        for (int index = 0; index < 9; ++index)
        {
          if (buffer[begin + convertData] != (byte) 16)
          {
            numArray[index] = buffer[begin + convertData];
          }
          else
          {
            ++convertData;
            numArray[index] = buffer[begin + convertData];
            numArray[index] = (byte) ((uint) numArray[index] ^ (uint) byte.MaxValue);
          }
          ++convertData;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetConvertDataError: " + ex.Message));
      }
      DataConvert dataConvert = (DataConvert) numArray;
      dataConvert.Convert();
      siteIdFrom = (int) dataConvert.sideIdFrom;
      siteIdTo = (int) dataConvert.sideIdTo;
      return convertData;
    }

    public string NormalizeIP(string fullIP)
    {
      try
      {
        return fullIP.Remove(fullIP.IndexOf(':'));
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("NormalizeIPError: " + ex.Message));
        return "";
      }
    }

    public string NormalizeNameVS(string str)
    {
      try
      {
        return str.Remove(str.IndexOf(char.MinValue));
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("NormalizeNameVSError: " + ex.Message));
        return str;
      }
    }

    public string GetIPAddress(Socket client)
    {
      try
      {
        return string.Format("{0}:{1}", (object) ((IPEndPoint) client.RemoteEndPoint).Address.ToString(), (object) ((IPEndPoint) client.RemoteEndPoint).Port.ToString());
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetIPAddressError: " + ex.Message));
        return "";
      }
    }

    public void SetFalse(Socket socket)
    {
      try
      {
        lock (this._myTable.SyncRoot)
          ((DynamicTable) this._myTable[(object) socket]).Flag = false;
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("SetFalseError: " + ex.Message));
      }
    }

    public void SetDynamicSt(Socket socket, int batchCount, int traffic, bool mbOut)
    {
      try
      {
        lock (this._myTable.SyncRoot)
        {
          if (mbOut)
          {
            ((DynamicTable) this._myTable[(object) socket]).OutTraffic += traffic;
            ((DynamicTable) this._myTable[(object) socket]).OutBatchCount += batchCount;
          }
          else
          {
            ((DynamicTable) this._myTable[(object) socket]).InTraffic += traffic;
            ((DynamicTable) this._myTable[(object) socket]).InBatchCount += batchCount;
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("SetDynamicStError: " + ex.Message));
      }
    }

    public void ConnectionOff()
    {
      this._closeFlag = true;
      try
      {
        lock (this._listeners.SyncRoot)
        {
          foreach (ListenerArgs args in (IEnumerable) this._listeners.Values)
            this.StopListener(args, false);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ConnectionOffError (_listeners): " + ex.Message));
      }
      try
      {
        lock (this._routingListEx.SyncRoot)
        {
          foreach (Routing routing in (IEnumerable) this._routingListEx.Values)
          {
            if (routing.MbConnect)
            {
              routing.MbConnect = false;
              this.UpdateModem(routing.Id, routing.ModemID, routing.SiteID, false, DateTime.Now, ModemUpdateFlag.Relcase, (string) null);
            }
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ConnectionOffError (_routingListEx): " + ex.Message));
      }
      try
      {
        lock (this._tempList.SyncRoot)
        {
          foreach (Socket socket in (IEnumerable) this._tempList.Values)
          {
            if (socket.Connected)
              socket.Shutdown(SocketShutdown.Both);
            socket.Close();
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ConnectionOffError (_tempList): " + ex.Message));
      }
      try
      {
        lock (this._portList.SyncRoot)
        {
          foreach (PortInfo portInfo in (IEnumerable) this._portList.Values)
            portInfo.Port.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ConnectionOffError (portList): " + ex.Message));
      }
      this.PutAllDBRecords();
    }

    private void AddModem(string modemID, int siteID, int id)
    {
      Routing routing = new Routing();
      routing.ModemID = modemID;
      routing.SiteID = siteID;
      routing.Id = id;
      routing.MbConnect = false;
      try
      {
        lock (this._routingList.SyncRoot)
          this._routingList[(object) routing.SiteID] = (object) routing;
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("AddModemError (_routingList): " + ex.Message));
      }
      try
      {
        lock (this._routingListEx.SyncRoot)
          this._routingListEx[(object) routing.ModemID] = (object) routing;
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("AddModemError (_routingListEx): " + ex.Message));
      }
    }

    private void RemoveModem(string modemID, int siteID)
    {
      string str = modemID;
      try
      {
        this.DeleteClientTCPIP(str);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveModemError (DeleteClientTCPIP): " + ex.Message));
      }
      try
      {
        lock (this._myTable.SyncRoot)
          this.CloseModemConnection(str);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveModemError (CloseModemConnection): " + ex.Message));
      }
      try
      {
        lock (this._routingList.SyncRoot)
          this._routingList.Remove((object) siteID);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveModemError (RoutingList): " + ex.Message));
      }
      try
      {
        lock (this._routingListEx.SyncRoot)
          this._routingListEx.Remove((object) str);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveModemError (RoutingListEx): " + ex.Message));
      }
    }

    private void RemoveAllModems()
    {
      try
      {
        lock (this._tempList.SyncRoot)
        {
          foreach (Socket socket in (IEnumerable) this._tempList.Values)
          {
            if (socket.Connected)
              socket.Shutdown(SocketShutdown.Both);
            socket.Close();
          }
          this._tempList.Clear();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveAllModemsError (tempList): " + ex.Message));
      }
      try
      {
        lock (this._myTable.SyncRoot)
          this._myTable.Clear();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveAllModemsError (myTable): " + ex.Message));
      }
      try
      {
        lock (this._routingList.SyncRoot)
          this._routingList.Clear();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveAllModemsError (RoutingList): " + ex.Message));
      }
      try
      {
        lock (this._routingListEx.SyncRoot)
          this._routingListEx.Clear();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("RemoveAllModemsError (RoutingListEx): " + ex.Message));
      }
    }

    public void ChangeModemInfo(string modemID, int siteID, string oldModemID)
    {
      string key1 = oldModemID;
      string key2 = modemID;
      int key3 = 0;
      Routing routing = (Routing) null;
      try
      {
        lock (this._routingListEx.SyncRoot)
        {
          routing = (Routing) this._routingListEx[(object) key1];
          if (routing != null)
          {
            key3 = routing.SiteID;
            routing.SiteID = siteID;
            routing.ModemID = key2;
            this._routingList.Remove((object) key1);
            this._routingList[(object) key2] = (object) routing;
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ChangeModemInfoError (RoutingListEx): " + ex.Message));
      }
      if (routing != null)
      {
        try
        {
          lock (this._routingListEx.SyncRoot)
          {
            this._routingListEx.Remove((object) key3);
            this._routingListEx[(object) key2] = (object) routing;
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("ChangeModemInfoError (RoutingListEx remove): " + ex.Message));
        }
      }
      Socket key4 = (Socket) null;
      try
      {
        lock (this._tempList.SyncRoot)
        {
          key4 = (Socket) this._tempList[(object) key1];
          if (key4 != null)
          {
            this._tempList.Remove((object) key1);
            this._tempList[(object) key2] = (object) key4;
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ChangeModemInfoError (_tempList): " + ex.Message));
      }
      if (key4 == null)
        return;
      try
      {
        lock (this._myTable.SyncRoot)
        {
          if (!(this._myTable[(object) key4] is DynamicTable dynamicTable))
            return;
          dynamicTable.ModemID = key2;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ChangeModemInfoError (_myTable): " + ex.Message));
      }
    }

    public void Break(string modemId) => this.CloseModemConnection(modemId);

    public void TestConnect(string modemId)
    {
      Socket socket = (Socket) null;
      try
      {
        lock (this._tempList.SyncRoot)
          socket = this._tempList[(object) modemId] as Socket;
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("TestConnectError : " + ex.Message));
      }
      if (socket == null)
        return;
      byte[] buffer = new byte[2]{ (byte) 10, (byte) 13 };
      try
      {
        socket.Send(buffer, SocketFlags.None);
      }
      catch (Exception ex)
      {
        this.CloseModemConnection(modemId);
      }
    }

    public void UpdateLogs(
      int logId,
      int siteID,
      string COMPort,
      DateTime time,
      string message,
      int statusId)
    {
      if (this._machineName == null)
        return;
      NewLogs data = new NewLogs(logId, siteID, COMPort, time, message, statusId);
      try
      {
        lock (this._logList.SyncRoot)
          Utilities.InsertIntoQueue(this._logList, (object) data, this._queueSize);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("UpdateLogsError: " + ex.Message));
      }
    }

    public void UpdateModems(int id, bool mbConnect, DateTime time, ModemUpdateFlag flag)
    {
      if (this._machineName == null || flag == ModemUpdateFlag.None)
        return;
      UpdatedModems data = new UpdatedModems(id, mbConnect, time, flag);
      try
      {
        lock (this._modemList.SyncRoot)
          Utilities.InsertIntoQueue(this._modemList, (object) data, this._queueSize);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("UpdateModemsError: " + ex.Message));
      }
    }

    public void UpdateNewModems(int id, int siteID, string modemID)
    {
      if (this._machineName == null)
        return;
      NewModems data = new NewModems(id, siteID, modemID);
      try
      {
        lock (this._newModemList.SyncRoot)
          Utilities.InsertIntoQueue(this._newModemList, (object) data, this._queueSize);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("UpdateNewModemsError: " + ex.Message));
      }
    }

    public void UpdateRoutingInfo(
      int routingId,
      int siteID,
      int linkID,
      int portId,
      int beginRange,
      int endRange,
      int localSiteId,
      ITMCOMDataSet.RoutingRow row)
    {
      try
      {
        lock (this._tempListCOM.SyncRoot)
        {
          StaticTable staticTable1 = this._tempListCOM[(object) row.SiteID] as StaticTable;
          staticTable1.LocalSiteID = localSiteId;
          if (siteID == row.SiteID)
          {
            foreach (Redistribution portData in staticTable1.PortDataList)
            {
              if (portData.PortId == row.PortId && portData.LinkId == row.LinkID && portData.BeginSiteId == row.BeginRange && portData.EndSiteId == row.EndRange)
              {
                portData.PortId = portId;
                portData.LinkId = linkID;
                portData.BeginSiteId = beginRange;
                portData.EndSiteId = endRange;
                break;
              }
            }
          }
          else
          {
            foreach (Redistribution portData in staticTable1.PortDataList)
            {
              if (portData.PortId == row.PortId && portData.LinkId == row.LinkID && portData.BeginSiteId == row.BeginRange && portData.EndSiteId == row.EndRange)
              {
                staticTable1.PortDataList.Remove(portData);
                break;
              }
            }
            this._tempListCOM.Remove((object) row.SiteID);
            if (!(this._tempListCOM[(object) siteID] is StaticTable staticTable2))
              staticTable2 = new StaticTable(siteID, localSiteId);
            Redistribution redistribution = new Redistribution(beginRange, endRange, linkID, portId);
            staticTable2.PortDataList.Add(redistribution);
            this._tempListCOM[(object) siteID] = (object) staticTable2;
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("UpdateRoutingInfoError: " + ex.Message));
      }
    }

    public void UpdatePortInfo(
      int portId,
      string portName,
      int baudRate,
      int parity,
      int dataBits,
      int stopBits)
    {
      try
      {
        lock (this._portList.SyncRoot)
        {
          PortInfo port = (PortInfo) this._portList[(object) portId];
          if (port == null)
            return;
          if (port.Port.PortName != portName)
          {
            if (port.Port.IsOpen)
              port.Port.Close();
            port.Port.PortName = portName;
          }
          port.Port.BaudRate = baudRate;
          port.Port.Parity = (Parity) parity;
          port.Port.DataBits = dataBits;
          port.Port.StopBits = (StopBits) stopBits;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("UpdatePortInfoError: " + ex.Message));
      }
    }

    public List<BatchData> GetMonitorData(SendTo to, string caption, int index)
    {
      List<BatchData> monitorData = (List<BatchData>) null;
      if (to == SendTo.Modem)
      {
        string key = caption;
        try
        {
          lock (this._routingListEx.SyncRoot)
          {
            if (!(this._routingListEx[(object) key] is Routing routing))
              return (List<BatchData>) null;
            if (routing.MonitorInfo.MbInUse)
            {
              if (routing.MonitorInfo.MonitorQueue.Count > 0)
              {
                monitorData = new List<BatchData>();
                while (routing.MonitorInfo.MonitorQueue.Count > 0)
                  monitorData.Add((BatchData) routing.MonitorInfo.MonitorQueue.Dequeue());
              }
            }
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("GetMonitorDataError(SendTo.Modem): " + ex.Message));
        }
      }
      else
      {
        try
        {
          lock (this._portList.SyncRoot)
          {
            PortInfo port = (PortInfo) this._portList[(object) index];
            if (port == null)
              return (List<BatchData>) null;
            if (port.MonitorInfo.MbInUse)
            {
              if (port.MonitorInfo.MonitorQueue.Count > 0)
              {
                monitorData = new List<BatchData>();
                while (port.MonitorInfo.MonitorQueue.Count > 0)
                  monitorData.Add((BatchData) port.MonitorInfo.MonitorQueue.Dequeue());
              }
            }
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("GetMonitorDataError(SendTo.COM): " + ex.Message));
        }
      }
      return monitorData;
    }

    public bool SetUseMonitor(SendTo to, string caption, int index, bool mbUse)
    {
      if (to == SendTo.Modem)
      {
        string key = caption;
        try
        {
          lock (this._routingListEx.SyncRoot)
          {
            if (!(this._routingListEx[(object) key] is Routing routing))
              return false;
            routing.MonitorInfo.MbInUse = mbUse;
            if (!mbUse)
              routing.MonitorInfo.MonitorQueue.Clear();
            return true;
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("SetUseMonitorError(SendTo.Modem): " + ex.Message));
        }
      }
      else
      {
        try
        {
          lock (this._portList.SyncRoot)
          {
            PortInfo port = (PortInfo) this._portList[(object) index];
            if (port == null)
              return false;
            port.MonitorInfo.MbInUse = mbUse;
            if (!mbUse)
              port.MonitorInfo.MonitorQueue.Clear();
            return true;
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("SetUseMonitorError(SendTo.COM): " + ex.Message));
        }
      }
      return false;
    }

    public List<NewLogs> GetLogsData()
    {
      List<NewLogs> logsData = new List<NewLogs>();
      try
      {
        lock (this._logList.SyncRoot)
        {
          if (this._logList.Count == 0)
            return (List<NewLogs>) null;
          while (this._logList.Count > 0)
            logsData.Add((NewLogs) this._logList.Dequeue());
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetLogsDataError: " + ex.Message));
      }
      return logsData;
    }

    public List<UpdatedModems> GetModemsData()
    {
      List<UpdatedModems> modemsData = new List<UpdatedModems>();
      try
      {
        lock (this._modemList.SyncRoot)
        {
          while (this._modemList.Count > 0)
            modemsData.Add((UpdatedModems) this._modemList.Dequeue());
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetModemsDataError: " + ex.Message));
      }
      return modemsData;
    }

    public List<NewModems> GetNewModems()
    {
      List<NewModems> newModems = new List<NewModems>();
      try
      {
        lock (this._newModemList.SyncRoot)
        {
          while (this._newModemList.Count > 0)
            newModems.Add((NewModems) this._newModemList.Dequeue());
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetNewModemsError: " + ex.Message));
      }
      return newModems;
    }

    public List<NewModems> GetModemsQuery()
    {
      List<NewModems> modemsQuery = new List<NewModems>();
      try
      {
        lock (this._forConnection.SyncRoot)
        {
          foreach (NewModems newModems in (IEnumerable) this._forConnection.Values)
            modemsQuery.Add(newModems);
          this._forConnection.Clear();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetModemsQuerError: " + ex.Message));
      }
      return modemsQuery;
    }

    public void SetDontForConnection(List<NewModems> data)
    {
      try
      {
        lock (this._dontForConnection.SyncRoot)
        {
          foreach (NewModems newModems in data)
          {
            this._dontForConnection[(object) newModems.ModemID] = (object) newModems.SiteID;
            this.InsertIllegalModem(newModems.ModemID, newModems.SiteID);
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("SetDontForConnectionError: " + ex.Message));
      }
    }

    public void SetConnection(List<NewModems> data)
    {
      foreach (NewModems newModems in data)
      {
        string modemId = newModems.ModemID;
        if (this.GetRoutingItem(modemId) == null)
          this.InsertModemInDataBase(modemId, newModems.SiteID);
      }
    }

    public string DoCommand(
      byte[] data,
      int begin,
      int size,
      string identifier,
      ATSWPBatchType batchType)
    {
      try
      {
        ModemInfo modemInfo = new ModemInfo();
        modemInfo.BufferSize = size;
        modemInfo.ModemID = identifier;
        modemInfo.BatchType = batchType;
        modemInfo._buffer = new byte[size];
        for (int index = 0; index < modemInfo.BufferSize; ++index)
          modemInfo._buffer[index] = data[index + begin];
        Socket key = (Socket) null;
        lock (this._tempList.SyncRoot)
          key = this._tempList[(object) modemInfo.ModemID] as Socket;
        if (key == null)
          return Resource.CommandError + Resource.NoModemConnection;
        lock (this._myTable.SyncRoot)
        {
          if (!(this._myTable[(object) key] is DynamicTable dynamicTable))
            return Resource.DoCommand + Resource.Error;
          if (dynamicTable.ModemType == ModemType.Legacy)
            return Resource.CommandError + Resource.ModemProtocolError;
          if (dynamicTable.WriteQueue.Count == 0)
            dynamicTable.LastWriteTime = DateTime.Now;
          dynamicTable.WriteQueue.Enqueue((object) modemInfo);
          this._myTable[(object) key] = (object) dynamicTable;
          return (string) null;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("DoCommand Error: " + ex.Message));
        return Resource.CommandError + ex.Message;
      }
    }

    public bool GetUseCommand(string modemID, int siteId)
    {
      try
      {
        lock (this._myTable.SyncRoot)
          return this._tempList[(object) modemID] is Socket temp && this._myTable[(object) temp] is DynamicTable dynamicTable && dynamicTable.ModemType == ModemType.ATSWP;
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetUseCommand Error: " + ex.Message));
      }
      return false;
    }

    public bool SetUseCommand(string modemID, int siteId, bool mbUse)
    {
      try
      {
        lock (this._routingListEx.SyncRoot)
        {
          if (!(this._routingListEx[(object) modemID] is Routing routing))
            return false;
          routing.CommandInfo.MbInUse = mbUse;
          if (!mbUse)
            routing.CommandInfo.MonitorQueue.Clear();
          return true;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("SetUseCommandError: " + ex.Message));
      }
      return false;
    }

    public List<BatchData> GetCommandData(string caption, int index)
    {
      List<BatchData> commandData = (List<BatchData>) null;
      string key = caption;
      try
      {
        lock (this._routingListEx.SyncRoot)
        {
          if (!(this._routingListEx[(object) key] is Routing routing))
            return (List<BatchData>) null;
          if (routing.CommandInfo.MbInUse)
          {
            if (routing.CommandInfo.MonitorQueue.Count > 0)
            {
              commandData = new List<BatchData>();
              while (routing.CommandInfo.MonitorQueue.Count > 0)
                commandData.Add((BatchData) routing.CommandInfo.MonitorQueue.Dequeue());
            }
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("GetCommandData: " + ex.Message));
      }
      return commandData;
    }

    public ITMCOMDataSet GetDataSet()
    {
      this.GetDataFromDB(false);
      return this._mainDataSet;
    }

    public void GetDataFromDB(bool mbWait)
    {
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      while (true)
      {
        try
        {
          using (SqlConnection sqlConnection = new SqlConnection(connectionString))
          {
            if (mbWait)
              EventLogger.Info((object) "Сервер вышел на связь...");
            else
              EventLogger.Info((object) "Клиент запрашивает базу");
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            SqlCommand sqlCommand = new SqlCommand();
            this._mainDataSet.Clear();
            this._mainDataSet.EnforceConstraints = false;
            sqlCommand.CommandText = "EXECUTE dbo.SelectModem;";
            sqlCommand.Connection = sqlConnection;
            sqlDataAdapter.SelectCommand = sqlCommand;
            sqlConnection.Open();
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.Modem);
            EventLogger.Info((object) string.Format("Modem.Rows.Count: {0}", (object) this._mainDataSet.Modem.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectTCPIP;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.TCPIP);
            EventLogger.Info((object) string.Format("TCPIP.Rows.Count: {0}", (object) this._mainDataSet.TCPIP.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectDynamicTCPIP;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.DynamicTCPIP);
            EventLogger.Info((object) string.Format("DynamicTCPIP.Rows.Count: {0}", (object) this._mainDataSet.DynamicTCPIP.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectTCPIPClient;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.TCPIPClient);
            EventLogger.Info((object) string.Format("TCPIPClient.Rows.Count: {0}", (object) this._mainDataSet.TCPIPClient.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectDynamicTCPIPClient;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.DynamicTCPIPClient);
            EventLogger.Info((object) string.Format("DynamicTCPIPClient.Rows.Count: {0}", (object) this._mainDataSet.DynamicTCPIPClient.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "SELECT [IllegalModemId],[SiteID],[ModemID] FROM [IllegalModem]";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.IllegalModem);
            EventLogger.Info((object) string.Format("IllegalModem.Rows.Count: {0}", (object) this._mainDataSet.IllegalModem.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectPort;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.Port);
            EventLogger.Info((object) string.Format("Port.Rows.Count: {0}", (object) this._mainDataSet.Port.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectDynamic;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.Dynamic);
            EventLogger.Info((object) string.Format("Dynamic.Rows.Count: {0}", (object) this._mainDataSet.Dynamic.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectLogStatus;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.LogStatus);
            EventLogger.Info((object) string.Format("LogStatus.Rows.Count: {0}", (object) this._mainDataSet.LogStatus.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectRouting;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.Routing);
            EventLogger.Info((object) string.Format("Routing.Rows.Count: {0}", (object) this._mainDataSet.Routing.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectStatic;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.Static);
            EventLogger.Info((object) string.Format("Static.Rows.Count: {0}", (object) this._mainDataSet.Static.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "SELECT [ConfigTypeId],[Name] FROM [ConfigType]";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.ConfigType);
            EventLogger.Info((object) string.Format("ConfigType.Rows.Count: {0}", (object) this._mainDataSet.ConfigType.Rows.Count.ToString()));
            sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectConfig;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.Config);
            EventLogger.Info((object) string.Format("Config.Rows.Count: {0}", (object) this._mainDataSet.Config.Rows.Count.ToString()));
            if (mbWait)
            {
              sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.InsertLog @SiteID, @COMPort, @Message, @SatusId, \r\n\t\t\t\t\t\t\t\t@Time, @RecordsCount; EXECUTE dbo.SelectLog;";
              sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
              sqlCommand.Parameters[0].Value = (object) DBNull.Value;
              sqlCommand.Parameters.Add("@COMPort", SqlDbType.VarChar, 20, "COMPort");
              sqlCommand.Parameters[1].Value = (object) "";
              sqlCommand.Parameters.Add("@Message", SqlDbType.VarChar, 1024, "Message");
              sqlCommand.Parameters[2].Value = (object) Resource.StartServer;
              sqlCommand.Parameters.Add("@SatusId", SqlDbType.Int, 4, "SatusId");
              sqlCommand.Parameters[3].Value = (object) 1;
              sqlCommand.Parameters.Add("@Time", SqlDbType.DateTime, 8, "Time");
              sqlCommand.Parameters[4].Value = (object) DateTime.Now;
              sqlCommand.Parameters.Add("@RecordsCount", SqlDbType.Int, 4, "RecordsCount");
              sqlCommand.Parameters[5].Value = (object) this._logRecordsCount;
            }
            else
              sqlDataAdapter.SelectCommand.CommandText = "EXECUTE dbo.SelectLog;";
            sqlDataAdapter.Fill((DataTable) this._mainDataSet.Log);
            EventLogger.Info((object) string.Format("Log.Rows.Count: {0}", (object) this._mainDataSet.Log.Rows.Count.ToString()));
            sqlConnection.Close();
            this._mainDataSet.EnforceConstraints = true;
            break;
          }
        }
        catch (SqlException ex)
        {
          EventLogger.Info((object) string.Format("SqlException: {0}", (object) ex.Message));
          if (!mbWait)
            break;
          Thread.Sleep(100);
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) string.Format("DBException: {0}:{1}", (object) ex.Message, (object) ex.StackTrace));
          break;
        }
      }
    }

    public ITMCOMDataSet SelectConfig()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.SelectConfig;";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Config);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException: {0}", (object) ex.Message));
      }
      return itmcomDataSet;
    }

    public bool DeleteAllConfig()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "DELETE FROM Config";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Config);
          sqlConnection.Close();
          this.DeleteAllListeners();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Config): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool DeleteConfig(int сonfigId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.DeleteConfig @ConfigId;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@ConfigId", SqlDbType.Int, 4, "ConfigId");
          sqlCommand.Parameters[0].Value = (object) сonfigId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Config);
          sqlConnection.Close();
          this.DeleteListener(сonfigId);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Config): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public int InsertConfig(
      string iPAddress,
      int portNumber,
      string serverID,
      bool mbCurrent,
      int typeId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.InsertConfig @IPAddress, @PortNumber, @ServerID, \r\n\t\t\t\t\t\t@mbCurrent, @TypeId; EXECUTE dbo.SelectConfig;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@IPAddress", SqlDbType.VarChar, 20, "IPAddress");
          sqlCommand.Parameters[0].Value = (object) iPAddress;
          sqlCommand.Parameters.Add("@PortNumber", SqlDbType.Int, 4, "PortNumber");
          sqlCommand.Parameters[1].Value = (object) portNumber;
          sqlCommand.Parameters.Add("@ServerID", SqlDbType.VarChar, 20, "ServerID");
          sqlCommand.Parameters[2].Value = (object) serverID;
          if (serverID == null)
            sqlCommand.Parameters[2].Value = (object) "";
          sqlCommand.Parameters.Add("@mbCurrent", SqlDbType.Bit, 1, nameof (mbCurrent));
          sqlCommand.Parameters[3].Value = (object) mbCurrent;
          sqlCommand.Parameters.Add("@TypeId", SqlDbType.Int, 4, "TypeId");
          sqlCommand.Parameters[4].Value = (object) typeId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Config);
          sqlConnection.Close();
          if (itmcomDataSet.Config.Rows.Count > 0)
          {
            if (mbCurrent)
              this.StartListThread(IPAddress.Parse(iPAddress), portNumber, serverID, (ConfigType) typeId, (int) itmcomDataSet.Config.Rows[itmcomDataSet.Config.Rows.Count - 1]["ConfigId"]);
            return (int) itmcomDataSet.Config.Rows[itmcomDataSet.Config.Rows.Count - 1]["ConfigId"];
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (insert into Config): {0}", (object) ex.Message));
      }
      return -1;
    }

    public bool UpdateConfig(
      int configId,
      string iPAddress,
      int portNumber,
      string serverID,
      bool mbCurrent,
      int typeId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.UpdateConfig @ConfigId, @IPAddress, @PortNumber, \r\n\t\t\t\t\t\t@ServerID, @mbCurrent, @TypeId;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@ConfigId", SqlDbType.Int, 4, "ConfigId");
          sqlCommand.Parameters[0].Value = (object) configId;
          sqlCommand.Parameters.Add("@IPAddress", SqlDbType.VarChar, 20, "IPAddress");
          sqlCommand.Parameters[1].Value = (object) iPAddress;
          sqlCommand.Parameters.Add("@PortNumber", SqlDbType.Int, 4, "PortNumber");
          sqlCommand.Parameters[2].Value = (object) portNumber;
          sqlCommand.Parameters.Add("@ServerID", SqlDbType.VarChar, 20, "ServerID");
          sqlCommand.Parameters[3].Value = (object) serverID;
          sqlCommand.Parameters.Add("@mbCurrent", SqlDbType.Bit, 1, nameof (mbCurrent));
          sqlCommand.Parameters[4].Value = (object) mbCurrent;
          sqlCommand.Parameters.Add("@TypeId", SqlDbType.Int, 4, "TypeId");
          sqlCommand.Parameters[5].Value = (object) typeId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Config);
          sqlConnection.Close();
          if (mbCurrent)
            this.ChangeListener(IPAddress.Parse(iPAddress), portNumber, serverID, (ConfigType) typeId, configId);
          else
            this.DeleteListener(configId);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (update Config): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public ITMCOMDataSet SelectDynamic()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.SelectDynamic;";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Dynamic);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException: {0}", (object) ex.Message));
      }
      return itmcomDataSet;
    }

    public bool DeleteDynamic(int dynamicId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.DeleteDynamic @DynamicId;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@DynamicId", SqlDbType.Int, 4, "DynamicId");
          sqlCommand.Parameters[0].Value = (object) dynamicId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Dynamic);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Dynamic): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool DeleteAllDynamics()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "DELETE FROM Dynamic; DELETE FROM Statistics WHERE StatisticsId NOT IN (SELECT StatisticId FROM Static);";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Dynamic);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Dynamic): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public int InsertDynamic(
      int siteID,
      string modemIP,
      string modemID,
      DateTime connectionTime,
      DateTime releaseTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int outTraffic)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      int num = 0;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.InsertDynamic @SiteID, @ModemIP, @ModemID, @ConnectionTime,\r\n\t\t\t\t\t\t@ReleaseTime, @InBatchCount, @OutBatchCount, @InTraffic, @OutTraffic, @label, @RecordsCount; EXECUTE dbo.SelectDynamic;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
          sqlCommand.Parameters[0].Value = (object) siteID;
          sqlCommand.Parameters.Add("@ModemIP", SqlDbType.VarChar, 20, "ModemIP");
          sqlCommand.Parameters[1].Value = (object) modemIP;
          sqlCommand.Parameters.Add("@ModemID", SqlDbType.VarChar, 20, "ModemID");
          sqlCommand.Parameters[2].Value = (object) modemID;
          sqlCommand.Parameters.Add("@ConnectionTime", SqlDbType.DateTime, 8, "ConnectionTime");
          sqlCommand.Parameters[3].Value = (object) connectionTime;
          sqlCommand.Parameters.Add("@InBatchCount", SqlDbType.Int, 4, "InBatchCount");
          sqlCommand.Parameters[4].Value = (object) inBatchCount;
          sqlCommand.Parameters.Add("@OutBatchCount", SqlDbType.Int, 4, "OutBatchCount");
          sqlCommand.Parameters[5].Value = (object) outBatchCount;
          sqlCommand.Parameters.Add("@InTraffic", SqlDbType.Int, 4, "InTraffic");
          sqlCommand.Parameters[6].Value = (object) inTraffic;
          sqlCommand.Parameters.Add("@OutTraffic", SqlDbType.Int, 4, "OutTraffic");
          sqlCommand.Parameters[7].Value = (object) outTraffic;
          sqlCommand.Parameters.Add("@ReleaseTime", SqlDbType.DateTime, 8, "ReleaseTime");
          sqlCommand.Parameters[8].Value = (object) releaseTime;
          sqlCommand.Parameters.Add("@label", SqlDbType.DateTime, 8, "ReleaseTime");
          DateTime dateTime = new DateTime(releaseTime.Year, releaseTime.Month, releaseTime.Day, releaseTime.Hour, 0, 0, releaseTime.Millisecond);
          sqlCommand.Parameters[9].Value = (object) dateTime;
          sqlCommand.Parameters.Add("@RecordsCount", SqlDbType.Int, 4, "OutTraffic");
          sqlCommand.Parameters[10].Value = (object) this._statisticHoursCount;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Dynamic);
          sqlConnection.Close();
          if (itmcomDataSet.Dynamic.Rows.Count > 0)
            num = (int) itmcomDataSet.Dynamic.Rows[itmcomDataSet.Dynamic.Rows.Count - 1]["DynamicId"];
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (insert into Dynamic): {0}", (object) ex.Message));
      }
      return num;
    }

    public bool UpdateDynamic(
      int dynamicId,
      int siteID,
      string modemIP,
      string modemID,
      DateTime connectionTime,
      DateTime releaseTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int outTraffic)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.UpdateDynamic @DynamicId, @SiteID, @ModemIP, @ModemID, @ConnectionTime,\r\n\t\t\t\t\t\t@ReleaseTime, @InBatchCount, @OutBatchCount, @InTraffic, @OutTraffic;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@DynamicId", SqlDbType.Int, 4, "DynamicId");
          sqlCommand.Parameters[0].Value = (object) dynamicId;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
          sqlCommand.Parameters[1].Value = (object) siteID;
          sqlCommand.Parameters.Add("@ModemIP", SqlDbType.VarChar, 20, "ModemIP");
          sqlCommand.Parameters[2].Value = (object) modemIP;
          sqlCommand.Parameters.Add("@ModemID", SqlDbType.VarChar, 20, "ModemID");
          sqlCommand.Parameters[3].Value = (object) modemID;
          sqlCommand.Parameters.Add("@ConnectionTime", SqlDbType.DateTime, 8, "ConnectionTime");
          sqlCommand.Parameters[4].Value = (object) connectionTime;
          sqlCommand.Parameters.Add("@ReleaseTime", SqlDbType.DateTime, 8, "ReleaseTime");
          sqlCommand.Parameters[5].Value = (object) releaseTime;
          sqlCommand.Parameters.Add("@InBatchCount", SqlDbType.Int, 4, "InBatchCount");
          sqlCommand.Parameters[6].Value = (object) inBatchCount;
          sqlCommand.Parameters.Add("@OutBatchCount", SqlDbType.Int, 4, "OutBatchCount");
          sqlCommand.Parameters[7].Value = (object) outBatchCount;
          sqlCommand.Parameters.Add("@InTraffic", SqlDbType.Int, 4, "InTraffic");
          sqlCommand.Parameters[8].Value = (object) inTraffic;
          sqlCommand.Parameters.Add("@OutTraffic", SqlDbType.Int, 4, "OutTraffic");
          sqlCommand.Parameters[9].Value = (object) outTraffic;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Dynamic);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (update Dynamic): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public ITMCOMDataSet SelectStatic()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.SelectStatic;";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Static);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException: {0}", (object) ex.Message));
      }
      return itmcomDataSet;
    }

    public bool DeleteStatic(int staticId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.DeleteStatic @StaticId;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@StaticId", SqlDbType.Int, 4, "StaticId");
          sqlCommand.Parameters[0].Value = (object) staticId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Static);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Static): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool DeleteAllStatics()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "DELETE FROM [Static]; DELETE FROM Statistics WHERE StatisticsId NOT IN (SELECT StatisticId FROM Dynamic)";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Static);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Static): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public int InsertStatic(
      string COMPort,
      DateTime connectionTime,
      DateTime releaseTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int outTraffic)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.InsertStatic @COMPort, @ConnectionTime, @ReleaseTime, \r\n\t\t\t\t\t\t@InBatchCount, @OutBatchCount, @InTraffic, @OutTraffic, @label, @RecordsCount; EXECUTE dbo.SelectStatic;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@COMPort", SqlDbType.VarChar, 20, nameof (COMPort));
          sqlCommand.Parameters[0].Value = (object) COMPort;
          sqlCommand.Parameters.Add("@ConnectionTime", SqlDbType.DateTime, 8, "ConnectionTime");
          sqlCommand.Parameters[1].Value = (object) connectionTime;
          sqlCommand.Parameters.Add("@InBatchCount", SqlDbType.Int, 4, "InBatchCount");
          sqlCommand.Parameters[2].Value = (object) inBatchCount;
          sqlCommand.Parameters.Add("@OutBatchCount", SqlDbType.Int, 4, "OutBatchCount");
          sqlCommand.Parameters[3].Value = (object) outBatchCount;
          sqlCommand.Parameters.Add("@InTraffic", SqlDbType.Int, 4, "InTraffic");
          sqlCommand.Parameters[4].Value = (object) inTraffic;
          sqlCommand.Parameters.Add("@OutTraffic", SqlDbType.Int, 4, "OutTraffic");
          sqlCommand.Parameters[5].Value = (object) outTraffic;
          sqlCommand.Parameters.Add("@ReleaseTime", SqlDbType.DateTime, 8, "ReleaseTime");
          sqlCommand.Parameters[6].Value = (object) releaseTime;
          sqlCommand.Parameters.Add("@label", SqlDbType.DateTime, 8, "ReleaseTime");
          DateTime dateTime = new DateTime(releaseTime.Year, releaseTime.Month, releaseTime.Day, releaseTime.Hour, 0, 0, releaseTime.Millisecond);
          sqlCommand.Parameters[7].Value = (object) dateTime;
          sqlCommand.Parameters.Add("@RecordsCount", SqlDbType.Int, 4, "OutTraffic");
          sqlCommand.Parameters[8].Value = (object) this._statisticHoursCount;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Static);
          sqlConnection.Close();
          if (itmcomDataSet.Static.Rows.Count > 0)
          {
            int num = (int) itmcomDataSet.Static.Rows[itmcomDataSet.Static.Rows.Count - 1]["StaticId"];
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (insert into Static): {0}", (object) ex.Message));
      }
      return 0;
    }

    public bool UpdateStatic(
      int staticId,
      string COMPort,
      DateTime connectionTime,
      DateTime releaseTime,
      int inBatchCount,
      int outBatchCount,
      int inTraffic,
      int outTraffic)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.UpdateStatic @StaticId, @COMPort, @ConnectionTime, \r\n\t\t\t\t\t\t@ReleaseTime, @InBatchCount, @OutBatchCount, @InTraffic, @OutTraffic;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@StaticId", SqlDbType.Int, 4, "StaticId");
          sqlCommand.Parameters[0].Value = (object) staticId;
          sqlCommand.Parameters.Add("@COMPort", SqlDbType.VarChar, 20, nameof (COMPort));
          sqlCommand.Parameters[1].Value = (object) COMPort;
          sqlCommand.Parameters.Add("@ConnectionTime", SqlDbType.DateTime, 8, "ConnectionTime");
          sqlCommand.Parameters[2].Value = (object) connectionTime;
          sqlCommand.Parameters.Add("@ReleaseTime", SqlDbType.DateTime, 8, "ReleaseTime");
          sqlCommand.Parameters[3].Value = (object) releaseTime;
          sqlCommand.Parameters.Add("@InBatchCount", SqlDbType.Int, 4, "InBatchCount");
          sqlCommand.Parameters[4].Value = (object) inBatchCount;
          sqlCommand.Parameters.Add("@OutBatchCount", SqlDbType.Int, 4, "OutBatchCount");
          sqlCommand.Parameters[5].Value = (object) outBatchCount;
          sqlCommand.Parameters.Add("@InTraffic", SqlDbType.Int, 4, "InTraffic");
          sqlCommand.Parameters[6].Value = (object) inTraffic;
          sqlCommand.Parameters.Add("@OutTraffic", SqlDbType.Int, 4, "OutTraffic");
          sqlCommand.Parameters[7].Value = (object) outTraffic;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Static);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (update Static): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public ITMCOMDataSet SelectRouting()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.SelectRouting;";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Routing);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException: {0}", (object) ex.Message));
      }
      return itmcomDataSet;
    }

    private ITMCOMDataSet.RoutingRow GetRoutingRow(int routingId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = string.Format("SELECT RoutingId, \r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t SiteID, \r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t LinkID, \r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t PortId,\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t BeginRange,\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t EndRange,\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t LocalSiteId\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t  FROM [Routing] WHERE RoutingId={0}", (object) routingId.ToString());
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Routing);
          sqlConnection.Close();
          if (itmcomDataSet.Routing.Rows.Count > 0)
            return (ITMCOMDataSet.RoutingRow) itmcomDataSet.Routing.Rows[0];
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException: {0}", (object) ex.Message));
      }
      return (ITMCOMDataSet.RoutingRow) null;
    }

    public bool DeleteRouting(int routingId, int siteID)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.DeleteRouting @RoutingId;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@RoutingId", SqlDbType.Int, 4, "RoutingId");
          sqlCommand.Parameters[0].Value = (object) routingId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Routing);
          sqlConnection.Close();
          this.RemoveRouting(siteID);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Routing): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool DeleteAllRouting()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "DELETE FROM [Routing]";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Routing);
          sqlConnection.Close();
          this.RemoveAllRouting();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Routing): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public int InsertRouting(
      int siteID,
      int linkID,
      int portId,
      int beginRange,
      int endRange,
      int localSiteId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.InsertRouting @SiteID, @LinkID, @PortId, @BeginRange, \r\n\t\t\t\t\t\t@EndRange, @LocalSiteId; EXECUTE dbo.SelectRouting;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
          sqlCommand.Parameters[0].Value = (object) siteID;
          sqlCommand.Parameters.Add("@LinkID", SqlDbType.Int, 4, "LinkID");
          sqlCommand.Parameters[1].Value = (object) linkID;
          sqlCommand.Parameters.Add("@PortId", SqlDbType.Int, 4, "PortId");
          sqlCommand.Parameters[2].Value = (object) portId;
          sqlCommand.Parameters.Add("@BeginRange", SqlDbType.Int, 4, "BeginRange");
          sqlCommand.Parameters[3].Value = (object) beginRange;
          sqlCommand.Parameters.Add("@EndRange", SqlDbType.Int, 4, "EndRange");
          sqlCommand.Parameters[4].Value = (object) endRange;
          sqlCommand.Parameters.Add("@LocalSiteId", SqlDbType.Int, 4, "LocalSiteId");
          sqlCommand.Parameters[5].Value = (object) localSiteId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Routing);
          sqlConnection.Close();
          this.AddNewRouting(siteID, linkID, portId, beginRange, endRange, localSiteId);
          if (itmcomDataSet.Routing.Rows.Count > 0)
            return (int) itmcomDataSet.Routing.Rows[itmcomDataSet.Routing.Rows.Count - 1]["RoutingId"];
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (insert into Routing): {0}", (object) ex.Message));
      }
      return -1;
    }

    public bool UpdateRouting(
      int routingId,
      int siteID,
      int linkID,
      int portId,
      int beginRange,
      int endRange,
      int localSiteId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      ITMCOMDataSet.RoutingRow routingRow = this.GetRoutingRow(routingId);
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.UpdateRouting @RoutingId, @SiteID, @LinkID, @PortId, \r\n\t\t\t\t\t\t@BeginRange, @EndRange, @LocalSiteId;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@RoutingId", SqlDbType.Int, 4, "RoutingId");
          sqlCommand.Parameters[0].Value = (object) routingId;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
          sqlCommand.Parameters[1].Value = (object) siteID;
          sqlCommand.Parameters.Add("@LinkID", SqlDbType.Int, 4, "LinkID");
          sqlCommand.Parameters[2].Value = (object) linkID;
          sqlCommand.Parameters.Add("@PortId", SqlDbType.Int, 4, "PortId");
          sqlCommand.Parameters[3].Value = (object) portId;
          sqlCommand.Parameters.Add("@BeginRange", SqlDbType.Int, 4, "BeginRange");
          sqlCommand.Parameters[4].Value = (object) beginRange;
          sqlCommand.Parameters.Add("@EndRange", SqlDbType.Int, 4, "EndRange");
          sqlCommand.Parameters[5].Value = (object) endRange;
          sqlCommand.Parameters.Add("@LocalSiteId", SqlDbType.Int, 4, "LocalSiteId");
          sqlCommand.Parameters[6].Value = (object) localSiteId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Routing);
          sqlConnection.Close();
          if (routingRow != null)
            this.UpdateRoutingInfo(routingId, siteID, linkID, portId, beginRange, endRange, localSiteId, routingRow);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (update Routing): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public ITMCOMDataSet SelectPort()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.SelectPort;";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Port);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException: {0}", (object) ex.Message));
      }
      return itmcomDataSet;
    }

    public bool DeletePort(int portId, string portName)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.DeletePort @PortId;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@PortId", SqlDbType.Int, 4, "PortId");
          sqlCommand.Parameters[0].Value = (object) portId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Port);
          sqlConnection.Close();
          this.RemovePort(portId, portName);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Ports): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool DeleteAllPorts()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "DELETE FROM [Ports]; DELETE FROM [Static]; DELETE FROM Statistics WHERE StatisticsId NOT IN (SELECT StatisticId FROM Dynamic);";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Port);
          sqlConnection.Close();
          this.RemoveAllPorts();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Ports): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public int InsertPort(string portName, int baudRate, int parity, int dataBits, int stopBits)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.InsertPort @PortName, @BaudRate, @Parity, @DataBits, \r\n\t\t\t\t\t\t@StopBits; EXECUTE dbo.SelectPort;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@PortName", SqlDbType.VarChar, 20, "PortName");
          sqlCommand.Parameters[0].Value = (object) portName;
          sqlCommand.Parameters.Add("@BaudRate", SqlDbType.Int, 4, "BaudRate");
          sqlCommand.Parameters[1].Value = (object) baudRate;
          sqlCommand.Parameters.Add("@Parity", SqlDbType.Int, 4, "Parity");
          sqlCommand.Parameters[2].Value = (object) parity;
          sqlCommand.Parameters.Add("@DataBits", SqlDbType.Int, 4, "DataBits");
          sqlCommand.Parameters[3].Value = (object) dataBits;
          sqlCommand.Parameters.Add("@StopBits", SqlDbType.Int, 4, "StopBits");
          sqlCommand.Parameters[4].Value = (object) stopBits;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Port);
          sqlConnection.Close();
          if (itmcomDataSet.Port.Rows.Count > 0)
          {
            this.AddNewPort((int) itmcomDataSet.Port.Rows[itmcomDataSet.Port.Rows.Count - 1]["PortId"], portName, baudRate, parity, dataBits, stopBits);
            return (int) itmcomDataSet.Port.Rows[itmcomDataSet.Port.Rows.Count - 1]["PortId"];
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (insert into Port): {0}", (object) ex.Message));
      }
      return -1;
    }

    public bool UpdatePort(
      int portId,
      string portName,
      int baudRate,
      int parity,
      int dataBits,
      int stopBits)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.UpdatePort @PortId, @PortName, @BaudRate, @Parity, @DataBits, @StopBits;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@PortId", SqlDbType.Int, 4, "PortId");
          sqlCommand.Parameters[0].Value = (object) portId;
          sqlCommand.Parameters.Add("@PortName", SqlDbType.VarChar, 20, "PortName");
          sqlCommand.Parameters[1].Value = (object) portName;
          sqlCommand.Parameters.Add("@BaudRate", SqlDbType.Int, 4, "BaudRate");
          sqlCommand.Parameters[2].Value = (object) baudRate;
          sqlCommand.Parameters.Add("@Parity", SqlDbType.Int, 4, "Parity");
          sqlCommand.Parameters[3].Value = (object) parity;
          sqlCommand.Parameters.Add("@DataBits", SqlDbType.Int, 4, "DataBits");
          sqlCommand.Parameters[4].Value = (object) dataBits;
          sqlCommand.Parameters.Add("@StopBits", SqlDbType.Int, 4, "StopBits");
          sqlCommand.Parameters[5].Value = (object) stopBits;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Port);
          sqlConnection.Close();
          this.UpdatePortInfo(portId, portName, baudRate, parity, dataBits, stopBits);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (update Ports): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public ITMCOMDataSet SelectLog()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.SelectLog;";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Log);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException: {0}", (object) ex.Message));
      }
      return itmcomDataSet;
    }

    public bool DeleteLog(int logId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.DeleteLog @LogId;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@LogId", SqlDbType.Int, 4, "LogId");
          sqlCommand.Parameters[0].Value = (object) logId;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Log);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Log): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool DeleteAllLogs()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "DELETE FROM [Log]";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Log);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Log): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool InsertLog(int siteID, string COMPort, string message, int statusId)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.InsertLog @SiteID, @COMPort, @Message, @SatusId, \r\n\t\t\t\t\t\t@Time, @RecordsCount; EXECUTE dbo.SelectLog;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
          if (siteID == -1)
            sqlCommand.Parameters[0].Value = (object) DBNull.Value;
          else
            sqlCommand.Parameters[0].Value = (object) siteID;
          sqlCommand.Parameters.Add("@COMPort", SqlDbType.VarChar, 20, nameof (COMPort));
          sqlCommand.Parameters[1].Value = (object) COMPort;
          sqlCommand.Parameters.Add("@Message", SqlDbType.VarChar, 1024, "Message");
          sqlCommand.Parameters[2].Value = (object) message;
          sqlCommand.Parameters.Add("@SatusId", SqlDbType.Int, 4, "SatusId");
          sqlCommand.Parameters[3].Value = (object) statusId;
          sqlCommand.Parameters.Add("@Time", SqlDbType.DateTime, 8, "Time");
          sqlCommand.Parameters[4].Value = (object) DateTime.Now;
          sqlCommand.Parameters.Add("@RecordsCount", SqlDbType.Int, 4, "RecordsCount");
          sqlCommand.Parameters[5].Value = (object) this._logRecordsCount;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Log);
          sqlConnection.Close();
          int logId = 0;
          if (itmcomDataSet.Log.Rows.Count > 0)
            logId = (int) itmcomDataSet.Log.Rows[itmcomDataSet.Log.Rows.Count - 1]["LogId"];
          this.UpdateLogs(logId, siteID, COMPort, DateTime.Now, message, statusId);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (insert into Log): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool UpdateLog(
      int logId,
      int siteID,
      string COMPort,
      string message,
      int statusId,
      DateTime time)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.UpdateLog @LogId, @SiteID, @COMPort, @Message, @SatusId, @Time;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@LogId", SqlDbType.Int, 4, "LogId");
          sqlCommand.Parameters[0].Value = (object) logId;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
          sqlCommand.Parameters[1].Value = (object) siteID;
          sqlCommand.Parameters.Add("@COMPort", SqlDbType.VarChar, 20, nameof (COMPort));
          sqlCommand.Parameters[2].Value = (object) COMPort;
          sqlCommand.Parameters.Add("@Message", SqlDbType.VarChar, 1024, "Message");
          sqlCommand.Parameters[3].Value = (object) message;
          sqlCommand.Parameters.Add("@SatusId", SqlDbType.Int, 4, "SatusId");
          sqlCommand.Parameters[4].Value = (object) statusId;
          sqlCommand.Parameters.Add("@Time", SqlDbType.DateTime, 8, "Time");
          sqlCommand.Parameters[5].Value = (object) time;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Log);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (update Log): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public ITMCOMDataSet SelectModem()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.SelectModem;";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Modem);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException: {0}", (object) ex.Message));
      }
      return itmcomDataSet;
    }

    public bool DeleteModem(int id, string modemID, int siteID)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          itmcomDataSet.Modem.Clear();
          sqlCommand.CommandText = "EXECUTE dbo.DeleteModem @Id;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@Id", SqlDbType.Int, 4, "Id");
          sqlCommand.Parameters[0].Value = (object) id;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Modem);
          sqlConnection.Close();
          this.RemoveModem(modemID, siteID);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Modem): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public bool DeleteAllModems()
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          itmcomDataSet.Modem.Clear();
          sqlCommand.CommandText = "DELETE FROM Modem; DELETE FROM [IllegalModem]; DELETE FROM Dynamic; DELETE FROM Statistics WHERE StatisticsId NOT IN (SELECT StatisticId FROM Static);";
          sqlCommand.Connection = sqlConnection;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Modem);
          sqlConnection.Close();
          this.RemoveAllModems();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (delete from Modem): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public int InsertModem(string modemID, int siteID, ConfigType type)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          switch (type)
          {
            case ConfigType.Modem:
              sqlCommand.CommandText = "EXECUTE dbo.InsertModem @ModemID, @SiteID; EXECUTE dbo.SelectModem;";
              break;
            case ConfigType.TCPIP_Client:
              sqlCommand.CommandText = "EXECUTE dbo.InsertTCTIPClient @ModemID, @SiteID; EXECUTE dbo.SelectTCPIPClient;";
              break;
            default:
              sqlCommand.CommandText = "EXECUTE dbo.InsertTCTIP @ModemID, @SiteID; EXECUTE dbo.SelectTCPIP;";
              break;
          }
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@ModemID", SqlDbType.VarChar, 20, "ModemID");
          sqlCommand.Parameters[0].Value = (object) modemID;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
          sqlCommand.Parameters[1].Value = (object) siteID;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Modem);
          sqlConnection.Close();
          int id = 0;
          for (int index = itmcomDataSet.Modem.Rows.Count - 1; index >= 0; --index)
          {
            if ((string) itmcomDataSet.Modem.Rows[index]["ModemId"] == modemID && (int) itmcomDataSet.Modem.Rows[index]["SiteID"] == siteID)
            {
              id = (int) itmcomDataSet.Modem.Rows[index]["Id"];
              break;
            }
          }
          if (id > 0)
          {
            if (type == ConfigType.TCPIP_Client)
              this.AddClientTCPIP(modemID, siteID, id);
            else
              this.AddModem(modemID, siteID, id);
            return id;
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (insert into Modem): {0}", (object) ex.Message));
        return -1;
      }
      return -1;
    }

    public bool UpdateModem(
      int Id,
      string ModemID,
      int SiteID,
      bool mbConnect,
      DateTime time,
      ModemUpdateFlag flag,
      string oldModemID)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "EXECUTE dbo.UpdateModem @Id, @ModemID, @SiteID, @mbConnect, @Time, @Flag;";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@Id", SqlDbType.Int, 4, nameof (Id));
          sqlCommand.Parameters[0].Value = (object) Id;
          sqlCommand.Parameters.Add("@ModemID", SqlDbType.VarChar, 20, nameof (ModemID));
          sqlCommand.Parameters[1].Value = (object) ModemID;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, nameof (SiteID));
          sqlCommand.Parameters[2].Value = (object) SiteID;
          sqlCommand.Parameters.Add("@mbConnect", SqlDbType.Bit, 1, nameof (mbConnect));
          sqlCommand.Parameters[3].Value = (object) mbConnect;
          sqlCommand.Parameters.Add("@Time", SqlDbType.DateTime, 8, "ConnectionTime");
          sqlCommand.Parameters[4].Value = (object) time;
          sqlCommand.Parameters.Add("@Flag", SqlDbType.Int, 4, nameof (SiteID));
          sqlCommand.Parameters[5].Value = (object) (int) flag;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.Modem);
          sqlConnection.Close();
          this.UpdateModems(Id, mbConnect, time, flag);
          if (oldModemID != null)
          {
            this.ChangeModemInfo(ModemID, SiteID, oldModemID);
            this.UpdateClientTCPIP(oldModemID, ModemID, SiteID);
          }
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (update Modem): {0}", (object) ex.Message));
        return false;
      }
      return true;
    }

    public void InsertIllegalModem(string modemID, int siteID)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      string connectionString = ConfigurationManager.ConnectionStrings["ITMCOMConnectStr"].ConnectionString;
      try
      {
        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
        {
          SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
          SqlCommand sqlCommand = new SqlCommand();
          sqlCommand.CommandText = "IF NOT EXISTS (SELECT * FROM [IllegalModem] WHERE [ModemID]=@ModemID) INSERT INTO [IllegalModem]([SiteID],[ModemID]) VALUES(@SiteID, @ModemID)";
          sqlCommand.Connection = sqlConnection;
          sqlCommand.Parameters.Add("@ModemID", SqlDbType.VarChar, 20, "ModemID");
          sqlCommand.Parameters[0].Value = (object) modemID;
          sqlCommand.Parameters.Add("@SiteID", SqlDbType.Int, 4, "SiteID");
          sqlCommand.Parameters[1].Value = (object) siteID;
          sqlDataAdapter.SelectCommand = sqlCommand;
          sqlConnection.Open();
          sqlDataAdapter.Fill((DataTable) itmcomDataSet.IllegalModem);
          sqlConnection.Close();
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("DBException (insert into IllegalModem): {0}", (object) ex.Message));
      }
    }

    public void FillListsAndConfig()
    {
      for (int index = 0; index < this._mainDataSet.Modem.Rows.Count; ++index)
      {
        try
        {
          this.AddModem((string) this._mainDataSet.Modem.Rows[index]["ModemId"], (int) this._mainDataSet.Modem.Rows[index]["SiteID"], (int) this._mainDataSet.Modem.Rows[index]["Id"]);
          if (this._mainDataSet.Modem.Rows[index]["mbConnect"].GetType() == typeof (bool))
          {
            if ((bool) this._mainDataSet.Modem.Rows[index]["mbConnect"])
              this.UpdateModem((int) this._mainDataSet.Modem.Rows[index]["Id"], (string) this._mainDataSet.Modem.Rows[index]["ModemId"], (int) this._mainDataSet.Modem.Rows[index]["SiteID"], false, DateTime.Now, ModemUpdateFlag.Relcase, (string) null);
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("FillListsAndConfigError(AddModem): " + ex.Message));
        }
      }
      for (int index = 0; index < this._mainDataSet.TCPIP.Rows.Count; ++index)
      {
        try
        {
          this.AddModem((string) this._mainDataSet.TCPIP.Rows[index]["ModemId"], (int) this._mainDataSet.TCPIP.Rows[index]["SiteID"], (int) this._mainDataSet.TCPIP.Rows[index]["Id"]);
          if (this._mainDataSet.TCPIP.Rows[index]["mbConnect"].GetType() == typeof (bool))
          {
            if ((bool) this._mainDataSet.TCPIP.Rows[index]["mbConnect"])
              this.UpdateModem((int) this._mainDataSet.TCPIP.Rows[index]["Id"], (string) this._mainDataSet.TCPIP.Rows[index]["ModemId"], (int) this._mainDataSet.TCPIP.Rows[index]["SiteID"], false, DateTime.Now, ModemUpdateFlag.Relcase, (string) null);
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("FillListsAndConfigError(AddModem - TCPIP): " + ex.Message));
        }
      }
      try
      {
        lock (this._dontForConnection.SyncRoot)
        {
          foreach (ITMCOMDataSet.IllegalModemRow illegalModemRow in (TypedTableBase<ITMCOMDataSet.IllegalModemRow>) this._mainDataSet.IllegalModem)
            this._dontForConnection[(object) illegalModemRow.ModemID] = (object) illegalModemRow.SiteID;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("FillListsAndConfigError(_dontForConnection): " + ex.Message));
      }
      for (int index = 0; index < this._mainDataSet.Config.Rows.Count; ++index)
      {
        try
        {
          if ((bool) this._mainDataSet.Config.Rows[index]["mbCurrent"])
          {
            ConfigType type = (ConfigType) this._mainDataSet.Config.Rows[index]["TypeId"];
            int portNumber = (int) this._mainDataSet.Config.Rows[index]["PortNumber"];
            int configurationId = (int) this._mainDataSet.Config.Rows[index]["ConfigId"];
            IPAddress address = IPAddress.Parse(this._mainDataSet.Config.Rows[index]["IPAddress"] as string);
            string serverID = this._mainDataSet.Config.Rows[index]["ServerID"] as string;
            this.StartListThread(address, portNumber, serverID, type, configurationId);
          }
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("FillListsAndConfigError(Config): " + ex.Message));
        }
      }
      this._clientList.Clear();
      for (int index = 0; index < this._mainDataSet.TCPIPClient.Rows.Count; ++index)
      {
        try
        {
          this.AddClientTCPIP(this._mainDataSet.TCPIPClient.Rows[index]["ModemId"] as string, (int) this._mainDataSet.TCPIPClient.Rows[index]["SiteID"], (int) this._mainDataSet.TCPIPClient.Rows[index]["Id"]);
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("FillListsAndConfigError(AddModem - TCPIPClient): " + ex.Message));
        }
      }
      this.StartTCPClientThread();
    }

    protected void StartListThread(
      IPAddress address,
      int portNumber,
      string serverID,
      ConfigType type,
      int configurationId)
    {
      try
      {
        Thread thread = new Thread(new ParameterizedThreadStart(this.DoListener));
        ListenerArgs parameter = new ListenerArgs(address, portNumber, serverID, type, configurationId);
        parameter._thread = thread;
        this._listeners[(object) configurationId] = (object) parameter;
        thread.Start((object) parameter);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("StartListenerError: {0} - {1}", (object) ex.Message, (object) ex.StackTrace));
      }
    }

    protected void DeleteListener(int configurationId)
    {
      lock (this._listeners.SyncRoot)
      {
        try
        {
          this.StopListener(this._listeners[(object) configurationId] as ListenerArgs, false);
          this._listeners.Remove((object) configurationId);
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("DeleteListenerError: " + ex.Message));
        }
      }
    }

    protected void DeleteAllListeners()
    {
      lock (this._listeners.SyncRoot)
      {
        try
        {
          foreach (ListenerArgs listener in this._listeners)
            this.StopListener(listener, false);
          this._listeners.Clear();
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("DeleteAllListenersError: " + ex.Message));
        }
      }
    }

    private void StopListener(ListenerArgs args, bool mbReuse)
    {
      try
      {
        if (args == null)
          return;
        if (args._listener != null)
          args._listener.Stop();
        args._thread.Abort();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("StopListenerError: {0} - {1}", (object) ex.Message, (object) ex.StackTrace));
      }
    }

    protected void ChangeListener(
      IPAddress address,
      int portNumber,
      string serverID,
      ConfigType type,
      int configurationId)
    {
      lock (this._listeners.SyncRoot)
      {
        try
        {
          if (this._listeners[(object) configurationId] is ListenerArgs listener)
          {
            if (listener._portNumber == portNumber && listener._address == address && listener._type == type)
            {
              listener._serverID = serverID;
            }
            else
            {
              this.StopListener(listener, true);
              this._listeners.Remove((object) configurationId);
            }
          }
          this.StartListThread(address, portNumber, serverID, type, configurationId);
        }
        catch (Exception ex)
        {
          EventLogger.Info((object) ("ChangeListenerError: " + ex.Message));
        }
      }
    }

    public void DoListener(object sender)
    {
      if (!(sender is ListenerArgs listenerArgs))
        return;
      TcpListener tcpListener = (TcpListener) null;
      while (true)
      {
        try
        {
          tcpListener = new TcpListener(listenerArgs._address, listenerArgs._portNumber);
          Socket server = tcpListener.Server;
          LingerOption optionValue = new LingerOption(true, 10);
          server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, (object) optionValue);
          server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
          lock (listenerArgs)
            listenerArgs._listener = tcpListener;
          tcpListener.Start();
          while (!this._closeFlag)
          {
            if (tcpListener.Pending())
            {
              Socket socket = tcpListener.AcceptSocket();
              EventLogger.Info((object) string.Format("Client connection attempt (IP: {0})", (object) this.GetIPAddress(socket)));
              if (socket.Connected)
              {
                if (listenerArgs._type == ConfigType.Modem || listenerArgs._type == ConfigType.ModemATSWP)
                {
                  ModemType type = listenerArgs._type == ConfigType.Modem ? ModemType.Legacy : ModemType.ATSWP;
                  new Thread(new ParameterizedThreadStart(this.ModemConnection)).Start((object) new ClientArgs(socket, listenerArgs, type));
                }
                if (listenerArgs._type == ConfigType.TCPIP_Static)
                  new Thread(new ParameterizedThreadStart(this.TCPIPStaticConnection)).Start((object) new ClientArgs(socket, listenerArgs, ModemType.Legacy));
                if (listenerArgs._type != ConfigType.TCPIP_Dynamic)
                  ;
              }
              else
                Utilities.CloseSocket(socket);
            }
            Thread.Sleep(100);
          }
          tcpListener.Stop();
          break;
        }
        catch (SocketException ex)
        {
          EventLogger.Info((object) string.Format("ListenerSocketException ({1}:{2}): {0}", (object) ex.Message, (object) listenerArgs._address.ToString(), (object) listenerArgs._portNumber.ToString()));
          Thread.Sleep(100);
        }
        finally
        {
          tcpListener?.Stop();
          tcpListener = (TcpListener) null;
        }
      }
    }

    public void ModemConnection(object sender)
    {
      ClientArgs clientArgs = sender as ClientArgs;
      try
      {
        if (this._mbDetailedConnectionLog)
          this.InsertRecord((DBRecord) new LogRecord(-1, "", string.Format(Resource.ConnectionAttempt, (object) this.GetIPAddress(clientArgs._client)), LogStatus.Diagnostic));
        byte[] numArray1 = new byte[20];
        DateTime now1 = DateTime.Now;
        TimeSpan timeSpan = new TimeSpan(0, 0, this._recieveVal / 1000);
        clientArgs._client.ReceiveTimeout = this._recieveVal;
        clientArgs._client.SendTimeout = this._sendVal;
        int num1;
        int num2;
        for (num1 = 0; num1 < 20 && DateTime.Now - now1 < timeSpan; num1 += num2)
          num2 = clientArgs._client.Receive(numArray1, num1, numArray1.Length - num1, SocketFlags.None);
        if (num1 < 20 || numArray1[0] == (byte) 10 && clientArgs._type == ModemType.Legacy || numArray1[0] == (byte) 192 && clientArgs._type == ModemType.ATSWP)
        {
          EventLogger.Info((object) string.Format("Неверный идентификатор модема (начало...): {0}", (object) Utils.TranslateModemBatch(numArray1, 0, num1)));
          byte[] numArray2 = new byte[8192];
          DateTime now2 = DateTime.Now;
          int num3 = 0;
          timeSpan = new TimeSpan(0, 0, 10);
          while (DateTime.Now - now2 < timeSpan)
          {
            int num4 = clientArgs._client.Receive(numArray2, num3, numArray2.Length - num3, SocketFlags.None);
            num3 += num4;
          }
          clientArgs._client.Close();
          throw new Exception(string.Format("Неверный идентификатор модема (окончание): {0}", (object) Utils.TranslateModemBatch(numArray2, 0, num3)));
        }
        string str1 = Encoding.ASCII.GetString(numArray1, 0, num1);
        EventLogger.Info((object) string.Format("modemID: {0}", (object) Utils.TranslateModemBatch(numArray1, 0, numArray1.Length)));
        string str2 = this.NormalizeNameVS(str1);
        EventLogger.Info((object) string.Format("Modem connection attempt: {0} ({1})", (object) str2, (object) this.GetIPAddress(clientArgs._client)));
        this.CloseModemConnection(str2);
        if (this._mbDetailedConnectionLog)
          this.InsertRecord((DBRecord) new LogRecord(-1, "", string.Format(Resource.ConnectionRecieveData, (object) this.GetIPAddress(clientArgs._client), (object) str2), LogStatus.Diagnostic));
        Routing modemInfo = this.GetModemInfo(str2);
        if (modemInfo != null)
        {
          EventLogger.Info((object) string.Format("Modem connectied: {0} ({1})", (object) str2, (object) this.GetIPAddress(clientArgs._client)));
          this.InsertRecord((DBRecord) new LogRecord(modemInfo.SiteID, "", string.Format(Resource.ModemConnection, (object) this.GetIPAddress(clientArgs._client)), LogStatus.Diagnostic));
          modemInfo.MbConnect = true;
          this.InsertRecord((DBRecord) new ModemRecord(modemInfo.Id, modemInfo.ModemID, modemInfo.MbConnect, DateTime.Now, ModemUpdateFlag.Connection, (string) null, modemInfo.SiteID));
          DynamicTable dynamicTable = new DynamicTable(clientArgs._client, str2, clientArgs._type);
          dynamicTable.Client.LingerState = new LingerOption(true, 0);
          dynamicTable.Client.NoDelay = true;
          dynamicTable.Flag = false;
          string s = "";
          lock (this._listeners.SyncRoot)
          {
            if (this._listeners[(object) clientArgs._listenerArgs._configurationId] is ListenerArgs listener)
              s = Utils.NormalizeName(listener._serverID);
          }
          try
          {
            lock (this._myTable.SyncRoot)
            {
              this._tempList[(object) str2] = (object) dynamicTable.Client;
              this._myTable[(object) dynamicTable.Client] = (object) dynamicTable;
              byte[] bytes = Encoding.ASCII.GetBytes(s);
              clientArgs._client.Send(bytes, bytes.Length, SocketFlags.None);
              if (!this._mbDetailedConnectionLog || listener == null)
                return;
              this.InsertRecord((DBRecord) new LogRecord(-1, "", string.Format(Resource.ConnectionSendData, (object) this.GetIPAddress(clientArgs._client), (object) str2, (object) listener._serverID), LogStatus.Diagnostic));
            }
          }
          catch (Exception ex)
          {
            EventLogger.Info((object) ("ModemConnectionError: " + ex.Message));
          }
        }
        else
        {
          EventLogger.Info((object) string.Format("Modem not connectied: {0} ({1})", (object) str2, (object) this.GetIPAddress(clientArgs._client)));
          this.InsertRecord((DBRecord) new LogRecord(-1, "", string.Format(Resource.UnknownModem, (object) str2), LogStatus.Diagnostic));
          Utilities.CloseSocket(clientArgs._client);
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("TcpClientError: {0}", (object) ex.Message));
        if (this._myTable.Contains((object) clientArgs._client))
          this.CloseModemConnection(((DynamicTable) this._myTable[(object) clientArgs._client]).ModemID);
        else
          Utilities.CloseSocket(clientArgs._client);
      }
    }

    private void TCPIPConnection(Socket client)
    {
      try
      {
        string str = this.NormalizeIP(client.RemoteEndPoint.ToString());
        this.CloseModemConnection(str);
        Routing routingItem = this.GetRoutingItem(str);
        if (routingItem != null)
        {
          this.InsertRecord((DBRecord) new LogRecord(routingItem.SiteID, "", Resource.TCPIPConnection, LogStatus.Diagnostic));
          routingItem.MbConnect = true;
          this.InsertRecord((DBRecord) new ModemRecord(routingItem.Id, routingItem.ModemID, routingItem.MbConnect, DateTime.Now, ModemUpdateFlag.Connection, (string) null, routingItem.SiteID));
          DynamicTable dynamicTable = new DynamicTable(client, str, ModemType.Legacy);
          dynamicTable.Client.LingerState = new LingerOption(false, 0);
          dynamicTable.Client.ReceiveTimeout = this._recieveVal;
          dynamicTable.Client.SendTimeout = this._sendVal;
          dynamicTable.Client.NoDelay = true;
          dynamicTable.Flag = false;
          lock (this._myTable.SyncRoot)
          {
            this._tempList[(object) str] = (object) dynamicTable.Client;
            this._myTable[(object) dynamicTable.Client] = (object) dynamicTable;
          }
        }
        else
        {
          this.InsertRecord((DBRecord) new LogRecord(-1, "", string.Format(Resource.UnknownTcpIp, (object) str), LogStatus.Diagnostic));
          EventLogger.Info((object) string.Format(Resource.UnknownTcpIp, (object) str));
          lock (this._TCPIPCacheList.SyncRoot)
            this._TCPIPCacheList[(object) client] = (object) DateTime.Now;
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("TcpClientError: {0}", (object) ex.Message));
        if (this._myTable.Contains((object) client))
          this.CloseModemConnection(((DynamicTable) this._myTable[(object) client]).ModemID);
        else
          Utilities.CloseSocket(client);
      }
    }

    public void TCPIPStaticConnection(object sender)
    {
      this.TCPIPConnection((sender as ClientArgs)._client);
    }

    public void TCPIPCache()
    {
      while (!this._closeFlag)
      {
        List<Socket> socketList = new List<Socket>();
        lock (this._TCPIPCacheList.SyncRoot)
        {
          foreach (Socket key in (IEnumerable) this._TCPIPCacheList.Keys)
          {
            if (((DateTime) this._TCPIPCacheList[(object) key]).AddSeconds((double) this._cleanupTime) <= DateTime.Now)
              socketList.Add(key);
          }
          foreach (Socket key in socketList)
          {
            key.Close();
            this._TCPIPCacheList.Remove((object) key);
          }
        }
        Thread.Sleep(100);
      }
    }

    public void Listen()
    {
      EventLogger.Initialize();
      try
      {
        int completionPortThreads;
        ThreadPool.GetMinThreads(out int _, out completionPortThreads);
        ThreadPool.SetMinThreads(10, completionPortThreads);
        ThreadPool.SetMaxThreads(100, completionPortThreads);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("ListenError (Пул потоков не проинициализирован): ({0}:{1})", (object) ex.Message, (object) ex.StackTrace));
      }
      try
      {
        Hashtable section = (Hashtable) ConfigurationManager.GetSection("timeoutValues");
        if (section != null)
        {
          int num = 0;
          if (section[(object) "send"] != null)
            num = int.Parse(section[(object) "send"].ToString());
          if (num > 0 && num <= 60000)
            this._sendVal = num;
          if (section[(object) "receive"] != null)
            num = int.Parse(section[(object) "receive"].ToString());
          if (num > 0 && num <= 60000)
            this._recieveVal = num;
          if (section[(object) "portSendVal"] != null)
            this._sendVal = int.Parse(section[(object) "portSendVal"].ToString());
          if (section[(object) "portReceiveVal"] != null)
            this._recievePortVal = int.Parse(section[(object) "portReceiveVal"].ToString());
          if (section[(object) "queuesize"] != null)
            this._queueSize = int.Parse(section[(object) "queuesize"].ToString());
          if (section[(object) "connectUnknown"] != null)
            this._mbConnectUnknown = bool.Parse(section[(object) "connectUnknown"].ToString());
          if (section[(object) "askClient"] != null)
            this._mbAskClient = bool.Parse(section[(object) "askClient"].ToString());
          if (section[(object) "autoCloseDisconnected"] != null)
            this._mbAutoCloseDisconnected = bool.Parse(section[(object) "autoCloseDisconnected"].ToString());
          if (section[(object) "checkTimeDifference"] != null)
            this._mbCheckTimeDifference = bool.Parse(section[(object) "checkTimeDifference"].ToString());
          if (section[(object) "timeDifference"] != null)
            this._timeDifference = int.Parse(section[(object) "timeDifference"].ToString());
          if (section[(object) "cleanupTime"] != null)
            this._cleanupTime = int.Parse(section[(object) "cleanupTime"].ToString());
          if (section[(object) "detailedConnectionLog"] != null)
            this._mbDetailedConnectionLog = bool.Parse(section[(object) "detailedConnectionLog"].ToString());
          if (section[(object) "logRecordsCount"] != null)
            this._logRecordsCount = int.Parse(section[(object) "logRecordsCount"].ToString());
          if (section[(object) "statisticHoursCount"] != null)
            this._statisticHoursCount = int.Parse(section[(object) "statisticHoursCount"].ToString());
          if (section[(object) "portSendVal"] != null)
            this._portSendVal = int.Parse(section[(object) "portSendVal"].ToString());
          if (section[(object) "portRecieveVal"] != null)
            this._portRecieveVal = int.Parse(section[(object) "portRecieveVal"].ToString());
        }
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) string.Format("ListenError(Ошибка чтения файла конфигурации): ({0}:{1})", (object) ex.Message, (object) ex.StackTrace));
      }
      this.GetDataFromDB(true);
      try
      {
        this.FillListsAndConfig();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ListenError(FillListsAndConfig): " + ex.Message));
      }
      try
      {
        this.COMPorts();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("COMPortsError(FillListsAndConfig): " + ex.Message));
      }
      try
      {
        this._readWriteThread = new Thread(new ThreadStart(this.ReadWrite));
        this._readWriteThread.Start();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ListenError(ReadWrite): " + ex.Message));
      }
      try
      {
        this._statisticsThread = new Thread(new ThreadStart(this.Statistics));
        this._statisticsThread.Start();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ListenError(Statistics): " + ex.Message));
      }
      try
      {
        this._writePortThread = new Thread(new ThreadStart(this.DataSend));
        this._writePortThread.Start();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ListenError(DataSend): " + ex.Message));
      }
      try
      {
        this._cacheThread = new Thread(new ThreadStart(this.TCPIPCache));
        this._cacheThread.Start();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ListenError(TCPIPCache): " + ex.Message));
      }
      try
      {
        this._DBThread = new Thread(new ThreadStart(this.ProcessingDBRecords));
        this._DBThread.Start();
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ListenError(ProcessingDBRecords): " + ex.Message));
      }
      while (!this._closeFlag)
        Thread.Sleep(100);
    }
  }
}
