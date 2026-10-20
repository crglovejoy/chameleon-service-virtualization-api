using Chameleon.CommonLib;

namespace UnitTest.Documents
{
    public class UnitTestProjectDoc
    {
        // Validate RespDoc property values.
        [Test]
        public void TestProjectDocConstructor()
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


        [Test]
        public void TestProjectDocAddProjectName()
        {

            ProjectDoc projDoc = GetProjectDoc();
            projDoc.AddProjectName("project3");

            Assert.Multiple(() =>
            {
                Assert.That(projDoc.projectNames.Length, Is.EqualTo(3));
                Assert.That(projDoc.projectNames[0], Is.EqualTo(_proj1));
                Assert.That(projDoc.projectNames[1], Is.EqualTo(_proj2));
                Assert.That(projDoc.projectNames[2], Is.EqualTo("project3"));
            });
        }


        // Validate ProjectDoc Id property value.
        [Test]
        public void TestProjectDocIdSpecifiedValue()
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
