
namespace Chameleon.CommonLib
{

    /*******************/
    /**  ROUTING DOC  **/
    /*******************/

    public class RoutingDoc : DocumentBase
    {
        public readonly string schemaSignature;

        private Route[] _routes;
        public Route[] routes => _routes;

        private string _noRouteMatchResponseDocumentId;
        public string noRouteMatchResponseDocumentId => _noRouteMatchResponseDocumentId;

        public readonly string originalMessageStr;


        /*****************/
        /*  Constructor  */
        /*****************/

        public RoutingDoc(string project, string schemaSignature, Route[] routes, string noRouteMatchResponseDocumentId, 
            string originalMessageStr, string id = "") : base(project, DocumentTypeEnum.RoutingDoc, id)
        {
            this.schemaSignature = schemaSignature;
            this._routes = routes;
            this._noRouteMatchResponseDocumentId = noRouteMatchResponseDocumentId;
            this.originalMessageStr = originalMessageStr;
        }


        /*******************/
        /*  Add New Route  */
        /*******************/

        public void AddRoute(Route newRoute)
        {
            _routes = ArrayHelpers.AddObjectToArray<Route>(newRoute, _routes);
        }


        /***************************/
        /*  Find a matching route  */
        /***************************/

        // Determine if a request message matches the filters for a route and return the matching Route or null
        public Route? GetMatchingRoute(Dictionary<string, object> messageDict)
        {
            foreach (var route in _routes)
            {
                if (route.IsMatchingRoute(messageDict))
                {
                    return route;
                }
            }
            return null;
        }
    }
}
