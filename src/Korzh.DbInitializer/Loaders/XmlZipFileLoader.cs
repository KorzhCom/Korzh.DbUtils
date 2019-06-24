using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Korzh.DbInitializer.Loaders
{
    public class XmlZipFileLoaderException : ZipFileLoaderException
    {
        public XmlZipFileLoaderException() : base()
        {
        }

        public XmlZipFileLoaderException(string message) : base(message)
        {
        }
    }

    public class XmlZipFileLoader: ZipFileLoader
    {

        public XmlZipFileLoader(string fileName) : base(fileName)
        {
        }

        public XmlZipFileLoader(Stream stream) : base(stream)
        {
        }

        public override IEnumerable<IDataItem> LoadTableData(string entityName)
        {
            var entry = ZipArchive.GetEntry(entityName + ".xml") ?? throw new XmlZipFileLoaderException($"File {entityName + ".xml"} is not found");

            using (var streamReader = new StreamReader(entry.Open()))
            using (var xmlReader = new XmlTextReader(streamReader))
            {
                xmlReader.WhitespaceHandling = WhitespaceHandling.None;

                xmlReader.Read();
                xmlReader.Read();
                if (xmlReader.Name.ToLowerInvariant().Equals("root"))
                {
                    while (xmlReader.Read() && xmlReader.Name.ToLowerInvariant().Equals("row"))
                    {
                        var item = new DataItem();
                        XElement element = null;

                        using (XmlReader subXmlReader = xmlReader.ReadSubtree())
                        {
                            element = XElement.Load(subXmlReader);
                        }

                        foreach (var propery in element.Elements())
                        {
                            item.SetProperty(propery.Name.LocalName, propery.Value);
                        }

                        yield return item;

                    }         
                }
                else
                {

                    throw new XmlZipFileLoaderException($"Wrong file fotmat at {xmlReader.LineNumber}:{xmlReader.LinePosition}");
                }

            }

            yield break;
        }
    }
}
