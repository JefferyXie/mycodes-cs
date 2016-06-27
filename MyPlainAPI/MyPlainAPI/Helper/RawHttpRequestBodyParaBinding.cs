using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace MyPlainAPI.Helper
{
    /// <summary>
    /// http://weblog.west-wind.com/posts/2013/Dec/13/Accepting-Raw-Request-Body-Content-with-ASPNET-Web-API
    /// Reads the Request body into a string/byte[] and
    /// assigns it to the parameter bound.
    /// 
    /// Can be used with multiple paramters on
    /// a Web API method using the [RawHttpRequestBody] attribute
    /// </summary>
    public class RawHttpRequestBodyParaBinding : HttpParameterBinding
    {
        public RawHttpRequestBodyParaBinding(HttpParameterDescriptor descriptor)
            : base(descriptor)
        { }
        public override Task ExecuteBindingAsync(
            ModelMetadataProvider metadataProvider, 
            HttpActionContext actionContext, 
            CancellationToken cancellationToken)
        {
            if (actionContext.Request.Method == HttpMethod.Get)
            {
                SetValue(actionContext, null);

                var tsc = new TaskCompletionSource<object>();
                tsc.SetResult(null);
                return tsc.Task;
            }
            else if (Descriptor.ParameterType == typeof(string))
            {
                return actionContext.Request.Content
                        .ReadAsStringAsync()
                        .ContinueWith((task) =>
                        {
                            var stringResult = task.Result;
                            SetValue(actionContext, stringResult);
                        });
            }
            else if (Descriptor.ParameterType == typeof(byte[]))
            {
                return actionContext.Request.Content
                    .ReadAsByteArrayAsync()
                    .ContinueWith((task) =>
                    {
                        byte[] result = task.Result;
                        SetValue(actionContext, result);
                    });
            }
            throw new InvalidOperationException("Only string and byte[] are supported for [NakedBody] parameters");
        }

        public override bool WillReadBody
        {
            get
            {
                return true;
            }
        }
    }
}