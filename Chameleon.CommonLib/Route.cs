

namespace Chameleon.CommonLib
{
    /*************/
    /**  ROUTE  **/
    /*************/

    /* A Route consists of one or more match filters that are used to compare against the content of a request message
     * to see if there is a match.  If there is a match, the Route also contains the response document id that should be 
     * used to return a response. Property variable names are used to replace the variables in the response string with
     * property values from the request message - see the RespDoc implementation */

    public class Route(MatchFilter[] matchFilters, string[] propertyVariableNames, string responseDocumentId)
    {
        private MatchFilter[] _matchFilters = matchFilters;
        public MatchFilter[] matchFilters => _matchFilters;

        private string[] _propertyVariableNames = propertyVariableNames;
        public string[] propertyVariableNames => _propertyVariableNames;

        private string _responseDocumentId = responseDocumentId;
        public string responseDocumentId => _responseDocumentId;


        /****************************/
        /*  Add a new match filter  */
        /****************************/

        public void AddMatchFilter(MatchFilter newMatchFilter)
        {
            if (_matchFilters.Any(item => item.propertyName == newMatchFilter.propertyName))
            {
                throw new InvalidOperationException($"A match filter with the property name '{newMatchFilter.propertyName}' already exists in this route.");
            }

            _matchFilters = ArrayHelpers.AddObjectToArray<MatchFilter>(newMatchFilter, _matchFilters);
        }


        /*************************************************************/
        /*  Determine if a route is a match for the request message  */
        /*************************************************************/

        public Boolean IsMatchingRoute(Dictionary<string, object> messageDict)
        {
            if (matchFilters.Length == 0) return false;
            if (matchFilters.Length == 1 && matchFilters[0].propertyName == "*") return true;

            foreach (var matchFilt in matchFilters)
            {
                if (!messageDict.TryGetValue(matchFilt.propertyName, out var value) || !value.Equals(matchFilt.propertyValue))
                {
                    return false;
                }
            }

            return true;
        }


        /*********************************/
        /*  Add a property variable name */
        /*********************************/

        public void AddPropertyVariableName(string newVariableName)
        {
            if (string.IsNullOrWhiteSpace(newVariableName))
            {
                return;
            }

            if (_propertyVariableNames.Any(item => item == newVariableName))
            {
                throw new InvalidOperationException($"A variable name for '{newVariableName}' already exists in this route.");
            }

            _propertyVariableNames = ArrayHelpers.AddObjectToArray<string>(newVariableName, _propertyVariableNames);
        }
    }


    /********************/
    /**  MATCH FILTER  **/
    /********************/

    public class MatchFilter(string propertyName, object propertyValue)
    {
        private string _propertyName = propertyName;
        public string propertyName => _propertyName;

        private object _propertyValue = propertyValue;
        public object propertyValue => _propertyValue;
    }
}
