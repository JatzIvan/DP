using CoreLibrary.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public class Logger
    {
        BlockingCollection<LogMessage> bc = new BlockingCollection<LogMessage>();

        private static Logger INSTACE;

        public static Logger GetLogger()
        {
            if(INSTACE == null)
            {
                INSTACE = new Logger();
            }

            return INSTACE;
        }

        public Logger()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (LogMessage p in bc.GetConsumingEnumerable())
                {
                    const string LINE_MSG = "[{0} - {1}:{2}] {3}";
                    Console.WriteLine(String.Format(LINE_MSG, LogTimeStamp(), p.CallerLine, p.CallerMethod, p.Message));

                }
            });
        }

        ~Logger()
        {
            // Free the writing thread
            bc.CompleteAdding();
        }

        public void WriteLine(string msg, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            bc.Add(new LogMessage(msg, memberName, sourceLineNumber));
        }

        string LogTimeStamp()
        {
            DateTime now = DateTime.Now;
            return now.ToShortTimeString();
        }

    }
}

