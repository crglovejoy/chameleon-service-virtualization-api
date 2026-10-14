namespace Chameleon.CommonLib
{
    // An enumeration of document types.
    public enum DocumentTypeEnum
    {
        RoutingDoc = 0,
        RespDoc = 1,
        NoSchemaMatchRespDoc = 2,
        ProjectDoc = 3
    }


    // A common class that multiple message doc types will use for assigning response headers
    public class HeaderItem(string propertyName, string propertyValue)
    {
        public string propertyName = propertyName;
        public string propertyValue = propertyValue;
    }


    // A base class for all document types.
    public class DocumentBase
    {
        public readonly string id;
        public readonly string project;
        public readonly DocumentTypeEnum documentType;
        public const string CommonProjectName = "common";

        public DocumentBase(string project, DocumentTypeEnum documentType, string id = "")
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                this.id = GetIdString(documentType);
            }
            else
            {
                this.id = id;
            }

            this.project = project;
            this.documentType = documentType;
        }

        private static string GetIdString(DocumentTypeEnum documentType)
        {
            return $"{documentType}-{Guid.NewGuid()}";
        }
    }
}
