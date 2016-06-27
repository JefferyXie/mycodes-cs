using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlainAPI.Formatter
{
    public interface IFormatter
    {
        string SerializeObjectToString<T>(T obj);
        T DeserializeFromString<T>(string text);
    }
}
