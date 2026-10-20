using Chameleon.CommonLib;
using System.Text.RegularExpressions;

namespace UnitTest.Documents
{
    public partial class UnitTestResponseDocFlavors
    {
        private static readonly Boolean _debug = (false || CommonTestVals.Debug);
        private const string _project = "RespDocUnitTest";
        private const string _respContent = "yabba dabba do";
        private const string _hdr1 = "thingone";
        private const string _hdr1Val = "chaos";
        private const string _hdr2 = "thingtwo";
        private const string _hdr2Val = "disaster";
        private const string _id = "12345";


        /***************************/
        /*  Test Methods - RespDoc */
        /***************************/

        private static RespDoc GetRespDoc(Boolean useIdVal = false)
        {
            HeaderItem[] hdrs = [
                new HeaderItem(_hdr1, _hdr1Val),
                new HeaderItem(_hdr2, _hdr2Val)
            ];

            if (useIdVal)
            {
                // Use the Id passed as a parameter for the document id.
                return new RespDoc(_project, hdrs, _respContent, _id);
            }

            // Use the auto-generated Id prefixed with the document type string.
            return new RespDoc(_project, hdrs, _respContent);
        }


        [Test]
        public void TestRespDocConstructor()
        {

            RespDoc respDoc = GetRespDoc();

            Assert.Multiple(() =>
            {
                Assert.That(respDoc.id.StartsWith(typeof(RespDoc).Name));
                Assert.That(respDoc.responseContent, Is.EqualTo(_respContent));
                Assert.That(respDoc.project, Is.EqualTo(_project));
                Assert.That(respDoc.documentType, Is.EqualTo(DocumentTypeEnum.RespDoc));

                Assert.That(respDoc.headerItems.Length, Is.EqualTo(2));
                Assert.That(respDoc.headerItems[0].propertyName, Is.EqualTo(_hdr1));
                Assert.That(respDoc.headerItems[0].propertyValue, Is.EqualTo(_hdr1Val));
                Assert.That(respDoc.headerItems[1].propertyName, Is.EqualTo(_hdr2));
                Assert.That(respDoc.headerItems[1].propertyValue, Is.EqualTo(_hdr2Val));
            });
        }


        // Validate RespDoc Id property value.
        [Test]
        public void TestRespDocIdPropertyValue()
        {
            RespDoc respDoc = GetRespDoc(true);
            Assert.That(respDoc.id, Is.EqualTo(_id));
        }


        [Test]
        public void TestWrapStringWithVarDelimiter()
        {
            string varName = "TacoTuesday";
            string newVarDelimited = RespDoc.WrapStringWithVariableDelimiter(varName);
            Assert.That(newVarDelimited, Is.EqualTo($"{RespDoc.variableDelimiter}{varName}{RespDoc.variableDelimiter}"));
        }


