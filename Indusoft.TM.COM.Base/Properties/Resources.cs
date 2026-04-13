// Decompiled with JetBrains decompiler
// Type: Indusoft.TM.COM.Base.Properties.Resources
// Assembly: Indusoft.TM.COM.Base, Version=2.1.5.1, Culture=neutral, PublicKeyToken=null
// MVID: C829CFD6-D2C7-458A-AEBD-9AB2658A683B
// Assembly location: C:\Users\Admin\Desktop\source\Moscad\Сервер связи системы телемеханики\Indusoft.TM.COM.Base.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Indusoft.TM.COM.Base.Properties
{
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) Indusoft.TM.COM.Base.Properties.Resources.resourceMan, (object) null))
          Indusoft.TM.COM.Base.Properties.Resources.resourceMan = new ResourceManager("Indusoft.TM.COM.Base.Properties.Resources", typeof (Indusoft.TM.COM.Base.Properties.Resources).Assembly);
        return Indusoft.TM.COM.Base.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.resourceCulture;
      set => Indusoft.TM.COM.Base.Properties.Resources.resourceCulture = value;
    }

    internal static string Answer
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.ResourceManager.GetString(nameof (Answer), Indusoft.TM.COM.Base.Properties.Resources.resourceCulture);
    }

    internal static string Command
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.ResourceManager.GetString(nameof (Command), Indusoft.TM.COM.Base.Properties.Resources.resourceCulture);
    }

    internal static string CommandAnswer
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.ResourceManager.GetString(nameof (CommandAnswer), Indusoft.TM.COM.Base.Properties.Resources.resourceCulture);
    }

    internal static string Error
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.ResourceManager.GetString(nameof (Error), Indusoft.TM.COM.Base.Properties.Resources.resourceCulture);
    }

    internal static string NoReceiver
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.ResourceManager.GetString(nameof (NoReceiver), Indusoft.TM.COM.Base.Properties.Resources.resourceCulture);
    }

    internal static string Receive
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.ResourceManager.GetString(nameof (Receive), Indusoft.TM.COM.Base.Properties.Resources.resourceCulture);
    }

    internal static string Request
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.ResourceManager.GetString(nameof (Request), Indusoft.TM.COM.Base.Properties.Resources.resourceCulture);
    }

    internal static string Send
    {
      get => Indusoft.TM.COM.Base.Properties.Resources.ResourceManager.GetString(nameof (Send), Indusoft.TM.COM.Base.Properties.Resources.resourceCulture);
    }
  }
}
