using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Newtonsoft.Json;
using System.Diagnostics;

namespace UnitTest
{
    public class UnitTestRoutingDoc
    {
        private const string _matchingDocId = "matching_doc_id";
        private const string _notMatchingDocId = "no_match_doc_id";
        private const string _projectName = "RoutingDocUnitTest";
        private const string _id = "abcd";


        // Test that when there is a matching route, we get the expected reponse doc Id
        [Test]
        public void TestHikerRoutingDocumentRouteMatch()
        {
            JsonParser jsonParser = new();
            MessageProperties mProps = jsonParser.ProcessMessageIntoClassSpecificMessageProperties<TestHiker>(JsonConvert.SerializeObject(new TestHiker()));
            RoutingDoc routingDoc = GetRoutingDoc(mProps);
            string respDocId = routingDoc.GetResponseDocumentId(mProps.messageDict);

            // Route 3 should match
            Assert.That(respDocId, Is.EqualTo(_matchingDocId));
        }

        
        // Test that when there is no matching route, we get the no schema match response doc Id
        [Test]
        public void TestHikerRoutingDocumentRouteNoMatch()
        {
            JsonParser jsonParser = new();
            MessageProperties mProps = jsonParser.ProcessMessageIntoClassSpecificMessageProperties<TestHiker>(JsonConvert.SerializeObject(new TestHiker()));
            RoutingDoc routingDoc = GetRoutingDoc(mProps, false);
            string respDocId = routingDoc.GetResponseDocumentId(mProps.messageDict);

            // No routes should match
            Assert.That(respDocId, Is.EqualTo(_notMatchingDocId));
        }

        // Validate other RoutingDoc property values that we didn't check above.
        [Test]
        public void TestRoutingDocumentPropertyValues()
        {
            JsonParser jsonParser = new();
            MessageProperties mProps = jsonParser.ProcessMessageIntoClassSpecificMessageProperties<TestHiker>(JsonConvert.SerializeObject(new TestHiker()));
            RoutingDoc routingDoc = GetRoutingDoc(mProps, false);

            Assert.Multiple(() =>
            {
                Assert.That(routingDoc.id.StartsWith(typeof(RoutingDoc).Name));
                Assert.That(routingDoc.originalMessageStr, Is.EqualTo(mProps.originalMessageStr));
                Assert.That(string.IsNullOrWhiteSpace(routingDoc.schemaSignature), Is.False);
                Assert.That(routingDoc.project, Is.EqualTo(_projectName));
                Assert.That(routingDoc.documentType, Is.EqualTo(DocumentTypeEnum.RoutingDoc));
            });
        }


        // Validate RoutingDoc Id property value.
        [Test]
        public void TestRoutingDocumentIdPropertyValue()
        {
            JsonParser jsonParser = new();
            MessageProperties mProps = jsonParser.ProcessMessageIntoClassSpecificMessageProperties<TestHiker>(JsonConvert.SerializeObject(new TestHiker()));
            RoutingDoc routingDoc = GetRoutingDoc(mProps, false, true);
            Assert.That(routingDoc.id, Is.EqualTo(_id));
        }