        [Test]
        public void TestSuccessfulRespVaribleResolution()
        {
            Dictionary<string, object> msgDict = new();
            msgDict.Add("FirstName", "CrazyBat");
            msgDict.Add("LastName", "CrazyMan");
            msgDict.Add("FavoriteDecimal", 11.33m);
            msgDict.Add("FavoriteInt", 11L);
            msgDict.Add("FavoriteDouble", 11.33d);
            msgDict.Add("FavoriteFloat", 11.33d);
            msgDict.Add("FavoriteLong", 11L);
            msgDict.Add("HikerId", null!);

            string firstNameVar = RespDoc.WrapStringWithVariableDelimiter("FirstName");
            string lastNameVar = RespDoc.WrapStringWithVariableDelimiter("LastName");
            string favDecimalVar = RespDoc.WrapStringWithVariableDelimiter("FavoriteDecimal");
            string favIntVar = RespDoc.WrapStringWithVariableDelimiter("FavoriteInt");
            string favDblVar = RespDoc.WrapStringWithVariableDelimiter("FavoriteDouble");
            string favFloatVar = RespDoc.WrapStringWithVariableDelimiter("FavoriteFloat");
            string favLongVar = RespDoc.WrapStringWithVariableDelimiter("FavoriteLong");
            string hikerIdVar = RespDoc.WrapStringWithVariableDelimiter("HikerId");

            List<string> variableNames = new();
            foreach(var key in msgDict.Keys)
            {
                variableNames.Add(key);
            }

            string respStr = $$"""{"FirstName":"{{firstNameVar}}","LastName":"{{lastNameVar}}","HikerId":"{{hikerIdVar}}","BirthDate":"1990-01-01T00:00:00","Address":{"Street":"The Big Mansion Street","City":"Gotham City","State":"New York","Zip":12345},"FavoriteHikeArr":[{"Name":"Dark Cave","State":"New York"},{"Name":"Penguin Rapids","State":"Idaho"},{"Name":"Joker Falls","State":"New Jersey"}],"FavoriteFloat":{{favFloatVar}},"FavoriteInt":{{favIntVar}},"FavoriteLong":{{favLongVar}},"FavoriteDecimal":{{favDecimalVar}},"FavoriteDouble":{{favDblVar}},"FavoriteEmptyArr":[],"FavoriteColorArr":["blue","green","red"],"FavoriteNullValue":null,"FavoriteBool":true,"LeastFavoriteBool":false,"FavoriteDateTime":"2026-10-08T12:30:00.123"}""";
            string newRespStr = RespDoc.GetVariablesResolvedResponse(respStr, msgDict, [.. variableNames]);

            if (_debug)
            {
                TestContext.Out.WriteLine($"newRespStr = {newRespStr}");
            }
            

            Assert.Multiple(() =>
            {
                Assert.That(Regex.IsMatch(newRespStr, @"""FirstName"":""CrazyBat"""), Is.True);
                Assert.That(Regex.IsMatch(newRespStr, @"""LastName"":""CrazyMan"""), Is.True);
                Assert.That(Regex.IsMatch(newRespStr, @"""HikerId"":""NOT FOUND"""), Is.True);  // This happens when the value in msgDict for the key is null
                Assert.That(Regex.IsMatch(newRespStr, @"""FavoriteDecimal"":11.33,"), Is.True);
                Assert.That(Regex.IsMatch(newRespStr, @"""FavoriteInt"":11,"), Is.True);
                Assert.That(Regex.IsMatch(newRespStr, @"""FavoriteDouble"":11.33,"), Is.True);
                Assert.That(Regex.IsMatch(newRespStr, @"""FavoriteFloat"":11.33,"), Is.True);
                Assert.That(Regex.IsMatch(newRespStr, @"""FavoriteLong"":11,"), Is.True);
            });
        }


        /****************************************/
        /*  Test Methods - NoSchemaMatchRespDoc */
        /****************************************/

        private static NoSchemaMatchRespDoc GetNoSchemaMatchRespDoc(Boolean useIdVal = false)
        {
            HeaderItem[] hdrs = [
                new HeaderItem(_hdr1, _hdr1Val),
                new HeaderItem(_hdr2, _hdr2Val)
            ];

            if (useIdVal)
            {
                return new NoSchemaMatchRespDoc(_project, hdrs, _respContent, _id);
            }

            return new NoSchemaMatchRespDoc(_project, hdrs, _respContent);
        }


        // Validate NoSchemaMatchRespDoc property values.
        [Test]
        public void TestNoSchemaMatchRespDocConstructor()
        {

            NoSchemaMatchRespDoc respDoc = GetNoSchemaMatchRespDoc();

            Assert.Multiple(() =>
            {
                Assert.That(respDoc.id.StartsWith(typeof(NoSchemaMatchRespDoc).Name));
                Assert.That(respDoc.responseContent, Is.EqualTo(_respContent));
                Assert.That(respDoc.project, Is.EqualTo(_project));
                Assert.That(respDoc.documentType, Is.EqualTo(DocumentTypeEnum.NoSchemaMatchRespDoc));

                Assert.That(respDoc.headerItems.Length, Is.EqualTo(2));
                Assert.That(respDoc.headerItems[0].propertyName, Is.EqualTo(_hdr1));
                Assert.That(respDoc.headerItems[0].propertyValue, Is.EqualTo(_hdr1Val));
                Assert.That(respDoc.headerItems[1].propertyName, Is.EqualTo(_hdr2));
                Assert.That(respDoc.headerItems[1].propertyValue, Is.EqualTo(_hdr2Val));
            });
        }

        // Validate NoSchemaMatchRespDoc Id property value.
        [Test]
        public void TestNoSchemaMatchRespDocSpecifiedId()
        {
            NoSchemaMatchRespDoc respDoc = GetNoSchemaMatchRespDoc(true);
            Assert.That(respDoc.id, Is.EqualTo(_id));
        }
    }
}
