using Chameleon.CommonLib;
using System.Net;

namespace Chameleon.Api
{
    public class RequestResult
    {
        public readonly Boolean Success = false;
        public readonly Boolean ServerErr = false;
        public readonly string ResponseStr;
        private readonly HeaderItem[] _headerItems;
        public readonly string[] LoggingError;

        public RequestResult(Boolean success, Boolean serverError, string responseStr, HeaderItem[] headerItems, Exception? ex = null)
        {
            Success = success;
            ServerErr = serverError;
            ResponseStr = responseStr;
            _headerItems = headerItems;

            if (ex != null)
            {
                LoggingError = [$"{ex}"];
            }
            else
            {
                LoggingError = [];
            }
        }


        // Replace/add response headers and values as specified in the response document.
        public void HandleResponseHeaders(IHeaderDictionary headerDict)
        {
            foreach (var item in _headerItems)
            {
                if (headerDict.TryGetValue(item.propertyName, out var existing))
                {
                    if (existing.Any(v => string.Equals(v, item.propertyValue, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                }

                headerDict[item.propertyName] = item.propertyValue;
            }
        }
    }
}
