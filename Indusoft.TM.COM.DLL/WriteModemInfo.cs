using Indusoft.TM.COM.Base;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class WriteModemInfo
  {
    public ModemInfo Info;
    public ModemType Type;

    public WriteModemInfo(ModemInfo info, ModemType type)
    {
      this.Info = info;
      this.Type = type;
    }
  }
}
