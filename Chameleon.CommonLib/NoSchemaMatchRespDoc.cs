
namespace Chameleon.CommonLib
{
    /*************************************************************************************************
     * NoSchemaMatchRespDoc class represents a response document that does not match any schema.
     * It inherits from the RespDoc base class and is used to handle cases where the response content
     * does not conform to any predefined schema.
     *************************************************************************************************/

    public class NoSchemaMatchRespDoc : RespDoc
    {
        public NoSchemaMatchRespDoc(string project, HeaderItem[] headerItems, string responseContent, string id = "") 
            : base(project, headerItems, responseContent, id, DocumentTypeEnum.NoSchemaMatchRespDoc)
        { }
    }
}
