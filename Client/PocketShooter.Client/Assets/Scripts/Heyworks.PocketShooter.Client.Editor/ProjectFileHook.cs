using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;

namespace Heyworks.PocketShooter
{
    public class ProjectFileHook : AssetPostprocessor
    {
        public static string OnGeneratedCSProject(string path, string contents)
        {
            // parse the document and make some changes
            var document = XDocument.Parse(contents);
            if (document.Root != null)
            {
                XNamespace ns = document.Root.Name.Namespace;

                foreach (XElement xe in document.Root
                    .Descendants()
                    .Where(x => x.Name.LocalName == "PropertyGroup")
                    .Descendants()
                    .Where(x => x.Name == ns + "NoWarn"))
                {
                    xe.Value = xe.Value + ";0649";
                }
            }

            // save the changes using the Utf8StringWriter
            var str = new Utf8StringWriter();
            document.Save(str);

            return str.ToString();
        }

        // necessary for XLinq to save the xml project file in utf8
        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}