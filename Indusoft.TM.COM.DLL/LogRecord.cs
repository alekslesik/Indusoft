#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class LogRecord : DBRecord
  {
    public string COMPort;
    public string Message;
    public LogStatus Status;

    public LogRecord(int siteId, string comPort, string message, LogStatus status)
      : base(RecordType.Log, siteId)
    {
      this.COMPort = comPort;
      this.Message = message;
      this.Status = status;
    }
  }
}
