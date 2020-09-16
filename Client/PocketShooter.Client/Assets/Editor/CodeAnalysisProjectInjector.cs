using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;

namespace Editor
{
    public class CodeAnalysisProjectInjector : AssetPostprocessor
    {
        private static readonly char DS = Path.DirectorySeparatorChar;

        private static XNamespace xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";

        private static string Heyworks_Ruleset          = $"..{DS}..{DS}CARuleSets{DS}Heyworks.ruleset";
        private static string Heyworks_Internal_Ruleset = $"..{DS}..{DS}CARuleSets{DS}Heyworks.Internal.ruleset";
        private static string Heyworks_None_Ruleset     = $"..{DS}..{DS}CARuleSets{DS}Heyworks.None.ruleset";

        private static IDictionary<string, string> projectFilesWithRulesets = new Dictionary<string, string>
        {
            { "Heyworks.PocketShooter.Common",                 Heyworks_Internal_Ruleset },
            { "Heyworks.PocketShooter.Client",                 Heyworks_Internal_Ruleset },
            { "Heyworks.PocketShooter.Client.Editor",          Heyworks_Internal_Ruleset },
            { "Heyworks.PocketShooter.Client.Entities",        Heyworks_Internal_Ruleset },
            { "Heyworks.PocketShooter.Meta.Client",            Heyworks_None_Ruleset },
            { "Heyworks.PocketShooter.Realtime.Client",        Heyworks_Internal_Ruleset },
            { "Heyworks.PocketShooter.Realtime.Client.Common", Heyworks_Internal_Ruleset },
            { "Heyworks.PocketShooter.Realtime.Common",        Heyworks_Ruleset },
            { "Heyworks.PocketShooter.Realtime.Server",        Heyworks_Internal_Ruleset },
            { "Microsoft.Extensions.Logging.Unity",            Heyworks_Internal_Ruleset }
        };

        private static List<string> analyzerFolders = new List<string>
        {
            $"CodeAnalysis{DS}StyleCop.Analyzers"
        };

        private static string[] additionalFiles =
        {
            $"..{DS}..{DS}CARuleSets{DS}stylecop.json"
        };

        public static string OnGeneratedCSProject(string path, string contents)
        {
            var projectFileName = Path.GetFileNameWithoutExtension(path);

            if (projectFilesWithRulesets.ContainsKey(projectFileName))
            {
                // parse the document and make some changes
                var document = XDocument.Parse(contents);

                if (document.Root != null)
                {
                    var itemGroups = document.Root.Elements(XName.Get("ItemGroup", xmlns.NamespaceName));

                    var groups = itemGroups.ToList();
                    AddPropertyGroup(groups.First(), "CodeAnalysisRuleSet", new[] { projectFilesWithRulesets[projectFileName] });

                    var analyzerFiles = new List<string>();

                    analyzerFolders.ForEach(folder => analyzerFiles.AddRange(Directory.GetFiles(folder)));

                    AddItemGroup(groups.Last(), "Analyzer", analyzerFiles);
                    AddItemGroup(groups.Last(), "AdditionalFiles", additionalFiles);
                }

                //// save the changes using the Utf8StringWriter
                var str = new Utf8StringWriter();
                document.Save(str);

                return str.ToString();
            }

            return contents;
        }

        private static void AddPropertyGroup(XElement addBefore, string propertyName, IEnumerable<string> propertyValues)
        {
            var propertyGroup = new XElement(XName.Get("PropertyGroup", xmlns.NamespaceName));

            foreach (var propertyValue in propertyValues)
            {
                propertyGroup.Add(new XElement(XName.Get(propertyName, xmlns.NamespaceName), propertyValue));
            }

            addBefore.AddBeforeSelf(propertyGroup);
        }

        private static void AddItemGroup(XElement addAfter, string itemName, IEnumerable<string> includeFiles)
        {
            var itemGroup = new XElement(XName.Get("ItemGroup", xmlns.NamespaceName));

            foreach (var includeFile in includeFiles)
            {
                itemGroup.Add(new XElement(XName.Get(itemName, xmlns.NamespaceName), new XAttribute("Include", includeFile)));
            }

            addAfter.AddAfterSelf(itemGroup);
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}