        private static RoutingDoc GetRoutingDoc(MessageProperties mProps, Boolean achieveMatch = true, Boolean useIdVal = false)
        {
            // Do these checks to elimiante the warnings that we may be using null values later in this method.
            Assert.Multiple(() =>
            {
                Assert.That(mProps.messageDict.ContainsKey("FirstName"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("FirstName"), Is.Not.EqualTo(null));
                Assert.That(mProps.messageDict.ContainsKey("LastName"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("LastName"), Is.Not.EqualTo(null));
                Assert.That(mProps.messageDict.ContainsKey("FavoriteFloat"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("FavoriteFloat"), Is.Not.EqualTo(null));
                Assert.That(mProps.messageDict.ContainsKey("FavoriteColorArr[1]"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("FavoriteColorArr[1]"), Is.Not.EqualTo(null));
                Assert.That(mProps.messageDict.ContainsKey("FavoriteInt"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("FavoriteInt"), Is.Not.EqualTo(null));
            });

            // altering the route 1 value for 'FavoriteFloat' so route1 is not a match.
            Route route1 = new(
                [ new RoutingItem("FirstName", mProps.messageDict.GetValueOrDefault("FirstName")),
                  new RoutingItem("LastName", mProps.messageDict.GetValueOrDefault("LastName")),
                  new RoutingItem("FavoriteFloat", 3.0f),
                  new RoutingItem("FavoriteColorArr[1]", mProps.messageDict.GetValueOrDefault("FavoriteColorArr[1]")),
                  new RoutingItem("FavoriteInt", mProps.messageDict.GetValueOrDefault("FavoriteInt"))
                ],
                "route1_should_not_match"
            );

            // altering the route 2 value for 'FavoriteColorArr[1]' so route2 is not a match.
            Route route2 = new(
                [ new RoutingItem("FirstName", mProps.messageDict.GetValueOrDefault("FirstName")),
                  new RoutingItem("LastName", mProps.messageDict.GetValueOrDefault("LastName")),
                  new RoutingItem("FavoriteFloat", mProps.messageDict.GetValueOrDefault("FavoriteFloat")),
                  new RoutingItem("FavoriteColorArr[1]", "X" + mProps.messageDict.GetValueOrDefault("FavoriteColorArr[1]")),
                  new RoutingItem("FavoriteInt", mProps.messageDict.GetValueOrDefault("FavoriteInt"))
                ],
                "route2_should_not_match"
            );
            

            /* route 3 is always the matching route UNLESS achieveMatch parameter is passed to us with false, then we're
             * going to make certain route3 is not a match as well by altering the value for 'FavoriteInt'.
             */
            object favIntObj = mProps.messageDict.GetValueOrDefault("FavoriteInt");
            if (!achieveMatch)
            {
                favIntObj = 666;
            }
            Route route3 = new(
                [ new RoutingItem("FirstName", mProps.messageDict.GetValueOrDefault("FirstName")),
                  new RoutingItem("LastName", mProps.messageDict.GetValueOrDefault("LastName")),
                  new RoutingItem("FavoriteFloat", mProps.messageDict.GetValueOrDefault("FavoriteFloat")),
                  new RoutingItem("FavoriteColorArr[1]", mProps.messageDict.GetValueOrDefault("FavoriteColorArr[1]")),
                  new RoutingItem("FavoriteInt", favIntObj)
                ],
                _matchingDocId
            );

            // return the new RoutingDoc

            if (useIdVal)
            {
                // Use a specific document id.
                return new RoutingDoc(_projectName, mProps.schemaSignatureHash, [route1, route2, route3], _notMatchingDocId, mProps.originalMessageStr, _id);
            }

            // Let the RoutingDoc constructor create a document id.
            return new RoutingDoc(_projectName, mProps.schemaSignatureHash, [route1, route2, route3], _notMatchingDocId, mProps.originalMessageStr);
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

            RoutingItem[] routingItems = [
                new RoutingItem("FirstName", "Bat"),
                new RoutingItem("LastName", "Man"),
                new RoutingItem("BirthDate", DateTime.Parse("01/01/1990")),
                new RoutingItem("FavoriteHikeArr[0].Name", "Dark Cave"),
                new RoutingItem("FavoriteHikeArr[0].State", "New York"),
                new RoutingItem("FavoriteLong", 987654321L)
                ];

            Route route = new(routingItems, matchRespDocId);
            RoutingDoc routingDoc = new("RoutingSpeedTest", "1234Signature", [route], noMatchRespDocId, jsonStr);
            string respId = string.Empty;

            for (int i = 0; i < numIters; i++)
            {
                stopW.Start();
                respId = routingDoc.GetResponseDocumentId(msgProps.messageDict);
                stopW.Stop();
            }

            Assert.That(respId, Is.EqualTo(matchRespDocId));
            Assert.That(stopW.ElapsedMilliseconds, Is.EqualTo(0L));
            decimal elapsedUs = stopW.Elapsed.Microseconds;
            decimal avgRoutingTimeUs = (decimal)elapsedUs / (decimal)numIters;

            TestContext.Out.WriteLine($"Total time taken to route {numIters} messages: {elapsedUs} us");
            TestContext.Out.WriteLine($"Average time taken to route a single message: {avgRoutingTimeUs} us");

            Assert.That(avgRoutingTimeUs, Is.LessThan(40.0d));

            /* RESULTS CAN VARY DEPENDING ON MACHINE AND OTHER LOAD
             * Last test output on a windows PC...
             * Total time taken to route 25 messages: 367 us
             * Average time taken to route a single message: 14.68 us
             */
        }
    }
}
