namespace SFE
{
    public partial class SearchFileEditorConfig
    {
        public class EditFileInfo
        {
            public enum EFileType
            {
                yaml,
                json,
            }

            public EFileType FileType;
            public string File_RelativePath;
            public string FieldName;
            public string Value;

            public EditFileInfo(EFileType fileType, string fileRelative_Path, string fieldName, string value)
            {
                this.FileType = fileType;
                this.File_RelativePath = fileRelative_Path;
                this.FieldName = fieldName;
                this.Value = value;
            }
        }

        public EditFileInfo[] arrEditFile;
    }
}