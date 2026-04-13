using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

#nullable disable
namespace Indusoft.TM.COM.Service
{
  [RunInstaller(true)]
  public class AnCOMServiceInstaller : Installer
  {
    private const string _serviceName = "I-TM-COM-Service";
    private const string _displayName = "Сервер связи I-TM-COM";
    private const string _description = "Сервер связи системы телемеханики I-TM-COM";
    private ServiceInstaller _serviceInstaller;

    public AnCOMServiceInstaller()
    {
      ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
      this._serviceInstaller = new ServiceInstaller();
      processInstaller.Account = ServiceAccount.LocalSystem;
      this._serviceInstaller.StartType = ServiceStartMode.Automatic;
      this._serviceInstaller.ServiceName = "I-TM-COM-Service";
      this._serviceInstaller.DisplayName = "Сервер связи I-TM-COM";
      this._serviceInstaller.Description = "Сервер связи системы телемеханики I-TM-COM";
      this.Installers.Add((Installer) processInstaller);
      this.Installers.Add((Installer) this._serviceInstaller);
    }

    public override void Install(IDictionary stateSaver)
    {
      foreach (ServiceController service in ServiceController.GetServices())
      {
        if (service.ServiceName == this._serviceInstaller.ServiceName)
          this.Uninstall(stateSaver);
      }
      base.Install(stateSaver);
    }
  }
}
