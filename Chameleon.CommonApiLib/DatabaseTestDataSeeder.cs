using Chameleon.CommonLib;
using Newtonsoft.Json;


namespace Chameleon.CommonApiLib
{
    /*************************/
    /**  DATA SEED PROJECT  **/
    /*************************/

    // A class used only here to manage configuration data used to create test projects/data in the database.
    internal class DataSeedProject
    {
        internal IDataAccess DataAccess;
        internal string Project;
        internal string ContentHdrVal;
        internal string RouteMatchResp;
        internal string NoSchemaMatchResp;
        internal string NoRouteMatchResp;
        internal MessageProperties MsgProps;
        internal string ReqMsgStr;
        internal string[] PropertyVariableNames;
        internal string NoSchemaMatchRespDocId;
        internal string RouteMatchRespDocId;
        internal string NoRouteMatchRespDocId;

        internal DataSeedProject(IDataAccess dataAccess, string project, string contentHdrVal, string routeMatchResp, 
            string noSchemaMatchResp, string noRouteMatchResp, string reqMsgStr, string[] propertyVariableNames, MessageProperties msgProps)
        {
            DataAccess = dataAccess;
            Project = project;
            ContentHdrVal = contentHdrVal;
            RouteMatchResp = routeMatchResp;
            NoSchemaMatchResp = noSchemaMatchResp;
            NoRouteMatchResp = noRouteMatchResp;
            ReqMsgStr = reqMsgStr;
            PropertyVariableNames = propertyVariableNames;
            MsgProps = msgProps;
            NoSchemaMatchRespDocId = string.Empty;
            RouteMatchRespDocId = string.Empty;
            NoRouteMatchRespDocId = string.Empty;
        }
    }


    /****************************/
    /**  DATABASE DATA SEEDER  **/
    /****************************/

    public class DatabaseDataSeeder
    {
        private DataSeedProject[] _seedProjArr;

        public DatabaseDataSeeder(IDataAccess dataAccess, MessageParserBase jsonParser, MessageParserBase xmlParser)
        {
            // Create configurations for a JSON project and an XML project that we want built in our database for testing.
            _seedProjArr =
            [
                // Json project
                new DataSeedProject(
                    dataAccess: dataAccess,
                    project: "AutoTestProjectJson",
                    contentHdrVal: "application/json",
                    routeMatchResp: JsonConvert.SerializeObject(new TestHikerResponse(true)),
                    noSchemaMatchResp: @"{""message"":""NoSchemaMatchFoundResponse""}",
                    noRouteMatchResp: @"{""message"":""NoMatchingRouteFoundResponse""}",
                    reqMsgStr: JsonConvert.SerializeObject(new TestHiker()),
                    propertyVariableNames: TestHikerResponse.GetPropertyVariablesArr(),
                    msgProps: jsonParser.ProcessMessageIntoMessageProperties(JsonConvert.SerializeObject(new TestHiker()))
                ),
                // Xml project
                new DataSeedProject(
                    dataAccess: dataAccess,
                    project: "AutoTestProjectXml",
                    contentHdrVal: "application/xml",
                    routeMatchResp: TestXmlBooks.GetResponseStr(true),
                    noSchemaMatchResp: @"<?xml version=""1.0"" encoding=""UTF-8""?><Message>NoSchemaMatchFoundResponse</Message>",
                    noRouteMatchResp: @"<?xml version=""1.0"" encoding=""UTF-8""?><Message>NoMatchingRouteFoundResponse</Message>",
                    reqMsgStr: TestXmlBooks.XmlRequestMessage,
                    propertyVariableNames: TestXmlBooks.GetPropertyVariablesArr(),
                    msgProps: xmlParser.ProcessMessageIntoMessageProperties(TestXmlBooks.XmlRequestMessage)
                )
            ];
        }


        /*************************/
        /*  CreateTestDataAsync  */
        /*************************/

        public async Task CreateTestDataAsync()
        {
            try
            {
                // For each project, create the database content.
                foreach (var proj in _seedProjArr)
                {
                    // Delete all data for the project
                    await DeleteProjectData.DeleteData(proj);

                    // Create the project in the project document
                    await ProjectDocCreation.ProjectDocCreate(proj);

                    // Create the no schema match response for the project.
                    await NoSchemaMatchDocCreation.NoSchemaMatchDocCreate(proj);

                    // Create the route match response for the project.
                    await RespDocCreation.RespDocCreate(proj, true);

                    // Create the schema match - no route match response.
                    await RespDocCreation.RespDocCreate(proj, false);

                    // Create the routing document.
                    await RoutingDocCreation.RoutingDocCreate(proj);
                }
            }
            catch
            {
                throw;
            }
        }
    }

    
    /**************************/
    /**  DELETE PRJECT DATA  **/
    /**************************/

    internal static class DeleteProjectData
    {
        internal static async Task DeleteData(DataSeedProject proj)
        {
            List<DocumentBase> docList = await proj.DataAccess.GetAllDocumentBaseByProject(proj.Project);

            foreach(var doc in docList)
            {
                await proj.DataAccess.DeleteDataById<DocumentBase>(doc.id, proj.Project);
            }
        }
    }

    
    /****************************/
    /**  PROJECT DOC CREATION  **/
    /****************************/

    internal static class ProjectDocCreation
    {
        // Add the ProjectDoc to the data source if it's not already there...
        internal static async Task ProjectDocCreate(DataSeedProject proj)
        {
            ProjectDoc projectDoc = null!;
            var docListProj = await proj.DataAccess.GetDataByTypeAsync<ProjectDoc>(DocumentTypeEnum.ProjectDoc, DocumentBase.CommonProjectName);
            if (docListProj == null || docListProj.Count == 0)
            {
                projectDoc = GetProjectDoc(proj.Project);
            }
            else
            {
                projectDoc = docListProj[0];
                projectDoc.AddProjectName(proj.Project);
            }

            await proj.DataAccess.UpdateData<ProjectDoc>(projectDoc);
        }

