

using Chameleon.CommonLib;

namespace UnitTest.Api
{
    internal static class ApiTestCommon
    {
        internal const string NoSchemaMatchRespStr = @"{""Message"":""No schema match response""}";
        internal static readonly HeaderItem[] JsonHeaderItems = [ new HeaderItem("Content-Type", "application/json") ];
    }
}
