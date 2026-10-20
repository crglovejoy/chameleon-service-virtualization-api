using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Chameleon.CommonLib;
using Chameleon.CommonApiLib;
using System.Xml;

namespace Chameleon.Parsers
{
    public class XmlParser : MessageParserBase
    {
        public override MessageProperties ProcessMessageIntoMessageProperties(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null, empty or whitespace.", nameof(message));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(message);
            var jToken = JToken.Parse(JsonConvert.SerializeXmlNode(doc));
            return GetMessageProperties(jToken, message);
        }

        public override MessageProperties ProcessMessageIntoClassSpecificMessageProperties<T>(string message)
        {
            throw new NotImplementedException("ProcessMessageIntoClassSpecificMessageProperties<T> is not implemented by the XmlParser class");
        }
    }
}
