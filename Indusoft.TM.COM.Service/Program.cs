using System.ServiceProcess;

#nullable disable
namespace Indusoft.TM.COM.Service
{
  internal static class Program
  {
    private static void Main()
    {
      ServiceBase.Run(new ServiceBase[1]
      {
        (ServiceBase) new ITMCOMService()
      });
    }
  }
}
