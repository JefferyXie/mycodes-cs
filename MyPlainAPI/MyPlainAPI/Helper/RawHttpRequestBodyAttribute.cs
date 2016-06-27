using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MyPlainAPI.Helper
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public sealed class RawHttpRequestBodyAttribute : ParameterBindingAttribute
    {
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            if (null == parameter) throw new ArgumentException("Invalid parameter");
            return new RawHttpRequestBodyParaBinding(parameter);
        }
    }
}