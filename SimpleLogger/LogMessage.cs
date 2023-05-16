using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Log
{
    internal class LogMessage
    {

        public string Message {  get; set; }
        public string CallerMethod { get; set; }

        public int CallerLine { get; set; }


        public LogMessage(string msg, string callerMethod, int callerLine) { 
            this.Message = msg;
            this.CallerMethod = callerMethod;
            this.CallerLine = callerLine;
        }

    }
}
