
namespace Chameleon.CommonLib
{
    public class RoutingDoc : DocumentBase
    {
        /* The schema signature that this routing document is associated with.  The schema signature of new request messages 
         * will be used to query the dataset for a routing document that has the same schema signature.  If a routing document 
         * is found, the routes in that routing document will be used to determine the response document id for the request 
         * message.
         */
        public readonly string schemaSignature;

         /* A colleciton of routes.  Each route consists of a label/value pairs that are used to match against the request message.
         * If a route matches, the response document id for that route is returned.  If no route matches, the 
         * noSchemaMatchResponseDocumentId is returned.
         */
        private Route[] _routes;
        public Route[] routes => _routes;

        // The response document id to return if no route matches the request message.
        private string _noRouteMatchResponseDocumentId;
        public string noRouteMatchResponseDocumentId => _noRouteMatchResponseDocumentId;

        // The original message string that was used to create the routing document.  This is used for debugging purposes.
        public readonly string originalMessageStr;

        public RoutingDoc(string project, string schemaSignature, Route[] routes, string noRouteMatchResponseDocumentId, 
            string originalMessageStr, string id = "") : base(project, DocumentTypeEnum.RoutingDoc, id)
        {
            this.schemaSignature = schemaSignature;
            this._routes = routes;
            this._noRouteMatchResponseDocumentId = noRouteMatchResponseDocumentId;
            this.originalMessageStr = originalMessageStr;
        }

        // Add a new route to the routing document.
        public void AddRoute(Route route)
        {
            var routesList = new List<Route>(_routes);
            routesList.Add(route);
            _routes = routesList.ToArray();
        }

        /* Determine if we have a route that matches the request message, and if so, return the response 
         * document id for that route.  If no route matches, return the noSchemaMatchResponseDocumentId.
         */

        public string GetResponseDocumentId(Dictionary<string, object> messageDict)
        {
            foreach (var route in _routes)
            {
                bool isMatch = true;
                foreach (var routingItem in route.routingItems)
                {
                    if (!messageDict.TryGetValue(routingItem.propertyName, out var value) || !value.Equals(routingItem.propertyValue))
                    {
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch)
                {
                    return route.responseDocumentId;
                }
            }
            return _noRouteMatchResponseDocumentId;
        }
    }


    /* A Route consists of one or more label/value pairs that are used to match against the content of a request message.
     * The Route also contains the response document id to return if the route matches the request message content.
     */
    public class Route(RoutingItem[] routingItems, string responseDocumentId)
    {
        private RoutingItem[] _routingItems = routingItems;
        public RoutingItem[] routingItems => _routingItems;
        private string _responseDocumentId = responseDocumentId;
        public string responseDocumentId => _responseDocumentId;


        /* Add a routing item to the Route.  If a routing item with the same property name already exists, throw an 
         * exception.
         */
        public void AddRoutingItem(RoutingItem routingItem)
        {
            if (_routingItems.Any(item => item.propertyName == routingItem.propertyName))
            {
                throw new InvalidOperationException($"A routing item with the property name '{routingItem.propertyName}' already exists in this route.");
            }

            var routingItemsList = new List<RoutingItem>(routingItems);
            routingItemsList.Add(routingItem);
            _routingItems = [.. routingItemsList];
        }
    }

    /* A RoutingItem consists of a property name and a property value that are used to match against the content 
     * of a request message. 
     */
    public class RoutingItem(string propertyName, object propertyValue)
    {
        private string _propertyName = propertyName;
        public string propertyName => _propertyName;

        private object _propertyValue = propertyValue;
        public object propertyValue => _propertyValue;
    }   
}
