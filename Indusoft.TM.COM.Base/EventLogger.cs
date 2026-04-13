using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;

#nullable disable
namespace Indusoft.TM.COM.Base
{
  public class EventLogger
  {
    private static ILog _logger = (ILog) null;
    private static RollingFileAppender _appender = new RollingFileAppender();
    private static readonly string _fileName = "server.log";

    public static void Info(object message, Exception e) => EventLogger._logger.Info(message, e);

    public static void Info(object message) => EventLogger._logger.Info(message);

    public static void Error(object message, Exception e) => EventLogger._logger.Error(message, e);

    public static void Error(object message) => EventLogger._logger.Error(message);

    public static void Debug(object message, Exception e) => EventLogger._logger.Debug(message, e);

    public static void Debug(object message) => EventLogger._logger.Debug(message);

    public static void Initialize()
    {
      EventLogger._logger = LogManager.GetLogger("string");
      EventLogger._appender.AppendToFile = true;
      EventLogger._appender.File = EventLogger._fileName;
      EventLogger._appender.StaticLogFileName = true;
      EventLogger._appender.MaxSizeRollBackups = 0;
      EventLogger._appender.MaximumFileSize = "10MB";
      EventLogger._appender.RollingStyle = RollingFileAppender.RollingMode.Size;
      EventLogger._appender.Name = "RollingAppender";
      PatternLayout patternLayout = new PatternLayout("%d  %m%n");
      patternLayout.Footer = Environment.NewLine;
      patternLayout.Header = "";
      EventLogger._appender.Layout = (ILayout) patternLayout;
      EventLogger._appender.ActivateOptions();
      BasicConfigurator.Configure((IAppender) EventLogger._appender);
    }

    public static void Log(string header, byte[] data, int begin, int size)
    {
      string message = header;
      if (size > 0)
      {
        for (int index = 0; index < size; ++index)
          message += string.Format("{0:X2} ", (object) data[begin + index]);
      }
      EventLogger.Info((object) message);
    }
  }
}
