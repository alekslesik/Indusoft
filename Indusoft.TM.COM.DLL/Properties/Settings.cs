// Decompiled with JetBrains decompiler
// Type: Indusoft.TM.COM.DLL.Properties.Settings
// Assembly: Indusoft.TM.COM.DLL, Version=2.1.5.1, Culture=neutral, PublicKeyToken=null
// MVID: CB488564-15ED-4F27-95D8-9C52636FCEAD
// Assembly location: C:\Users\Admin\Desktop\source\Moscad\Сервер связи системы телемеханики\Indusoft.TM.COM.DLL.dll

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Indusoft.TM.COM.DLL.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default => Settings.defaultInstance;

    [DefaultSettingValue("Data Source=SEKRETAR\\SQLExpress;Initial Catalog=AnCom;User ID=ancom")]
    [ApplicationScopedSetting]
    [DebuggerNonUserCode]
    [SpecialSetting(SpecialSetting.ConnectionString)]
    public string AnComConnectStr => (string) this[nameof (AnComConnectStr)];
  }
}
