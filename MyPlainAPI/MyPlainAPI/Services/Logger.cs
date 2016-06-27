using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPlainAPI.Services
{
    abstract public class Logger
    {
        abstract public void Write(string log);
        abstract public void Write(string logfmt, params object[] args);
        abstract public void Error(object err);
        abstract public void Error(object err, params object[] args);
    }
    public class DiagnosticsLog : Logger
    {
        public override void Write(string log)
        {
            System.Diagnostics.Debug.WriteLine(log);
        }
        public override void Write(string logfmt, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(logfmt, args));
        }
        public override void Error(object err)
        {
            System.Diagnostics.Debug.WriteLine("Error:" + err);
        }
        public override void Error(object err, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Error:" + err, args));
        }
    }
    public class ConsoleLog : Logger
    {
        public override void Write(string log)
        {
            Console.WriteLine(log);
        }
        public override void Write(string logfmt, params object[] args)
        {
            Console.WriteLine(string.Format(logfmt, args));
        }
        public override void Error(object err)
        {
            Console.WriteLine("Error:" + err);
        }
        public override void Error(object err, params object[] args)
        {
            Console.WriteLine(string.Format("Error:" + err, args));
        }
    }
}
