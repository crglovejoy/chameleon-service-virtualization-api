
namespace Chameleon.CommonLib
{
    // ProjectDoc class represents a document that contains information about projects used in the dataset.

    public class ProjectDoc : DocumentBase
    {
        public string[] projectNames;

        public ProjectDoc(string[] projectNames, string id = "") : base(CommonProjectName, DocumentTypeEnum.ProjectDoc, id)
        {
            this.projectNames = projectNames;
        }

        public void AddProjectName(string newProjName)
        {
            if (string.IsNullOrWhiteSpace(newProjName) || projectNames.Contains<string>(newProjName)) return;

            List<string> projectList = [.. projectNames];
            projectList.AddRange(newProjName);
            projectNames = [.. projectList];
        }
    }
}
