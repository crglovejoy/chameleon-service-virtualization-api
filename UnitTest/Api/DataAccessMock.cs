using Chameleon.CommonApiLib;
using Chameleon.CommonLib;

namespace UnitTest.Api
{
    public class DataAccessMock : IDataAccess
    {
        private int _scenario;
        private int _step;

        public DataAccessMock(int scenario)
        {
            _scenario = scenario;
            _step = 1;
        }

        public async Task UpdateData<T>(T document) where T : DocumentBase
        {
            throw new ArgumentException($"Scenario {_scenario} is not supported here");
        }


        public async Task UpdateData(DocumentBase document)
        {
            throw new ArgumentException($"Scenario {_scenario} is not supported here");
        }


        public async Task DeleteDataById<T>(string id, string project) where T : DocumentBase
        {
            throw new ArgumentException($"Scenario {_scenario} is not supported here");
        }

        public async Task<List<T>> GetDataByIdAsync<T>(string id, string project) where T : DocumentBase
        {
            var returnList = new List<T>();
            if (_scenario == 3)
            {
                if (id != RoutingDocHelper.RouteMatchingRespDocId)
                {
                    throw new ArgumentException($"Expected the Id to be {RoutingDocHelper.RouteMatchingRespDocId} but was {id} for scenario {_scenario}");
                }
                var respHelper = new RespDocHelper();
                returnList.Add((T)(object)respHelper.GetRespDoc(project, true, false));
                return returnList;
            }
            else if (_scenario == 4)
            {
                if (id != RoutingDocHelper.RouteNotMatchingRespDocId)
                {
                    throw new ArgumentException($"Expected the Id to be {RoutingDocHelper.RouteNotMatchingRespDocId} but was {id} for scenario {_scenario}");
                }
                var respHelper = new RespDocHelper();
                returnList.Add((T)(object)respHelper.GetRespDoc(project, false, false));
                return returnList;
            }
            else if (_scenario == 5)
            {
                if (id != RoutingDocHelper.RouteMatchingRespDocId)
                {
                    throw new ArgumentException($"Expected the Id to be {RoutingDocHelper.RouteNotMatchingRespDocId} but was {id} for scenario {_scenario}");
                }
                return returnList;
            }
            else if (_scenario == 6)
            {
                if (id != RoutingDocHelper.RouteMatchingRespDocId)
                {
                    throw new ArgumentException($"Expected the Id to be {RoutingDocHelper.RouteMatchingRespDocId} but was {id} for scenario {_scenario}");
                }
                var respHelper = new RespDocHelper();
                returnList.Add((T)(object)respHelper.GetRespDoc(project, true, true));
                return returnList;
            }
            else
            {
                throw new ArgumentException($"Scenario {_scenario} is not supported here");
            }
        }


        public async Task<List<T>> GetDataByTypeAsync<T>(DocumentTypeEnum documentType, string project) where T : DocumentBase
        {
            var returnList = new List<T>();
            if (_scenario == 1)
            {
                // Send back an empty NoSchemaMatchRespDoc
                return returnList;
            }
            else if (_scenario == 2)
            {
                // Send back a NoSchemaMatchRespDoc
                returnList.Add((T)(object)new NoSchemaMatchRespDoc(project, ApiTestCommon.JsonHeaderItems, ApiTestCommon.NoSchemaMatchRespStr));
                return returnList;
            }
            else
            {
                throw new ArgumentException($"Scenario {_scenario} is not supported here");
            }
        }


        public async Task<List<T>> GetDataBySchemaSignature<T>(string signatureStr, string project) where T : DocumentBase
        {
            var returnList = new List<T>();

            if (_scenario == 1)
            {
                // Send back an empty list for a no schema match scenario
                return returnList;
            }
            else if (_scenario == 2)
            {
                // Send back an empty list for a no schema match scenario
                return returnList;
            }
            else if (_scenario == 3)
            {
                // Send back a routing doc with a matching route
                RoutingDocHelper rteDocHelper = new();
                returnList.Add((T)(object)rteDocHelper.GetRoutingDoc(achieveMatch:true, useWildcard:false, useIdVal:false, project));
                return returnList;
            }
            else if (_scenario == 4)
            {
                // Send back a routing doc without a matching route
                RoutingDocHelper rteDocHelper = new();
                returnList.Add((T)(object)rteDocHelper.GetRoutingDoc(achieveMatch: false, useWildcard: false, useIdVal: false, project));
                return returnList;
            }
            else if (_scenario == 5)
            {
                // Send back a routing doc with a matching route
                RoutingDocHelper rteDocHelper = new();
                returnList.Add((T)(object)rteDocHelper.GetRoutingDoc(achieveMatch: true, useWildcard: false, useIdVal: false, project));
                return returnList;
            }
            else if (_scenario == 6)
            {
                // Send back a routing doc with a matching route
                RoutingDocHelper rteDocHelper = new();
                returnList.Add((T)(object)rteDocHelper.GetRoutingDoc(achieveMatch: true, useWildcard: false, useIdVal: false, project));
                return returnList;
            }
            else
            {
                throw new ArgumentException($"Scenario {_scenario} is not supported here");
            }
        }


        public async Task<List<DocumentBase>> GetAllDocumentBaseByProject(string project)
        {
            throw new ArgumentException($"Scenario {_scenario} is not supported here");
        }


        public void Dispose()
        {
            return;
        }
    }
}
