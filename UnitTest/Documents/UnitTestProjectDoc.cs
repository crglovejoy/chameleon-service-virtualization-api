using Chameleon.CommonLib;

namespace UnitTest
{
    public class UnitTestProjectDoc
    {
        // Validate RespDoc property values.
        [Test]
        public void TestProjectDocPropertyValues()
        {
     
            ProjectDoc projDoc = GetProjectDoc();

            Assert.Multiple(() =>
            {
                Assert.That(projDoc.id.StartsWith(typeof(ProjectDoc).Name));
                Assert.That(projDoc.project, Is.EqualTo(DocumentBase.CommonProjectName));
                Assert.That(projDoc.documentType, Is.EqualTo(DocumentTypeEnum.ProjectDoc));
                Assert.That(projDoc.projectNames.Length, Is.EqualTo(2));
                Assert.That(projDoc.projectNames[0], Is.EqualTo(_proj1));
                Assert.That(projDoc.projectNames[1], Is.EqualTo(_proj2));
            });
        }


        // Validate ProjectDoc Id property value.
        [Test]
        public void TestProjectDocIdPropertyValue()
        {

            ProjectDoc projDoc = GetProjectDoc(true);
            Assert.That(projDoc.id, Is.EqualTo(_id));
        }


        private const string _proj1 = "project1";
        private const string _proj2 = "project2";
        private const string _id = "ABCD";

        private static ProjectDoc GetProjectDoc(Boolean useIdVal = false)
        {
            string[] projNames = [
                new string(_proj1),
                new string(_proj2)
            ];

            if (useIdVal)
            {
                return new ProjectDoc(projNames, _id);
            }
                
            return new ProjectDoc(projNames);
        }
    }
}
