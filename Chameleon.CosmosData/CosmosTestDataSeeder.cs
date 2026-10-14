using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Newtonsoft.Json;


namespace Chameleon.CosmosData
{
    public class CosmosDataSeeder(IDataAccess dataAccess)
    {
        private string _testProject = "AutoTestProject";
        private const string _hdrContentType = "Content-Type";
        private const string _hdrValJson = "application/json";
        private const string _matchRouteFoundResp = @"{""message"":""MatchingRouteFoundResponse""}";
        private const string _matchRouteNotFoundResp = @"{""message"":""NoMatchingRouteFoundResponse""}";
        private const string _noSchemaMatchFoundResp = @"{""message"":""NoSchemaMatchFoundResponse""}";

        public async Task CreateAsync()
        {
            try
            {
                await DeleteProjectData.DeleteData(dataAccess, _testProject);
                await ProjectDocCreation.ProjectDocCreate(dataAccess, _testProject);
                string nsmRespId = await NoSchemaMatchDocCreation.NoSchemaMatchDocCreateAndReturnId(dataAccess, _testProject, _noSchemaMatchFoundResp, _hdrContentType, _hdrValJson);
                string rmRespId = await RespDocCreation.RespDocCreateAndReturnId(dataAccess, _testProject, _matchRouteFoundResp, _hdrContentType, _hdrValJson);
                string nrmRespId = await RespDocCreation.RespDocCreateAndReturnId(dataAccess, _testProject, _matchRouteNotFoundResp, _hdrContentType, _hdrValJson);
                await RoutingDocCreation.RoutingDocCreate(dataAccess, _testProject, rmRespId, nrmRespId);
            }
            catch
            {
                throw;
            }
        }
    }

    internal static class DeleteProjectData
    {
        internal static async Task DeleteData(IDataAccess dataAccess, string project)
        {
            List<DocumentBase> docList = await dataAccess.GetAllDocumentBaseByProject(project);

            foreach(var doc in docList)
            {
                await dataAccess.DeleteDataById<DocumentBase>(doc.id, project);
            }
        }
    }


    internal static class ProjectDocCreation
    {
        // Add the ProjectDoc to the data source if it's not already there...
        internal static async Task ProjectDocCreate(IDataAccess dataAccess, string project)
        {
            ProjectDoc projectDoc = null!;
            var docListProj = await dataAccess.GetDataByTypeAsync<ProjectDoc>(DocumentTypeEnum.ProjectDoc, DocumentBase.CommonProjectName);
            if (docListProj == null || docListProj.Count == 0)
            {
                projectDoc = GetProjectDoc(project);
            }
            else
            {
                projectDoc = docListProj[0];
                projectDoc.AddProjectName(project);
            }

            await dataAccess.UpdateData<ProjectDoc>(projectDoc);
        }

        private static ProjectDoc GetProjectDoc(string project)
        {
            string[] projNames = [
                new string(project),
                new string(project)
            ];

            return new ProjectDoc(projNames);
        }
    }

    internal static class NoSchemaMatchDocCreation
    {
        internal static async Task<string> NoSchemaMatchDocCreateAndReturnId(IDataAccess dataAccess, string project, string respStr, string hdrContentType, string hdrContentValue)
        {
            NoSchemaMatchRespDoc noSchemaMatchRespDoc = null!;
            var docListNoSchema = await dataAccess.GetDataByTypeAsync<NoSchemaMatchRespDoc>(DocumentTypeEnum.NoSchemaMatchRespDoc, project);
            if (docListNoSchema == null || docListNoSchema.Count == 0)
            {
                noSchemaMatchRespDoc = GetNoSchemaMatchRespDoc(project, respStr, hdrContentType, hdrContentValue);
                await dataAccess.UpdateData<NoSchemaMatchRespDoc>(noSchemaMatchRespDoc);
            }
            else
            {
                noSchemaMatchRespDoc = docListNoSchema[0];
            }

            return noSchemaMatchRespDoc.id;
        }

