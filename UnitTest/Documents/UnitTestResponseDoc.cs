using Chameleon.CommonApiLib;
using Chameleon.CommonLib;

namespace UnitTest
{
    public class UnitTestResponseDoc
    {
        private const string _project = "RespDocUnitTest";
        private const string _respContent = "yabba dabba do";
        private const string _hdr1 = "thingone";
        private const string _hdr1Val = "chaos";
        private const string _hdr2 = "thingtwo";
        private const string _hdr2Val = "disaster";
        private const string _id = "12345";


        // Validate RespDoc property values.
        [Test]
        public void TestRespDocPropertyValues()
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


        // Validate NoSchemaMatchRespDoc property values.
        [Test]
        public void TestNoSchemaMatchRespDocPropertyValues()
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
        public void TestNoSchemaMatchRespDocIdPropertyValue()
        {
            NoSchemaMatchRespDoc respDoc = GetNoSchemaMatchRespDoc(true);
            Assert.That(respDoc.id, Is.EqualTo(_id));
        }


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
    }
}
