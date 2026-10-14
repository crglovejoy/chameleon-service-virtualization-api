using Chameleon.CommonLib;

namespace Chameleon.CommonApiLib
{
    public interface IDataAccess
    {
        public Task UpdateData<T>(T document) where T : DocumentBase;
        public Task UpdateData(DocumentBase document);
        public Task DeleteDataById<T>(string id, string project) where T : DocumentBase;
        public Task<List<T>> GetDataByIdAsync<T>(string id, string project) where T : DocumentBase;
        public Task<List<T>> GetDataByTypeAsync<T>(DocumentTypeEnum documentType, string project) where T : DocumentBase;
        public Task<List<T>> GetDataBySchemaSignature<T>(string signatureStr, string project) where T : DocumentBase;
        public Task<List<DocumentBase>> GetAllDocumentBaseByProject(string project);
        public void Dispose();
    }
}
