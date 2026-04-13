#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class ReadModemObject
  {
    public const int BufferSize = 8192;
    public DynamicTable Work;
    public byte[] Buffer = new byte[8192];
  }
}
