#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class WriteObject
  {
    public byte[] _data;
    public int _begin;
    public int _size;
    public int _siteIdFrom;

    public WriteObject(byte[] data, int begin, int size, int siteIdFrom)
    {
      this._data = data;
      this._begin = begin;
      this._size = size;
      this._siteIdFrom = siteIdFrom;
    }
  }
}
