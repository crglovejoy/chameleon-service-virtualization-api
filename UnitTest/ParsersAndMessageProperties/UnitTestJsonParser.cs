using Chameleon.CommonApiLib;
using Chameleon.CommonLib;
using Newtonsoft.Json;
using NUnit.Framework.Internal;
using System.Diagnostics;

namespace UnitTest
{
    public class UnitTestJsonParser
    {
        private static readonly Boolean _debug = (false || CommonTestVals.Debug);

        [SetUp]
        public void Setup()
        { }

        /* The next 3 methods validate that parsing a Json string into a specific class or not providing a
         * specific class results in the same MessageProperties values.
         */

        // Providing a specific class (Hiker) to the parser.
        [Test]
        public void TestHikerJsonRequestMessageProperties()
        {
            TestHiker hiker = new();
            string jsonStr = JsonConvert.SerializeObject(hiker);
            JsonParser jsonParser = new JsonParser();
            MessageProperties mProps = jsonParser.ProcessMessageIntoClassSpecificMessageProperties<TestHiker>(jsonStr);

            if (_debug)
            {
                OutputHelpers.WriteOutMessageProperties(mProps);
            }

            DoHikerMessagePropertiesAssertions(mProps, jsonStr);
        }


        // Not providing a specific class to the parser.
        [Test]
        public void TestGenericJsonRequestMessageProperties()
        {
            TestHiker hiker = new();
            string jsonStr = JsonConvert.SerializeObject(hiker);
            JsonParser jsonParser = new JsonParser();
            MessageProperties mProps = jsonParser.ProcessMessageIntoMessageProperties(jsonStr);

            if (_debug)
            {
                OutputHelpers.WriteOutMessageProperties(mProps);
            }

            DoHikerMessagePropertiesAssertions(mProps, jsonStr);
        }


