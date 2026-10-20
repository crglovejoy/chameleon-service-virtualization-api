using Newtonsoft.Json.Linq;
using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Newtonsoft.Json;

namespace Chameleon.Parsers
{
    public class JsonParser: MessageParserBase
    {
        public JsonParser() { }

        public override MessageProperties ProcessMessageIntoMessageProperties(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null, empty or whitespace.", nameof(message));
            }

            /* In unit testing, could not achieve jsonObj = null.  Including the null check to prevent the IDE 'could be null' warning
             * with passing jToken as a parameter to GetMessageProperties. */

            var jToken = JToken.Parse(message) ?? throw new ArgumentException($"Could not convert {nameof(message)} to a jToken. Got a null result");

            return GetMessageProperties(jToken, message);
        }


        // Currently used for test data creation, but not live requests.
        public override MessageProperties ProcessMessageIntoClassSpecificMessageProperties<T>(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));
            }

            /* In unit testing, could not achieve jsonObj = null.  Including the null check to prevent the IDE 'could be null' warning
             * with passing jToken as a parameter to GetMessageProperties. */

             var jsonObj = JsonConvert.DeserializeObject<T>(message) ?? throw new ArgumentException($"Could not convert {nameof(message)} to the specified class {typeof(T).Name}. Got a null result");

            var jToken = JToken.FromObject(jsonObj);
            return GetMessageProperties(jToken, message);
        }
    }
}
