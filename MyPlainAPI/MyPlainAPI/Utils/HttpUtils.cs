using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPlainAPI.Utils
{
    public class HttpUtils
    {
        public static readonly string DefaultDomain = @".morningstar.com";
        public static readonly string URL_LOGIN = @"https://gladmainnew.morningstar.com/loginsrf/login_noui.srf?rbtn=btnEmail&strPassword={1}&email={0}&ProductCode=DIRECT";
        public static readonly string ServerHost = @"https://adcalcdata.morningstar.com";
        // for internal use only, no cookie or login is needed
        public static readonly Uri URI_ResearchDataCenter_Internaluse = new Uri(ServerHost+@"/internaluse/adcalcdata/researchdatacenter/researchdatacenter.aspx?retXML=yes");
        // for external use, login is needed to get cookie
        public static readonly Uri URI_ResearchDataCenter = new Uri(ServerHost+@"/researchdatacenter/researchdatacenter.aspx?retXML=yes");

        public static readonly string ContentTypeApplicationXml = @"application/xml";
        public static readonly string ContentTypeJson = @"application/json";
        public static readonly TimeSpan DefaultTimeOut = TimeSpan.FromSeconds(600);

        public static readonly string LOGIN_Email = "jeffery.xie@morningstar.com";
        public static readonly string LOGIN_Password = "Welcome123";
    }
}