        // Common assertion logic for both methods of Json string parsing (specific class vs no class).
        private static void DoHikerMessagePropertiesAssertions(MessageProperties mProps, string jsonStr)
        {
            var msgDict = mProps.messageDict;

            Assert.Multiple(() =>
            {
                // FirstName / LastName
                Assert.That(msgDict.ContainsKey("FirstName"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FirstName"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FirstName"), Is.EqualTo("Bat"));

                Assert.That(msgDict.ContainsKey("LastName"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("LastName"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("LastName"), Is.EqualTo("Man"));

                // Guid
                Assert.That(msgDict.ContainsKey("HikerId"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("HikerId"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("HikerId"), Is.EqualTo("b1754c14-d296-4b0f-a09a-030017f4461f"));

                // Address
                Assert.That(msgDict.ContainsKey("Address.Street"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("Address.Street"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("Address.Street"), Is.EqualTo("The Big Mansion Street"));

                Assert.That(msgDict.ContainsKey("Address.City"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("Address.City"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("Address.City"), Is.EqualTo("Gotham City"));

                Assert.That(msgDict.ContainsKey("Address.State"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("Address.State"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("Address.State"), Is.EqualTo("New York"));

                Assert.That(msgDict.ContainsKey("Address.Zip"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("Address.Zip"), Is.TypeOf<Int64>());
                Assert.That(msgDict.GetValueOrDefault("Address.Zip"), Is.EqualTo(12345));

                // FavoriteHikeArr
                Assert.That(msgDict.ContainsKey("FavoriteHikeArr[0].Name"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[0].Name"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[0].Name"), Is.EqualTo("Dark Cave"));
                Assert.That(msgDict.ContainsKey("FavoriteHikeArr[0].State"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[0].State"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[0].State"), Is.EqualTo("New York"));

                Assert.That(msgDict.ContainsKey("FavoriteHikeArr[1].Name"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[1].Name"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[1].Name"), Is.EqualTo("Penguin Rapids"));
                Assert.That(msgDict.ContainsKey("FavoriteHikeArr[1].State"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[1].State"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[1].State"), Is.EqualTo("Idaho"));

                Assert.That(msgDict.ContainsKey("FavoriteHikeArr[2].Name"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[2].Name"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[2].Name"), Is.EqualTo("Joker Falls"));
                Assert.That(msgDict.ContainsKey("FavoriteHikeArr[2].State"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[2].State"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteHikeArr[2].State"), Is.EqualTo("New Jersey"));

                // Numeric values
                Assert.That(msgDict.ContainsKey("FavoriteFloat"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteFloat"), Is.TypeOf<double>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteFloat"), Is.EqualTo(25.5d));

                Assert.That(msgDict.ContainsKey("FavoriteInt"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteInt"), Is.TypeOf<Int64>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteInt"), Is.EqualTo(8));

                Assert.That(msgDict.ContainsKey("FavoriteLong"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteLong"), Is.TypeOf<Int64>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteLong"), Is.EqualTo(987654321L));

                Assert.That(msgDict.ContainsKey("FavoriteDecimal"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteDecimal"), Is.TypeOf<double>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteDecimal"), Is.EqualTo(30.33d));

                Assert.That(msgDict.ContainsKey("FavoriteDouble"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteDouble"), Is.TypeOf<double>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteDouble"), Is.EqualTo(66.66d));

                // FavoriteColorArr
                Assert.That(msgDict.ContainsKey("FavoriteColorArr[0]"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteColorArr[0]"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteColorArr[0]"), Is.EqualTo("blue"));

                Assert.That(msgDict.ContainsKey("FavoriteColorArr[1]"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteColorArr[1]"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteColorArr[1]"), Is.EqualTo("green"));

                Assert.That(msgDict.ContainsKey("FavoriteColorArr[2]"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteColorArr[2]"), Is.TypeOf<string>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteColorArr[2]"), Is.EqualTo("red"));

                // Booleans
                Assert.That(msgDict.ContainsKey("FavoriteBool"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteBool"), Is.TypeOf<Boolean>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteBool"), Is.True);

                Assert.That(msgDict.ContainsKey("LeastFavoriteBool"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("LeastFavoriteBool"), Is.TypeOf<Boolean>());
                Assert.That(msgDict.GetValueOrDefault("LeastFavoriteBool"), Is.False);

                // DateTime
                Assert.That(msgDict.ContainsKey("BirthDate"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("BirthDate"), Is.TypeOf<DateTime>());
                Assert.That(msgDict.GetValueOrDefault("BirthDate"), Is.EqualTo(DateTime.Parse("01/01/1990")));

                Assert.That(msgDict.ContainsKey("FavoriteDateTime"), Is.True);
                Assert.That(msgDict.GetValueOrDefault("FavoriteDateTime"), Is.TypeOf<DateTime>());
                Assert.That(msgDict.GetValueOrDefault("FavoriteDateTime"), Is.EqualTo(DateTime.Parse("10/08/2026 12:30:00.123 PM")));

                // Nulls
                Assert.That(msgDict.ContainsKey("FavoriteNullValue"), Is.False);

                // Signature
                Assert.That(mProps.schemaSignatureHash, Is.EqualTo(CommonTestVals.HikerExpectedSignatureHash));

                // Original Message String
                Assert.That(mProps.originalMessageStr, Is.EqualTo(jsonStr));
            });
        }


        // Validate that providing an empty string to the parser gets an exception.
        [Test]
        public void TestJsonParserWithEmptyMessageGetsArgException()
        {
            JsonParser jsonParser = new();
            Boolean gotArgExceptType = false;

            try
            {
                MessageProperties mProps = jsonParser.ProcessMessageIntoMessageProperties("");
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
        public void JsonParserSpeedTest()
        {
            int numIters = 100;
            Stopwatch stopW = new Stopwatch();
            JsonParser jsonParser = new();
            MessageProperties msgProp = null!;
            TestHiker hiker = new();
            string jsonStr = JsonConvert.SerializeObject(hiker);

            for (int i = 0; i < numIters; i++)
            {
                stopW.Start();
                msgProp = jsonParser.ProcessMessageIntoMessageProperties(jsonStr);
                stopW.Stop();
            }

            Assert.That(msgProp, Is.Not.Null);
            decimal avgParserTimeMs = (decimal)stopW.ElapsedMilliseconds / (decimal)numIters;

            TestContext.Out.WriteLine($"Total time taken to process {numIters} messages: {stopW.ElapsedMilliseconds} ms");
            TestContext.Out.WriteLine($"Average time taken to process a single message: {avgParserTimeMs} ms");

            Assert.That(avgParserTimeMs, Is.LessThan(.4m));

            /* RESULTS CAN VARY DEPENDING ON MACHINE AND OTHER LOAD
             * Last test output on a windows PC...
             * Total time taken to process 100 messages: 7 ms
             * Average time taken to process a single message: 0.07 ms
             */
        }

        [Test]
        public void JsonParserSpecificClassSpeedTest()
        {
            int numIters = 100;
            Stopwatch stopW = new Stopwatch();
            JsonParser jsonParser = new();
            MessageProperties msgProp = null!;
            TestHiker hiker = new();
            string jsonStr = JsonConvert.SerializeObject(hiker);

            for (int i = 0; i < numIters; i++)
            {
                stopW.Start();
                msgProp = jsonParser.ProcessMessageIntoClassSpecificMessageProperties<TestHiker>(jsonStr);
                stopW.Stop();
            }

            Assert.That(msgProp, Is.Not.Null);
            decimal avgParserTimeMs = (decimal)stopW.ElapsedMilliseconds / (decimal)numIters;

            TestContext.Out.WriteLine($"Total time taken to process {numIters} messages: {stopW.ElapsedMilliseconds} ms");
            TestContext.Out.WriteLine($"Average time taken to process a single message: {avgParserTimeMs} ms");

            Assert.That(avgParserTimeMs, Is.LessThan(1.20m));

            /* RESULTS CAN VARY DEPENDING ON MACHINE AND OTHER LOAD
             * Last test output on a windows PC...
             * Total time taken to process 100 messages: 51 ms
             * Average time taken to process a single message: 0.51 ms
             */
        }
    }
}
