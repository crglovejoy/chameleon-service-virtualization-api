using Chameleon.CommonLib;
using Microsoft.AspNetCore.WebUtilities;

namespace Chameleon.Api
{
    public enum ReturnResultStatus { None, Result200, Result400, Result500 };


    /**********************/
    /**  REQUEST RESULT  **/
    /**********************/

    public class RequestResult
    {
        private Boolean _success;
        public Boolean Success => _success;
        private Boolean _serverErr;
        public Boolean ServerErr => _serverErr;
        private string _responseStr;
        public string ResponseStr => _responseStr;
        private HeaderItem[] _headerItems;
        public HeaderItem[] HeaderItems => _headerItems;
        private string _loggingError;
        public string LoggingError => _loggingError;
        private ReturnResultStatus _resultStatus;
        public ReturnResultStatus ResultStatus => _resultStatus;


        /*****************/
        /*  Constructor  */
        /*****************/

        public RequestResult()
        {
            _success = false;
            _serverErr = false;
            _responseStr = string.Empty;
            _headerItems = [];
            _loggingError = string.Empty;
            _resultStatus = ReturnResultStatus.None;
        }


        /********************/
        /*  SetAsServerErr  */
        /********************/

        public void SetAsServerErr(string responseStr, HeaderItem[] headerItems, string loggingErr = "")
        {
            _serverErr = true;
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
            _resultStatus = ReturnResultStatus.Result500;
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
            _success = true;
            _responseStr = responseStr;
            _headerItems = headerItems;
            _resultStatus = ReturnResultStatus.Result200;
        }


        /*********************/
        /*  SetAsBadRequest  */
        /*********************/

        public void SetAsBadRequest(string responseStr, HeaderItem[] headerItems)
        {
            _responseStr = responseStr;
            _headerItems = headerItems;
            _resultStatus = ReturnResultStatus.Result400;
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
