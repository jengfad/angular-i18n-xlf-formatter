using System;
using System.Collections.Generic;
using System.Text;

namespace XlfFormatter.Utilities
{
    public class Logger
    {
        public Logger Msg()
        {
            WriteLogMessage(String.Empty);
            return this;
        }

        public Logger Msg(string logMessage)
        {
            WriteLogMessage(logMessage);
            return this;
        }

        public Logger Msg(string messageFormat, params object[] messageArgs)
        {
            WriteLogMessage(String.Format(messageFormat, messageArgs));
            return this;
        }

        public Logger Msg(Exception ex)
        {
            for (Exception x = ex; x != null; x = x.InnerException)
            {
                WriteLogMessage(String.Format("{0}-{1}", x.GetType().FullName, x.Message));
            }
            return this;
        }

        private void WriteLogMessage(string logMessage)
        {
            Console.WriteLine(logMessage);
        }
    }
}
