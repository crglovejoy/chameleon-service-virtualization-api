using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Chameleon.Parsers;
using System.Diagnostics;


namespace UnitTest.ParsersAndMessageProperties
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
            MessageProperties mProps = xmlParser.ProcessMessageIntoMessageProperties(TestXmlBooks.XmlRequestMessage);

            if (_debug)
            {
                OutputHelpers.WriteOutMessageProperties(mProps);
            }

            Assert.Multiple(() =>
            {
                Assert.That(mProps.messageDict.ContainsKey("?xml.@version"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("?xml.@version"), Is.EqualTo("1.0"));
                Assert.That(mProps.messageDict.ContainsKey("?xml.@encoding"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("?xml.@encoding"), Is.EqualTo("UTF-8"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[0].@category"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[0].@category"), Is.EqualTo("fiction"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[0].title.@lang"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[0].title.@lang"), Is.EqualTo("en"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[0].title.#text"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[0].title.#text"), Is.EqualTo("The Great Gatsby"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[0].author"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[0].author"), Is.EqualTo("F. Scott Fitzgerald"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[0].year"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[0].year"), Is.EqualTo("1925"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[0].price"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[0].price"), Is.EqualTo("14.99"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[1].@category"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[1].@category"), Is.EqualTo("programming"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[1].title.@lang"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[1].title.@lang"), Is.EqualTo("en"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[1].title.#text"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[1].title.#text"), Is.EqualTo("Learning XML"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[1].author"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[1].author"), Is.EqualTo("Erik T. Ray"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[1].year"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[1].year"), Is.EqualTo("2003"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[1].price"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[1].price"), Is.EqualTo("39.95"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[2].@category"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[2].@category"), Is.EqualTo("self improvement"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[2].title.@lang"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[2].title.@lang"), Is.EqualTo("en"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[2].title.#text"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[2].title.#text"), Is.EqualTo("Mastering the Flop"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[2].author"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[2].author"), Is.EqualTo("Luis Suarez"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[2].year"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[2].year"), Is.EqualTo("2030"));
                Assert.That(mProps.messageDict.ContainsKey("bookstore.book[2].price"), Is.True);
                Assert.That(mProps.messageDict.GetValueOrDefault("bookstore.book[2].price"), Is.EqualTo("400.00"));

                Assert.That(mProps.schemaSignatureHash, Is.EqualTo(TestXmlBooks.SchemaHashString));
                Assert.That(mProps.originalMessageStr, Is.EqualTo(StringHelpers.CleanToSingleLineString(TestXmlBooks.XmlRequestMessage)));
            });
        }


        [Test]
        public void TestXmlParserWithSpecificClassGetsNotImplException()
        {
            XmlParser xmlParser = new XmlParser();
            Boolean gotNotImplExceptType = false;

            try
            {
                MessageProperties mProps = xmlParser.ProcessMessageIntoClassSpecificMessageProperties<string>(TestXmlBooks.XmlRequestMessage);
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
            XmlParser xmlParser = new();
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

        [Test]
        public void XmlParserSpeedTest()
        {
            int numIters = 100;
            Stopwatch stopW = new Stopwatch();
            XmlParser xmlParser = new();
            MessageProperties msgProp = null!;
            string xmlStr = TestXmlBooks.XmlRequestMessage;

            for (int i = 0; i < numIters; i++)
            {
                stopW.Start();
                msgProp = xmlParser.ProcessMessageIntoMessageProperties(xmlStr);
                stopW.Stop();
            }

            Assert.That(msgProp, Is.Not.Null);
            decimal avgParserTimeMs = (decimal)stopW.ElapsedMilliseconds / (decimal)numIters;

            TestContext.Out.WriteLine($"Total time taken to process {numIters} messages: {stopW.ElapsedMilliseconds} ms");
            TestContext.Out.WriteLine($"Average time taken to process a single message: {avgParserTimeMs} ms");

            Assert.That(avgParserTimeMs, Is.LessThan(.5m));

            /* RESULTS CAN VARY DEPENDING ON MACHINE AND OTHER LOAD
             * Last test output on a windows PC...
             * Total time taken to process 100 messages: 7 ms
             * Average time taken to process a single message: 0.07 ms
             */
        }
    }
}
