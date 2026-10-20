using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Newtonsoft.Json;
using System.Diagnostics;
using Chameleon.Parsers;

namespace UnitTest.Documents
{
    public class UnitTestRoutingDoc
    {
        //private static readonly Boolean _debug = (false || CommonTestVals.Debug);
        private const string _projectName = "RoutingDocUnitTest";
        private RoutingDocHelper _rteDocHelper;


        [SetUp]
        public void SetUp()
        {
            _rteDocHelper = new RoutingDocHelper();
        }

        /******************/
        /*  Test Methods  */
        /******************/

        // Test that when there is a matching route, we get the expected reponse doc Id
        [Test]
        public void TestHikerRoutingDocumentRouteMatch()
        {
            RoutingDoc routingDoc = _rteDocHelper.GetRoutingDoc(achieveMatch:true, useWildcard:false, useIdVal:false, _projectName);
            Route? route = routingDoc.GetMatchingRoute(_rteDocHelper.MsgProps.messageDict);

            // Route 3 should match
            Assert.Multiple(() =>
            {
                Assert.That(route, Is.Not.Null);
                Assert.That(route?.matchFilters?.Length ?? -1, Is.EqualTo(5));
                Assert.That(route?.matchFilters[0]?.propertyName ?? string.Empty, Is.EqualTo("FirstName"));
                Assert.That(route?.matchFilters[0]?.propertyValue ?? string.Empty, Is.EqualTo("Bat"));
            });

            Assert.That(route.responseDocumentId, Is.EqualTo(RoutingDocHelper.RouteMatchingRespDocId));
        }


        // Test that when there is a matching route when using a wildcard, we get the expected reponse doc Id
        [Test]
        public void TestHikerRoutingDocumentRouteWildcardMatch()
        {
            RoutingDoc routingDoc = _rteDocHelper.GetRoutingDoc(achieveMatch:true, useWildcard:true, useIdVal:false, _projectName);
            Route? route = routingDoc.GetMatchingRoute(_rteDocHelper.MsgProps.messageDict);

            // We should have gotten a Route 3 wildcard route match...
            Assert.Multiple(() =>
            {
                Assert.That(route, Is.Not.Null);
                Assert.That(route?.matchFilters?.Length ?? -1, Is.EqualTo(1));
                Assert.That(route?.matchFilters[0]?.propertyName ?? string.Empty, Is.EqualTo("*"));
                Assert.That(route?.matchFilters[0]?.propertyValue ?? string.Empty, Is.EqualTo("*"));
            });

            Assert.That(route.responseDocumentId, Is.EqualTo(RoutingDocHelper.RouteMatchingRespDocId));
        }


        // Test that when there is no matching route, we get the no schema match response doc Id
        [Test]
        public void TestHikerRoutingDocumentRouteNoMatch()
        {
            RoutingDoc routingDoc = _rteDocHelper.GetRoutingDoc(achieveMatch:false, useWildcard:false, useIdVal:false, _projectName);
            Route? route = routingDoc.GetMatchingRoute(_rteDocHelper.MsgProps.messageDict);

            // We should not have found a routing match.
            Assert.That(route, Is.Null);
            Assert.That(routingDoc.noRouteMatchResponseDocumentId, Is.EqualTo(RoutingDocHelper.RouteNotMatchingRespDocId));
        }

        // Validate other RoutingDoc property values that we didn't check above.
        [Test]
        public void TestRoutingDocumentOtherPropertyValues()
        {
            RoutingDoc routingDoc = _rteDocHelper.GetRoutingDoc(achieveMatch: false, useWildcard: false, useIdVal: false, _projectName);

            Assert.Multiple(() =>
            {
                Assert.That(routingDoc.id.StartsWith(typeof(RoutingDoc).Name));
                Assert.That(routingDoc.originalMessageStr, Is.EqualTo(_rteDocHelper.MsgProps.originalMessageStr));
                Assert.That(string.IsNullOrWhiteSpace(routingDoc.schemaSignature), Is.False);
                Assert.That(routingDoc.project, Is.EqualTo(_projectName));
                Assert.That(routingDoc.documentType, Is.EqualTo(DocumentTypeEnum.RoutingDoc));
            });
        }


