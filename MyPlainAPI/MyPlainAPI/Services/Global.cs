using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPlainAPI.Services
{
    public static class Global
    {
        static Logger log = new DiagnosticsLog();
        public static Logger Log { get { return log; } }
    }
}
