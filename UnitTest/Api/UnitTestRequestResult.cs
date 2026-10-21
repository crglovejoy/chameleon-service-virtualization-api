using Chameleon.Api;
using System.Net;

namespace UnitTest.Api
{

    public class UnitTestRequestResult
    {
        private RequestResult _reqResult;

        [SetUp]
        public void Setup()
        {
            _reqResult = new RequestResult();
        }

        [Test]
        public void TestRequestResultConstructor()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.ResponseStr, Is.EqualTo(string.Empty));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(0));
                Assert.That(_reqResult.LoggingError, Is.EqualTo(string.Empty));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.Unused));
            });
        }

        [Test]
        public void TestRequestResultSetAsServerErrorWithEx()
        {
            _reqResult.SetAsServerErr(new Exception("This is an exception"), [new Chameleon.CommonLib.HeaderItem("header1", "value1")]);

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("This is an exception"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo("header1"));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo("value1"));
                Assert.That(_reqResult.LoggingError, Is.EqualTo("System.Exception: This is an exception"));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.InternalServerError));
            });
        }

        [Test]
        public void TestRequestResultSetAsServerErrorWithStr()
        {
            _reqResult.SetAsServerErr("We got an error!", [new Chameleon.CommonLib.HeaderItem("header1", "value1")]);

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("We got an error!"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo("header1"));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo("value1"));
                Assert.That(_reqResult.LoggingError, Is.EqualTo("We got an error!"));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.InternalServerError));
            });
        }

        [Test]
        public void TestRequestResultSetAsSuccess()
        {
            _reqResult.SetAsSuccess("Success!", [new Chameleon.CommonLib.HeaderItem("header1", "value1")]);

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("Success!"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo("header1"));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo("value1"));
                Assert.That(_reqResult.LoggingError, Is.EqualTo(string.Empty));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.OK));
            });
        }

        [Test]
        public void TestRequestResultSetAsBadRequest()
        {
            _reqResult.SetAsNotFound("Bad Request!", [new Chameleon.CommonLib.HeaderItem("header1", "value1")]);

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("Bad Request!"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo("header1"));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo("value1"));
                Assert.That(_reqResult.LoggingError, Is.EqualTo(string.Empty));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [Test]
        public void TestRequestResultSetAsNotInitErr()
        {
            _reqResult.SetAsNotInitErr();

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("RequestResult was not set to a result"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo(RequestResult.ContentPlainTextHeader[0].propertyName));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo(RequestResult.ContentPlainTextHeader[0].propertyValue));
                Assert.That(_reqResult.LoggingError, Is.EqualTo("RequestResult was not set to a result"));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(HttpStatusCode.InternalServerError));
            });
        }
    }
}
