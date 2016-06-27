using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPlainAPI.Models
{
    // <r i="FOUSA05B5A;FO"></r>
    public class Security
    {
        public string Name { get; set; }
        public string SecId { get; set; }
        public string Ticker { get; set; }
        public List<DataPoint> Datapoints { get; set; }
    }
    public class DataPoint
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}