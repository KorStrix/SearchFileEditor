using SFE.Example;

namespace SFE
{
    public partial class SearchFileEditorConfig
    {
        public static readonly string ExampleConfigFileName = "SFEExample.json";

        public static SearchFileEditorConfig ExampleFile
        {
            get
            {
                SearchFileEditorConfig pConfig = new SearchFileEditorConfig();
                pConfig.arrEditFile = new EditFileInfo[3];
                pConfig.arrEditFile[0] = new EditFileInfo(EditFileInfo.EFileType.json, $"/Example/{JsonExample.const_FileName}", "User.Nickname", "Strix", "this is comment, blahblah");
                pConfig.arrEditFile[1] = new EditFileInfo(EditFileInfo.EFileType.json, $"/Example/{JsonExample.const_FileName}", "GithubURL", "https://github.com/KorStrix", "blah blah");
                pConfig.arrEditFile[2] = new EditFileInfo(EditFileInfo.EFileType.yaml, $"/Example/{YamlExample.const_FileName}", "Example.thisProgramName", "SFE", "comment~~");

                return pConfig;
            }
        }
    }
}

namespace SFE.Example
{
    public class JsonExample
    {
        public const string const_FileName = "JsonExample.json";

        public class UserExample
        {
            public string Nickname = "DummyNickName";
            public int Age = 100;
        }

        public UserExample User = new UserExample();
        public string GithubURL = "DummyURL";
    }

    public class YamlExample
    {
        public const string const_FileName = "YamlExample.yml";

        public const string const_ExampleYMLText =
            @"
name: Example YAML File for SFE

Example:
    thisProgramName: Dummy
    ";

    }
}
