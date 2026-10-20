using Newtonsoft.Json.Linq;
using Chameleon.CommonLib;

namespace Chameleon.CommonApiLib
{
    public enum ParserType { JSON, XML }

    /*************************/
    /**  MESSAGEPARSERBASE  **/
    /*************************/
    public abstract class MessageParserBase
    {
        public abstract MessageProperties ProcessMessageIntoMessageProperties(string message);
        public abstract MessageProperties ProcessMessageIntoClassSpecificMessageProperties<T>(string message);


        /**************************/
        /*  GetMessageProperties  */
        /**************************/

        protected static MessageProperties GetMessageProperties(JToken jToken, string originalMessage)
        {
            if (jToken == null || jToken is not JObject)
                throw new ArgumentException("Invalid JSON token", nameof(jToken));

            var realValsDict = new Dictionary<string, object>();
            var cleanValsDict = new Dictionary<string, object>();
            FlattenJToken(jToken, string.Empty, realValsDict, cleanValsDict);

            return new MessageProperties(originalMessage, realValsDict, cleanValsDict);
        }


        /**************************************************/
        /*  Convert a JToken to 2 flattened dictionaries  */
        /**************************************************/

        protected static void FlattenJToken(JToken token, string prefix, Dictionary<string, object> resultValsDict, 
            Dictionary<string, object> resultValsStrippedDict)
        {
            /* FlattenJToken populates two dictionaries concurrently: */
             
            /* 1) resultValsStrippedDict will be built with property name/value pairs found in the request - with null properties avoided and
             * remaining property values set to default values.  This dictionary is used for calculating the schema signature hash value - this 
             * will be stored in a RoutingDoc and requests having the same schema will always land on the same RoutingDoc.  (Same schema = after 
             * properties having null values are removed and remaining property values set to default values, the signature hash value is the same).
             *  
             * 2) resultValsDict will be built with property name/value pairs found in the request - with null properites avoided and remaining
             * property values set to the request provided values.  This dictionary is used for comparing request message property values against
             * matching rules for determining which response document content to use in generating a response to a request
             *
             * Value type conversions are also done in this method to bridge the supported JToken data types and Cosmos database supported data
             * types so when we retrieve our matching filters/rules from Cosmos later to do comparisons against the resultValsDict values, we're 
             * working with the same data type (Example: Cosmos stores all numbers as 64-bit - int needs to be comvered to long.) */

            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    string newPrefix = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                    FlattenJToken(property.Value, newPrefix, resultValsDict, resultValsStrippedDict);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    string newPrefix = $"{prefix}[{i}]";
                    FlattenJToken(array[i], newPrefix, resultValsDict, resultValsStrippedDict);
                }
            }
            else if (token is JValue value)
            {
                switch(value.Type)
                {
                    case JTokenType.Null:
                        // Leave this key out of the result dictionary if the value is null
                        break;
                    case JTokenType.Boolean:
                        resultValsDict[prefix] = value.Value<Boolean>();
                        resultValsStrippedDict[prefix] = false;
                        break;
                    case JTokenType.Integer:   // Cosmos stores integers as 64-bits, so convert here...
                        resultValsDict[prefix] = value.Value<long>();
                        resultValsStrippedDict[prefix] = 0L;
                        break;
                    case JTokenType.Float:   // Cosmos stores floats as 64-bits, so convert here...
                        resultValsDict[prefix] = value.Value<double>();
                        resultValsStrippedDict[prefix] = 0D;
                        break;
                    case JTokenType.String:
                        resultValsDict[prefix] = DetermineStringValueToAssign(value);  // Call this method to solve null reference warnings by the IDE
                        resultValsStrippedDict[prefix] = string.Empty;
                        break;
                    case JTokenType.Date:   // Cosmos doesn't support the date datatype, but testing shows a DateTime will be converted to a string by default...
                        resultValsDict[prefix] = (DateTime)value;
                        resultValsStrippedDict[prefix] = DateTime.MinValue;
                        break;
                    case JTokenType.Guid:   // Guid datatype is not supported by Cosmos, so convert to string...
                        resultValsDict[prefix] = value.ToString();
                        resultValsStrippedDict[prefix] = string.Empty;
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported JValue type: {value.Type}");
                }
            }
        }


        private static string DetermineStringValueToAssign(JValue value)
        {
            string? strValue = value.Value<string>();
            if (strValue == null)
                return string.Empty;

            return strValue;
        }
    }
}

