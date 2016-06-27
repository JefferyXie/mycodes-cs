using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using MyPlainAPI.Models;
using MyPlainAPI.Services.Retriever;

namespace MyPlainAPI.Controllers
{
    public class ResearchDataController : ApiController
    {
        [HttpPost]
        [ActionName("performanceid")]
        public async Task<List<Security>> GetPerformanceId(List<Security> securities)
        {
            var reqContent = this.Request.Content.ReadAsStringAsync().Result;
            string[] tickers = { "POAGX", "VASVX" };
            var retriever = new ResearchDataRetriever();
            var secList = await retriever.RetrieveSecId(tickers);
            List<string> secIds = new List<string>();
            secList.ForEach(s => secIds.Add(s.SecId));
            var perf = await retriever.RetrievePerformanceId(secIds.ToArray());

            return perf;
        }
    }
}
