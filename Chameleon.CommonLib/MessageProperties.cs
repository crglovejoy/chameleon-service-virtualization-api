using System.Text.Json;
using System.IO.Hashing;
using System.Text;

namespace Chameleon.CommonLib
{
    /**************************/
    /**  MESSAGE PROPERTIES  **/
    /**************************/

    public class MessageProperties
    {
        public readonly string originalMessageStr;
        public readonly Dictionary<string, object> messageDict;
        public readonly string schemaSignatureHash;


        /*****************/
        /*  Constructor  */
        /*****************/

        public MessageProperties(string originalMessageStr, Dictionary<string, object> valuesMessageDict, Dictionary<string, object> signatureMessageDict)
        {
            this.originalMessageStr = StringHelpers.CleanToSingleLineString(originalMessageStr);
            this.messageDict = valuesMessageDict;

            byte[] sourceBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(signatureMessageDict));
            this.schemaSignatureHash = XxHash64.HashToUInt64(sourceBytes).ToString();
        }
    }
}
