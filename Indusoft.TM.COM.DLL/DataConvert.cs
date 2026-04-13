using Indusoft.TM.COM.Base;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  [StructLayout(LayoutKind.Explicit, Size = 9)]
  internal struct DataConvert
  {
    [FieldOffset(0)]
    public byte first;
    [FieldOffset(1)]
    public ushort sideIdTo;
    [FieldOffset(3)]
    public byte linkId;
    [FieldOffset(4)]
    public ushort sideIdFrom;
    [FieldOffset(6)]
    public ushort counter;
    [FieldOffset(8)]
    public byte factor;

    public void Convert()
    {
      try
      {
        this.sideIdTo = DataConvert.ViceVersa(this.sideIdTo);
        this.sideIdFrom = DataConvert.ViceVersa(this.sideIdFrom);
        this.counter = DataConvert.ViceVersa(this.counter);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ConvertError: " + (object) ex));
      }
    }

    public static unsafe ushort ViceVersa(ushort data)
    {
      ushort num = 0;
      try
      {
        byte* numPtr1 = (byte*) &data;
        byte* numPtr2 = (byte*) &num;
        *numPtr2 = numPtr1[1];
        numPtr2[1] = *numPtr1;
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ViceVersaError: " + (object) ex));
      }
      return num;
    }

    public static unsafe byte[] UShortToBytes(ushort data)
    {
      byte[] bytes = new byte[2];
      try
      {
        byte* numPtr = (byte*) &data;
        bytes[0] = *numPtr;
        bytes[1] = numPtr[1];
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ViceVersaError: " + (object) ex));
      }
      return bytes;
    }

    public static unsafe byte[] UShortToBytesVV(ushort data)
    {
      byte[] bytesVv = new byte[2];
      try
      {
        byte* numPtr = (byte*) &data;
        bytesVv[0] = numPtr[1];
        bytesVv[1] = *numPtr;
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("ViceVersaError: " + (object) ex));
      }
      return bytesVv;
    }

    public static unsafe explicit operator DataConvert(byte[] data)
    {
      DataConvert dataConvert = new DataConvert();
      try
      {
        byte* numPtr = (byte*) &dataConvert;
        int num = sizeof (DataConvert);
        for (int index = 0; index < num; ++index)
          numPtr[index] = data[index];
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("DataConvert(Byte[] data)Error: " + ex.Message));
      }
      return dataConvert;
    }

    public static unsafe explicit operator byte[](DataConvert query)
    {
      int length = sizeof (DataConvert);
      byte[] numArray = new byte[length];
      try
      {
        byte* numPtr = (byte*) &query;
        for (int index = 0; index < length; ++index)
          numArray[index] = numPtr[index];
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("Byte[](DataConvert query)Error: " + ex.Message));
      }
      return numArray;
    }
  }
}
