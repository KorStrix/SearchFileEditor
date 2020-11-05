using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFE.Example;
using YamlDotNet.RepresentationModel;

namespace SFE
{
    class Program
    {
        enum ECommand
        {
            example,
            run,
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("this Program require commandline\n"
                + "-example         is generate ExampleFolder & ExampleConfigFile in current directory\n"
                + "-run (SFE config file relative path) : is Search File Editing Start");
                return;
            }

            string[] arrCommandLine = Environment.GetCommandLineArgs();
            for (int i = 0; i < arrCommandLine.Length; i++)
            {
                string strCommand = arrCommandLine[i];
                if (strCommand.StartsWith("-") == false)
                    continue;

                if (Enum.TryParse(strCommand.Substring(1, strCommand.Length - 1),
                    true, out ECommand eCommand) == false)
                {
                    Console.WriteLine($"Error - invalid Commandline {strCommand}");
                    return;
                }


                string strCurrentDirectory = Directory.GetCurrentDirectory();
                switch (eCommand)
                {
                    case ECommand.example:
                        StartExample(strCurrentDirectory);
                        break;

                    case ECommand.run:
                        if (StartEditFile(arrCommandLine, eCommand, strCurrentDirectory, ref i) == false)
                            return;

                        break;


                    default:
                        Console.WriteLine($"Error - undefine Commandline {strCommand}");
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void StartExample(string strCurrentDirectory)
        {
            string strExampleConfigText = JsonConvert.SerializeObject(SearchFileEditorConfig.ExampleFile, Formatting.Indented);
            File.WriteAllText($"{strCurrentDirectory}/{SearchFileEditorConfig.ExampleConfigFileName}", strExampleConfigText);

            string strExampleFolder = $"{strCurrentDirectory}/Example";
            if (Directory.Exists(strExampleFolder) == false)
                Directory.CreateDirectory(strExampleFolder);

            string strJsonText = JsonConvert.SerializeObject(new JsonExample(), Formatting.Indented);
            File.WriteAllText($"{strExampleFolder}/{JsonExample.const_FileName}", strJsonText);
            File.WriteAllText($"{strExampleFolder}/{YamlExample.const_FileName}", YamlExample.const_ExampleYMLText);

            Console.WriteLine($"Welcome Example, See this Folder {strExampleFolder} and Execute -run {SearchFileEditorConfig.ExampleConfigFileName}");
        }

        private static bool StartEditFile(string[] arrCommandLine, ECommand eCommand, string strCurrentDirectory, ref int i)
        {
            if (i + 1 >= arrCommandLine.Length)
            {
                Console.WriteLine($"Run Error, {eCommand} is require more argument");
                return false;
            }

            string strConfigFilePath = $"{strCurrentDirectory}/{arrCommandLine[++i]}";
            string strFileContents = File.ReadAllText(strConfigFilePath);
            SearchFileEditorConfig pConfig = JsonConvert.DeserializeObject<SearchFileEditorConfig>(strFileContents);

            var arrFileInfo = pConfig.arrEditFile;
            for (int j = 0; j < arrFileInfo.Length; j++)
            {
                var pFileInfo = arrFileInfo[j];
                string strFilePath = $"{strCurrentDirectory}/{pFileInfo.File_RelativePath}";
                if (File.Exists(strFilePath) == false)
                {
                    Console.WriteLine($"Not Found File {strFilePath}");
                    continue;
                }

                string strFileText = File.ReadAllText(strFilePath);
                switch (pFileInfo.FileType)
                {
                    case SearchFileEditorConfig.EditFileInfo.EFileType.yaml:
                        EditYAML(strFilePath, strFileText, pFileInfo);
                        break;

                    case SearchFileEditorConfig.EditFileInfo.EFileType.json:
                        if (EditJson(strFileText, pFileInfo, out string strNewJson))
                            File.WriteAllText(strFilePath, strNewJson);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Console.WriteLine($"Run strConfigFilePath : {strConfigFilePath}, strFileContents : {strFileContents}");
            return true;
        }

        private static void EditYAML(string strFilePath, string strFileText, SearchFileEditorConfig.EditFileInfo pFileInfo)
        {
            StringReader pStrReader = new StringReader(strFileText);
            var yaml = new YamlStream();
            yaml.Load(pStrReader);

            YamlMappingNode pRootObject = (YamlMappingNode)yaml.Documents[0].RootNode;
            YamlNode pObject = pRootObject;
            Queue<string> queueField = new Queue<string>(pFileInfo.FieldName.Split("."));
            while (queueField.TryDequeue(out string strFieldName))
            {
                YamlMappingNode pObjectParsed = (YamlMappingNode)pObject;
                if (pObjectParsed == null)
                {
                    Console.WriteLine("Run YAML Error");
                    return;
                }

                if (pObjectParsed.Children.TryGetValue(strFieldName, out pObject) == false)
                {
                    Console.WriteLine("Run YAML Error");
                    return;
                }

                if (queueField.Count == 0)
                    pObjectParsed.Children[strFieldName] = pFileInfo.Value;
            }


            StreamWriter pWriter = new StreamWriter(strFilePath);
            yaml.Save(pWriter);

            pStrReader.Close();
            pStrReader.Dispose();
            pWriter.Close();
            pWriter.Dispose();
        }

        private static bool EditJson(string strFileText, SearchFileEditorConfig.EditFileInfo pFileInfo, out string strFileTextNew)
        {
            strFileTextNew = "";

            JObject pRootObject = JObject.Parse(strFileText);
            JToken pField = pRootObject.SelectToken(pFileInfo.FieldName);
            if (pField == null)
            {
                Console.WriteLine($"Run Json Error // NotFound Field : {pFileInfo.FieldName}");
                return false;
            }

            pRootObject.SelectToken(pFileInfo.FieldName)?.Replace(pFileInfo.Value);
            strFileTextNew = pRootObject.ToString(Formatting.Indented);

            return true;
        }
    }
}
