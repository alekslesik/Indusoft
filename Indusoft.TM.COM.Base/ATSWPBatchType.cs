#nullable disable
namespace Indusoft.TM.COM.Base
{
  public enum ATSWPBatchType
  {
    DataUART1 = 0,
    DataUART2 = 1,
    DataI2C = 2,
    DataSPI1 = 3,
    DataSPI2 = 4,
    DataUSB = 5,
    Command = 196, // 0x000000C4
    Config = 197, // 0x000000C5
    CommunicationState = 198, // 0x000000C6
  }
}
