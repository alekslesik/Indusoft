using Indusoft.TM.COM.Base;
using System;
using System.Collections;
using System.Net.Sockets;

#nullable disable
namespace Indusoft.TM.COM.DLL
{
  public class Utilities
  {
    public static void InsertIntoQueue(Queue queue, object data, int size)
    {
      try
      {
        while (size <= queue.Count)
          queue.Dequeue();
        queue.Enqueue(data);
      }
      catch (Exception ex)
      {
        EventLogger.Info((object) ("InsertIntoQueueError: " + (object) ex));
      }
    }

    public static void CloseSocket(Socket socket)
    {
      if (socket.Connected)
        socket.Shutdown(SocketShutdown.Both);
      socket.Close();
    }
  }
}