        [Test]
        public void TestAddRoute()
        {
            RoutingDoc routingDoc = _rteDocHelper.GetRoutingDoc(achieveMatch: false, useWildcard: false, useIdVal: false, _projectName);
            routingDoc.AddRoute(new Route([new MatchFilter("goofy", "dog")], ["property_goofy"], RoutingDocHelper.RouteMatchingRespDocId));

            Assert.Multiple(() =>
            {
                Assert.That(routingDoc.routes.Length, Is.EqualTo(4));
                Assert.That(routingDoc.routes[0].matchFilters[0].propertyName, Is.EqualTo("FirstName"));
                Assert.That(routingDoc.routes[0].matchFilters[0].propertyValue, Is.EqualTo("Bat"));
                Assert.That(routingDoc.routes[3].matchFilters[0].propertyName, Is.EqualTo("goofy"));
                Assert.That(routingDoc.routes[3].matchFilters[0].propertyValue, Is.EqualTo("dog"));
                Assert.That(routingDoc.routes[3].propertyVariableNames.Length, Is.EqualTo(1));
                Assert.That(routingDoc.routes[3].propertyVariableNames[0], Is.EqualTo("property_goofy"));
                Assert.That(routingDoc.routes[3].responseDocumentId, Is.EqualTo(RoutingDocHelper.RouteMatchingRespDocId));
            });
        }


        // Validate RoutingDoc Id property value.
        [Test]
        public void TestRoutingDocumentIdPropertyValue()
        {
            RoutingDoc routingDoc = _rteDocHelper.GetRoutingDoc(achieveMatch:false, useWildcard:false, useIdVal:true, _projectName);
            Assert.That(routingDoc.id, Is.EqualTo(RoutingDocHelper.Id));
        }


        [Test]
        public void RoutingSpeedTest()
        {
            int numIters = 25;
            string matchRespDocId = "MatchRespId";
            string noMatchRespDocId = "NoMatchRespId";
            Stopwatch stopW = new Stopwatch();
            JsonParser jsonParser = new();
            TestHiker hiker = new();
            string jsonStr = JsonConvert.SerializeObject(hiker);
            MessageProperties msgProps = jsonParser.ProcessMessageIntoMessageProperties(jsonStr);

            MatchFilter[] matchFilters = [
                new MatchFilter("FirstName", "Bat"),
                new MatchFilter("LastName", "Man"),
                new MatchFilter("BirthDate", DateTime.Parse("01/01/1990")),
                new MatchFilter("FavoriteHikeArr[0].Name", "Dark Cave"),
                new MatchFilter("FavoriteHikeArr[0].State", "New York"),
                new MatchFilter("FavoriteLong", 987654321L)
                ];

            Route newRoute = new(matchFilters, RoutingDocHelper.ReplacePropertyNames, matchRespDocId);
            RoutingDoc routingDoc = new("RoutingSpeedTest", "1234Signature", [newRoute], noMatchRespDocId, jsonStr);
            string respId = string.Empty;

            for (int i = 0; i < numIters; i++)
            {
                stopW.Start();
                Route? route = routingDoc.GetMatchingRoute(msgProps.messageDict);
                if (route == null)
                {
                    throw new ArgumentException("Unexpected that the route would be null");
                }
                respId = route.responseDocumentId;
                stopW.Stop();
            }

            Assert.Multiple(() =>
            {
                Assert.That(respId, Is.EqualTo(matchRespDocId));
                Assert.That(stopW.ElapsedMilliseconds, Is.EqualTo(0L));
            });

            decimal elapsedUs = stopW.Elapsed.Microseconds;
            decimal avgRoutingTimeUs = (decimal)elapsedUs / (decimal)numIters;

            TestContext.Out.WriteLine($"Total time taken to route {numIters} messages: {elapsedUs} us");
            TestContext.Out.WriteLine($"Average time taken to route a single message: {avgRoutingTimeUs} us");

            Assert.That(avgRoutingTimeUs, Is.LessThan(40.0d));

            /* RESULTS CAN VARY DEPENDING ON MACHINE AND OTHER LOAD
             * Last test output on a windows PC...
             * Total time taken to route 25 messages: 367 us
             * Average time taken to route a single message: 14.68 us */
        }
    }
}
