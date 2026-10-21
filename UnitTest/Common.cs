
using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Newtonsoft.Json;
using Chameleon.Parsers;

namespace UnitTest
{
    internal static class CommonTestVals
    {
        internal const Boolean Debug = false;
    }

    public static class OutputHelpers
    {
        public static void WriteOutMessageProperties(MessageProperties mProps)
        {
            TestContext.Out.WriteLine("***  MESSAGE PROPERTIES  ***");
            TestContext.Out.WriteLine($"Flattened Dictionary:{Environment.NewLine}=====================\"");
            foreach (var kvp in mProps.messageDict)
                TestContext.Out.WriteLine($"{kvp.Key} = {kvp.Value}");
            TestContext.Out.WriteLine($"{Environment.NewLine}Other Property Values:{Environment.NewLine}======================");
            TestContext.Out.WriteLine($"digital signature hash = {mProps.schemaSignatureHash}");
            TestContext.Out.WriteLine($"original message = {mProps.originalMessageStr}");
        }
    }


    /***********************************************/
    /*  GetRoutingDoc: Routing Doc Staging Method  */
    /***********************************************/

    public class RoutingDocHelper()
    {
        public const string RouteMatchingRespDocId = "matching_doc_id";
        public const string RouteNotMatchingRespDocId = "no_match_doc_id";
        public readonly static string[] ReplacePropertyNames = ["FirstName", "LastName"];
        private MessageProperties _msgProps = null!;
        public MessageProperties MsgProps => _msgProps;
        public const string Id = "abcd";

        public RoutingDoc GetRoutingDoc(Boolean achieveMatch, Boolean useWildcard, Boolean useIdVal, string projectName)
        {
            if (!achieveMatch && useWildcard)
            {
                throw new ArgumentException("Cannot use achieveMatch=false and useWildCard=true");
            }

            JsonParser jsonParser = new();
            TestHiker hiker = new();
            string jsonStr = JsonConvert.SerializeObject(hiker);
            _msgProps = jsonParser.ProcessMessageIntoMessageProperties(jsonStr);

            if (_msgProps == null)
            {
                throw new Exception($"MsgProps was null!");
            }

            // Do these checks to elimiante the warnings that we may be using null values later in this method.
            Assert.Multiple(() =>
            {
                Assert.That(_msgProps.messageDict.ContainsKey("FirstName"), Is.True);
                Assert.That(_msgProps.messageDict.GetValueOrDefault("FirstName"), Is.Not.Null);
                Assert.That(_msgProps.messageDict.ContainsKey("LastName"), Is.True);
                Assert.That(_msgProps.messageDict.GetValueOrDefault("LastName"), Is.Not.Null);
                Assert.That(_msgProps.messageDict.ContainsKey("FavoriteFloat"), Is.True);
                Assert.That(_msgProps.messageDict.GetValueOrDefault("FavoriteFloat"), Is.Not.Null);
                Assert.That(_msgProps.messageDict.ContainsKey("FavoriteColorArr[1]"), Is.True);
                Assert.That(_msgProps.messageDict.GetValueOrDefault("FavoriteColorArr[1]"), Is.Not.Null);
                Assert.That(_msgProps.messageDict.ContainsKey("FavoriteInt"), Is.True);
                Assert.That(_msgProps.messageDict.GetValueOrDefault("FavoriteInt"), Is.Not.Null);
            });

            // altering the route 1 value for 'FavoriteFloat' so route1 is not a match.
            Route route1 = new(
                [
                    new MatchFilter("FirstName", _msgProps.messageDict.GetValueOrDefault("FirstName")),
                    new MatchFilter("LastName", _msgProps.messageDict.GetValueOrDefault("LastName")),
                    new MatchFilter("FavoriteFloat", 3.0f),
                    new MatchFilter("FavoriteColorArr[1]", _msgProps.messageDict.GetValueOrDefault("FavoriteColorArr[1]")),
                    new MatchFilter("FavoriteInt", _msgProps.messageDict.GetValueOrDefault("FavoriteInt"))
                ],
                [],
                "route1_should_not_match"
            );

            // altering the route 2 value for 'FavoriteColorArr[1]' so route2 is not a match.
            Route route2 = new(
                [
                    new MatchFilter("FirstName", _msgProps.messageDict.GetValueOrDefault("FirstName")),
                    new MatchFilter("LastName", _msgProps.messageDict.GetValueOrDefault("LastName")),
                    new MatchFilter("FavoriteFloat", _msgProps.messageDict.GetValueOrDefault("FavoriteFloat")),
                    new MatchFilter("FavoriteColorArr[1]", "X" + _msgProps.messageDict.GetValueOrDefault("FavoriteColorArr[1]")),
                    new MatchFilter("FavoriteInt", _msgProps.messageDict.GetValueOrDefault("FavoriteInt"))
                ],
                [],
                "route2_should_not_match"
            );

            /* route 3 is always the matching route UNLESS achieveMatch parameter is passed to us with false, then we're
                * going to make certain route3 is not a match by altering the value for 'FavoriteInt'. */
            object favIntObj = _msgProps.messageDict.GetValueOrDefault("FavoriteInt");
            if (!achieveMatch)
            {
                favIntObj = 666;
            }

            Route route3 = null!;
            if (!useWildcard)
            {
                route3 = new(
                    [
                        new MatchFilter("FirstName", _msgProps.messageDict.GetValueOrDefault("FirstName")),
                        new MatchFilter("LastName", _msgProps.messageDict.GetValueOrDefault("LastName")),
                        new MatchFilter("FavoriteFloat", _msgProps.messageDict.GetValueOrDefault("FavoriteFloat")),
                        new MatchFilter("FavoriteColorArr[1]", _msgProps.messageDict.GetValueOrDefault("FavoriteColorArr[1]")),
                        new MatchFilter("FavoriteInt", favIntObj)
                    ],
                    ReplacePropertyNames,
                    RouteMatchingRespDocId
                );
            }
            else // Just need a single wildcard entry.
            {
                route3 = new([new MatchFilter("*", "*")], ReplacePropertyNames, RouteMatchingRespDocId);
            }

            // return the new RoutingDoc

            if (useIdVal)
            {
                // Use a specific document id.
                return new RoutingDoc(projectName, _msgProps.schemaSignatureHash, [route1, route2, route3], RouteNotMatchingRespDocId, _msgProps.originalMessageStr, Id);
            }

            // Let the RoutingDoc constructor create a document id.
            return new RoutingDoc(projectName, _msgProps.schemaSignatureHash, [route1, route2, route3], RouteNotMatchingRespDocId, _msgProps.originalMessageStr);
        }
    }


    public class RespDocHelper
    {
        public static readonly HeaderItem[] JsonHeaderItems = [new HeaderItem("Content-Type", "application/json")];
        public const string RespDocMatchRespString = @"{""Message"":""Matching Response""}";
        public const string RespDocNoMatchRespString = @"{""Message"":""Not Matching Response""}";
        public static readonly string RespDocMatchRespStrWithVars = @"{""Message"":""`FirstName` `LastName` is an awesome name!""}";
        public static readonly string ExpectedRespDocMatchRespStrWithVars = @"{""Message"":""Bat Man is an awesome name!""}";

        public RespDoc GetRespDoc(string project, Boolean matchingResp, Boolean respWithVars)
        {
            if (matchingResp && respWithVars)
            {
                return new RespDoc(project, JsonHeaderItems, RespDocMatchRespStrWithVars);
            }

            return new RespDoc(project, JsonHeaderItems, matchingResp ? RespDocMatchRespString : RespDocNoMatchRespString);
        }
    }
}
