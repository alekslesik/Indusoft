using Indusoft.TM.COM.Base.Properties;
using System;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  public class Utils
  {
    public static string GetStringCommandType(StringCommandType type)
    {
      switch (type)
      {
        case StringCommandType.Request:
          return Resources.Request;
        case StringCommandType.Answer:
          return Resources.Answer;
        default:
          return Resources.Error;
      }
    }

    public static string TranslateModemBatch(byte[] data, int begin, int size)
    {
      try
      {
        string str = "";
        if (size > 0)
        {
          for (int index = 0; index < size; ++index)
            str += string.Format("{0:X2} ", (object) data[begin + index]);
        }
        return str;
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("TranslateModemBatchError: " + ex.Message));
        return "";
      }
    }

    public static string NormalizeName(string str)
    {
      char[] chArray = new char[20];
      try
      {
        char[] charArray = str.ToCharArray();
        int length = charArray.Length;
        for (int index = 0; index < 20; ++index)
          chArray[index] = index < length ? charArray[index] : char.MinValue;
        return new string(chArray);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("NormalizeNameError: " + ex.Message));
        return "";
      }
    }
  }
}
