using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Chameleon.Parsers;

namespace Chameleon.Api
{
    /************************************/
    /**  PROCESS REQUEST ACTION LOGIC  **/
    /************************************/

    public class ProcessRequestActionLogic(IDataAccess dataAccess, JsonParser jsonParser, XmlParser xmlParser) : IProcessRequestActionLogic
    {
        private readonly IDataAccess _dataAccess = dataAccess;
        private readonly JsonParser _jsonParser = jsonParser;
        private readonly XmlParser _xmlParser = xmlParser;

      
        /********************************/
        /*  ProcessRequestMessageAsync  */
        /********************************/

        public async Task<RequestResult> ProcessRequestMessageAsync(ParserType parserType, Stream messageStream, string project)
        {
            RequestResult reqResult = new();
            string messageStr = await StringHelpers.GetStringFromStreamAsync(messageStream);

            try
            {
                MessageProperties? mProps;
                
                // Call the appropriate parser to acquire the request message MessageProperties.
                switch (parserType)
                {
                    case ParserType.JSON:
                        mProps = _jsonParser.ProcessMessageIntoMessageProperties(messageStr);
                        break;
                    case ParserType.XML:
                        mProps = _xmlParser.ProcessMessageIntoMessageProperties(messageStr);
                        break;
                    default:
                        reqResult.SetAsServerErr($"{parserType} is not supported by the controller action.", RequestResult.ContentPlainTextHeader);
                        return reqResult;
                }

                // Look for a routing document that matches the schema signature of the request message.
                List<RoutingDoc> routingDocList = await _dataAccess.GetDataBySchemaSignature<RoutingDoc>(mProps.schemaSignatureHash, project);

                if (routingDocList == null || routingDocList.Count == 0)
                {
                     /* We did not achieve a schema match, so get the no schema match response document for the project
                     * and return a response. */
                    return await GetNoSchemaMatchRequestResult(project, reqResult);
                }

                // We have a schema match, now look for a route match response to return.
                return await GetSchemaMatchRequestResult(routingDocList[0], mProps.messageDict, project, reqResult);
            }
            catch(Exception ex)
            {
                reqResult.SetAsServerErr(ex, RequestResult.ContentPlainTextHeader);
                return reqResult;
            }
        }


        /***********************************/
        /*  GetNoSchemaMatchRequestResult  */
        /***********************************/
        
        private async Task<RequestResult> GetNoSchemaMatchRequestResult(string project, RequestResult reqResult)
        {
            List<NoSchemaMatchRespDoc> nsmList = await _dataAccess.GetDataByTypeAsync<NoSchemaMatchRespDoc>(DocumentTypeEnum.NoSchemaMatchRespDoc, project);

            if (nsmList == null || nsmList.Count == 0)
            {
                // UNIT TEST SCENARIO #1
                reqResult.SetAsNotFound($"Found no schema match and could not find a NoSchemaMatchResponse to provide for project {project}", 
                    RequestResult.ContentPlainTextHeader);
                return reqResult;
            }

            // UNIT TEST SCENARIO #2
            reqResult.SetAsSuccess(nsmList[0].responseContent, nsmList[0].headerItems);
            return reqResult;
        }


        /*********************************/
        /*  GetSchemaMatchRequestResult  */
        /*********************************/

        private async Task<RequestResult> GetSchemaMatchRequestResult(RoutingDoc routingDoc, Dictionary<string, object> reqMsgDict, string project,
            RequestResult reqResult)
        {
            CommonLib.Route? matchRoute = routingDoc.GetMatchingRoute(reqMsgDict);

            string respDocId;
            
            if (matchRoute != null)
            {
                /* UNIT TEST SCENARIO #3
                 * This is the matching route, so use the route response document id to retrieve the RespDoc from the database. */
                respDocId = matchRoute.responseDocumentId;
            }
            else
            {
                /* UNIT TEST SCENARIO #4
                 * We did not get a route match, so use the no route match response doc id to retrieve the RespDoc from the database. */
                respDocId = routingDoc.noRouteMatchResponseDocumentId;
            }

            // Retrieve the RespDoc (using respDocId) from the database.
            var respDocList = await _dataAccess.GetDataByIdAsync<RespDoc>(respDocId, project);

            if (respDocList == null || respDocList.Count == 0)
            {
                // UNIT TEST SCENARIO #5
                reqResult.SetAsServerErr($"Found a schema match but failed to find a response document with id {respDocId}", RequestResult.ContentPlainTextHeader);
                return reqResult;
            }

            // Acquire the response message string to use in the response.
            string respContent;
            if (matchRoute != null)
            {
                /* UNIT TEST SCENARIOS #3 & #6
                 * This the route match response, so do any variable string replacements in the response string before we return it. */
                respContent = RespDoc.GetVariablesResolvedResponse(respDocList[0].responseContent, reqMsgDict, matchRoute.propertyVariableNames);
            }
            else
            {
                /* UNIT TEST SCENARIO #5
                 * This is the no route match response - no variable replacement work to do here. */
                respContent = respDocList[0].responseContent;
            }

            reqResult.SetAsSuccess(respContent, respDocList[0].headerItems);
            return reqResult;
        }
    }
}
