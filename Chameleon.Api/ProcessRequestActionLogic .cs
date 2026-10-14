using Chameleon.CommonApiLib;
using Chameleon.CommonLib;

namespace Chameleon.Api
{
    public class ProcessRequestActionLogic(IDataAccess dataAccess, JsonParser jsonParser, XmlParser xmlParser) : IProcessRequestActionLogic
    {
        private readonly IDataAccess _dataAccess = dataAccess;
        private readonly JsonParser _jsonParser = jsonParser;
        private readonly XmlParser _xmlParser = xmlParser;

        public async Task<RequestResult> ProcessRequestMessageAsync(ParserType parserType, Stream messageStream, string project)
        {
            string messageStr = await GetStringFromStreamAsync(messageStream);
            return await ProcessRequestMessageAsync(parserType, messageStr, project);
        }


        public async Task<RequestResult> ProcessRequestMessageAsync(ParserType parserType, string messageStr, string project)
        {
            try
            {
                MessageProperties? mProps;
                
                switch (parserType)
                {
                    case ParserType.JSON:
                        mProps = _jsonParser.ProcessMessageIntoMessageProperties(messageStr);
                        break;
                    case ParserType.XML:
                        mProps = _xmlParser.ProcessMessageIntoMessageProperties(messageStr);
                        break;
                    default:
                        return new RequestResult(false, true, $"{parserType} is not supported by the controller action.", []);
                }

                List<RoutingDoc> routingDocList = await _dataAccess.GetDataBySchemaSignature<RoutingDoc>(mProps.schemaSignatureHash, project);

                if (routingDocList == null || routingDocList.Count == 0)
                {
                    return await GetNoSchemaMatchRequestResult(project);
                }

                return await GetSchemaMatchRequestResult(routingDocList[0], mProps, project);
            }
            catch(Exception ex)
            {
                return new RequestResult(false, true, $"Processing error: {ex.Message}", ContentPlainTextHeader, ex);
            }
        }

        public async Task<RequestResult> GetNoSchemaMatchRequestResult(string project)
        {
            List<NoSchemaMatchRespDoc> nsmList = await _dataAccess.GetDataByTypeAsync<NoSchemaMatchRespDoc>(DocumentTypeEnum.NoSchemaMatchRespDoc, project);

            if (nsmList == null || nsmList.Count == 0)
            {
                return new RequestResult(false, false, $"Found no schema match and could not find a NoSchemaMatchResponse to provide for project {project}", ContentPlainTextHeader);
            }

            return new RequestResult(true, false, nsmList[0].responseContent, nsmList[0].headerItems);
        }

        public async Task<RequestResult> GetSchemaMatchRequestResult(RoutingDoc routingDoc, MessageProperties mProps, string project)
        {
            string respId = routingDoc.GetResponseDocumentId(mProps.messageDict);
            List<RespDoc> respDocList = await _dataAccess.GetDataByIdAsync<RespDoc>(respId, project);

            if (respDocList == null || respDocList.Count == 0)
            {
                return new RequestResult(false, true, $"Found a schema match but failed to find a response document with id {respId}", ContentPlainTextHeader);
            }

            return new RequestResult(true, false, respDocList[0].responseContent, respDocList[0].headerItems);
        }

        private static async Task<string> GetStringFromStreamAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static HeaderItem[] ContentPlainTextHeader => [new HeaderItem("Content-Type", "text/plain")]; 
    }
}
