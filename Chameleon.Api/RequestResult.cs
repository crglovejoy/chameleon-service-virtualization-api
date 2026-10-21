using Chameleon.CommonLib;
using System.Net;

namespace Chameleon.Api
{
    /**********************/
    /**  REQUEST RESULT  **/
    /**********************/

    public class RequestResult
    {
        private string _responseStr;
        public string ResponseStr => _responseStr;
        private HeaderItem[] _headerItems;
        public HeaderItem[] HeaderItems => _headerItems;
        private string _loggingError;
        public string LoggingError => _loggingError;
        private HttpStatusCode _resultStatus;
        public HttpStatusCode ResultStatus => _resultStatus;
        public static HeaderItem[] ContentPlainTextHeader => [new HeaderItem("Content-Type", "text/plain")];


        /*****************/
        /*  Constructor  */
        /*****************/

        public RequestResult()
        {
            _responseStr = string.Empty;
            _headerItems = [];
            _loggingError = string.Empty;
            _resultStatus = HttpStatusCode.Unused;
        }


        /********************/
        /*  SetAsServerErr  */
        /********************/

        public void SetAsServerErr(string responseStr, HeaderItem[] headerItems, string loggingErr = "")
        {
            _responseStr = responseStr;

            if (string.IsNullOrWhiteSpace(loggingErr))
            {
                _loggingError = responseStr;
            }
            else
            {
                _loggingError = loggingErr;
            }

            _headerItems = headerItems;
            _resultStatus = HttpStatusCode.InternalServerError;
        }


        /*************************************/
        /*  SetAsServerErr - with Exception  */
        /*************************************/

        public void SetAsServerErr(Exception ex, HeaderItem[] headerItems)
        {
            /* Want the logged error to contain the exception message + stacktrace.
             * Want the response string to contain JUST the exception message */
            SetAsServerErr(ex.Message, headerItems, $"{ex}");
        }


        /******************/
        /*  SetAsSuccess  */
        /******************/

        public void SetAsSuccess(string responseStr, HeaderItem[] headerItems)
        {
            _responseStr = responseStr;
            _headerItems = headerItems;
            _resultStatus = HttpStatusCode.OK;
        }


        /*******************/
        /*  SetAsNotFound  */
        /*******************/

        public void SetAsNotFound(string responseStr, HeaderItem[] headerItems)
        {
            _responseStr = responseStr;
            _headerItems = headerItems;
            _resultStatus = HttpStatusCode.NotFound;
        }

        public void SetAsNotInitErr()
        {
            SetAsServerErr("RequestResult was not set to a result", ContentPlainTextHeader);
        }


        /*******************************/
        /*  Handle our custom headers  */
        /*******************************/

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
