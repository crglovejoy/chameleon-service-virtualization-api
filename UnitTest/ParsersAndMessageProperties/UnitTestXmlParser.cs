using Chameleon.CommonLib;
using Chameleon.CommonApiLib;


namespace UnitTest
{
    internal class UnitTestXmlParser
    {
        private static readonly Boolean _debug = (false || CommonTestVals.Debug);


        [SetUp]
        public void Setup()
        { }

        [Test]
        public void TestXmlRequestMessageProperties()
        {
            XmlParser xmlParser = new XmlParser();
            MessageProperties mProps = xmlParser.ProcessMessageIntoMessageProperties(CommonTestVals.MessageXmlString);

            if (_debug)
            {
                OutputHelpers.WriteOutMessageProperties(mProps);
            }

            Assert.Multiple(() =>
            {
                Assert.That(mProps.messageDict.ContainsKey("?xml.@version"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("?xml.@version"), Is.EqualTo("1.0"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.@id"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.@id"), Is.EqualTo("55000"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.name"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.name"), Is.EqualTo("Charter Group"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[0].street"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[0].street"), Is.EqualTo("100 Main"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[0].city"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[0].city"), Is.EqualTo("Framingham"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[0].state"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[0].state"), Is.EqualTo("MA"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[1].street"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[1].street"), Is.EqualTo("720 Prospect"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[1].city"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[1].city"), Is.EqualTo("Framingham"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[1].state"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[1].state"), Is.EqualTo("MA"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[2].street"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[2].street"), Is.EqualTo("120 Ridge"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[2].state"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[2].state"), Is.EqualTo("MA"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.address[2].zip"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.address[2].zip"), Is.EqualTo("01760"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.wealth.bank"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.wealth.bank"), Is.EqualTo("100.25"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.wealth.stock"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.wealth.stock"), Is.EqualTo(".25"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.wealth.car"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.wealth.car"), Is.EqualTo("0.0"));
                Assert.That(mProps.messageDict.ContainsKey("customers.customer.wealth.realestate"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("customers.customer.wealth.realestate"), Is.EqualTo("0.10"));
                Assert.That(mProps.schemaSignatureHash, Is.EqualTo(CommonTestVals.XmlStringExpectedSignatureHash));
                Assert.That(mProps.originalMessageStr, Is.EqualTo(StringHelpers.CleanToSingleLineString(CommonTestVals.MessageXmlString)));
            });
        }


        [Test]
        public void TestXmlParserWithSpecificClassGetsNotImplException()
        {
            XmlParser xmlParser = new XmlParser();
            Boolean gotNotImplExceptType = false;

            try
            {
                MessageProperties mProps = xmlParser.ProcessMessageIntoClassSpecificMessageProperties<string>(CommonTestVals.MessageXmlString);
            }
            catch (NotImplementedException niex)
            {
                gotNotImplExceptType = true;
                Assert.That(niex.Message, Is.EqualTo("ProcessMessageIntoClassSpecificMessageProperties<T> is not implemented by the XmlParser class"));
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine(ex);
            }

            Assert.That(gotNotImplExceptType, Is.True);
        }

        [Test]
        public void TestXmlParserWithEmptyMessageGetsArgException()
        {
            XmlParser xmlParser = new XmlParser();
            Boolean gotArgExceptType = false;

            try
            {
                MessageProperties mProps = xmlParser.ProcessMessageIntoMessageProperties("");
            }
            catch (ArgumentException argex)
            {
                gotArgExceptType = true;
                Assert.That(argex.Message, Is.EqualTo("Message cannot be null, empty or whitespace. (Parameter 'message')"));
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine(ex);
            }

            Assert.That(gotArgExceptType, Is.True);
        }
    }
}
