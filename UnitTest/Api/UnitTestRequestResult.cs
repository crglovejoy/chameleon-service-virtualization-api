using Chameleon.Api;

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
        public void RequestResultConstructor()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.Success, Is.False);
                Assert.That(_reqResult.ServerErr, Is.False);
                Assert.That(_reqResult.ResponseStr, Is.EqualTo(string.Empty));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(0));
                Assert.That(_reqResult.LoggingError, Is.EqualTo(string.Empty));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(ReturnResultStatus.None));
            });
        }

        [Test]
        public void RequestResultSetAsServerErrorWithEx()
        {
            _reqResult.SetAsServerErr(new Exception("This is an exception"), [new Chameleon.CommonLib.HeaderItem("header1", "value1")]);

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.Success, Is.False);
                Assert.That(_reqResult.ServerErr, Is.True);
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("This is an exception"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo("header1"));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo("value1"));
                Assert.That(_reqResult.LoggingError, Is.EqualTo("System.Exception: This is an exception"));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(ReturnResultStatus.Result500));
            });
        }

        [Test]
        public void RequestResultSetAsServerErrorWithStr()
        {
            _reqResult.SetAsServerErr("We got an error!", [new Chameleon.CommonLib.HeaderItem("header1", "value1")]);

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.Success, Is.False);
                Assert.That(_reqResult.ServerErr, Is.True);
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("We got an error!"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo("header1"));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo("value1"));
                Assert.That(_reqResult.LoggingError, Is.EqualTo("We got an error!"));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(ReturnResultStatus.Result500));
            });
        }

        [Test]
        public void RequestResultSetAsSuccess()
        {
            _reqResult.SetAsSuccess("Success!", [new Chameleon.CommonLib.HeaderItem("header1", "value1")]);

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.Success, Is.True);
                Assert.That(_reqResult.ServerErr, Is.False);
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("Success!"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo("header1"));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo("value1"));
                Assert.That(_reqResult.LoggingError, Is.EqualTo(string.Empty));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(ReturnResultStatus.Result200));
            });
        }

        [Test]
        public void RequestResultSetAsBadRequest()
        {
            _reqResult.SetAsBadRequest("Bad Request!", [new Chameleon.CommonLib.HeaderItem("header1", "value1")]);

            Assert.Multiple(() =>
            {
                Assert.That(_reqResult.Success, Is.False);
                Assert.That(_reqResult.ServerErr, Is.False);
                Assert.That(_reqResult.ResponseStr, Is.EqualTo("Bad Request!"));
                Assert.That(_reqResult.HeaderItems.Length, Is.EqualTo(1));
                Assert.That(_reqResult.HeaderItems[0].propertyName, Is.EqualTo("header1"));
                Assert.That(_reqResult.HeaderItems[0].propertyValue, Is.EqualTo("value1"));
                Assert.That(_reqResult.LoggingError, Is.EqualTo(string.Empty));
                Assert.That(_reqResult.ResultStatus, Is.EqualTo(ReturnResultStatus.Result400));
            });
        }
    }
}
