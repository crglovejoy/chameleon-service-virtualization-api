
using System.Text;

namespace Chameleon.CommonLib
{
    /****************/
    /**  RESP DOC  **/
    /****************/

    public class RespDoc : DocumentBase
    {
        public HeaderItem[] headerItems { get; set; }
        public string responseContent { get; set; }


        /*****************/
        /*  Constructor  */
        /*****************/

        public RespDoc(string project, HeaderItem[] headerItems, string responseContent, string id = "",
            DocumentTypeEnum responseDocType = DocumentTypeEnum.RespDoc) : base(project, responseDocType, id)
        {
            this.headerItems = headerItems;
            this.responseContent = responseContent;
        }


        /*****************************************************/
        /*  Resolve variable string in the response message  */
        /*****************************************************/

        public static string GetVariablesResolvedResponse(string respStr, Dictionary<string, object> reqPropsDict, string[] propVarNames)
        {
            if (propVarNames.Length == 0)
            {
                return respStr;
            }

            string newRespStr = respStr;

            var replacements = propVarNames
                .Where(p => reqPropsDict.ContainsKey(p))
                .ToDictionary(p => RespDoc.WrapStringWithVariableDelimiter(p), p => reqPropsDict[p]?.ToString() ?? "NOT FOUND");

            StringBuilder sb = new();
            foreach (var kv in replacements)
                newRespStr = newRespStr.Replace(kv.Key, kv.Value);

            return newRespStr;
        }


        /*****************************************************************/
        /*  Wrap the variable name strings with the delimiter character  */
        /*****************************************************************/

        public const Char variableDelimiter = '`';
        public static string WrapStringWithVariableDelimiter(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Provided string input cannot be null or empty", nameof(str));
            }

            var newStr = str.Trim();
            if (!newStr.StartsWith(variableDelimiter))
            {
                newStr = $"{variableDelimiter}{newStr}";
            }
            if (!newStr.EndsWith(variableDelimiter))
            {
                newStr = $"{newStr}{variableDelimiter}";
            }

            return newStr;
        }
    }
}
