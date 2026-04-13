using Indusoft.TM.COM.DLL;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.ServiceProcess;
using System.Threading;

#nullable disable
namespace Indusoft.TM.COM.Service
{
  public class ITMCOMService : ServiceBase
  {
    private IContainer components;
    private Thread m_thread;
    private EventLog myLog = new EventLog();
    private TcpChannel channel;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new System.ComponentModel.Container();
      this.ServiceName = "I-TM-COM-Service";
    }

    public ITMCOMService() => this.InitializeComponent();

    protected override void OnStart(string[] args)
    {
      if (!EventLog.SourceExists("Сервер связи I-TM-COM"))
        EventLog.CreateEventSource("Сервер связи I-TM-COM", "Сообщения от I-TM-COM");
      this.myLog.Source = "Сервер связи I-TM-COM";
      try
      {
        RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);
        RemotingConfiguration.CustomErrorsEnabled(false);
        BinaryServerFormatterSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider();
        serverSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
        IDictionary properties = (IDictionary) new Hashtable();
        properties[(object) "port"] = (object) "8008";
        properties[(object) "exclusiveAddressUse"] = (object) false;
        Hashtable section = (Hashtable) ConfigurationManager.GetSection("timeoutValues");
        if (section != null && section[(object) "portNumber"] != null)
          properties[(object) "port"] = (object) int.Parse(section[(object) "portNumber"].ToString());
        properties[(object) "typeFilterLevel"] = (object) TypeFilterLevel.Full;
        this.channel = new TcpChannel(properties, (IClientChannelSinkProvider) null, (IServerChannelSinkProvider) serverSinkProvider);
        ChannelServices.RegisterChannel((IChannel) this.channel, false);
        RemotingServices.Marshal((MarshalByRefObject) ITMCOMDLL.Instance, RemotingConfiguration.GetRegisteredWellKnownServiceTypes()[0].ObjectUri);
        this.m_thread = new Thread(new ThreadStart(ITMCOMDLL.Instance.Listen));
        this.m_thread.Start();
        base.OnStart(args);
      }
      catch (Exception ex)
      {
        this.myLog.WriteEntry(string.Format("Ошибка запуска {0}:{1}", (object) ex.Message, (object) ex.StackTrace), EventLogEntryType.Error);
      }
    }

    protected override void OnStop()
    {
      try
      {
        try
        {
          ITMCOMDLL.Instance.WriteStatistics();
          ITMCOMDLL.Instance.ConnectionOff();
          try
          {
            this.channel.StopListening(this.channel.ChannelData);
            if (this.m_thread == null || !this.m_thread.IsAlive)
              return;
            this.m_thread.Abort();
          }
          catch (Exception ex)
          {
          }
        }
        finally
        {
          base.OnStop();
        }
      }
      catch (Exception ex)
      {
        this.myLog.WriteEntry(string.Format("Ошибка останова {0}:{1}", (object) ex.Message, (object) ex.StackTrace), EventLogEntryType.Error);
      }
    }
  }
}