        private static NoSchemaMatchRespDoc GetNoSchemaMatchRespDoc(string project, string respStr, string hdrContentType, string hdrContentValue)
        {
            HeaderItem[] hdrs = [];

            if (!string.IsNullOrWhiteSpace(hdrContentType) && !string.IsNullOrWhiteSpace(hdrContentValue))
            {
                hdrs = [
                    new HeaderItem(hdrContentType, hdrContentValue)
                ];
            }

            return new NoSchemaMatchRespDoc(project, hdrs, respStr);
        }
    }


    internal static class RespDocCreation
    {
        internal static async Task<string> RespDocCreateAndReturnId(IDataAccess dataAccess, string project, string respStr, string hdrContentType, string hdrContentValue)
        {
            RespDoc respDoc = null!;
            var docListResp = await dataAccess.GetDataByTypeAsync<RespDoc>(DocumentTypeEnum.RespDoc, project);

            foreach (RespDoc doc in docListResp)
            {
                if (doc.responseContent == respStr)
                {
                    respDoc = doc;
                    break;
                }
            }

            if (respDoc == null)
            {
                respDoc = GetRespDoc(project, respStr, hdrContentType, hdrContentValue);
                await dataAccess.UpdateData<RespDoc>(respDoc);
            }

            return respDoc.id;
        }

        private static RespDoc GetRespDoc(string project, string respStr, string hdrContentType, string hdrContentValue)
        {
            HeaderItem[] hdrs = [];

            if (!string.IsNullOrWhiteSpace(hdrContentType) && !string.IsNullOrWhiteSpace(hdrContentValue))
            {
                hdrs = [
                    new HeaderItem(hdrContentType, hdrContentValue)
                ];
            }

            return new RespDoc(project, hdrs, respStr);
        }
    }


    internal static class RoutingDocCreation
    {
        internal static async Task RoutingDocCreate(IDataAccess dataAccess, string project, string routeMatchRespDocId, string noRouteMatchRespDocId)
        {
            TestHiker hiker = new();
            JsonParser jsonParser = new();
            MessageProperties mProps = jsonParser.ProcessMessageIntoClassSpecificMessageProperties<TestHiker>(JsonConvert.SerializeObject(hiker));
            string existingRoutingDocId = string.Empty;

            var docListRoute = await dataAccess.GetDataBySchemaSignature<RoutingDoc>(mProps.schemaSignatureHash, project);
            if (docListRoute != null && docListRoute.Count > 0)
            {
                existingRoutingDocId = docListRoute[0].id;
            }

            RoutingDoc newRoutingDoc = GetRoutingDoc(project, mProps, noRouteMatchRespDocId, routeMatchRespDocId, existingRoutingDocId);
            await dataAccess.UpdateData<RoutingDoc>(newRoutingDoc);
        }

        private static RoutingDoc GetRoutingDoc(string project, MessageProperties mProps, string noRouteMatchRespDocId, string respDocId, string existingRoutingDocId)
        {
            var route = GetRoute(mProps, respDocId);
            return new RoutingDoc(project, mProps.schemaSignatureHash, [route], noRouteMatchRespDocId, mProps.originalMessageStr, existingRoutingDocId);
        }

        // Creates a route containing a filter for EVERY key/value pair in the MessageProperties messageDict.
        private static Route GetRoute(MessageProperties mProps, string respDocId)
        {
            List<RoutingItem> routingItems = new();
            foreach (var key in mProps.messageDict.Keys)
            {
                // Skip adding FirstName and LastName keys and values to the route - let these values in the request be
                // variable for getting a route match.
                if (key == "FirstName" || key == "LastName")
                {
                    continue;
                }

                // Add a route that matches all remaining properties in the MessageProperties messageDict as a test.
                if (mProps.messageDict.TryGetValue(key, out var value))
                {
                    routingItems.Add(new RoutingItem(key, value));
                }
                else
                {
                    throw new Exception($"Couldn't find a value for key {key}");
                }
            }

            return new Route([.. routingItems], respDocId);
        }
    }
}
