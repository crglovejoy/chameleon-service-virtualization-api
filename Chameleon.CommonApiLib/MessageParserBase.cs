using Newtonsoft.Json.Linq;
using Chameleon.CommonLib;

namespace Chameleon.CommonApiLib
{
    public enum ParserType { JSON, XML }

    public abstract class MessageParserBase
    {
        public abstract MessageProperties ProcessMessageIntoMessageProperties(string message);
        public abstract MessageProperties ProcessMessageIntoClassSpecificMessageProperties<T>(string message);

        protected static MessageProperties GetMessageProperties(JToken jToken, string originalMessage)
        {
            if (jToken == null || jToken is not JObject)
                throw new ArgumentException("Invalid JSON token", nameof(jToken));

            var realValsDict = new Dictionary<string, object>();
            var cleanValsDict = new Dictionary<string, object>();
            FlattenJToken(jToken, string.Empty, realValsDict, cleanValsDict);

            return new MessageProperties(originalMessage, realValsDict, cleanValsDict);
        }


        // Convert our JToken to a flattened dictionary.
        protected static void FlattenJToken(JToken token, string prefix, Dictionary<string, object> resultValsDict, Dictionary<string, object> resultValsStrippedDict)
        {
            /* resultValsStrippedDict will be built with key/value pairs found in the request - with values set to default values and
             * used for calculating the schema signature hash value for the corresponding RoutingDoc creation or lookup in the database.
             * 
             * resultValsDict will be built with key/value pairs found in the request - with values set to the request provided values and
             * used for making comparisons against the RoutingDoc assigned Route filters for determining a response to send back.  Value type
             * conversions are also done here to align the supported JToken and Cosmos database supported data types.
             */

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
                        resultValsDict[prefix] = DetermineValueToAssignString(value);  // Call this method to solve null reference warnings by the IDE
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


        private static string DetermineValueToAssignString(JValue value)
        {
            string? strValue = value.Value<string>();
            if (strValue == null)
                return string.Empty;

            return strValue;
        }
    }
}

