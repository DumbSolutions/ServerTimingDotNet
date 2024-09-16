using System.Web;
using ServerTiming.DotNet.Core;

namespace ServerTiming.DotNet.NetFramework4_7_2
{
    public class HttpResponseWrapper : IResponseWrapper
    {
        private readonly HttpResponse _httpResponse;

        private HttpResponseWrapper(HttpResponse httpResponse)
        {
            _httpResponse = httpResponse;
        }

        public static HttpResponseWrapper Wrap(HttpResponse response)
        {
            return new HttpResponseWrapper(response);
        }

        public void AddHeader(string key, string value)
        {
            _httpResponse.AddHeader(key, value);
        }

        public bool HeadersWritten => _httpResponse.HeadersWritten;
    }
}