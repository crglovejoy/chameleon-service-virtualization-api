using Chameleon.Api;
using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Chameleon.Parsers;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using UnitTest;


namespace UnitTest.Api
{
    public class UnitTestProcessRequestActionLogic
    {
        private static readonly Boolean _debug = (false || CommonTestVals.Debug);
        private const string _project = "TestProject";
        private MemoryStream _hikerStream;


        [SetUp]
        public void Setup()
        {
            TestHiker hiker = new();
            var hikerStr = JsonConvert.SerializeObject(hiker);
            byte[] byteArray = Encoding.UTF8.GetBytes(hikerStr);
            _hikerStream = new MemoryStream(byteArray);
        }

        private void Dispose()
        {
            _hikerStream.Dispose();
        }

        [Test]
        // Test that when we get a no schema match and fail to retrieve a NoSchemaMatchRespDoc, we get the expected result.
        public async Task TestNoSchemaMatchRespDocMissing()
        {
            var scenario = 1;
            var dataAccess = new DataAccessMock(scenario);
            var procActLogic = new ProcessRequestActionLogic(dataAccess, new JsonParser(), new XmlParser());
            RequestResult reqResult = await procActLogic.ProcessRequestMessageAsync(ParserType.JSON, _hikerStream, _project);
            Assert.Multiple(() =>
            {
                Assert.That(reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(reqResult.ResponseStr, Is.EqualTo($"Found no schema match and could not find a NoSchemaMatchResponse to provide for project {_project}"));
                Assert.That(reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(reqResult.HeaderItems[0].propertyName, Is.EqualTo("Content-Type"));
                Assert.That(reqResult.HeaderItems[0].propertyValue, Is.EqualTo("text/plain"));
            });

            Dispose();
        }

        [Test]
        // Test that when we do not get a schema match and acquire a NoSchemaMatchRespDoc, we get the expected result.
        public async Task TestNoSchemaMatchRespDoc()
        {
            var scenario = 2;
            var dataAccess = new DataAccessMock(scenario);
            var procActLogic = new ProcessRequestActionLogic(dataAccess, new JsonParser(), new XmlParser());
            RequestResult reqResult = await procActLogic.ProcessRequestMessageAsync(ParserType.JSON, _hikerStream, _project);
            Assert.Multiple(() =>
            {
                Assert.That(reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(reqResult.ResponseStr, Is.EqualTo(ApiTestCommon.NoSchemaMatchRespStr));
                Assert.That(reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(reqResult.HeaderItems[0].propertyName, Is.EqualTo(ApiTestCommon.JsonHeaderItems[0].propertyName));
                Assert.That(reqResult.HeaderItems[0].propertyValue, Is.EqualTo(ApiTestCommon.JsonHeaderItems[0].propertyValue));
            });

            Dispose();
        }

        [Test]
        // Test that when we do get a schema match and a resp doc we get the expected result.
        public async Task TestSchemaMatchRespDoc()
        {
            var scenario = 3;
            var dataAccess = new DataAccessMock(scenario);
            var procActLogic = new ProcessRequestActionLogic(dataAccess, new JsonParser(), new XmlParser());
            RequestResult reqResult = await procActLogic.ProcessRequestMessageAsync(ParserType.JSON, _hikerStream, _project);
            Assert.Multiple(() =>
            {
                Assert.That(reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(reqResult.ResponseStr, Is.EqualTo(RespDocHelper.RespDocMatchRespString));
                Assert.That(reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(reqResult.HeaderItems[0].propertyName, Is.EqualTo(RespDocHelper.JsonHeaderItems[0].propertyName));
                Assert.That(reqResult.HeaderItems[0].propertyValue, Is.EqualTo(RespDocHelper.JsonHeaderItems[0].propertyValue));
            });

            Dispose();

        }


        [Test]
        // Test that when we do not get a route match and acquire the resp doc, we get the expected result.
        public async Task TestSchemaNoMatchRespDoc()
        {
            var scenario = 4;
            var dataAccess = new DataAccessMock(scenario);
            var procActLogic = new ProcessRequestActionLogic(dataAccess, new JsonParser(), new XmlParser());
            RequestResult reqResult = await procActLogic.ProcessRequestMessageAsync(ParserType.JSON, _hikerStream, _project);
            Assert.Multiple(() =>
            {
                Assert.That(reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(reqResult.ResponseStr, Is.EqualTo(RespDocHelper.RespDocNoMatchRespString));
                Assert.That(reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(reqResult.HeaderItems[0].propertyName, Is.EqualTo(RespDocHelper.JsonHeaderItems[0].propertyName));
                Assert.That(reqResult.HeaderItems[0].propertyValue, Is.EqualTo(RespDocHelper.JsonHeaderItems[0].propertyValue));
            });

            Dispose();
        }


        [Test]
        // Test that when we do get a route match but fail to acquire the resp doc, we get the expected result.
        public async Task TestSchemaNoMatchRespDocFound()
        {
            var scenario = 5;
            var dataAccess = new DataAccessMock(scenario);
            var procActLogic = new ProcessRequestActionLogic(dataAccess, new JsonParser(), new XmlParser());
            RequestResult reqResult = await procActLogic.ProcessRequestMessageAsync(ParserType.JSON, _hikerStream, _project);
            Assert.Multiple(() =>
            {
                Assert.That(reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.InternalServerError));
                Assert.That(reqResult.ResponseStr, Is.EqualTo($"Found a schema match but failed to find a response document with id {RoutingDocHelper.RouteMatchingRespDocId}"));
                Assert.That(reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(reqResult.HeaderItems[0].propertyName, Is.EqualTo("Content-Type"));
                Assert.That(reqResult.HeaderItems[0].propertyValue, Is.EqualTo("text/plain"));
            });

            Dispose();

        }

        [Test]
        // Test that we get correct variable replacement in the response/result.
        public async Task TestSchemaMatchRespDocVars()
        {
            var scenario = 6;
            var dataAccess = new DataAccessMock(scenario);
            var procActLogic = new ProcessRequestActionLogic(dataAccess, new JsonParser(), new XmlParser());
            RequestResult reqResult = await procActLogic.ProcessRequestMessageAsync(ParserType.JSON, _hikerStream, _project);

            if (_debug)
            {
                TestContext.Out.WriteLine($"Response string = {reqResult.ResponseStr}");
            }

            Assert.Multiple(() =>
            {
                Assert.That(reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(reqResult.ResponseStr, Is.EqualTo(RespDocHelper.ExpectedRespDocMatchRespStrWithVars));
                Assert.That(reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(reqResult.HeaderItems[0].propertyName, Is.EqualTo(RespDocHelper.JsonHeaderItems[0].propertyName));
                Assert.That(reqResult.HeaderItems[0].propertyValue, Is.EqualTo(RespDocHelper.JsonHeaderItems[0].propertyValue));
            });

            Dispose();

        }
    }
}
