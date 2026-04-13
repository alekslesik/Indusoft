// Decompiled with JetBrains decompiler
// Type: Indusoft.TM.COM.DLL.Properties.Resource
// Assembly: Indusoft.TM.COM.DLL, Version=2.1.5.1, Culture=neutral, PublicKeyToken=null
// MVID: CB488564-15ED-4F27-95D8-9C52636FCEAD
// Assembly location: C:\Users\Admin\Desktop\source\Moscad\Сервер связи системы телемеханики\Indusoft.TM.COM.DLL.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Indusoft.TM.COM.DLL.Properties
{
  [DebuggerNonUserCode]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [CompilerGenerated]
  internal class Resource
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resource()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) Resource.resourceMan, (object) null))
          Resource.resourceMan = new ResourceManager("Indusoft.TM.COM.DLL.Properties.Resource", typeof (Resource).Assembly);
        return Resource.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Resource.resourceCulture;
      set => Resource.resourceCulture = value;
    }

    internal static string BreakSocketConnection
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (BreakSocketConnection), Resource.resourceCulture);
      }
    }

    internal static string CanNotBlock
    {
      get => Resource.ResourceManager.GetString(nameof (CanNotBlock), Resource.resourceCulture);
    }

    internal static string CommandError
    {
      get => Resource.ResourceManager.GetString(nameof (CommandError), Resource.resourceCulture);
    }

    internal static string Connection
    {
      get => Resource.ResourceManager.GetString(nameof (Connection), Resource.resourceCulture);
    }

    internal static string ConnectionAttempt
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (ConnectionAttempt), Resource.resourceCulture);
      }
    }

    internal static string ConnectionRecieveData
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (ConnectionRecieveData), Resource.resourceCulture);
      }
    }

    internal static string ConnectionSendData
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (ConnectionSendData), Resource.resourceCulture);
      }
    }

    internal static string DifferentModemID
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (DifferentModemID), Resource.resourceCulture);
      }
    }

    internal static string DifferentSiteID
    {
      get => Resource.ResourceManager.GetString(nameof (DifferentSiteID), Resource.resourceCulture);
    }

    internal static string DoCommand
    {
      get => Resource.ResourceManager.GetString(nameof (DoCommand), Resource.resourceCulture);
    }

    internal static string Error
    {
      get => Resource.ResourceManager.GetString(nameof (Error), Resource.resourceCulture);
    }

    internal static string IllegalModem
    {
      get => Resource.ResourceManager.GetString(nameof (IllegalModem), Resource.resourceCulture);
    }

    internal static string ModemConnection
    {
      get => Resource.ResourceManager.GetString(nameof (ModemConnection), Resource.resourceCulture);
    }

    internal static string ModemError
    {
      get => Resource.ResourceManager.GetString(nameof (ModemError), Resource.resourceCulture);
    }

    internal static string ModemNotFound
    {
      get => Resource.ResourceManager.GetString(nameof (ModemNotFound), Resource.resourceCulture);
    }

    internal static string ModemProtocolError
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (ModemProtocolError), Resource.resourceCulture);
      }
    }

    internal static string NoModemConnection
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (NoModemConnection), Resource.resourceCulture);
      }
    }

    internal static string PortConnection
    {
      get => Resource.ResourceManager.GetString(nameof (PortConnection), Resource.resourceCulture);
    }

    internal static string PortInitError
    {
      get => Resource.ResourceManager.GetString(nameof (PortInitError), Resource.resourceCulture);
    }

    internal static string PortOpenningError
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (PortOpenningError), Resource.resourceCulture);
      }
    }

    internal static string PortOpenningErrorEx
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (PortOpenningErrorEx), Resource.resourceCulture);
      }
    }

    internal static string ReadFromModemError
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (ReadFromModemError), Resource.resourceCulture);
      }
    }

    internal static string ReadFromPort
    {
      get => Resource.ResourceManager.GetString(nameof (ReadFromPort), Resource.resourceCulture);
    }

    internal static string StartServer
    {
      get => Resource.ResourceManager.GetString(nameof (StartServer), Resource.resourceCulture);
    }

    internal static string TCPIPClientConnection
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (TCPIPClientConnection), Resource.resourceCulture);
      }
    }

    internal static string TCPIPConnection
    {
      get => Resource.ResourceManager.GetString(nameof (TCPIPConnection), Resource.resourceCulture);
    }

    internal static string UnknownModem
    {
      get => Resource.ResourceManager.GetString(nameof (UnknownModem), Resource.resourceCulture);
    }

    internal static string UnknownTcpIp
    {
      get => Resource.ResourceManager.GetString(nameof (UnknownTcpIp), Resource.resourceCulture);
    }

    internal static string WriteToModemError
    {
      get
      {
        return Resource.ResourceManager.GetString(nameof (WriteToModemError), Resource.resourceCulture);
      }
    }
  }
}
