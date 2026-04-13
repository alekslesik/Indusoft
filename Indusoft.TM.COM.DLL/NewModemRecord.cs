using Indusoft.TM.COM.Base;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class NewModemRecord : DBRecord
  {
    public string ModemId;
    public ConfigType ConfigType;

    public NewModemRecord(string modemId, ConfigType configType, int siteId)
      : base(RecordType.NewModem, siteId)
    {
      this.ModemId = modemId;
      this.ConfigType = configType;
    }
  }
}