        private static ProjectDoc GetProjectDoc(string project)
        {
            string[] projNames = [
                new string(project),
            ];

            return new ProjectDoc(projNames);
        }
    }


    /**********************************/
    /**  NO SCHEMA MATCH DOC CREATE  **/
    /**********************************/

    internal static class NoSchemaMatchDocCreation
    {
        internal static async Task NoSchemaMatchDocCreate(DataSeedProject proj)
        {
            NoSchemaMatchRespDoc noSchemaMatchRespDoc = null!;
            var docListNoSchema = await proj.DataAccess.GetDataByTypeAsync<NoSchemaMatchRespDoc>(DocumentTypeEnum.NoSchemaMatchRespDoc, proj.Project);
            if (docListNoSchema == null || docListNoSchema.Count == 0)
            {
                noSchemaMatchRespDoc = GetNoSchemaMatchRespDoc(proj.Project, proj.NoSchemaMatchResp, proj.ContentHdrVal);
                await proj.DataAccess.UpdateData<NoSchemaMatchRespDoc>(noSchemaMatchRespDoc);
            }
            else
            {
                noSchemaMatchRespDoc = docListNoSchema[0];
            }

            proj.NoSchemaMatchRespDocId = noSchemaMatchRespDoc.id;
        }

        private static NoSchemaMatchRespDoc GetNoSchemaMatchRespDoc(string project, string respStr, string hdrContentValue)
        {
            HeaderItem[] hdrs = [];

            if (!string.IsNullOrWhiteSpace(hdrContentValue))
            {
                hdrs = [
                    new HeaderItem("Content-Type", hdrContentValue)
                ];
            }

            return new NoSchemaMatchRespDoc(project, hdrs, respStr);
        }
    }


    /*************************/
    /**  RESP DOC CREATION  **/
    /*************************/

    internal static class RespDocCreation
    {
        internal static async Task RespDocCreate(DataSeedProject proj, Boolean matchingResp)
        {
            RespDoc respDoc = null!;
            var docListResp = await proj.DataAccess.GetDataByTypeAsync<RespDoc>(DocumentTypeEnum.RespDoc, proj.Project);

            foreach (RespDoc doc in docListResp)
            {
                if (doc.responseContent == proj.ReqMsgStr)
                {
                    respDoc = doc;
                    break;
                }
            }

            if (respDoc == null)
            {
                respDoc = GetRespDoc(proj.Project, matchingResp ? proj.RouteMatchResp : proj.NoRouteMatchResp, proj.ContentHdrVal);
                await proj.DataAccess.UpdateData<RespDoc>(respDoc);
            }

            if (matchingResp)
            {
                proj.RouteMatchRespDocId = respDoc.id;
            }
            else
            {
                proj.NoRouteMatchRespDocId = respDoc.id;
            }
        }

        private static RespDoc GetRespDoc(string project, string respStr, string hdrContentValue)
        {
            HeaderItem[] hdrs = [];

            if (!string.IsNullOrWhiteSpace(hdrContentValue))
            {
                hdrs = [
                    new HeaderItem("Content-Type", hdrContentValue)
                ];
            }

            return new RespDoc(project, hdrs, respStr);
        }
    }


    /****************************/
    /**  ROUTING DOC CREATION  **/
    /****************************/

    internal static class RoutingDocCreation
    {
        internal static async Task RoutingDocCreate(DataSeedProject proj)
        {
            string existingRoutingDocId = string.Empty;

            var docListRoute = await proj.DataAccess.GetDataBySchemaSignature<RoutingDoc>(proj.MsgProps.schemaSignatureHash, proj.Project);
            if (docListRoute != null && docListRoute.Count > 0)
            {
                existingRoutingDocId = docListRoute[0].id;
            }

            RoutingDoc newRoutingDoc = GetRoutingDoc(proj.Project, proj.MsgProps, proj.NoRouteMatchRespDocId, proj.RouteMatchRespDocId, proj.PropertyVariableNames);
            await proj.DataAccess.UpdateData<RoutingDoc>(newRoutingDoc);
        }

        private static RoutingDoc GetRoutingDoc(string project, MessageProperties mProps, string noRouteMatchRespDocId, 
                string respDocId, string[] propertyVariableNames)
        {
            var route = GetRoute(mProps, respDocId, propertyVariableNames);
            return new RoutingDoc(project, mProps.schemaSignatureHash, [route], noRouteMatchRespDocId, mProps.originalMessageStr);
        }

        /* Creates a route containing a filter for key/value pairs (except those exluded in the propsToReplace string array)
         * in the MessageProperties messageDict. */
        private static Route GetRoute(MessageProperties mProps, string respDocId, string[] propertyVariableNames)
        {
            List<MatchFilter> routingItems = new();
            List<string> replaceList = new();

            foreach (var key in mProps.messageDict.Keys)
            {
                if (propertyVariableNames.Contains(key))
                {
                    continue;
                }

                // Add a route that matches all remaining properties in the MessageProperties messageDict as a test.
                if (mProps.messageDict.TryGetValue(key, out var value))
                {
                    routingItems.Add(new MatchFilter(key, value));
                }
                else
                {
                    throw new Exception($"Couldn't find a value for key {key}");
                }
            }

            foreach (var str in propertyVariableNames)
            {
                if (!replaceList.Contains(str))
                {
                    replaceList.Add(str);
                }
            }

            return new Route([.. routingItems], [.. replaceList], respDocId);
        }
    }
}
