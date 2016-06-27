using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace MyPlainAPI.Services.Retriever
{
    internal interface IRetriever
    {
        Task<XPathNavigator> Retrieve();
        Task<XPathNavigator> Retrieve(string postData);
    }
}
