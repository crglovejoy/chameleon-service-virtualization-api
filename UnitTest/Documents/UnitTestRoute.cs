using Chameleon.CommonLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Documents
{
    public class UnitTestRoute
    {
        private const string matchRespId = "matching_resp_doc_id";
        private Route _route;


        /****************/
        /*  Test SetUp  */
        /****************/

        [SetUp]
        public void GetRoute()
        {
            MatchFilter[] matchFilters = [ new MatchFilter("TastesGood", "PeanutButter"), new MatchFilter("TastesBad", "Anchovies") ];
            string[] propertyVarNames = ["TastesOkay", "DoNotEat"];
            _route = new Route(matchFilters, propertyVarNames, matchRespId);
        }


        /******************/
        /*  Test Methods  */
        /******************/

        [Test]
        public void TestRouteConstructor()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_route.matchFilters.Length, Is.EqualTo(2));
                Assert.That(_route.matchFilters[0].propertyName, Is.EqualTo("TastesGood"));
                Assert.That(_route.matchFilters[0].propertyValue, Is.EqualTo("PeanutButter"));
                Assert.That(_route.matchFilters[1].propertyName, Is.EqualTo("TastesBad"));
                Assert.That(_route.matchFilters[1].propertyValue, Is.EqualTo("Anchovies"));
                Assert.That(_route.propertyVariableNames[0], Is.EqualTo("TastesOkay"));
                Assert.That(_route.propertyVariableNames[1], Is.EqualTo("DoNotEat"));
                Assert.That(_route.responseDocumentId, Is.EqualTo(matchRespId));
            });
        }


        [Test]
        public void TestAddMatchFilter()
        {
            _route.AddMatchFilter(new MatchFilter("TastesOkay", "Asparagus"));

            Assert.Multiple(() =>
            {
                Assert.That(_route.matchFilters.Length, Is.EqualTo(3));
                Assert.That(_route.matchFilters[0].propertyName, Is.EqualTo("TastesGood"));
                Assert.That(_route.matchFilters[0].propertyValue, Is.EqualTo("PeanutButter"));
                Assert.That(_route.matchFilters[1].propertyName, Is.EqualTo("TastesBad"));
                Assert.That(_route.matchFilters[1].propertyValue, Is.EqualTo("Anchovies"));
                Assert.That(_route.matchFilters[2].propertyName, Is.EqualTo("TastesOkay"));
                Assert.That(_route.matchFilters[2].propertyValue, Is.EqualTo("Asparagus"));
            });
        }


        [Test]
        public void TestAddPropertyName()
        {
            _route.AddPropertyVariableName("TastesTerrible");

            Assert.Multiple(() =>
            {
                Assert.That(_route.propertyVariableNames[0], Is.EqualTo("TastesOkay"));
                Assert.That(_route.propertyVariableNames[1], Is.EqualTo("DoNotEat"));
                Assert.That(_route.propertyVariableNames[2], Is.EqualTo("TastesTerrible"));
            });
        }


        [Test]
        public void TestIsMatchingRouteSuccess()
        {
            var msgProps = new Dictionary<string, object>();
            msgProps.Add("TastesGood", "PeanutButter");
            msgProps.Add("TastesBad", "Anchovies");
            msgProps.Add("TastesOkay", "Asparagus");
            msgProps.Add("DoNotEat", "RatPoison");

            Assert.That(_route.IsMatchingRoute(msgProps), Is.True);
        }

        [Test]
        public void TestIsMatchingRouteFail()
        {
            var msgProps = new Dictionary<string, object>();
            msgProps.Add("TastesGood", "PeanutButter");
            msgProps.Add("TastesBad", "Cardboard");  // this should fail to match.
            msgProps.Add("TastesOkay", "Asparagus");
            msgProps.Add("DoNotEat", "RatPoison");

            Assert.That(_route.IsMatchingRoute(msgProps), Is.False);
        }
    }
}
