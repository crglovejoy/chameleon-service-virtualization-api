
namespace Chameleon.CommonLib
{
    // A response document consists of a set of header items and a response content string.

    public class RespDoc : DocumentBase
    {
        public HeaderItem[] headerItems { get; set; }
        public string responseContent { get; set; }
        public RespDoc(string project, HeaderItem[] headerItems, string responseContent, string id = "",
            DocumentTypeEnum responseDocType = DocumentTypeEnum.RespDoc) : base(project, responseDocType, id)
        {
            this.headerItems = headerItems;
            this.responseContent = responseContent;
        }
    }
}
