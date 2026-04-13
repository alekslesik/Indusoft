using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  [XmlRoot("ITMCOMDataSet")]
  [DesignerCategory("code")]
  [HelpKeyword("vs.data.DataSet")]
  [ToolboxItem(true)]
  [XmlSchemaProvider("GetTypedDataSetSchema")]
  [Serializable]
  public class ITMCOMDataSet : DataSet
  {
    private ITMCOMDataSet.ConfigDataTable tableConfig;
    private ITMCOMDataSet.DynamicDataTable tableDynamic;
    private ITMCOMDataSet.StaticDataTable tableStatic;
    private ITMCOMDataSet.LogDataTable tableLog;
    private ITMCOMDataSet.RoutingDataTable tableRouting;
    private ITMCOMDataSet.LogStatusDataTable tableLogStatus;
    private ITMCOMDataSet.PortDataTable tablePort;
    private ITMCOMDataSet.ModemDataTable tableModem;
    private ITMCOMDataSet.IllegalModemDataTable tableIllegalModem;
    private ITMCOMDataSet.ConfigTypeDataTable tableConfigType;
    private ITMCOMDataSet.TCPIPDataTable tableTCPIP;
    private ITMCOMDataSet.DynamicTCPIPDataTable tableDynamicTCPIP;
    private ITMCOMDataSet.TCPIPClientDataTable tableTCPIPClient;
    private ITMCOMDataSet.DynamicTCPIPClientDataTable tableDynamicTCPIPClient;
    private DataRelation relationFK_Log_LogStatus;
    private DataRelation relationConfigType_Config;
    private SchemaSerializationMode _schemaSerializationMode = SchemaSerializationMode.IncludeSchema;

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public ITMCOMDataSet()
    {
      this.BeginInit();
      this.InitClass();
      CollectionChangeEventHandler changeEventHandler = new CollectionChangeEventHandler(this.SchemaChanged);
      base.Tables.CollectionChanged += changeEventHandler;
      base.Relations.CollectionChanged += changeEventHandler;
      this.EndInit();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    protected ITMCOMDataSet(SerializationInfo info, StreamingContext context)
      : base(info, context, false)
    {
      if (this.IsBinarySerialized(info, context))
      {
        this.InitVars(false);
        CollectionChangeEventHandler changeEventHandler = new CollectionChangeEventHandler(this.SchemaChanged);
        this.Tables.CollectionChanged += changeEventHandler;
        this.Relations.CollectionChanged += changeEventHandler;
      }
      else
      {
        string s = (string) info.GetValue("XmlSchema", typeof (string));
        if (this.DetermineSchemaSerializationMode(info, context) == SchemaSerializationMode.IncludeSchema)
        {
          DataSet dataSet = new DataSet();
          dataSet.ReadXmlSchema((XmlReader) new XmlTextReader((TextReader) new StringReader(s)));
          if (dataSet.Tables[nameof (Config)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.ConfigDataTable(dataSet.Tables[nameof (Config)]));
          if (dataSet.Tables[nameof (Dynamic)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.DynamicDataTable(dataSet.Tables[nameof (Dynamic)]));
          if (dataSet.Tables[nameof (Static)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.StaticDataTable(dataSet.Tables[nameof (Static)]));
          if (dataSet.Tables[nameof (Log)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.LogDataTable(dataSet.Tables[nameof (Log)]));
          if (dataSet.Tables[nameof (Routing)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.RoutingDataTable(dataSet.Tables[nameof (Routing)]));
          if (dataSet.Tables[nameof (LogStatus)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.LogStatusDataTable(dataSet.Tables[nameof (LogStatus)]));
          if (dataSet.Tables[nameof (Port)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.PortDataTable(dataSet.Tables[nameof (Port)]));
          if (dataSet.Tables[nameof (Modem)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.ModemDataTable(dataSet.Tables[nameof (Modem)]));
          if (dataSet.Tables[nameof (IllegalModem)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.IllegalModemDataTable(dataSet.Tables[nameof (IllegalModem)]));
          if (dataSet.Tables[nameof (ConfigType)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.ConfigTypeDataTable(dataSet.Tables[nameof (ConfigType)]));
          if (dataSet.Tables[nameof (TCPIP)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.TCPIPDataTable(dataSet.Tables[nameof (TCPIP)]));
          if (dataSet.Tables[nameof (DynamicTCPIP)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.DynamicTCPIPDataTable(dataSet.Tables[nameof (DynamicTCPIP)]));
          if (dataSet.Tables[nameof (TCPIPClient)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.TCPIPClientDataTable(dataSet.Tables[nameof (TCPIPClient)]));
          if (dataSet.Tables[nameof (DynamicTCPIPClient)] != null)
            base.Tables.Add((DataTable) new ITMCOMDataSet.DynamicTCPIPClientDataTable(dataSet.Tables[nameof (DynamicTCPIPClient)]));
          this.DataSetName = dataSet.DataSetName;
          this.Prefix = dataSet.Prefix;
          this.Namespace = dataSet.Namespace;
          this.Locale = dataSet.Locale;
          this.CaseSensitive = dataSet.CaseSensitive;
          this.EnforceConstraints = dataSet.EnforceConstraints;
          this.Merge(dataSet, false, MissingSchemaAction.Add);
          this.InitVars();
        }
        else
          this.ReadXmlSchema((XmlReader) new XmlTextReader((TextReader) new StringReader(s)));
        this.GetSerializationData(info, context);
        CollectionChangeEventHandler changeEventHandler = new CollectionChangeEventHandler(this.SchemaChanged);
        base.Tables.CollectionChanged += changeEventHandler;
        this.Relations.CollectionChanged += changeEventHandler;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DebuggerNonUserCode]
    [Browsable(false)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public ITMCOMDataSet.ConfigDataTable Config => this.tableConfig;

    [Browsable(false)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ITMCOMDataSet.DynamicDataTable Dynamic => this.tableDynamic;

    [Browsable(false)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DebuggerNonUserCode]
    public ITMCOMDataSet.StaticDataTable Static => this.tableStatic;

    [DebuggerNonUserCode]
    [Browsable(false)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ITMCOMDataSet.LogDataTable Log => this.tableLog;

    [DebuggerNonUserCode]
    [Browsable(false)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ITMCOMDataSet.RoutingDataTable Routing => this.tableRouting;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DebuggerNonUserCode]
    public ITMCOMDataSet.LogStatusDataTable LogStatus => this.tableLogStatus;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public ITMCOMDataSet.PortDataTable Port => this.tablePort;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    public ITMCOMDataSet.ModemDataTable Modem => this.tableModem;

    [Browsable(false)]
    [DebuggerNonUserCode]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public ITMCOMDataSet.IllegalModemDataTable IllegalModem => this.tableIllegalModem;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    public ITMCOMDataSet.ConfigTypeDataTable ConfigType => this.tableConfigType;

    [Browsable(false)]
    [DebuggerNonUserCode]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public ITMCOMDataSet.TCPIPDataTable TCPIP => this.tableTCPIP;

    [Browsable(false)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ITMCOMDataSet.DynamicTCPIPDataTable DynamicTCPIP => this.tableDynamicTCPIP;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DebuggerNonUserCode]
    [Browsable(false)]
    public ITMCOMDataSet.TCPIPClientDataTable TCPIPClient => this.tableTCPIPClient;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    public ITMCOMDataSet.DynamicTCPIPClientDataTable DynamicTCPIPClient
    {
      get => this.tableDynamicTCPIPClient;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [Browsable(true)]
    [DebuggerNonUserCode]
    public override SchemaSerializationMode SchemaSerializationMode
    {
      get => this._schemaSerializationMode;
      set => this._schemaSerializationMode = value;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DebuggerNonUserCode]
    public new DataTableCollection Tables => base.Tables;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DebuggerNonUserCode]
    public new DataRelationCollection Relations => base.Relations;

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    protected override void InitializeDerivedDataSet()
    {
      this.BeginInit();
      this.InitClass();
      this.EndInit();
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    public override DataSet Clone()
    {
      ITMCOMDataSet itmcomDataSet = (ITMCOMDataSet) base.Clone();
      itmcomDataSet.InitVars();
      itmcomDataSet.SchemaSerializationMode = this.SchemaSerializationMode;
      return (DataSet) itmcomDataSet;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    protected override bool ShouldSerializeTables() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    protected override bool ShouldSerializeRelations() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    protected override void ReadXmlSerializable(XmlReader reader)
    {
      if (this.DetermineSchemaSerializationMode(reader) == SchemaSerializationMode.IncludeSchema)
      {
        this.Reset();
        DataSet dataSet = new DataSet();
        int num = (int) dataSet.ReadXml(reader);
        if (dataSet.Tables["Config"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.ConfigDataTable(dataSet.Tables["Config"]));
        if (dataSet.Tables["Dynamic"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.DynamicDataTable(dataSet.Tables["Dynamic"]));
        if (dataSet.Tables["Static"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.StaticDataTable(dataSet.Tables["Static"]));
        if (dataSet.Tables["Log"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.LogDataTable(dataSet.Tables["Log"]));
        if (dataSet.Tables["Routing"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.RoutingDataTable(dataSet.Tables["Routing"]));
        if (dataSet.Tables["LogStatus"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.LogStatusDataTable(dataSet.Tables["LogStatus"]));
        if (dataSet.Tables["Port"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.PortDataTable(dataSet.Tables["Port"]));
        if (dataSet.Tables["Modem"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.ModemDataTable(dataSet.Tables["Modem"]));
        if (dataSet.Tables["IllegalModem"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.IllegalModemDataTable(dataSet.Tables["IllegalModem"]));
        if (dataSet.Tables["ConfigType"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.ConfigTypeDataTable(dataSet.Tables["ConfigType"]));
        if (dataSet.Tables["TCPIP"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.TCPIPDataTable(dataSet.Tables["TCPIP"]));
        if (dataSet.Tables["DynamicTCPIP"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.DynamicTCPIPDataTable(dataSet.Tables["DynamicTCPIP"]));
        if (dataSet.Tables["TCPIPClient"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.TCPIPClientDataTable(dataSet.Tables["TCPIPClient"]));
        if (dataSet.Tables["DynamicTCPIPClient"] != null)
          base.Tables.Add((DataTable) new ITMCOMDataSet.DynamicTCPIPClientDataTable(dataSet.Tables["DynamicTCPIPClient"]));
        this.DataSetName = dataSet.DataSetName;
        this.Prefix = dataSet.Prefix;
        this.Namespace = dataSet.Namespace;
        this.Locale = dataSet.Locale;
        this.CaseSensitive = dataSet.CaseSensitive;
        this.EnforceConstraints = dataSet.EnforceConstraints;
        this.Merge(dataSet, false, MissingSchemaAction.Add);
        this.InitVars();
      }
      else
      {
        int num = (int) this.ReadXml(reader);
        this.InitVars();
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    protected override XmlSchema GetSchemaSerializable()
    {
      MemoryStream memoryStream = new MemoryStream();
      this.WriteXmlSchema((XmlWriter) new XmlTextWriter((Stream) memoryStream, (Encoding) null));
      memoryStream.Position = 0L;
      return XmlSchema.Read((XmlReader) new XmlTextReader((Stream) memoryStream), (ValidationEventHandler) null);
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    internal void InitVars() => this.InitVars(true);

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    internal void InitVars(bool initTable)
    {
      this.tableConfig = (ITMCOMDataSet.ConfigDataTable) base.Tables["Config"];
      if (initTable && this.tableConfig != null)
        this.tableConfig.InitVars();
      this.tableDynamic = (ITMCOMDataSet.DynamicDataTable) base.Tables["Dynamic"];
      if (initTable && this.tableDynamic != null)
        this.tableDynamic.InitVars();
      this.tableStatic = (ITMCOMDataSet.StaticDataTable) base.Tables["Static"];
      if (initTable && this.tableStatic != null)
        this.tableStatic.InitVars();
      this.tableLog = (ITMCOMDataSet.LogDataTable) base.Tables["Log"];
      if (initTable && this.tableLog != null)
        this.tableLog.InitVars();
      this.tableRouting = (ITMCOMDataSet.RoutingDataTable) base.Tables["Routing"];
      if (initTable && this.tableRouting != null)
        this.tableRouting.InitVars();
      this.tableLogStatus = (ITMCOMDataSet.LogStatusDataTable) base.Tables["LogStatus"];
      if (initTable && this.tableLogStatus != null)
        this.tableLogStatus.InitVars();
      this.tablePort = (ITMCOMDataSet.PortDataTable) base.Tables["Port"];
      if (initTable && this.tablePort != null)
        this.tablePort.InitVars();
      this.tableModem = (ITMCOMDataSet.ModemDataTable) base.Tables["Modem"];
      if (initTable && this.tableModem != null)
        this.tableModem.InitVars();
      this.tableIllegalModem = (ITMCOMDataSet.IllegalModemDataTable) base.Tables["IllegalModem"];
      if (initTable && this.tableIllegalModem != null)
        this.tableIllegalModem.InitVars();
      this.tableConfigType = (ITMCOMDataSet.ConfigTypeDataTable) base.Tables["ConfigType"];
      if (initTable && this.tableConfigType != null)
        this.tableConfigType.InitVars();
      this.tableTCPIP = (ITMCOMDataSet.TCPIPDataTable) base.Tables["TCPIP"];
      if (initTable && this.tableTCPIP != null)
        this.tableTCPIP.InitVars();
      this.tableDynamicTCPIP = (ITMCOMDataSet.DynamicTCPIPDataTable) base.Tables["DynamicTCPIP"];
      if (initTable && this.tableDynamicTCPIP != null)
        this.tableDynamicTCPIP.InitVars();
      this.tableTCPIPClient = (ITMCOMDataSet.TCPIPClientDataTable) base.Tables["TCPIPClient"];
      if (initTable && this.tableTCPIPClient != null)
        this.tableTCPIPClient.InitVars();
      this.tableDynamicTCPIPClient = (ITMCOMDataSet.DynamicTCPIPClientDataTable) base.Tables["DynamicTCPIPClient"];
      if (initTable && this.tableDynamicTCPIPClient != null)
        this.tableDynamicTCPIPClient.InitVars();
      this.relationFK_Log_LogStatus = this.Relations["FK_Log_LogStatus"];
      this.relationConfigType_Config = this.Relations["ConfigType_Config"];
    }

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    private void InitClass()
    {
      this.DataSetName = nameof (ITMCOMDataSet);
      this.Prefix = "";
      this.Namespace = "http://tempuri.org/ITMCOMDataSet.xsd";
      this.EnforceConstraints = true;
      this.SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
      this.tableConfig = new ITMCOMDataSet.ConfigDataTable();
      base.Tables.Add((DataTable) this.tableConfig);
      this.tableDynamic = new ITMCOMDataSet.DynamicDataTable();
      base.Tables.Add((DataTable) this.tableDynamic);
      this.tableStatic = new ITMCOMDataSet.StaticDataTable();
      base.Tables.Add((DataTable) this.tableStatic);
      this.tableLog = new ITMCOMDataSet.LogDataTable();
      base.Tables.Add((DataTable) this.tableLog);
      this.tableRouting = new ITMCOMDataSet.RoutingDataTable();
      base.Tables.Add((DataTable) this.tableRouting);
      this.tableLogStatus = new ITMCOMDataSet.LogStatusDataTable();
      base.Tables.Add((DataTable) this.tableLogStatus);
      this.tablePort = new ITMCOMDataSet.PortDataTable();
      base.Tables.Add((DataTable) this.tablePort);
      this.tableModem = new ITMCOMDataSet.ModemDataTable();
      base.Tables.Add((DataTable) this.tableModem);
      this.tableIllegalModem = new ITMCOMDataSet.IllegalModemDataTable();
      base.Tables.Add((DataTable) this.tableIllegalModem);
      this.tableConfigType = new ITMCOMDataSet.ConfigTypeDataTable();
      base.Tables.Add((DataTable) this.tableConfigType);
      this.tableTCPIP = new ITMCOMDataSet.TCPIPDataTable();
      base.Tables.Add((DataTable) this.tableTCPIP);
      this.tableDynamicTCPIP = new ITMCOMDataSet.DynamicTCPIPDataTable();
      base.Tables.Add((DataTable) this.tableDynamicTCPIP);
      this.tableTCPIPClient = new ITMCOMDataSet.TCPIPClientDataTable();
      base.Tables.Add((DataTable) this.tableTCPIPClient);
      this.tableDynamicTCPIPClient = new ITMCOMDataSet.DynamicTCPIPClientDataTable();
      base.Tables.Add((DataTable) this.tableDynamicTCPIPClient);
      this.relationFK_Log_LogStatus = new DataRelation("FK_Log_LogStatus", new DataColumn[1]
      {
        this.tableLogStatus.LogStatusIdColumn
      }, new DataColumn[1]{ this.tableLog.StatusIdColumn }, false);
      this.Relations.Add(this.relationFK_Log_LogStatus);
      this.relationConfigType_Config = new DataRelation("ConfigType_Config", new DataColumn[1]
      {
        this.tableConfigType.ConfigTypeIdColumn
      }, new DataColumn[1]{ this.tableConfig.TypeIdColumn }, false);
      this.Relations.Add(this.relationConfigType_Config);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    private bool ShouldSerializeConfig() => false;

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    private bool ShouldSerializeDynamic() => false;

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    private bool ShouldSerializeStatic() => false;

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    private bool ShouldSerializeLog() => false;

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    private bool ShouldSerializeRouting() => false;

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    private bool ShouldSerializeLogStatus() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    private bool ShouldSerializePort() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    private bool ShouldSerializeModem() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    private bool ShouldSerializeIllegalModem() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    private bool ShouldSerializeConfigType() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    private bool ShouldSerializeTCPIP() => false;

    [DebuggerNonUserCode]
    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    private bool ShouldSerializeDynamicTCPIP() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    private bool ShouldSerializeTCPIPClient() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    private bool ShouldSerializeDynamicTCPIPClient() => false;

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    private void SchemaChanged(object sender, CollectionChangeEventArgs e)
    {
      if (e.Action != CollectionChangeAction.Remove)
        return;
      this.InitVars();
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    [DebuggerNonUserCode]
    public static XmlSchemaComplexType GetTypedDataSetSchema(XmlSchemaSet xs)
    {
      ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
      XmlSchemaComplexType typedDataSetSchema = new XmlSchemaComplexType();
      XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
      xmlSchemaSequence.Items.Add((XmlSchemaObject) new XmlSchemaAny()
      {
        Namespace = itmcomDataSet.Namespace
      });
      typedDataSetSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
      XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
      if (xs.Contains(schemaSerializable.TargetNamespace))
      {
        MemoryStream memoryStream1 = new MemoryStream();
        MemoryStream memoryStream2 = new MemoryStream();
        try
        {
          schemaSerializable.Write((Stream) memoryStream1);
          IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
          while (enumerator.MoveNext())
          {
            XmlSchema current = (XmlSchema) enumerator.Current;
            memoryStream2.SetLength(0L);
            current.Write((Stream) memoryStream2);
            if (memoryStream1.Length == memoryStream2.Length)
            {
              memoryStream1.Position = 0L;
              memoryStream2.Position = 0L;
              do
                ;
              while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
              if (memoryStream1.Position == memoryStream1.Length)
                return typedDataSetSchema;
            }
          }
        }
        finally
        {
          memoryStream1?.Close();
          memoryStream2?.Close();
        }
      }
      xs.Add(schemaSerializable);
      return typedDataSetSchema;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void ConfigRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.ConfigRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void DynamicRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.DynamicRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void StaticRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.StaticRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void LogRowChangeEventHandler(object sender, ITMCOMDataSet.LogRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void RoutingRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.RoutingRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void LogStatusRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.LogStatusRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void PortRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.PortRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void ModemRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.ModemRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void IllegalModemRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.IllegalModemRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void ConfigTypeRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.ConfigTypeRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void TCPIPRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.TCPIPRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void DynamicTCPIPRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.DynamicTCPIPRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void TCPIPClientRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.TCPIPClientRowChangeEvent e);

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public delegate void DynamicTCPIPClientRowChangeEventHandler(
      object sender,
      ITMCOMDataSet.DynamicTCPIPClientRowChangeEvent e);

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class ConfigDataTable : TypedTableBase<ITMCOMDataSet.ConfigRow>
    {
      private DataColumn columnConfigId;
      private DataColumn columnIPAddress;
      private DataColumn columnPortNumber;
      private DataColumn columnServerID;
      private DataColumn columnmbCurrent;
      private DataColumn columnTypeId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ConfigDataTable()
      {
        this.TableName = "Config";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal ConfigDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected ConfigDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ConfigIdColumn => this.columnConfigId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn IPAddressColumn => this.columnIPAddress;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn PortNumberColumn => this.columnPortNumber;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ServerIDColumn => this.columnServerID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn mbCurrentColumn => this.columnmbCurrent;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn TypeIdColumn => this.columnTypeId;

      [DebuggerNonUserCode]
      [Browsable(false)]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Count => this.Rows.Count;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.ConfigRow this[int index] => (ITMCOMDataSet.ConfigRow) this.Rows[index];

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ConfigRowChangeEventHandler ConfigRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ConfigRowChangeEventHandler ConfigRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ConfigRowChangeEventHandler ConfigRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ConfigRowChangeEventHandler ConfigRowDeleted;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void AddConfigRow(ITMCOMDataSet.ConfigRow row) => this.Rows.Add((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.ConfigRow AddConfigRow(
        string IPAddress,
        int PortNumber,
        string ServerID,
        bool mbCurrent,
        ITMCOMDataSet.ConfigTypeRow parentConfigTypeRowByConfigType_Config)
      {
        ITMCOMDataSet.ConfigRow row = (ITMCOMDataSet.ConfigRow) this.NewRow();
        object[] objArray = new object[6]
        {
          null,
          (object) IPAddress,
          (object) PortNumber,
          (object) ServerID,
          (object) mbCurrent,
          null
        };
        if (parentConfigTypeRowByConfigType_Config != null)
          objArray[5] = parentConfigTypeRowByConfigType_Config[0];
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ConfigRow FindByConfigId(int ConfigId)
      {
        return (ITMCOMDataSet.ConfigRow) this.Rows.Find(new object[1]
        {
          (object) ConfigId
        });
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public override DataTable Clone()
      {
        ITMCOMDataSet.ConfigDataTable configDataTable = (ITMCOMDataSet.ConfigDataTable) base.Clone();
        configDataTable.InitVars();
        return (DataTable) configDataTable;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.ConfigDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnConfigId = this.Columns["ConfigId"];
        this.columnIPAddress = this.Columns["IPAddress"];
        this.columnPortNumber = this.Columns["PortNumber"];
        this.columnServerID = this.Columns["ServerID"];
        this.columnmbCurrent = this.Columns["mbCurrent"];
        this.columnTypeId = this.Columns["TypeId"];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      private void InitClass()
      {
        this.columnConfigId = new DataColumn("ConfigId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConfigId);
        this.columnIPAddress = new DataColumn("IPAddress", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnIPAddress);
        this.columnPortNumber = new DataColumn("PortNumber", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnPortNumber);
        this.columnServerID = new DataColumn("ServerID", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnServerID);
        this.columnmbCurrent = new DataColumn("mbCurrent", typeof (bool), (string) null, MappingType.Element);
        this.Columns.Add(this.columnmbCurrent);
        this.columnTypeId = new DataColumn("TypeId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnTypeId);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnConfigId
        }, true));
        this.columnConfigId.AutoIncrement = true;
        this.columnConfigId.AllowDBNull = false;
        this.columnConfigId.ReadOnly = true;
        this.columnConfigId.Unique = true;
        this.columnIPAddress.AllowDBNull = false;
        this.columnIPAddress.MaxLength = 20;
        this.columnPortNumber.AllowDBNull = false;
        this.columnServerID.AllowDBNull = false;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ConfigRow NewConfigRow() => (ITMCOMDataSet.ConfigRow) this.NewRow();

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.ConfigRow(builder);
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.ConfigRow);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.ConfigRowChanged == null)
          return;
        this.ConfigRowChanged((object) this, new ITMCOMDataSet.ConfigRowChangeEvent((ITMCOMDataSet.ConfigRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.ConfigRowChanging == null)
          return;
        this.ConfigRowChanging((object) this, new ITMCOMDataSet.ConfigRowChangeEvent((ITMCOMDataSet.ConfigRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.ConfigRowDeleted == null)
          return;
        this.ConfigRowDeleted((object) this, new ITMCOMDataSet.ConfigRowChangeEvent((ITMCOMDataSet.ConfigRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.ConfigRowDeleting == null)
          return;
        this.ConfigRowDeleting((object) this, new ITMCOMDataSet.ConfigRowChangeEvent((ITMCOMDataSet.ConfigRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveConfigRow(ITMCOMDataSet.ConfigRow row) => this.Rows.Remove((DataRow) row);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (ConfigDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class DynamicDataTable : TypedTableBase<ITMCOMDataSet.DynamicRow>
    {
      private DataColumn columnDynamicId;
      private DataColumn columnSiteID;
      private DataColumn columnModemIP;
      private DataColumn columnModemID;
      private DataColumn columnStatisticsId;
      private DataColumn columnConnectionTime;
      private DataColumn columnReleaseTime;
      private DataColumn columnInBatchCount;
      private DataColumn columnOutBatchCount;
      private DataColumn columnInTraffic;
      private DataColumn columnOutTraffic;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DynamicDataTable()
      {
        this.TableName = "Dynamic";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal DynamicDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected DynamicDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn DynamicIdColumn => this.columnDynamicId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ModemIPColumn => this.columnModemIP;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ModemIDColumn => this.columnModemID;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn StatisticsIdColumn => this.columnStatisticsId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ConnectionTimeColumn => this.columnConnectionTime;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ReleaseTimeColumn => this.columnReleaseTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn InBatchCountColumn => this.columnInBatchCount;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn OutBatchCountColumn => this.columnOutBatchCount;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn InTrafficColumn => this.columnInTraffic;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn OutTrafficColumn => this.columnOutTraffic;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [Browsable(false)]
      public int Count => this.Rows.Count;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicRow this[int index]
      {
        get => (ITMCOMDataSet.DynamicRow) this.Rows[index];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicRowChangeEventHandler DynamicRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicRowChangeEventHandler DynamicRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicRowChangeEventHandler DynamicRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicRowChangeEventHandler DynamicRowDeleted;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void AddDynamicRow(ITMCOMDataSet.DynamicRow row) => this.Rows.Add((DataRow) row);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicRow AddDynamicRow(
        int SiteID,
        string ModemIP,
        string ModemID,
        int StatisticsId,
        DateTime ConnectionTime,
        DateTime ReleaseTime,
        int InBatchCount,
        int OutBatchCount,
        int InTraffic,
        int OutTraffic)
      {
        ITMCOMDataSet.DynamicRow row = (ITMCOMDataSet.DynamicRow) this.NewRow();
        object[] objArray = new object[11]
        {
          null,
          (object) SiteID,
          (object) ModemIP,
          (object) ModemID,
          (object) StatisticsId,
          (object) ConnectionTime,
          (object) ReleaseTime,
          (object) InBatchCount,
          (object) OutBatchCount,
          (object) InTraffic,
          (object) OutTraffic
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicRow FindByDynamicId(int DynamicId)
      {
        return (ITMCOMDataSet.DynamicRow) this.Rows.Find(new object[1]
        {
          (object) DynamicId
        });
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public override DataTable Clone()
      {
        ITMCOMDataSet.DynamicDataTable dynamicDataTable = (ITMCOMDataSet.DynamicDataTable) base.Clone();
        dynamicDataTable.InitVars();
        return (DataTable) dynamicDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.DynamicDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnDynamicId = this.Columns["DynamicId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnModemIP = this.Columns["ModemIP"];
        this.columnModemID = this.Columns["ModemID"];
        this.columnStatisticsId = this.Columns["StatisticsId"];
        this.columnConnectionTime = this.Columns["ConnectionTime"];
        this.columnReleaseTime = this.Columns["ReleaseTime"];
        this.columnInBatchCount = this.Columns["InBatchCount"];
        this.columnOutBatchCount = this.Columns["OutBatchCount"];
        this.columnInTraffic = this.Columns["InTraffic"];
        this.columnOutTraffic = this.Columns["OutTraffic"];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      private void InitClass()
      {
        this.columnDynamicId = new DataColumn("DynamicId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnDynamicId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnModemIP = new DataColumn("ModemIP", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemIP);
        this.columnModemID = new DataColumn("ModemID", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemID);
        this.columnStatisticsId = new DataColumn("StatisticsId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnStatisticsId);
        this.columnConnectionTime = new DataColumn("ConnectionTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConnectionTime);
        this.columnReleaseTime = new DataColumn("ReleaseTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnReleaseTime);
        this.columnInBatchCount = new DataColumn("InBatchCount", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnInBatchCount);
        this.columnOutBatchCount = new DataColumn("OutBatchCount", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnOutBatchCount);
        this.columnInTraffic = new DataColumn("InTraffic", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnInTraffic);
        this.columnOutTraffic = new DataColumn("OutTraffic", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnOutTraffic);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnDynamicId
        }, true));
        this.columnDynamicId.AutoIncrement = true;
        this.columnDynamicId.AllowDBNull = false;
        this.columnDynamicId.ReadOnly = true;
        this.columnDynamicId.Unique = true;
        this.columnSiteID.AllowDBNull = false;
        this.columnModemIP.AllowDBNull = false;
        this.columnModemIP.MaxLength = 20;
        this.columnModemID.AllowDBNull = false;
        this.columnModemID.MaxLength = 20;
        this.columnStatisticsId.AllowDBNull = false;
        this.columnConnectionTime.AllowDBNull = false;
        this.columnInBatchCount.AllowDBNull = false;
        this.columnOutBatchCount.AllowDBNull = false;
        this.columnInTraffic.AllowDBNull = false;
        this.columnOutTraffic.AllowDBNull = false;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicRow NewDynamicRow() => (ITMCOMDataSet.DynamicRow) this.NewRow();

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.DynamicRow(builder);
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.DynamicRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.DynamicRowChanged == null)
          return;
        this.DynamicRowChanged((object) this, new ITMCOMDataSet.DynamicRowChangeEvent((ITMCOMDataSet.DynamicRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.DynamicRowChanging == null)
          return;
        this.DynamicRowChanging((object) this, new ITMCOMDataSet.DynamicRowChangeEvent((ITMCOMDataSet.DynamicRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.DynamicRowDeleted == null)
          return;
        this.DynamicRowDeleted((object) this, new ITMCOMDataSet.DynamicRowChangeEvent((ITMCOMDataSet.DynamicRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.DynamicRowDeleting == null)
          return;
        this.DynamicRowDeleting((object) this, new ITMCOMDataSet.DynamicRowChangeEvent((ITMCOMDataSet.DynamicRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveDynamicRow(ITMCOMDataSet.DynamicRow row) => this.Rows.Remove((DataRow) row);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (DynamicDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class StaticDataTable : TypedTableBase<ITMCOMDataSet.StaticRow>
    {
      private DataColumn columnStaticId;
      private DataColumn columnCOMPort;
      private DataColumn columnStatisticsId;
      private DataColumn columnConnectionTime;
      private DataColumn columnReleaseTime;
      private DataColumn columnInBatchCount;
      private DataColumn columnOutBatchCount;
      private DataColumn columnInTraffic;
      private DataColumn columnOutTraffic;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public StaticDataTable()
      {
        this.TableName = "Static";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal StaticDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected StaticDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn StaticIdColumn => this.columnStaticId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn COMPortColumn => this.columnCOMPort;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn StatisticsIdColumn => this.columnStatisticsId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ConnectionTimeColumn => this.columnConnectionTime;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ReleaseTimeColumn => this.columnReleaseTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn InBatchCountColumn => this.columnInBatchCount;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn OutBatchCountColumn => this.columnOutBatchCount;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn InTrafficColumn => this.columnInTraffic;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn OutTrafficColumn => this.columnOutTraffic;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [Browsable(false)]
      public int Count => this.Rows.Count;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.StaticRow this[int index] => (ITMCOMDataSet.StaticRow) this.Rows[index];

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.StaticRowChangeEventHandler StaticRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.StaticRowChangeEventHandler StaticRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.StaticRowChangeEventHandler StaticRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.StaticRowChangeEventHandler StaticRowDeleted;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void AddStaticRow(ITMCOMDataSet.StaticRow row) => this.Rows.Add((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.StaticRow AddStaticRow(
        string COMPort,
        int StatisticsId,
        DateTime ConnectionTime,
        DateTime ReleaseTime,
        int InBatchCount,
        int OutBatchCount,
        int InTraffic,
        int OutTraffic)
      {
        ITMCOMDataSet.StaticRow row = (ITMCOMDataSet.StaticRow) this.NewRow();
        object[] objArray = new object[9]
        {
          null,
          (object) COMPort,
          (object) StatisticsId,
          (object) ConnectionTime,
          (object) ReleaseTime,
          (object) InBatchCount,
          (object) OutBatchCount,
          (object) InTraffic,
          (object) OutTraffic
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.StaticRow FindByStaticId(int StaticId)
      {
        return (ITMCOMDataSet.StaticRow) this.Rows.Find(new object[1]
        {
          (object) StaticId
        });
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public override DataTable Clone()
      {
        ITMCOMDataSet.StaticDataTable staticDataTable = (ITMCOMDataSet.StaticDataTable) base.Clone();
        staticDataTable.InitVars();
        return (DataTable) staticDataTable;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.StaticDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnStaticId = this.Columns["StaticId"];
        this.columnCOMPort = this.Columns["COMPort"];
        this.columnStatisticsId = this.Columns["StatisticsId"];
        this.columnConnectionTime = this.Columns["ConnectionTime"];
        this.columnReleaseTime = this.Columns["ReleaseTime"];
        this.columnInBatchCount = this.Columns["InBatchCount"];
        this.columnOutBatchCount = this.Columns["OutBatchCount"];
        this.columnInTraffic = this.Columns["InTraffic"];
        this.columnOutTraffic = this.Columns["OutTraffic"];
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      private void InitClass()
      {
        this.columnStaticId = new DataColumn("StaticId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnStaticId);
        this.columnCOMPort = new DataColumn("COMPort", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnCOMPort);
        this.columnStatisticsId = new DataColumn("StatisticsId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnStatisticsId);
        this.columnConnectionTime = new DataColumn("ConnectionTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConnectionTime);
        this.columnReleaseTime = new DataColumn("ReleaseTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnReleaseTime);
        this.columnInBatchCount = new DataColumn("InBatchCount", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnInBatchCount);
        this.columnOutBatchCount = new DataColumn("OutBatchCount", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnOutBatchCount);
        this.columnInTraffic = new DataColumn("InTraffic", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnInTraffic);
        this.columnOutTraffic = new DataColumn("OutTraffic", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnOutTraffic);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnStaticId
        }, true));
        this.columnStaticId.AutoIncrement = true;
        this.columnStaticId.AllowDBNull = false;
        this.columnStaticId.ReadOnly = true;
        this.columnStaticId.Unique = true;
        this.columnCOMPort.AllowDBNull = false;
        this.columnCOMPort.MaxLength = 20;
        this.columnStatisticsId.AllowDBNull = false;
        this.columnConnectionTime.AllowDBNull = false;
        this.columnInBatchCount.AllowDBNull = false;
        this.columnOutBatchCount.AllowDBNull = false;
        this.columnInTraffic.AllowDBNull = false;
        this.columnOutTraffic.AllowDBNull = false;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.StaticRow NewStaticRow() => (ITMCOMDataSet.StaticRow) this.NewRow();

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.StaticRow(builder);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.StaticRow);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.StaticRowChanged == null)
          return;
        this.StaticRowChanged((object) this, new ITMCOMDataSet.StaticRowChangeEvent((ITMCOMDataSet.StaticRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.StaticRowChanging == null)
          return;
        this.StaticRowChanging((object) this, new ITMCOMDataSet.StaticRowChangeEvent((ITMCOMDataSet.StaticRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.StaticRowDeleted == null)
          return;
        this.StaticRowDeleted((object) this, new ITMCOMDataSet.StaticRowChangeEvent((ITMCOMDataSet.StaticRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.StaticRowDeleting == null)
          return;
        this.StaticRowDeleting((object) this, new ITMCOMDataSet.StaticRowChangeEvent((ITMCOMDataSet.StaticRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void RemoveStaticRow(ITMCOMDataSet.StaticRow row) => this.Rows.Remove((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (StaticDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class LogDataTable : TypedTableBase<ITMCOMDataSet.LogRow>
    {
      private DataColumn columnLogId;
      private DataColumn columnSiteID;
      private DataColumn columnCOMPort;
      private DataColumn columnMessage;
      private DataColumn columnStatusId;
      private DataColumn columnTime;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public LogDataTable()
      {
        this.TableName = "Log";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal LogDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected LogDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn LogIdColumn => this.columnLogId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn COMPortColumn => this.columnCOMPort;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn MessageColumn => this.columnMessage;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn StatusIdColumn => this.columnStatusId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn TimeColumn => this.columnTime;

      [DebuggerNonUserCode]
      [Browsable(false)]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Count => this.Rows.Count;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.LogRow this[int index] => (ITMCOMDataSet.LogRow) this.Rows[index];

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.LogRowChangeEventHandler LogRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.LogRowChangeEventHandler LogRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.LogRowChangeEventHandler LogRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.LogRowChangeEventHandler LogRowDeleted;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void AddLogRow(ITMCOMDataSet.LogRow row) => this.Rows.Add((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.LogRow AddLogRow(
        int SiteID,
        string COMPort,
        string Message,
        ITMCOMDataSet.LogStatusRow parentLogStatusRowByFK_Log_LogStatus,
        DateTime Time)
      {
        ITMCOMDataSet.LogRow row = (ITMCOMDataSet.LogRow) this.NewRow();
        object[] objArray = new object[6]
        {
          null,
          (object) SiteID,
          (object) COMPort,
          (object) Message,
          null,
          (object) Time
        };
        if (parentLogStatusRowByFK_Log_LogStatus != null)
          objArray[4] = parentLogStatusRowByFK_Log_LogStatus[0];
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.LogRow FindByLogId(int LogId)
      {
        return (ITMCOMDataSet.LogRow) this.Rows.Find(new object[1]
        {
          (object) LogId
        });
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public override DataTable Clone()
      {
        ITMCOMDataSet.LogDataTable logDataTable = (ITMCOMDataSet.LogDataTable) base.Clone();
        logDataTable.InitVars();
        return (DataTable) logDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance() => (DataTable) new ITMCOMDataSet.LogDataTable();

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal void InitVars()
      {
        this.columnLogId = this.Columns["LogId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnCOMPort = this.Columns["COMPort"];
        this.columnMessage = this.Columns["Message"];
        this.columnStatusId = this.Columns["StatusId"];
        this.columnTime = this.Columns["Time"];
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      private void InitClass()
      {
        this.columnLogId = new DataColumn("LogId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnLogId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnCOMPort = new DataColumn("COMPort", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnCOMPort);
        this.columnMessage = new DataColumn("Message", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnMessage);
        this.columnStatusId = new DataColumn("StatusId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnStatusId);
        this.columnTime = new DataColumn("Time", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnTime);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnLogId
        }, true));
        this.columnLogId.AutoIncrement = true;
        this.columnLogId.AllowDBNull = false;
        this.columnLogId.ReadOnly = true;
        this.columnLogId.Unique = true;
        this.columnCOMPort.MaxLength = 20;
        this.columnMessage.AllowDBNull = false;
        this.columnMessage.MaxLength = 1024;
        this.columnStatusId.AllowDBNull = false;
        this.columnTime.AllowDBNull = false;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.LogRow NewLogRow() => (ITMCOMDataSet.LogRow) this.NewRow();

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.LogRow(builder);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.LogRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.LogRowChanged == null)
          return;
        this.LogRowChanged((object) this, new ITMCOMDataSet.LogRowChangeEvent((ITMCOMDataSet.LogRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.LogRowChanging == null)
          return;
        this.LogRowChanging((object) this, new ITMCOMDataSet.LogRowChangeEvent((ITMCOMDataSet.LogRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.LogRowDeleted == null)
          return;
        this.LogRowDeleted((object) this, new ITMCOMDataSet.LogRowChangeEvent((ITMCOMDataSet.LogRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.LogRowDeleting == null)
          return;
        this.LogRowDeleting((object) this, new ITMCOMDataSet.LogRowChangeEvent((ITMCOMDataSet.LogRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveLogRow(ITMCOMDataSet.LogRow row) => this.Rows.Remove((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (LogDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class RoutingDataTable : TypedTableBase<ITMCOMDataSet.RoutingRow>
    {
      private DataColumn columnRoutingId;
      private DataColumn columnSiteID;
      private DataColumn columnLinkID;
      private DataColumn columnPortId;
      private DataColumn columnBeginRange;
      private DataColumn columnEndRange;
      private DataColumn columnLocalSiteID;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public RoutingDataTable()
      {
        this.TableName = "Routing";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal RoutingDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected RoutingDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn RoutingIdColumn => this.columnRoutingId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn LinkIDColumn => this.columnLinkID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn PortIdColumn => this.columnPortId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn BeginRangeColumn => this.columnBeginRange;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn EndRangeColumn => this.columnEndRange;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn LocalSiteIDColumn => this.columnLocalSiteID;

      [Browsable(false)]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int Count => this.Rows.Count;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.RoutingRow this[int index]
      {
        get => (ITMCOMDataSet.RoutingRow) this.Rows[index];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.RoutingRowChangeEventHandler RoutingRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.RoutingRowChangeEventHandler RoutingRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.RoutingRowChangeEventHandler RoutingRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.RoutingRowChangeEventHandler RoutingRowDeleted;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void AddRoutingRow(ITMCOMDataSet.RoutingRow row) => this.Rows.Add((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.RoutingRow AddRoutingRow(
        int SiteID,
        int LinkID,
        int PortId,
        int BeginRange,
        int EndRange,
        int LocalSiteID)
      {
        ITMCOMDataSet.RoutingRow row = (ITMCOMDataSet.RoutingRow) this.NewRow();
        object[] objArray = new object[7]
        {
          null,
          (object) SiteID,
          (object) LinkID,
          (object) PortId,
          (object) BeginRange,
          (object) EndRange,
          (object) LocalSiteID
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.RoutingRow FindByRoutingId(int RoutingId)
      {
        return (ITMCOMDataSet.RoutingRow) this.Rows.Find(new object[1]
        {
          (object) RoutingId
        });
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public override DataTable Clone()
      {
        ITMCOMDataSet.RoutingDataTable routingDataTable = (ITMCOMDataSet.RoutingDataTable) base.Clone();
        routingDataTable.InitVars();
        return (DataTable) routingDataTable;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.RoutingDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnRoutingId = this.Columns["RoutingId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnLinkID = this.Columns["LinkID"];
        this.columnPortId = this.Columns["PortId"];
        this.columnBeginRange = this.Columns["BeginRange"];
        this.columnEndRange = this.Columns["EndRange"];
        this.columnLocalSiteID = this.Columns["LocalSiteID"];
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      private void InitClass()
      {
        this.columnRoutingId = new DataColumn("RoutingId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnRoutingId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnLinkID = new DataColumn("LinkID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnLinkID);
        this.columnPortId = new DataColumn("PortId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnPortId);
        this.columnBeginRange = new DataColumn("BeginRange", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnBeginRange);
        this.columnEndRange = new DataColumn("EndRange", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnEndRange);
        this.columnLocalSiteID = new DataColumn("LocalSiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnLocalSiteID);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnRoutingId
        }, true));
        this.columnRoutingId.AutoIncrement = true;
        this.columnRoutingId.AllowDBNull = false;
        this.columnRoutingId.ReadOnly = true;
        this.columnRoutingId.Unique = true;
        this.columnSiteID.AllowDBNull = false;
        this.columnLinkID.AllowDBNull = false;
        this.columnPortId.AllowDBNull = false;
        this.columnBeginRange.DefaultValue = (object) 0;
        this.columnEndRange.DefaultValue = (object) 0;
        this.columnLocalSiteID.DefaultValue = (object) 0;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.RoutingRow NewRoutingRow() => (ITMCOMDataSet.RoutingRow) this.NewRow();

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.RoutingRow(builder);
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.RoutingRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.RoutingRowChanged == null)
          return;
        this.RoutingRowChanged((object) this, new ITMCOMDataSet.RoutingRowChangeEvent((ITMCOMDataSet.RoutingRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.RoutingRowChanging == null)
          return;
        this.RoutingRowChanging((object) this, new ITMCOMDataSet.RoutingRowChangeEvent((ITMCOMDataSet.RoutingRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.RoutingRowDeleted == null)
          return;
        this.RoutingRowDeleted((object) this, new ITMCOMDataSet.RoutingRowChangeEvent((ITMCOMDataSet.RoutingRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.RoutingRowDeleting == null)
          return;
        this.RoutingRowDeleting((object) this, new ITMCOMDataSet.RoutingRowChangeEvent((ITMCOMDataSet.RoutingRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveRoutingRow(ITMCOMDataSet.RoutingRow row) => this.Rows.Remove((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (RoutingDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class LogStatusDataTable : TypedTableBase<ITMCOMDataSet.LogStatusRow>
    {
      private DataColumn columnLogStatusId;
      private DataColumn columnName;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public LogStatusDataTable()
      {
        this.TableName = "LogStatus";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal LogStatusDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected LogStatusDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn LogStatusIdColumn => this.columnLogStatusId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn NameColumn => this.columnName;

      [DebuggerNonUserCode]
      [Browsable(false)]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Count => this.Rows.Count;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.LogStatusRow this[int index]
      {
        get => (ITMCOMDataSet.LogStatusRow) this.Rows[index];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.LogStatusRowChangeEventHandler LogStatusRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.LogStatusRowChangeEventHandler LogStatusRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.LogStatusRowChangeEventHandler LogStatusRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.LogStatusRowChangeEventHandler LogStatusRowDeleted;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void AddLogStatusRow(ITMCOMDataSet.LogStatusRow row) => this.Rows.Add((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.LogStatusRow AddLogStatusRow(string Name)
      {
        ITMCOMDataSet.LogStatusRow row = (ITMCOMDataSet.LogStatusRow) this.NewRow();
        object[] objArray = new object[2]
        {
          null,
          (object) Name
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.LogStatusRow FindByLogStatusId(int LogStatusId)
      {
        return (ITMCOMDataSet.LogStatusRow) this.Rows.Find(new object[1]
        {
          (object) LogStatusId
        });
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public override DataTable Clone()
      {
        ITMCOMDataSet.LogStatusDataTable logStatusDataTable = (ITMCOMDataSet.LogStatusDataTable) base.Clone();
        logStatusDataTable.InitVars();
        return (DataTable) logStatusDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.LogStatusDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnLogStatusId = this.Columns["LogStatusId"];
        this.columnName = this.Columns["Name"];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      private void InitClass()
      {
        this.columnLogStatusId = new DataColumn("LogStatusId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnLogStatusId);
        this.columnName = new DataColumn("Name", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnName);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnLogStatusId
        }, true));
        this.columnLogStatusId.AutoIncrement = true;
        this.columnLogStatusId.AllowDBNull = false;
        this.columnLogStatusId.ReadOnly = true;
        this.columnLogStatusId.Unique = true;
        this.columnName.AllowDBNull = false;
        this.columnName.MaxLength = 50;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.LogStatusRow NewLogStatusRow()
      {
        return (ITMCOMDataSet.LogStatusRow) this.NewRow();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.LogStatusRow(builder);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.LogStatusRow);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.LogStatusRowChanged == null)
          return;
        this.LogStatusRowChanged((object) this, new ITMCOMDataSet.LogStatusRowChangeEvent((ITMCOMDataSet.LogStatusRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.LogStatusRowChanging == null)
          return;
        this.LogStatusRowChanging((object) this, new ITMCOMDataSet.LogStatusRowChangeEvent((ITMCOMDataSet.LogStatusRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.LogStatusRowDeleted == null)
          return;
        this.LogStatusRowDeleted((object) this, new ITMCOMDataSet.LogStatusRowChangeEvent((ITMCOMDataSet.LogStatusRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.LogStatusRowDeleting == null)
          return;
        this.LogStatusRowDeleting((object) this, new ITMCOMDataSet.LogStatusRowChangeEvent((ITMCOMDataSet.LogStatusRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void RemoveLogStatusRow(ITMCOMDataSet.LogStatusRow row)
      {
        this.Rows.Remove((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (LogStatusDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class PortDataTable : TypedTableBase<ITMCOMDataSet.PortRow>
    {
      private DataColumn columnPortId;
      private DataColumn columnPortName;
      private DataColumn columnBaudRate;
      private DataColumn columnParity;
      private DataColumn columnDataBits;
      private DataColumn columnStopBits;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public PortDataTable()
      {
        this.TableName = "Port";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal PortDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected PortDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn PortIdColumn => this.columnPortId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn PortNameColumn => this.columnPortName;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn BaudRateColumn => this.columnBaudRate;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ParityColumn => this.columnParity;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn DataBitsColumn => this.columnDataBits;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn StopBitsColumn => this.columnStopBits;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      [Browsable(false)]
      public int Count => this.Rows.Count;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.PortRow this[int index] => (ITMCOMDataSet.PortRow) this.Rows[index];

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.PortRowChangeEventHandler PortRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.PortRowChangeEventHandler PortRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.PortRowChangeEventHandler PortRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.PortRowChangeEventHandler PortRowDeleted;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void AddPortRow(ITMCOMDataSet.PortRow row) => this.Rows.Add((DataRow) row);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.PortRow AddPortRow(
        string PortName,
        int BaudRate,
        int Parity,
        int DataBits,
        int StopBits)
      {
        ITMCOMDataSet.PortRow row = (ITMCOMDataSet.PortRow) this.NewRow();
        object[] objArray = new object[6]
        {
          null,
          (object) PortName,
          (object) BaudRate,
          (object) Parity,
          (object) DataBits,
          (object) StopBits
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.PortRow FindByPortId(int PortId)
      {
        return (ITMCOMDataSet.PortRow) this.Rows.Find(new object[1]
        {
          (object) PortId
        });
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public override DataTable Clone()
      {
        ITMCOMDataSet.PortDataTable portDataTable = (ITMCOMDataSet.PortDataTable) base.Clone();
        portDataTable.InitVars();
        return (DataTable) portDataTable;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.PortDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnPortId = this.Columns["PortId"];
        this.columnPortName = this.Columns["PortName"];
        this.columnBaudRate = this.Columns["BaudRate"];
        this.columnParity = this.Columns["Parity"];
        this.columnDataBits = this.Columns["DataBits"];
        this.columnStopBits = this.Columns["StopBits"];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      private void InitClass()
      {
        this.columnPortId = new DataColumn("PortId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnPortId);
        this.columnPortName = new DataColumn("PortName", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnPortName);
        this.columnBaudRate = new DataColumn("BaudRate", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnBaudRate);
        this.columnParity = new DataColumn("Parity", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnParity);
        this.columnDataBits = new DataColumn("DataBits", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnDataBits);
        this.columnStopBits = new DataColumn("StopBits", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnStopBits);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnPortId
        }, true));
        this.columnPortId.AutoIncrement = true;
        this.columnPortId.AllowDBNull = false;
        this.columnPortId.Unique = true;
        this.columnPortId.Caption = "RoutingId";
        this.columnPortName.AllowDBNull = false;
        this.columnPortName.Caption = "COMPort";
        this.columnPortName.MaxLength = 10;
        this.columnBaudRate.AllowDBNull = false;
        this.columnBaudRate.DefaultValue = (object) 9600;
        this.columnParity.AllowDBNull = false;
        this.columnParity.DefaultValue = (object) 0;
        this.columnDataBits.AllowDBNull = false;
        this.columnDataBits.DefaultValue = (object) 8;
        this.columnStopBits.AllowDBNull = false;
        this.columnStopBits.DefaultValue = (object) 1;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.PortRow NewPortRow() => (ITMCOMDataSet.PortRow) this.NewRow();

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.PortRow(builder);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.PortRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.PortRowChanged == null)
          return;
        this.PortRowChanged((object) this, new ITMCOMDataSet.PortRowChangeEvent((ITMCOMDataSet.PortRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.PortRowChanging == null)
          return;
        this.PortRowChanging((object) this, new ITMCOMDataSet.PortRowChangeEvent((ITMCOMDataSet.PortRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.PortRowDeleted == null)
          return;
        this.PortRowDeleted((object) this, new ITMCOMDataSet.PortRowChangeEvent((ITMCOMDataSet.PortRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.PortRowDeleting == null)
          return;
        this.PortRowDeleting((object) this, new ITMCOMDataSet.PortRowChangeEvent((ITMCOMDataSet.PortRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void RemovePortRow(ITMCOMDataSet.PortRow row) => this.Rows.Remove((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (PortDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class ModemDataTable : TypedTableBase<ITMCOMDataSet.ModemRow>
    {
      private DataColumn columnId;
      private DataColumn columnModemId;
      private DataColumn columnSiteID;
      private DataColumn columnmbConnect;
      private DataColumn columnConnectionTime;
      private DataColumn columnRelcaseTime;
      private DataColumn columnLastBatchTime;
      private DataColumn columnTypeId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ModemDataTable()
      {
        this.TableName = "Modem";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal ModemDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected ModemDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn IdColumn => this.columnId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ModemIdColumn => this.columnModemId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn mbConnectColumn => this.columnmbConnect;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ConnectionTimeColumn => this.columnConnectionTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn RelcaseTimeColumn => this.columnRelcaseTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn LastBatchTimeColumn => this.columnLastBatchTime;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn TypeIdColumn => this.columnTypeId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [Browsable(false)]
      public int Count => this.Rows.Count;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.ModemRow this[int index] => (ITMCOMDataSet.ModemRow) this.Rows[index];

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ModemRowChangeEventHandler ModemRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ModemRowChangeEventHandler ModemRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ModemRowChangeEventHandler ModemRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ModemRowChangeEventHandler ModemRowDeleted;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void AddModemRow(ITMCOMDataSet.ModemRow row) => this.Rows.Add((DataRow) row);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ModemRow AddModemRow(
        string ModemId,
        int SiteID,
        bool mbConnect,
        DateTime ConnectionTime,
        DateTime RelcaseTime,
        DateTime LastBatchTime,
        int TypeId)
      {
        ITMCOMDataSet.ModemRow row = (ITMCOMDataSet.ModemRow) this.NewRow();
        object[] objArray = new object[8]
        {
          null,
          (object) ModemId,
          (object) SiteID,
          (object) mbConnect,
          (object) ConnectionTime,
          (object) RelcaseTime,
          (object) LastBatchTime,
          (object) TypeId
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ModemRow FindById(int Id)
      {
        return (ITMCOMDataSet.ModemRow) this.Rows.Find(new object[1]
        {
          (object) Id
        });
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public override DataTable Clone()
      {
        ITMCOMDataSet.ModemDataTable modemDataTable = (ITMCOMDataSet.ModemDataTable) base.Clone();
        modemDataTable.InitVars();
        return (DataTable) modemDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.ModemDataTable();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal void InitVars()
      {
        this.columnId = this.Columns["Id"];
        this.columnModemId = this.Columns["ModemId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnmbConnect = this.Columns["mbConnect"];
        this.columnConnectionTime = this.Columns["ConnectionTime"];
        this.columnRelcaseTime = this.Columns["RelcaseTime"];
        this.columnLastBatchTime = this.Columns["LastBatchTime"];
        this.columnTypeId = this.Columns["TypeId"];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      private void InitClass()
      {
        this.columnId = new DataColumn("Id", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnId);
        this.columnModemId = new DataColumn("ModemId", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnmbConnect = new DataColumn("mbConnect", typeof (bool), (string) null, MappingType.Element);
        this.Columns.Add(this.columnmbConnect);
        this.columnConnectionTime = new DataColumn("ConnectionTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConnectionTime);
        this.columnRelcaseTime = new DataColumn("RelcaseTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnRelcaseTime);
        this.columnLastBatchTime = new DataColumn("LastBatchTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnLastBatchTime);
        this.columnTypeId = new DataColumn("TypeId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnTypeId);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnId
        }, true));
        this.columnId.AutoIncrement = true;
        this.columnId.AllowDBNull = false;
        this.columnId.Unique = true;
        this.columnModemId.AllowDBNull = false;
        this.columnModemId.MaxLength = 20;
        this.columnSiteID.AllowDBNull = false;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ModemRow NewModemRow() => (ITMCOMDataSet.ModemRow) this.NewRow();

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.ModemRow(builder);
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.ModemRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.ModemRowChanged == null)
          return;
        this.ModemRowChanged((object) this, new ITMCOMDataSet.ModemRowChangeEvent((ITMCOMDataSet.ModemRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.ModemRowChanging == null)
          return;
        this.ModemRowChanging((object) this, new ITMCOMDataSet.ModemRowChangeEvent((ITMCOMDataSet.ModemRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.ModemRowDeleted == null)
          return;
        this.ModemRowDeleted((object) this, new ITMCOMDataSet.ModemRowChangeEvent((ITMCOMDataSet.ModemRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.ModemRowDeleting == null)
          return;
        this.ModemRowDeleting((object) this, new ITMCOMDataSet.ModemRowChangeEvent((ITMCOMDataSet.ModemRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveModemRow(ITMCOMDataSet.ModemRow row) => this.Rows.Remove((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (ModemDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class IllegalModemDataTable : TypedTableBase<ITMCOMDataSet.IllegalModemRow>
    {
      private DataColumn columnIllegalModemId;
      private DataColumn columnSiteID;
      private DataColumn columnModemID;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public IllegalModemDataTable()
      {
        this.TableName = "IllegalModem";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal IllegalModemDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected IllegalModemDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn IllegalModemIdColumn => this.columnIllegalModemId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ModemIDColumn => this.columnModemID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      [Browsable(false)]
      public int Count => this.Rows.Count;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.IllegalModemRow this[int index]
      {
        get => (ITMCOMDataSet.IllegalModemRow) this.Rows[index];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.IllegalModemRowChangeEventHandler IllegalModemRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.IllegalModemRowChangeEventHandler IllegalModemRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.IllegalModemRowChangeEventHandler IllegalModemRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.IllegalModemRowChangeEventHandler IllegalModemRowDeleted;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void AddIllegalModemRow(ITMCOMDataSet.IllegalModemRow row)
      {
        this.Rows.Add((DataRow) row);
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.IllegalModemRow AddIllegalModemRow(
        int IllegalModemId,
        int SiteID,
        string ModemID)
      {
        ITMCOMDataSet.IllegalModemRow row = (ITMCOMDataSet.IllegalModemRow) this.NewRow();
        object[] objArray = new object[3]
        {
          (object) IllegalModemId,
          (object) SiteID,
          (object) ModemID
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.IllegalModemRow FindByIllegalModemId(int IllegalModemId)
      {
        return (ITMCOMDataSet.IllegalModemRow) this.Rows.Find(new object[1]
        {
          (object) IllegalModemId
        });
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public override DataTable Clone()
      {
        ITMCOMDataSet.IllegalModemDataTable illegalModemDataTable = (ITMCOMDataSet.IllegalModemDataTable) base.Clone();
        illegalModemDataTable.InitVars();
        return (DataTable) illegalModemDataTable;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.IllegalModemDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnIllegalModemId = this.Columns["IllegalModemId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnModemID = this.Columns["ModemID"];
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      private void InitClass()
      {
        this.columnIllegalModemId = new DataColumn("IllegalModemId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnIllegalModemId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnModemID = new DataColumn("ModemID", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemID);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnIllegalModemId
        }, true));
        this.columnIllegalModemId.AllowDBNull = false;
        this.columnIllegalModemId.Unique = true;
        this.columnSiteID.AllowDBNull = false;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.IllegalModemRow NewIllegalModemRow()
      {
        return (ITMCOMDataSet.IllegalModemRow) this.NewRow();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.IllegalModemRow(builder);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.IllegalModemRow);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.IllegalModemRowChanged == null)
          return;
        this.IllegalModemRowChanged((object) this, new ITMCOMDataSet.IllegalModemRowChangeEvent((ITMCOMDataSet.IllegalModemRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.IllegalModemRowChanging == null)
          return;
        this.IllegalModemRowChanging((object) this, new ITMCOMDataSet.IllegalModemRowChangeEvent((ITMCOMDataSet.IllegalModemRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.IllegalModemRowDeleted == null)
          return;
        this.IllegalModemRowDeleted((object) this, new ITMCOMDataSet.IllegalModemRowChangeEvent((ITMCOMDataSet.IllegalModemRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.IllegalModemRowDeleting == null)
          return;
        this.IllegalModemRowDeleting((object) this, new ITMCOMDataSet.IllegalModemRowChangeEvent((ITMCOMDataSet.IllegalModemRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void RemoveIllegalModemRow(ITMCOMDataSet.IllegalModemRow row)
      {
        this.Rows.Remove((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (IllegalModemDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class ConfigTypeDataTable : TypedTableBase<ITMCOMDataSet.ConfigTypeRow>
    {
      private DataColumn columnConfigTypeId;
      private DataColumn columnName;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ConfigTypeDataTable()
      {
        this.TableName = "ConfigType";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal ConfigTypeDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected ConfigTypeDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ConfigTypeIdColumn => this.columnConfigTypeId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn NameColumn => this.columnName;

      [Browsable(false)]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int Count => this.Rows.Count;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.ConfigTypeRow this[int index]
      {
        get => (ITMCOMDataSet.ConfigTypeRow) this.Rows[index];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ConfigTypeRowChangeEventHandler ConfigTypeRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ConfigTypeRowChangeEventHandler ConfigTypeRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ConfigTypeRowChangeEventHandler ConfigTypeRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.ConfigTypeRowChangeEventHandler ConfigTypeRowDeleted;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void AddConfigTypeRow(ITMCOMDataSet.ConfigTypeRow row) => this.Rows.Add((DataRow) row);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ConfigTypeRow AddConfigTypeRow(int ConfigTypeId, string Name)
      {
        ITMCOMDataSet.ConfigTypeRow row = (ITMCOMDataSet.ConfigTypeRow) this.NewRow();
        object[] objArray = new object[2]
        {
          (object) ConfigTypeId,
          (object) Name
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ConfigTypeRow FindByConfigTypeId(int ConfigTypeId)
      {
        return (ITMCOMDataSet.ConfigTypeRow) this.Rows.Find(new object[1]
        {
          (object) ConfigTypeId
        });
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public override DataTable Clone()
      {
        ITMCOMDataSet.ConfigTypeDataTable configTypeDataTable = (ITMCOMDataSet.ConfigTypeDataTable) base.Clone();
        configTypeDataTable.InitVars();
        return (DataTable) configTypeDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.ConfigTypeDataTable();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal void InitVars()
      {
        this.columnConfigTypeId = this.Columns["ConfigTypeId"];
        this.columnName = this.Columns["Name"];
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      private void InitClass()
      {
        this.columnConfigTypeId = new DataColumn("ConfigTypeId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConfigTypeId);
        this.columnName = new DataColumn("Name", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnName);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnConfigTypeId
        }, true));
        this.columnConfigTypeId.AllowDBNull = false;
        this.columnConfigTypeId.Unique = true;
        this.columnName.AllowDBNull = false;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ConfigTypeRow NewConfigTypeRow()
      {
        return (ITMCOMDataSet.ConfigTypeRow) this.NewRow();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.ConfigTypeRow(builder);
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.ConfigTypeRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.ConfigTypeRowChanged == null)
          return;
        this.ConfigTypeRowChanged((object) this, new ITMCOMDataSet.ConfigTypeRowChangeEvent((ITMCOMDataSet.ConfigTypeRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.ConfigTypeRowChanging == null)
          return;
        this.ConfigTypeRowChanging((object) this, new ITMCOMDataSet.ConfigTypeRowChangeEvent((ITMCOMDataSet.ConfigTypeRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.ConfigTypeRowDeleted == null)
          return;
        this.ConfigTypeRowDeleted((object) this, new ITMCOMDataSet.ConfigTypeRowChangeEvent((ITMCOMDataSet.ConfigTypeRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.ConfigTypeRowDeleting == null)
          return;
        this.ConfigTypeRowDeleting((object) this, new ITMCOMDataSet.ConfigTypeRowChangeEvent((ITMCOMDataSet.ConfigTypeRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveConfigTypeRow(ITMCOMDataSet.ConfigTypeRow row)
      {
        this.Rows.Remove((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (ConfigTypeDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class TCPIPDataTable : TypedTableBase<ITMCOMDataSet.TCPIPRow>
    {
      private DataColumn columnId;
      private DataColumn columnModemId;
      private DataColumn columnSiteID;
      private DataColumn columnmbConnect;
      private DataColumn columnConnectionTime;
      private DataColumn columnRelcaseTime;
      private DataColumn columnLastBatchTime;
      private DataColumn columnTypeId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public TCPIPDataTable()
      {
        this.TableName = "TCPIP";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal TCPIPDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected TCPIPDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn IdColumn => this.columnId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ModemIdColumn => this.columnModemId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn mbConnectColumn => this.columnmbConnect;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ConnectionTimeColumn => this.columnConnectionTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn RelcaseTimeColumn => this.columnRelcaseTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn LastBatchTimeColumn => this.columnLastBatchTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn TypeIdColumn => this.columnTypeId;

      [Browsable(false)]
      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Count => this.Rows.Count;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.TCPIPRow this[int index] => (ITMCOMDataSet.TCPIPRow) this.Rows[index];

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.TCPIPRowChangeEventHandler TCPIPRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.TCPIPRowChangeEventHandler TCPIPRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.TCPIPRowChangeEventHandler TCPIPRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.TCPIPRowChangeEventHandler TCPIPRowDeleted;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void AddTCPIPRow(ITMCOMDataSet.TCPIPRow row) => this.Rows.Add((DataRow) row);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.TCPIPRow AddTCPIPRow(
        string ModemId,
        int SiteID,
        bool mbConnect,
        DateTime ConnectionTime,
        DateTime RelcaseTime,
        DateTime LastBatchTime,
        int TypeId)
      {
        ITMCOMDataSet.TCPIPRow row = (ITMCOMDataSet.TCPIPRow) this.NewRow();
        object[] objArray = new object[8]
        {
          null,
          (object) ModemId,
          (object) SiteID,
          (object) mbConnect,
          (object) ConnectionTime,
          (object) RelcaseTime,
          (object) LastBatchTime,
          (object) TypeId
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.TCPIPRow FindById(int Id)
      {
        return (ITMCOMDataSet.TCPIPRow) this.Rows.Find(new object[1]
        {
          (object) Id
        });
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public override DataTable Clone()
      {
        ITMCOMDataSet.TCPIPDataTable tcpipDataTable = (ITMCOMDataSet.TCPIPDataTable) base.Clone();
        tcpipDataTable.InitVars();
        return (DataTable) tcpipDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.TCPIPDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnId = this.Columns["Id"];
        this.columnModemId = this.Columns["ModemId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnmbConnect = this.Columns["mbConnect"];
        this.columnConnectionTime = this.Columns["ConnectionTime"];
        this.columnRelcaseTime = this.Columns["RelcaseTime"];
        this.columnLastBatchTime = this.Columns["LastBatchTime"];
        this.columnTypeId = this.Columns["TypeId"];
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      private void InitClass()
      {
        this.columnId = new DataColumn("Id", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnId);
        this.columnModemId = new DataColumn("ModemId", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnmbConnect = new DataColumn("mbConnect", typeof (bool), (string) null, MappingType.Element);
        this.Columns.Add(this.columnmbConnect);
        this.columnConnectionTime = new DataColumn("ConnectionTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConnectionTime);
        this.columnRelcaseTime = new DataColumn("RelcaseTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnRelcaseTime);
        this.columnLastBatchTime = new DataColumn("LastBatchTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnLastBatchTime);
        this.columnTypeId = new DataColumn("TypeId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnTypeId);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnId
        }, true));
        this.columnId.AutoIncrement = true;
        this.columnId.AllowDBNull = false;
        this.columnId.Unique = true;
        this.columnModemId.AllowDBNull = false;
        this.columnModemId.MaxLength = 20;
        this.columnSiteID.AllowDBNull = false;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.TCPIPRow NewTCPIPRow() => (ITMCOMDataSet.TCPIPRow) this.NewRow();

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.TCPIPRow(builder);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.TCPIPRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.TCPIPRowChanged == null)
          return;
        this.TCPIPRowChanged((object) this, new ITMCOMDataSet.TCPIPRowChangeEvent((ITMCOMDataSet.TCPIPRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.TCPIPRowChanging == null)
          return;
        this.TCPIPRowChanging((object) this, new ITMCOMDataSet.TCPIPRowChangeEvent((ITMCOMDataSet.TCPIPRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.TCPIPRowDeleted == null)
          return;
        this.TCPIPRowDeleted((object) this, new ITMCOMDataSet.TCPIPRowChangeEvent((ITMCOMDataSet.TCPIPRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.TCPIPRowDeleting == null)
          return;
        this.TCPIPRowDeleting((object) this, new ITMCOMDataSet.TCPIPRowChangeEvent((ITMCOMDataSet.TCPIPRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveTCPIPRow(ITMCOMDataSet.TCPIPRow row) => this.Rows.Remove((DataRow) row);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (TCPIPDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class DynamicTCPIPDataTable : TypedTableBase<ITMCOMDataSet.DynamicTCPIPRow>
    {
      private DataColumn columnDynamicId;
      private DataColumn columnSiteID;
      private DataColumn columnModemIP;
      private DataColumn columnModemID;
      private DataColumn columnStatisticsId;
      private DataColumn columnConnectionTime;
      private DataColumn columnReleaseTime;
      private DataColumn columnInBatchCount;
      private DataColumn columnOutBatchCount;
      private DataColumn columnInTraffic;
      private DataColumn columnOutTraffic;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DynamicTCPIPDataTable()
      {
        this.TableName = "DynamicTCPIP";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal DynamicTCPIPDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected DynamicTCPIPDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn DynamicIdColumn => this.columnDynamicId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ModemIPColumn => this.columnModemIP;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ModemIDColumn => this.columnModemID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn StatisticsIdColumn => this.columnStatisticsId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ConnectionTimeColumn => this.columnConnectionTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ReleaseTimeColumn => this.columnReleaseTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn InBatchCountColumn => this.columnInBatchCount;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn OutBatchCountColumn => this.columnOutBatchCount;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn InTrafficColumn => this.columnInTraffic;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn OutTrafficColumn => this.columnOutTraffic;

      [Browsable(false)]
      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Count => this.Rows.Count;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicTCPIPRow this[int index]
      {
        get => (ITMCOMDataSet.DynamicTCPIPRow) this.Rows[index];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicTCPIPRowChangeEventHandler DynamicTCPIPRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicTCPIPRowChangeEventHandler DynamicTCPIPRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicTCPIPRowChangeEventHandler DynamicTCPIPRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicTCPIPRowChangeEventHandler DynamicTCPIPRowDeleted;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void AddDynamicTCPIPRow(ITMCOMDataSet.DynamicTCPIPRow row)
      {
        this.Rows.Add((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicTCPIPRow AddDynamicTCPIPRow(
        int SiteID,
        string ModemIP,
        string ModemID,
        int StatisticsId,
        DateTime ConnectionTime,
        DateTime ReleaseTime,
        int InBatchCount,
        int OutBatchCount,
        int InTraffic,
        int OutTraffic)
      {
        ITMCOMDataSet.DynamicTCPIPRow row = (ITMCOMDataSet.DynamicTCPIPRow) this.NewRow();
        object[] objArray = new object[11]
        {
          null,
          (object) SiteID,
          (object) ModemIP,
          (object) ModemID,
          (object) StatisticsId,
          (object) ConnectionTime,
          (object) ReleaseTime,
          (object) InBatchCount,
          (object) OutBatchCount,
          (object) InTraffic,
          (object) OutTraffic
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.DynamicTCPIPRow FindByDynamicId(int DynamicId)
      {
        return (ITMCOMDataSet.DynamicTCPIPRow) this.Rows.Find(new object[1]
        {
          (object) DynamicId
        });
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public override DataTable Clone()
      {
        ITMCOMDataSet.DynamicTCPIPDataTable dynamicTcpipDataTable = (ITMCOMDataSet.DynamicTCPIPDataTable) base.Clone();
        dynamicTcpipDataTable.InitVars();
        return (DataTable) dynamicTcpipDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.DynamicTCPIPDataTable();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal void InitVars()
      {
        this.columnDynamicId = this.Columns["DynamicId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnModemIP = this.Columns["ModemIP"];
        this.columnModemID = this.Columns["ModemID"];
        this.columnStatisticsId = this.Columns["StatisticsId"];
        this.columnConnectionTime = this.Columns["ConnectionTime"];
        this.columnReleaseTime = this.Columns["ReleaseTime"];
        this.columnInBatchCount = this.Columns["InBatchCount"];
        this.columnOutBatchCount = this.Columns["OutBatchCount"];
        this.columnInTraffic = this.Columns["InTraffic"];
        this.columnOutTraffic = this.Columns["OutTraffic"];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      private void InitClass()
      {
        this.columnDynamicId = new DataColumn("DynamicId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnDynamicId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnModemIP = new DataColumn("ModemIP", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemIP);
        this.columnModemID = new DataColumn("ModemID", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemID);
        this.columnStatisticsId = new DataColumn("StatisticsId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnStatisticsId);
        this.columnConnectionTime = new DataColumn("ConnectionTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConnectionTime);
        this.columnReleaseTime = new DataColumn("ReleaseTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnReleaseTime);
        this.columnInBatchCount = new DataColumn("InBatchCount", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnInBatchCount);
        this.columnOutBatchCount = new DataColumn("OutBatchCount", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnOutBatchCount);
        this.columnInTraffic = new DataColumn("InTraffic", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnInTraffic);
        this.columnOutTraffic = new DataColumn("OutTraffic", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnOutTraffic);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnDynamicId
        }, true));
        this.columnDynamicId.AutoIncrement = true;
        this.columnDynamicId.AllowDBNull = false;
        this.columnDynamicId.ReadOnly = true;
        this.columnDynamicId.Unique = true;
        this.columnSiteID.AllowDBNull = false;
        this.columnModemIP.AllowDBNull = false;
        this.columnModemIP.MaxLength = 20;
        this.columnModemID.AllowDBNull = false;
        this.columnModemID.MaxLength = 20;
        this.columnStatisticsId.AllowDBNull = false;
        this.columnConnectionTime.AllowDBNull = false;
        this.columnInBatchCount.AllowDBNull = false;
        this.columnOutBatchCount.AllowDBNull = false;
        this.columnInTraffic.AllowDBNull = false;
        this.columnOutTraffic.AllowDBNull = false;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.DynamicTCPIPRow NewDynamicTCPIPRow()
      {
        return (ITMCOMDataSet.DynamicTCPIPRow) this.NewRow();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.DynamicTCPIPRow(builder);
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.DynamicTCPIPRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.DynamicTCPIPRowChanged == null)
          return;
        this.DynamicTCPIPRowChanged((object) this, new ITMCOMDataSet.DynamicTCPIPRowChangeEvent((ITMCOMDataSet.DynamicTCPIPRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.DynamicTCPIPRowChanging == null)
          return;
        this.DynamicTCPIPRowChanging((object) this, new ITMCOMDataSet.DynamicTCPIPRowChangeEvent((ITMCOMDataSet.DynamicTCPIPRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.DynamicTCPIPRowDeleted == null)
          return;
        this.DynamicTCPIPRowDeleted((object) this, new ITMCOMDataSet.DynamicTCPIPRowChangeEvent((ITMCOMDataSet.DynamicTCPIPRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.DynamicTCPIPRowDeleting == null)
          return;
        this.DynamicTCPIPRowDeleting((object) this, new ITMCOMDataSet.DynamicTCPIPRowChangeEvent((ITMCOMDataSet.DynamicTCPIPRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void RemoveDynamicTCPIPRow(ITMCOMDataSet.DynamicTCPIPRow row)
      {
        this.Rows.Remove((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (DynamicTCPIPDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class TCPIPClientDataTable : TypedTableBase<ITMCOMDataSet.TCPIPClientRow>
    {
      private DataColumn columnId;
      private DataColumn columnModemId;
      private DataColumn columnSiteID;
      private DataColumn columnmbConnect;
      private DataColumn columnConnectionTime;
      private DataColumn columnRelcaseTime;
      private DataColumn columnLastBatchTime;
      private DataColumn columnTypeId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public TCPIPClientDataTable()
      {
        this.TableName = "TCPIPClient";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal TCPIPClientDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected TCPIPClientDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn IdColumn => this.columnId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ModemIdColumn => this.columnModemId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn mbConnectColumn => this.columnmbConnect;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ConnectionTimeColumn => this.columnConnectionTime;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn RelcaseTimeColumn => this.columnRelcaseTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn LastBatchTimeColumn => this.columnLastBatchTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn TypeIdColumn => this.columnTypeId;

      [Browsable(false)]
      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Count => this.Rows.Count;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.TCPIPClientRow this[int index]
      {
        get => (ITMCOMDataSet.TCPIPClientRow) this.Rows[index];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.TCPIPClientRowChangeEventHandler TCPIPClientRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.TCPIPClientRowChangeEventHandler TCPIPClientRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.TCPIPClientRowChangeEventHandler TCPIPClientRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.TCPIPClientRowChangeEventHandler TCPIPClientRowDeleted;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void AddTCPIPClientRow(ITMCOMDataSet.TCPIPClientRow row)
      {
        this.Rows.Add((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.TCPIPClientRow AddTCPIPClientRow(
        string ModemId,
        int SiteID,
        bool mbConnect,
        DateTime ConnectionTime,
        DateTime RelcaseTime,
        DateTime LastBatchTime,
        int TypeId)
      {
        ITMCOMDataSet.TCPIPClientRow row = (ITMCOMDataSet.TCPIPClientRow) this.NewRow();
        object[] objArray = new object[8]
        {
          null,
          (object) ModemId,
          (object) SiteID,
          (object) mbConnect,
          (object) ConnectionTime,
          (object) RelcaseTime,
          (object) LastBatchTime,
          (object) TypeId
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.TCPIPClientRow FindById(int Id)
      {
        return (ITMCOMDataSet.TCPIPClientRow) this.Rows.Find(new object[1]
        {
          (object) Id
        });
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public override DataTable Clone()
      {
        ITMCOMDataSet.TCPIPClientDataTable tcpipClientDataTable = (ITMCOMDataSet.TCPIPClientDataTable) base.Clone();
        tcpipClientDataTable.InitVars();
        return (DataTable) tcpipClientDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.TCPIPClientDataTable();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal void InitVars()
      {
        this.columnId = this.Columns["Id"];
        this.columnModemId = this.Columns["ModemId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnmbConnect = this.Columns["mbConnect"];
        this.columnConnectionTime = this.Columns["ConnectionTime"];
        this.columnRelcaseTime = this.Columns["RelcaseTime"];
        this.columnLastBatchTime = this.Columns["LastBatchTime"];
        this.columnTypeId = this.Columns["TypeId"];
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      private void InitClass()
      {
        this.columnId = new DataColumn("Id", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnId);
        this.columnModemId = new DataColumn("ModemId", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnmbConnect = new DataColumn("mbConnect", typeof (bool), (string) null, MappingType.Element);
        this.Columns.Add(this.columnmbConnect);
        this.columnConnectionTime = new DataColumn("ConnectionTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConnectionTime);
        this.columnRelcaseTime = new DataColumn("RelcaseTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnRelcaseTime);
        this.columnLastBatchTime = new DataColumn("LastBatchTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnLastBatchTime);
        this.columnTypeId = new DataColumn("TypeId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnTypeId);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnId
        }, true));
        this.columnId.AutoIncrement = true;
        this.columnId.AllowDBNull = false;
        this.columnId.Unique = true;
        this.columnModemId.AllowDBNull = false;
        this.columnModemId.MaxLength = 20;
        this.columnSiteID.AllowDBNull = false;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.TCPIPClientRow NewTCPIPClientRow()
      {
        return (ITMCOMDataSet.TCPIPClientRow) this.NewRow();
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.TCPIPClientRow(builder);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.TCPIPClientRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.TCPIPClientRowChanged == null)
          return;
        this.TCPIPClientRowChanged((object) this, new ITMCOMDataSet.TCPIPClientRowChangeEvent((ITMCOMDataSet.TCPIPClientRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.TCPIPClientRowChanging == null)
          return;
        this.TCPIPClientRowChanging((object) this, new ITMCOMDataSet.TCPIPClientRowChangeEvent((ITMCOMDataSet.TCPIPClientRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.TCPIPClientRowDeleted == null)
          return;
        this.TCPIPClientRowDeleted((object) this, new ITMCOMDataSet.TCPIPClientRowChangeEvent((ITMCOMDataSet.TCPIPClientRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.TCPIPClientRowDeleting == null)
          return;
        this.TCPIPClientRowDeleting((object) this, new ITMCOMDataSet.TCPIPClientRowChangeEvent((ITMCOMDataSet.TCPIPClientRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveTCPIPClientRow(ITMCOMDataSet.TCPIPClientRow row)
      {
        this.Rows.Remove((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (TCPIPClientDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    [XmlSchemaProvider("GetTypedTableSchema")]
    [Serializable]
    public class DynamicTCPIPClientDataTable : TypedTableBase<ITMCOMDataSet.DynamicTCPIPClientRow>
    {
      private DataColumn columnDynamicId;
      private DataColumn columnSiteID;
      private DataColumn columnModemIP;
      private DataColumn columnModemID;
      private DataColumn columnStatisticsId;
      private DataColumn columnConnectionTime;
      private DataColumn columnReleaseTime;
      private DataColumn columnInBatchCount;
      private DataColumn columnOutBatchCount;
      private DataColumn columnInTraffic;
      private DataColumn columnOutTraffic;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DynamicTCPIPClientDataTable()
      {
        this.TableName = "DynamicTCPIPClient";
        this.BeginInit();
        this.InitClass();
        this.EndInit();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal DynamicTCPIPClientDataTable(DataTable table)
      {
        this.TableName = table.TableName;
        if (table.CaseSensitive != table.DataSet.CaseSensitive)
          this.CaseSensitive = table.CaseSensitive;
        if (table.Locale.ToString() != table.DataSet.Locale.ToString())
          this.Locale = table.Locale;
        if (table.Namespace != table.DataSet.Namespace)
          this.Namespace = table.Namespace;
        this.Prefix = table.Prefix;
        this.MinimumCapacity = table.MinimumCapacity;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected DynamicTCPIPClientDataTable(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.InitVars();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn DynamicIdColumn => this.columnDynamicId;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn SiteIDColumn => this.columnSiteID;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ModemIPColumn => this.columnModemIP;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ModemIDColumn => this.columnModemID;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn StatisticsIdColumn => this.columnStatisticsId;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn ConnectionTimeColumn => this.columnConnectionTime;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn ReleaseTimeColumn => this.columnReleaseTime;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn InBatchCountColumn => this.columnInBatchCount;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn OutBatchCountColumn => this.columnOutBatchCount;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataColumn InTrafficColumn => this.columnInTraffic;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataColumn OutTrafficColumn => this.columnOutTraffic;

      [Browsable(false)]
      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Count => this.Rows.Count;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicTCPIPClientRow this[int index]
      {
        get => (ITMCOMDataSet.DynamicTCPIPClientRow) this.Rows[index];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicTCPIPClientRowChangeEventHandler DynamicTCPIPClientRowChanging;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicTCPIPClientRowChangeEventHandler DynamicTCPIPClientRowChanged;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicTCPIPClientRowChangeEventHandler DynamicTCPIPClientRowDeleting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public event ITMCOMDataSet.DynamicTCPIPClientRowChangeEventHandler DynamicTCPIPClientRowDeleted;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void AddDynamicTCPIPClientRow(ITMCOMDataSet.DynamicTCPIPClientRow row)
      {
        this.Rows.Add((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicTCPIPClientRow AddDynamicTCPIPClientRow(
        int SiteID,
        string ModemIP,
        string ModemID,
        int StatisticsId,
        DateTime ConnectionTime,
        DateTime ReleaseTime,
        int InBatchCount,
        int OutBatchCount,
        int InTraffic,
        int OutTraffic)
      {
        ITMCOMDataSet.DynamicTCPIPClientRow row = (ITMCOMDataSet.DynamicTCPIPClientRow) this.NewRow();
        object[] objArray = new object[11]
        {
          null,
          (object) SiteID,
          (object) ModemIP,
          (object) ModemID,
          (object) StatisticsId,
          (object) ConnectionTime,
          (object) ReleaseTime,
          (object) InBatchCount,
          (object) OutBatchCount,
          (object) InTraffic,
          (object) OutTraffic
        };
        row.ItemArray = objArray;
        this.Rows.Add((DataRow) row);
        return row;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.DynamicTCPIPClientRow FindByDynamicId(int DynamicId)
      {
        return (ITMCOMDataSet.DynamicTCPIPClientRow) this.Rows.Find(new object[1]
        {
          (object) DynamicId
        });
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public override DataTable Clone()
      {
        ITMCOMDataSet.DynamicTCPIPClientDataTable tcpipClientDataTable = (ITMCOMDataSet.DynamicTCPIPClientDataTable) base.Clone();
        tcpipClientDataTable.InitVars();
        return (DataTable) tcpipClientDataTable;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override DataTable CreateInstance()
      {
        return (DataTable) new ITMCOMDataSet.DynamicTCPIPClientDataTable();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal void InitVars()
      {
        this.columnDynamicId = this.Columns["DynamicId"];
        this.columnSiteID = this.Columns["SiteID"];
        this.columnModemIP = this.Columns["ModemIP"];
        this.columnModemID = this.Columns["ModemID"];
        this.columnStatisticsId = this.Columns["StatisticsId"];
        this.columnConnectionTime = this.Columns["ConnectionTime"];
        this.columnReleaseTime = this.Columns["ReleaseTime"];
        this.columnInBatchCount = this.Columns["InBatchCount"];
        this.columnOutBatchCount = this.Columns["OutBatchCount"];
        this.columnInTraffic = this.Columns["InTraffic"];
        this.columnOutTraffic = this.Columns["OutTraffic"];
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      private void InitClass()
      {
        this.columnDynamicId = new DataColumn("DynamicId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnDynamicId);
        this.columnSiteID = new DataColumn("SiteID", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnSiteID);
        this.columnModemIP = new DataColumn("ModemIP", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemIP);
        this.columnModemID = new DataColumn("ModemID", typeof (string), (string) null, MappingType.Element);
        this.Columns.Add(this.columnModemID);
        this.columnStatisticsId = new DataColumn("StatisticsId", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnStatisticsId);
        this.columnConnectionTime = new DataColumn("ConnectionTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnConnectionTime);
        this.columnReleaseTime = new DataColumn("ReleaseTime", typeof (DateTime), (string) null, MappingType.Element);
        this.Columns.Add(this.columnReleaseTime);
        this.columnInBatchCount = new DataColumn("InBatchCount", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnInBatchCount);
        this.columnOutBatchCount = new DataColumn("OutBatchCount", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnOutBatchCount);
        this.columnInTraffic = new DataColumn("InTraffic", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnInTraffic);
        this.columnOutTraffic = new DataColumn("OutTraffic", typeof (int), (string) null, MappingType.Element);
        this.Columns.Add(this.columnOutTraffic);
        this.Constraints.Add((Constraint) new UniqueConstraint("Constraint1", new DataColumn[1]
        {
          this.columnDynamicId
        }, true));
        this.columnDynamicId.AutoIncrement = true;
        this.columnDynamicId.AllowDBNull = false;
        this.columnDynamicId.ReadOnly = true;
        this.columnDynamicId.Unique = true;
        this.columnSiteID.AllowDBNull = false;
        this.columnModemIP.AllowDBNull = false;
        this.columnModemIP.MaxLength = 20;
        this.columnModemID.AllowDBNull = false;
        this.columnModemID.MaxLength = 20;
        this.columnStatisticsId.AllowDBNull = false;
        this.columnConnectionTime.AllowDBNull = false;
        this.columnInBatchCount.AllowDBNull = false;
        this.columnOutBatchCount.AllowDBNull = false;
        this.columnInTraffic.AllowDBNull = false;
        this.columnOutTraffic.AllowDBNull = false;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicTCPIPClientRow NewDynamicTCPIPClientRow()
      {
        return (ITMCOMDataSet.DynamicTCPIPClientRow) this.NewRow();
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
      {
        return (DataRow) new ITMCOMDataSet.DynamicTCPIPClientRow(builder);
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override Type GetRowType() => typeof (ITMCOMDataSet.DynamicTCPIPClientRow);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      protected override void OnRowChanged(DataRowChangeEventArgs e)
      {
        base.OnRowChanged(e);
        if (this.DynamicTCPIPClientRowChanged == null)
          return;
        this.DynamicTCPIPClientRowChanged((object) this, new ITMCOMDataSet.DynamicTCPIPClientRowChangeEvent((ITMCOMDataSet.DynamicTCPIPClientRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowChanging(DataRowChangeEventArgs e)
      {
        base.OnRowChanging(e);
        if (this.DynamicTCPIPClientRowChanging == null)
          return;
        this.DynamicTCPIPClientRowChanging((object) this, new ITMCOMDataSet.DynamicTCPIPClientRowChangeEvent((ITMCOMDataSet.DynamicTCPIPClientRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleted(DataRowChangeEventArgs e)
      {
        base.OnRowDeleted(e);
        if (this.DynamicTCPIPClientRowDeleted == null)
          return;
        this.DynamicTCPIPClientRowDeleted((object) this, new ITMCOMDataSet.DynamicTCPIPClientRowChangeEvent((ITMCOMDataSet.DynamicTCPIPClientRow) e.Row, e.Action));
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      protected override void OnRowDeleting(DataRowChangeEventArgs e)
      {
        base.OnRowDeleting(e);
        if (this.DynamicTCPIPClientRowDeleting == null)
          return;
        this.DynamicTCPIPClientRowDeleting((object) this, new ITMCOMDataSet.DynamicTCPIPClientRowChangeEvent((ITMCOMDataSet.DynamicTCPIPClientRow) e.Row, e.Action));
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void RemoveDynamicTCPIPClientRow(ITMCOMDataSet.DynamicTCPIPClientRow row)
      {
        this.Rows.Remove((DataRow) row);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
      {
        XmlSchemaComplexType typedTableSchema = new XmlSchemaComplexType();
        XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
        ITMCOMDataSet itmcomDataSet = new ITMCOMDataSet();
        XmlSchemaAny xmlSchemaAny1 = new XmlSchemaAny();
        xmlSchemaAny1.Namespace = "http://www.w3.org/2001/XMLSchema";
        xmlSchemaAny1.MinOccurs = 0M;
        xmlSchemaAny1.MaxOccurs = Decimal.MaxValue;
        xmlSchemaAny1.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny1);
        XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
        xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
        xmlSchemaAny2.MinOccurs = 1M;
        xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
        xmlSchemaSequence.Items.Add((XmlSchemaObject) xmlSchemaAny2);
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "namespace",
          FixedValue = itmcomDataSet.Namespace
        });
        typedTableSchema.Attributes.Add((XmlSchemaObject) new XmlSchemaAttribute()
        {
          Name = "tableTypeName",
          FixedValue = nameof (DynamicTCPIPClientDataTable)
        });
        typedTableSchema.Particle = (XmlSchemaParticle) xmlSchemaSequence;
        XmlSchema schemaSerializable = itmcomDataSet.GetSchemaSerializable();
        if (xs.Contains(schemaSerializable.TargetNamespace))
        {
          MemoryStream memoryStream1 = new MemoryStream();
          MemoryStream memoryStream2 = new MemoryStream();
          try
          {
            schemaSerializable.Write((Stream) memoryStream1);
            IEnumerator enumerator = xs.Schemas(schemaSerializable.TargetNamespace).GetEnumerator();
            while (enumerator.MoveNext())
            {
              XmlSchema current = (XmlSchema) enumerator.Current;
              memoryStream2.SetLength(0L);
              current.Write((Stream) memoryStream2);
              if (memoryStream1.Length == memoryStream2.Length)
              {
                memoryStream1.Position = 0L;
                memoryStream2.Position = 0L;
                do
                  ;
                while (memoryStream1.Position != memoryStream1.Length && memoryStream1.ReadByte() == memoryStream2.ReadByte());
                if (memoryStream1.Position == memoryStream1.Length)
                  return typedTableSchema;
              }
            }
          }
          finally
          {
            memoryStream1?.Close();
            memoryStream2?.Close();
          }
        }
        xs.Add(schemaSerializable);
        return typedTableSchema;
      }
    }

    public class ConfigRow : DataRow
    {
      private ITMCOMDataSet.ConfigDataTable tableConfig;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal ConfigRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableConfig = (ITMCOMDataSet.ConfigDataTable) this.Table;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int ConfigId
      {
        get => (int) this[this.tableConfig.ConfigIdColumn];
        set => this[this.tableConfig.ConfigIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string IPAddress
      {
        get => (string) this[this.tableConfig.IPAddressColumn];
        set => this[this.tableConfig.IPAddressColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int PortNumber
      {
        get => (int) this[this.tableConfig.PortNumberColumn];
        set => this[this.tableConfig.PortNumberColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public string ServerID
      {
        get => (string) this[this.tableConfig.ServerIDColumn];
        set => this[this.tableConfig.ServerIDColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool mbCurrent
      {
        get
        {
          try
          {
            return (bool) this[this.tableConfig.mbCurrentColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'mbCurrent' in table 'Config' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableConfig.mbCurrentColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int TypeId
      {
        get
        {
          try
          {
            return (int) this[this.tableConfig.TypeIdColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'TypeId' in table 'Config' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableConfig.TypeIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.ConfigTypeRow ConfigTypeRow
      {
        get
        {
          return (ITMCOMDataSet.ConfigTypeRow) this.GetParentRow(this.Table.ParentRelations["ConfigType_Config"]);
        }
        set => this.SetParentRow((DataRow) value, this.Table.ParentRelations["ConfigType_Config"]);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsmbCurrentNull() => this.IsNull(this.tableConfig.mbCurrentColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetmbCurrentNull() => this[this.tableConfig.mbCurrentColumn] = Convert.DBNull;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsTypeIdNull() => this.IsNull(this.tableConfig.TypeIdColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetTypeIdNull() => this[this.tableConfig.TypeIdColumn] = Convert.DBNull;
    }

    public class DynamicRow : DataRow
    {
      private ITMCOMDataSet.DynamicDataTable tableDynamic;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal DynamicRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableDynamic = (ITMCOMDataSet.DynamicDataTable) this.Table;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int DynamicId
      {
        get => (int) this[this.tableDynamic.DynamicIdColumn];
        set => this[this.tableDynamic.DynamicIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int SiteID
      {
        get => (int) this[this.tableDynamic.SiteIDColumn];
        set => this[this.tableDynamic.SiteIDColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public string ModemIP
      {
        get => (string) this[this.tableDynamic.ModemIPColumn];
        set => this[this.tableDynamic.ModemIPColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string ModemID
      {
        get => (string) this[this.tableDynamic.ModemIDColumn];
        set => this[this.tableDynamic.ModemIDColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int StatisticsId
      {
        get => (int) this[this.tableDynamic.StatisticsIdColumn];
        set => this[this.tableDynamic.StatisticsIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime ConnectionTime
      {
        get => (DateTime) this[this.tableDynamic.ConnectionTimeColumn];
        set => this[this.tableDynamic.ConnectionTimeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DateTime ReleaseTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableDynamic.ReleaseTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'ReleaseTime' in table 'Dynamic' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableDynamic.ReleaseTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int InBatchCount
      {
        get => (int) this[this.tableDynamic.InBatchCountColumn];
        set => this[this.tableDynamic.InBatchCountColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int OutBatchCount
      {
        get => (int) this[this.tableDynamic.OutBatchCountColumn];
        set => this[this.tableDynamic.OutBatchCountColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int InTraffic
      {
        get => (int) this[this.tableDynamic.InTrafficColumn];
        set => this[this.tableDynamic.InTrafficColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int OutTraffic
      {
        get => (int) this[this.tableDynamic.OutTrafficColumn];
        set => this[this.tableDynamic.OutTrafficColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsReleaseTimeNull() => this.IsNull(this.tableDynamic.ReleaseTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetReleaseTimeNull()
      {
        this[this.tableDynamic.ReleaseTimeColumn] = Convert.DBNull;
      }
    }

    public class StaticRow : DataRow
    {
      private ITMCOMDataSet.StaticDataTable tableStatic;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal StaticRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableStatic = (ITMCOMDataSet.StaticDataTable) this.Table;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int StaticId
      {
        get => (int) this[this.tableStatic.StaticIdColumn];
        set => this[this.tableStatic.StaticIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string COMPort
      {
        get => (string) this[this.tableStatic.COMPortColumn];
        set => this[this.tableStatic.COMPortColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int StatisticsId
      {
        get => (int) this[this.tableStatic.StatisticsIdColumn];
        set => this[this.tableStatic.StatisticsIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DateTime ConnectionTime
      {
        get => (DateTime) this[this.tableStatic.ConnectionTimeColumn];
        set => this[this.tableStatic.ConnectionTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime ReleaseTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableStatic.ReleaseTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'ReleaseTime' in table 'Static' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableStatic.ReleaseTimeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int InBatchCount
      {
        get => (int) this[this.tableStatic.InBatchCountColumn];
        set => this[this.tableStatic.InBatchCountColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int OutBatchCount
      {
        get => (int) this[this.tableStatic.OutBatchCountColumn];
        set => this[this.tableStatic.OutBatchCountColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int InTraffic
      {
        get => (int) this[this.tableStatic.InTrafficColumn];
        set => this[this.tableStatic.InTrafficColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int OutTraffic
      {
        get => (int) this[this.tableStatic.OutTrafficColumn];
        set => this[this.tableStatic.OutTrafficColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsReleaseTimeNull() => this.IsNull(this.tableStatic.ReleaseTimeColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetReleaseTimeNull() => this[this.tableStatic.ReleaseTimeColumn] = Convert.DBNull;
    }

    public class LogRow : DataRow
    {
      private ITMCOMDataSet.LogDataTable tableLog;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal LogRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableLog = (ITMCOMDataSet.LogDataTable) this.Table;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int LogId
      {
        get => (int) this[this.tableLog.LogIdColumn];
        set => this[this.tableLog.LogIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int SiteID
      {
        get
        {
          try
          {
            return (int) this[this.tableLog.SiteIDColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'SiteID' in table 'Log' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableLog.SiteIDColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string COMPort
      {
        get
        {
          try
          {
            return (string) this[this.tableLog.COMPortColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'COMPort' in table 'Log' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableLog.COMPortColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string Message
      {
        get => (string) this[this.tableLog.MessageColumn];
        set => this[this.tableLog.MessageColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int StatusId
      {
        get => (int) this[this.tableLog.StatusIdColumn];
        set => this[this.tableLog.StatusIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime Time
      {
        get => (DateTime) this[this.tableLog.TimeColumn];
        set => this[this.tableLog.TimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.LogStatusRow LogStatusRow
      {
        get
        {
          return (ITMCOMDataSet.LogStatusRow) this.GetParentRow(this.Table.ParentRelations["FK_Log_LogStatus"]);
        }
        set => this.SetParentRow((DataRow) value, this.Table.ParentRelations["FK_Log_LogStatus"]);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsSiteIDNull() => this.IsNull(this.tableLog.SiteIDColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetSiteIDNull() => this[this.tableLog.SiteIDColumn] = Convert.DBNull;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsCOMPortNull() => this.IsNull(this.tableLog.COMPortColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetCOMPortNull() => this[this.tableLog.COMPortColumn] = Convert.DBNull;
    }

    public class RoutingRow : DataRow
    {
      private ITMCOMDataSet.RoutingDataTable tableRouting;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal RoutingRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableRouting = (ITMCOMDataSet.RoutingDataTable) this.Table;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int RoutingId
      {
        get => (int) this[this.tableRouting.RoutingIdColumn];
        set => this[this.tableRouting.RoutingIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int SiteID
      {
        get => (int) this[this.tableRouting.SiteIDColumn];
        set => this[this.tableRouting.SiteIDColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int LinkID
      {
        get => (int) this[this.tableRouting.LinkIDColumn];
        set => this[this.tableRouting.LinkIDColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int PortId
      {
        get => (int) this[this.tableRouting.PortIdColumn];
        set => this[this.tableRouting.PortIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int BeginRange
      {
        get
        {
          try
          {
            return (int) this[this.tableRouting.BeginRangeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'BeginRange' in table 'Routing' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableRouting.BeginRangeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int EndRange
      {
        get
        {
          try
          {
            return (int) this[this.tableRouting.EndRangeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'EndRange' in table 'Routing' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableRouting.EndRangeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int LocalSiteID
      {
        get
        {
          try
          {
            return (int) this[this.tableRouting.LocalSiteIDColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'LocalSiteID' in table 'Routing' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableRouting.LocalSiteIDColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsBeginRangeNull() => this.IsNull(this.tableRouting.BeginRangeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetBeginRangeNull() => this[this.tableRouting.BeginRangeColumn] = Convert.DBNull;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsEndRangeNull() => this.IsNull(this.tableRouting.EndRangeColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetEndRangeNull() => this[this.tableRouting.EndRangeColumn] = Convert.DBNull;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsLocalSiteIDNull() => this.IsNull(this.tableRouting.LocalSiteIDColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetLocalSiteIDNull()
      {
        this[this.tableRouting.LocalSiteIDColumn] = Convert.DBNull;
      }
    }

    public class LogStatusRow : DataRow
    {
      private ITMCOMDataSet.LogStatusDataTable tableLogStatus;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal LogStatusRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableLogStatus = (ITMCOMDataSet.LogStatusDataTable) this.Table;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int LogStatusId
      {
        get => (int) this[this.tableLogStatus.LogStatusIdColumn];
        set => this[this.tableLogStatus.LogStatusIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public string Name
      {
        get => (string) this[this.tableLogStatus.NameColumn];
        set => this[this.tableLogStatus.NameColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.LogRow[] GetLogRows()
      {
        return this.Table.ChildRelations["FK_Log_LogStatus"] == null ? new ITMCOMDataSet.LogRow[0] : (ITMCOMDataSet.LogRow[]) this.GetChildRows(this.Table.ChildRelations["FK_Log_LogStatus"]);
      }
    }

    public class PortRow : DataRow
    {
      private ITMCOMDataSet.PortDataTable tablePort;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal PortRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tablePort = (ITMCOMDataSet.PortDataTable) this.Table;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int PortId
      {
        get => (int) this[this.tablePort.PortIdColumn];
        set => this[this.tablePort.PortIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string PortName
      {
        get => (string) this[this.tablePort.PortNameColumn];
        set => this[this.tablePort.PortNameColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int BaudRate
      {
        get => (int) this[this.tablePort.BaudRateColumn];
        set => this[this.tablePort.BaudRateColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Parity
      {
        get => (int) this[this.tablePort.ParityColumn];
        set => this[this.tablePort.ParityColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int DataBits
      {
        get => (int) this[this.tablePort.DataBitsColumn];
        set => this[this.tablePort.DataBitsColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int StopBits
      {
        get => (int) this[this.tablePort.StopBitsColumn];
        set => this[this.tablePort.StopBitsColumn] = (object) value;
      }
    }

    public class ModemRow : DataRow
    {
      private ITMCOMDataSet.ModemDataTable tableModem;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal ModemRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableModem = (ITMCOMDataSet.ModemDataTable) this.Table;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Id
      {
        get => (int) this[this.tableModem.IdColumn];
        set => this[this.tableModem.IdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public string ModemId
      {
        get => (string) this[this.tableModem.ModemIdColumn];
        set => this[this.tableModem.ModemIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int SiteID
      {
        get => (int) this[this.tableModem.SiteIDColumn];
        set => this[this.tableModem.SiteIDColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool mbConnect
      {
        get
        {
          try
          {
            return (bool) this[this.tableModem.mbConnectColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'mbConnect' in table 'Modem' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableModem.mbConnectColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime ConnectionTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableModem.ConnectionTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'ConnectionTime' in table 'Modem' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableModem.ConnectionTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime RelcaseTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableModem.RelcaseTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'RelcaseTime' in table 'Modem' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableModem.RelcaseTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime LastBatchTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableModem.LastBatchTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'LastBatchTime' in table 'Modem' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableModem.LastBatchTimeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int TypeId
      {
        get
        {
          try
          {
            return (int) this[this.tableModem.TypeIdColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'TypeId' in table 'Modem' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableModem.TypeIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsmbConnectNull() => this.IsNull(this.tableModem.mbConnectColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetmbConnectNull() => this[this.tableModem.mbConnectColumn] = Convert.DBNull;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsConnectionTimeNull() => this.IsNull(this.tableModem.ConnectionTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetConnectionTimeNull()
      {
        this[this.tableModem.ConnectionTimeColumn] = Convert.DBNull;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsRelcaseTimeNull() => this.IsNull(this.tableModem.RelcaseTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetRelcaseTimeNull() => this[this.tableModem.RelcaseTimeColumn] = Convert.DBNull;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsLastBatchTimeNull() => this.IsNull(this.tableModem.LastBatchTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetLastBatchTimeNull()
      {
        this[this.tableModem.LastBatchTimeColumn] = Convert.DBNull;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsTypeIdNull() => this.IsNull(this.tableModem.TypeIdColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetTypeIdNull() => this[this.tableModem.TypeIdColumn] = Convert.DBNull;
    }

    public class IllegalModemRow : DataRow
    {
      private ITMCOMDataSet.IllegalModemDataTable tableIllegalModem;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal IllegalModemRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableIllegalModem = (ITMCOMDataSet.IllegalModemDataTable) this.Table;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int IllegalModemId
      {
        get => (int) this[this.tableIllegalModem.IllegalModemIdColumn];
        set => this[this.tableIllegalModem.IllegalModemIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int SiteID
      {
        get => (int) this[this.tableIllegalModem.SiteIDColumn];
        set => this[this.tableIllegalModem.SiteIDColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string ModemID
      {
        get
        {
          try
          {
            return (string) this[this.tableIllegalModem.ModemIDColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'ModemID' in table 'IllegalModem' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableIllegalModem.ModemIDColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsModemIDNull() => this.IsNull(this.tableIllegalModem.ModemIDColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetModemIDNull() => this[this.tableIllegalModem.ModemIDColumn] = Convert.DBNull;
    }

    public class ConfigTypeRow : DataRow
    {
      private ITMCOMDataSet.ConfigTypeDataTable tableConfigType;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal ConfigTypeRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableConfigType = (ITMCOMDataSet.ConfigTypeDataTable) this.Table;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int ConfigTypeId
      {
        get => (int) this[this.tableConfigType.ConfigTypeIdColumn];
        set => this[this.tableConfigType.ConfigTypeIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public string Name
      {
        get => (string) this[this.tableConfigType.NameColumn];
        set => this[this.tableConfigType.NameColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.ConfigRow[] GetConfigRows()
      {
        return this.Table.ChildRelations["ConfigType_Config"] == null ? new ITMCOMDataSet.ConfigRow[0] : (ITMCOMDataSet.ConfigRow[]) this.GetChildRows(this.Table.ChildRelations["ConfigType_Config"]);
      }
    }

    public class TCPIPRow : DataRow
    {
      private ITMCOMDataSet.TCPIPDataTable tableTCPIP;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal TCPIPRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableTCPIP = (ITMCOMDataSet.TCPIPDataTable) this.Table;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int Id
      {
        get => (int) this[this.tableTCPIP.IdColumn];
        set => this[this.tableTCPIP.IdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public string ModemId
      {
        get => (string) this[this.tableTCPIP.ModemIdColumn];
        set => this[this.tableTCPIP.ModemIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int SiteID
      {
        get => (int) this[this.tableTCPIP.SiteIDColumn];
        set => this[this.tableTCPIP.SiteIDColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool mbConnect
      {
        get
        {
          try
          {
            return (bool) this[this.tableTCPIP.mbConnectColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'mbConnect' in table 'TCPIP' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIP.mbConnectColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime ConnectionTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableTCPIP.ConnectionTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'ConnectionTime' in table 'TCPIP' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIP.ConnectionTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime RelcaseTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableTCPIP.RelcaseTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'RelcaseTime' in table 'TCPIP' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIP.RelcaseTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime LastBatchTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableTCPIP.LastBatchTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'LastBatchTime' in table 'TCPIP' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIP.LastBatchTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int TypeId
      {
        get
        {
          try
          {
            return (int) this[this.tableTCPIP.TypeIdColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'TypeId' in table 'TCPIP' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIP.TypeIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsmbConnectNull() => this.IsNull(this.tableTCPIP.mbConnectColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetmbConnectNull() => this[this.tableTCPIP.mbConnectColumn] = Convert.DBNull;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsConnectionTimeNull() => this.IsNull(this.tableTCPIP.ConnectionTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetConnectionTimeNull()
      {
        this[this.tableTCPIP.ConnectionTimeColumn] = Convert.DBNull;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsRelcaseTimeNull() => this.IsNull(this.tableTCPIP.RelcaseTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetRelcaseTimeNull() => this[this.tableTCPIP.RelcaseTimeColumn] = Convert.DBNull;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsLastBatchTimeNull() => this.IsNull(this.tableTCPIP.LastBatchTimeColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetLastBatchTimeNull()
      {
        this[this.tableTCPIP.LastBatchTimeColumn] = Convert.DBNull;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsTypeIdNull() => this.IsNull(this.tableTCPIP.TypeIdColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetTypeIdNull() => this[this.tableTCPIP.TypeIdColumn] = Convert.DBNull;
    }

    public class DynamicTCPIPRow : DataRow
    {
      private ITMCOMDataSet.DynamicTCPIPDataTable tableDynamicTCPIP;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal DynamicTCPIPRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableDynamicTCPIP = (ITMCOMDataSet.DynamicTCPIPDataTable) this.Table;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int DynamicId
      {
        get => (int) this[this.tableDynamicTCPIP.DynamicIdColumn];
        set => this[this.tableDynamicTCPIP.DynamicIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int SiteID
      {
        get => (int) this[this.tableDynamicTCPIP.SiteIDColumn];
        set => this[this.tableDynamicTCPIP.SiteIDColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string ModemIP
      {
        get => (string) this[this.tableDynamicTCPIP.ModemIPColumn];
        set => this[this.tableDynamicTCPIP.ModemIPColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string ModemID
      {
        get => (string) this[this.tableDynamicTCPIP.ModemIDColumn];
        set => this[this.tableDynamicTCPIP.ModemIDColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int StatisticsId
      {
        get => (int) this[this.tableDynamicTCPIP.StatisticsIdColumn];
        set => this[this.tableDynamicTCPIP.StatisticsIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DateTime ConnectionTime
      {
        get => (DateTime) this[this.tableDynamicTCPIP.ConnectionTimeColumn];
        set => this[this.tableDynamicTCPIP.ConnectionTimeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DateTime ReleaseTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableDynamicTCPIP.ReleaseTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'ReleaseTime' in table 'DynamicTCPIP' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableDynamicTCPIP.ReleaseTimeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int InBatchCount
      {
        get => (int) this[this.tableDynamicTCPIP.InBatchCountColumn];
        set => this[this.tableDynamicTCPIP.InBatchCountColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int OutBatchCount
      {
        get => (int) this[this.tableDynamicTCPIP.OutBatchCountColumn];
        set => this[this.tableDynamicTCPIP.OutBatchCountColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int InTraffic
      {
        get => (int) this[this.tableDynamicTCPIP.InTrafficColumn];
        set => this[this.tableDynamicTCPIP.InTrafficColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int OutTraffic
      {
        get => (int) this[this.tableDynamicTCPIP.OutTrafficColumn];
        set => this[this.tableDynamicTCPIP.OutTrafficColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsReleaseTimeNull() => this.IsNull(this.tableDynamicTCPIP.ReleaseTimeColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetReleaseTimeNull()
      {
        this[this.tableDynamicTCPIP.ReleaseTimeColumn] = Convert.DBNull;
      }
    }

    public class TCPIPClientRow : DataRow
    {
      private ITMCOMDataSet.TCPIPClientDataTable tableTCPIPClient;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      internal TCPIPClientRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableTCPIPClient = (ITMCOMDataSet.TCPIPClientDataTable) this.Table;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int Id
      {
        get => (int) this[this.tableTCPIPClient.IdColumn];
        set => this[this.tableTCPIPClient.IdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public string ModemId
      {
        get => (string) this[this.tableTCPIPClient.ModemIdColumn];
        set => this[this.tableTCPIPClient.ModemIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int SiteID
      {
        get => (int) this[this.tableTCPIPClient.SiteIDColumn];
        set => this[this.tableTCPIPClient.SiteIDColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool mbConnect
      {
        get
        {
          try
          {
            return (bool) this[this.tableTCPIPClient.mbConnectColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'mbConnect' in table 'TCPIPClient' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIPClient.mbConnectColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DateTime ConnectionTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableTCPIPClient.ConnectionTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'ConnectionTime' in table 'TCPIPClient' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIPClient.ConnectionTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime RelcaseTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableTCPIPClient.RelcaseTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'RelcaseTime' in table 'TCPIPClient' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIPClient.RelcaseTimeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DateTime LastBatchTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableTCPIPClient.LastBatchTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'LastBatchTime' in table 'TCPIPClient' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIPClient.LastBatchTimeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int TypeId
      {
        get
        {
          try
          {
            return (int) this[this.tableTCPIPClient.TypeIdColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'TypeId' in table 'TCPIPClient' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableTCPIPClient.TypeIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsmbConnectNull() => this.IsNull(this.tableTCPIPClient.mbConnectColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetmbConnectNull()
      {
        this[this.tableTCPIPClient.mbConnectColumn] = Convert.DBNull;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsConnectionTimeNull() => this.IsNull(this.tableTCPIPClient.ConnectionTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetConnectionTimeNull()
      {
        this[this.tableTCPIPClient.ConnectionTimeColumn] = Convert.DBNull;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsRelcaseTimeNull() => this.IsNull(this.tableTCPIPClient.RelcaseTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetRelcaseTimeNull()
      {
        this[this.tableTCPIPClient.RelcaseTimeColumn] = Convert.DBNull;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsLastBatchTimeNull() => this.IsNull(this.tableTCPIPClient.LastBatchTimeColumn);

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public void SetLastBatchTimeNull()
      {
        this[this.tableTCPIPClient.LastBatchTimeColumn] = Convert.DBNull;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public bool IsTypeIdNull() => this.IsNull(this.tableTCPIPClient.TypeIdColumn);

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetTypeIdNull() => this[this.tableTCPIPClient.TypeIdColumn] = Convert.DBNull;
    }

    public class DynamicTCPIPClientRow : DataRow
    {
      private ITMCOMDataSet.DynamicTCPIPClientDataTable tableDynamicTCPIPClient;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      internal DynamicTCPIPClientRow(DataRowBuilder rb)
        : base(rb)
      {
        this.tableDynamicTCPIPClient = (ITMCOMDataSet.DynamicTCPIPClientDataTable) this.Table;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int DynamicId
      {
        get => (int) this[this.tableDynamicTCPIPClient.DynamicIdColumn];
        set => this[this.tableDynamicTCPIPClient.DynamicIdColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int SiteID
      {
        get => (int) this[this.tableDynamicTCPIPClient.SiteIDColumn];
        set => this[this.tableDynamicTCPIPClient.SiteIDColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string ModemIP
      {
        get => (string) this[this.tableDynamicTCPIPClient.ModemIPColumn];
        set => this[this.tableDynamicTCPIPClient.ModemIPColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public string ModemID
      {
        get => (string) this[this.tableDynamicTCPIPClient.ModemIDColumn];
        set => this[this.tableDynamicTCPIPClient.ModemIDColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int StatisticsId
      {
        get => (int) this[this.tableDynamicTCPIPClient.StatisticsIdColumn];
        set => this[this.tableDynamicTCPIPClient.StatisticsIdColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime ConnectionTime
      {
        get => (DateTime) this[this.tableDynamicTCPIPClient.ConnectionTimeColumn];
        set => this[this.tableDynamicTCPIPClient.ConnectionTimeColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DateTime ReleaseTime
      {
        get
        {
          try
          {
            return (DateTime) this[this.tableDynamicTCPIPClient.ReleaseTimeColumn];
          }
          catch (InvalidCastException ex)
          {
            throw new StrongTypingException("The value for column 'ReleaseTime' in table 'DynamicTCPIPClient' is DBNull.", (Exception) ex);
          }
        }
        set => this[this.tableDynamicTCPIPClient.ReleaseTimeColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int InBatchCount
      {
        get => (int) this[this.tableDynamicTCPIPClient.InBatchCountColumn];
        set => this[this.tableDynamicTCPIPClient.InBatchCountColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int OutBatchCount
      {
        get => (int) this[this.tableDynamicTCPIPClient.OutBatchCountColumn];
        set => this[this.tableDynamicTCPIPClient.OutBatchCountColumn] = (object) value;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public int InTraffic
      {
        get => (int) this[this.tableDynamicTCPIPClient.InTrafficColumn];
        set => this[this.tableDynamicTCPIPClient.InTrafficColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public int OutTraffic
      {
        get => (int) this[this.tableDynamicTCPIPClient.OutTrafficColumn];
        set => this[this.tableDynamicTCPIPClient.OutTrafficColumn] = (object) value;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public bool IsReleaseTimeNull()
      {
        return this.IsNull(this.tableDynamicTCPIPClient.ReleaseTimeColumn);
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public void SetReleaseTimeNull()
      {
        this[this.tableDynamicTCPIPClient.ReleaseTimeColumn] = Convert.DBNull;
      }
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class ConfigRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.ConfigRow eventRow;
      private DataRowAction eventAction;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ConfigRowChangeEvent(ITMCOMDataSet.ConfigRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.ConfigRow Row => this.eventRow;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class DynamicRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.DynamicRow eventRow;
      private DataRowAction eventAction;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DynamicRowChangeEvent(ITMCOMDataSet.DynamicRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicRow Row => this.eventRow;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class StaticRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.StaticRow eventRow;
      private DataRowAction eventAction;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public StaticRowChangeEvent(ITMCOMDataSet.StaticRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.StaticRow Row => this.eventRow;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class LogRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.LogRow eventRow;
      private DataRowAction eventAction;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public LogRowChangeEvent(ITMCOMDataSet.LogRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.LogRow Row => this.eventRow;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class RoutingRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.RoutingRow eventRow;
      private DataRowAction eventAction;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public RoutingRowChangeEvent(ITMCOMDataSet.RoutingRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.RoutingRow Row => this.eventRow;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class LogStatusRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.LogStatusRow eventRow;
      private DataRowAction eventAction;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public LogStatusRowChangeEvent(ITMCOMDataSet.LogStatusRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.LogStatusRow Row => this.eventRow;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class PortRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.PortRow eventRow;
      private DataRowAction eventAction;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public PortRowChangeEvent(ITMCOMDataSet.PortRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.PortRow Row => this.eventRow;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class ModemRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.ModemRow eventRow;
      private DataRowAction eventAction;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ModemRowChangeEvent(ITMCOMDataSet.ModemRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.ModemRow Row => this.eventRow;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class IllegalModemRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.IllegalModemRow eventRow;
      private DataRowAction eventAction;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public IllegalModemRowChangeEvent(ITMCOMDataSet.IllegalModemRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.IllegalModemRow Row => this.eventRow;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class ConfigTypeRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.ConfigTypeRow eventRow;
      private DataRowAction eventAction;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ConfigTypeRowChangeEvent(ITMCOMDataSet.ConfigTypeRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public ITMCOMDataSet.ConfigTypeRow Row => this.eventRow;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class TCPIPRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.TCPIPRow eventRow;
      private DataRowAction eventAction;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public TCPIPRowChangeEvent(ITMCOMDataSet.TCPIPRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.TCPIPRow Row => this.eventRow;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class DynamicTCPIPRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.DynamicTCPIPRow eventRow;
      private DataRowAction eventAction;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DynamicTCPIPRowChangeEvent(ITMCOMDataSet.DynamicTCPIPRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicTCPIPRow Row => this.eventRow;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class TCPIPClientRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.TCPIPClientRow eventRow;
      private DataRowAction eventAction;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public TCPIPClientRowChangeEvent(ITMCOMDataSet.TCPIPClientRow row, DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.TCPIPClientRow Row => this.eventRow;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataRowAction Action => this.eventAction;
    }

    [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
    public class DynamicTCPIPClientRowChangeEvent : EventArgs
    {
      private ITMCOMDataSet.DynamicTCPIPClientRow eventRow;
      private DataRowAction eventAction;

      [DebuggerNonUserCode]
      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      public DynamicTCPIPClientRowChangeEvent(
        ITMCOMDataSet.DynamicTCPIPClientRow row,
        DataRowAction action)
      {
        this.eventRow = row;
        this.eventAction = action;
      }

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public ITMCOMDataSet.DynamicTCPIPClientRow Row => this.eventRow;

      [GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
      [DebuggerNonUserCode]
      public DataRowAction Action => this.eventAction;
    }
  }
}
