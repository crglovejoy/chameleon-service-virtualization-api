using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Microsoft.Azure.Cosmos;

namespace Chameleon.CosmosData
{
    /************************/
    /**  CosmosDataAccess  **/
    /************************/

    /************************************************************************************************************************* 
     * This class provides a data access layer for interacting with Azure Cosmos DB. It implements the IDataAccess interface
     * and provides methods for updating and retrieving documents from a specified Cosmos DB container.
     *************************************************************************************************************************/
    public class CosmosDataAccess : IDataAccess
    {
        private readonly string _databaseName;
        private readonly string _containerName;
        private readonly string _partitionKeyPath;
        private readonly CosmosClient _client;
        private readonly Container _container;

        /*******************/
        /**  CONSTRUCTOR  **/
        /*******************/

        public CosmosDataAccess(CosmosClient client, string databaseName, string containerName, string partitionKeyPath)
        {
            _client = client;
            _databaseName = databaseName;
            _containerName = containerName;
            _partitionKeyPath = partitionKeyPath;
            _container = _client.GetContainer(_databaseName, _containerName);
        }


        /**********************/
        /**  DISPOSE METHOD  **/
        /**********************/

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }


        /***************************/
        /**  DATA DELETE METHODS  **/
        /***************************/

        public async Task DeleteDataById<T>(string id, string project) where T : DocumentBase
        {
            var response = await _container.DeleteItemAsync<T>(id, new PartitionKey(project));

            if (response.StatusCode != System.Net.HttpStatusCode.NoContent && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException($"Failed to delete document id {id} with type {typeof(T).Name} with return status code {response.StatusCode}");
            }
        }

        /***************************/
        /**  DATA UPDATE METHODS  **/
        /***************************/

        public async Task UpdateData<T>(T document) where T : DocumentBase
        {
            var response = await _container.UpsertItemAsync(document, new PartitionKey(document.project));

            // Upsert will return 200 (OK) for replace or 201 (Created) for insert. Throw if neither.
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new InvalidOperationException($"Failed to update document id {document.id} with type {document.documentType} with return status code {response.StatusCode}");
            }
        }

        // Backwards-compatible overload that accepts base type
        public Task UpdateData(DocumentBase document) => UpdateData<DocumentBase>(document);


        /************************/
        /**  DATA GET METHODS  **/
        /************************/

        public async Task<List<T>> GetDataByIdAsync<T>(string id, string project) where T : DocumentBase
        {
            var results = new List<T>();

            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(project));
                results.AddRange(response.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // If we didn't find a result, avoid having the exception propagate and just return the empty list below.
            }

            return results;
        }


        public async Task<List<T>> GetDataByTypeAsync<T>(DocumentTypeEnum documentType, string project) where T : DocumentBase
        {
            var queryDef = new QueryDefinition("SELECT * FROM c WHERE c.documentType = @docType")
                    .WithParameter("@docType", (int)documentType);
            return await GetQueryResults<T>(queryDef, project);
        }


        // Run a query by Schema Signature string value.
        public async Task<List<T>> GetDataBySchemaSignature<T>(string signatureStr, string project) where T : DocumentBase
        {
            var queryDef = new QueryDefinition("SELECT TOP 1 * FROM c WHERE c.schemaSignature = @signatureStr")
                    .WithParameter("@signatureStr", signatureStr);
            return await GetQueryResults<T>(queryDef, project);
        }


        // Run a query by Project for all documents cast as DocumentBase.
        public async Task<List<DocumentBase>> GetAllDocumentBaseByProject(string project)
        {
            var queryDef = new QueryDefinition($"SELECT * FROM c");
            return await GetQueryResults<DocumentBase>(queryDef, project);
        }


        // Returns our results list.
        private async Task<List<T>> GetQueryResults<T>(QueryDefinition queryDef, string project) where T : DocumentBase
        {
            var results = new List<T>();

            try
            {
                using var it = _container.GetItemQueryIterator<T>(queryDef, requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(project) });
                while (it.HasMoreResults)
                {
                    var page = await it.ReadNextAsync();
                    results.AddRange(page.Resource);
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // If we didn't find a result, avoid having the exception propagate and just return the empty list below.
            }

            return results;
        }
    }
}
