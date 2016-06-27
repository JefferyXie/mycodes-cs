using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.XPath;
using MyPlainAPI.Utils;
using MyPlainAPI.Models;
using System.Text;

namespace MyPlainAPI.Services.Retriever
{
    internal class ResearchDataRetriever : DataRetriever
    {
        internal async Task<List<Security>> RetrieveSecId(string[] tickers)
        {
            /*
            https://adcalcdata.morningtar.com/internaluse/researchdatacenter.aspx
            <req action = "find" subtype = "tickermatch">
                <dat>
                    <r i = "FMAGX" />
                    <r i = "more ticker……" />
                </dat>
            </req>
            */
            StringBuilder postData = new StringBuilder();
            postData.Append(@"<req action='find' subtype='tickermatch'><dat>");
            foreach (var ticker in tickers)
            {
                postData.AppendFormat("<r i='{0}'/>", ticker);
            }
            postData.Append(@"</dat></req>");
            return await retrieve(postData.ToString());
        }
        internal async Task<List<Security>> RetrievePerformanceId(string[] secIds)
        {
            /*
            https://adcalcdata.morningtar.com/internaluse/researchdatacenter.aspx
            <req action="get" subtype="investmentlist" type="60" univ="IL" chunklimit="1000">
                <flds>
                    <f i="OS01W" noret="1" />
                    <f i="OS06Y" />
                </flds>
                <dat id="">
                    <r i="FOUSA05B5A;FO" />
                    <r i="FOUSA00KDL;FO" />
                </dat>
            </req>
            */
            StringBuilder postData = new StringBuilder();
            postData.Append(
                        @"<req action='get' subtype='investmentlist' type='60' univ='IL' chunklimit='1000'>" +
                            @"<flds>" +
                                @"<f i='OS01W' noret='1' />" +
                                @"<f i='OS06Y' />" +
                                @"<f i='OS385' />" +
                            @"</flds>" +
                            @"<dat>");
            foreach (var secId in secIds)
            {
                postData.AppendFormat("<r i='{0}'/>", secId);
            }
            postData.Append("</dat></req>");

            return await retrieve(postData.ToString());
        }
        private async Task<List<Security>> retrieve(string postData)
        {
            var dataNav = await PostDataXmlFormatAsync(HttpUtils.URI_ResearchDataCenter, postData);
            // you may use 'internaluse' service which doesn't need cookie but this service won't
            // be available outside of M* network.
            //var dataNav = await PostDataXmlFormatAsync(HttpUtils.URI_ResearchDataCenter_Internaluse, postData);
            List<Security> response = new List<Security>();
            if (null != dataNav)
            {
                var rs = XmlUtils.GetNodeList(dataNav, "//res/dat/r");
                if (null != rs)
                {
                    foreach (XPathNavigator r in rs)
                    {
                        var security = new Security()
                        {
                            Name = r.GetAttribute("n", string.Empty),
                            SecId = r.GetAttribute("i", string.Empty),
                            Ticker = r.GetAttribute("t", string.Empty),
                            Datapoints = new List<DataPoint>(),
                        };
                        foreach (XPathNavigator c in r.SelectChildren(XPathNodeType.Element))
                        {
                            security.Datapoints.Add(new DataPoint()
                            {
                                Id = c.GetAttribute("i", string.Empty),
                                Value = c.GetAttribute("v", string.Empty),
                            });
                        }
                        response.Add(security);
                    }
                }
            }
            return response;
        }
    }
}
