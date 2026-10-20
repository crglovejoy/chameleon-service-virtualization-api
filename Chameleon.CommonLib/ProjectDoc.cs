
namespace Chameleon.CommonLib
{
    /*******************/
    /**  PROJECT DOC  **/
    /*******************/

    public class ProjectDoc : DocumentBase
    {
        private string[] _projectNames;
        public string[] projectNames => _projectNames;


        /*****************/
        /*  Constructor  */
        /*****************/

        public ProjectDoc(string[] projectNames, string id = "") : base(CommonProjectName, DocumentTypeEnum.ProjectDoc, id)
        {
            _projectNames = projectNames;
        }


        /********************/
        /*  AddProjectName  */
        /********************/

        public void AddProjectName(string newProjName)
        {
            if (string.IsNullOrWhiteSpace(newProjName) || projectNames.Contains<string>(newProjName)) return;
            _projectNames = ArrayHelpers.AddObjectToArray<string>(newProjName, _projectNames);
        }
    }
}
