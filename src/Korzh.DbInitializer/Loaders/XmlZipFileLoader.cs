using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace Korzh.DbInitializer.Loaders
{
    public class XmlZipFileLoaderException : Exception
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

        public override IEnumerable<IDataItem> LoadEntityData(string entityName)
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

        public override IEnumerable<object> LoadEntityData(string entityName, Type entityType)
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

                    var colProps = GetColumnProperies(entityType);
                    while (xmlReader.Read() && xmlReader.Name.ToLowerInvariant().Equals("row"))
                    {

                        var item = Activator.CreateInstance(entityType);

                        XElement element = null;

                        using (XmlReader subXmlReader = xmlReader.ReadSubtree())
                        {
                            element = XElement.Load(subXmlReader);
                        }

                        foreach (var propElement in element.Elements())
                        {
                            if (colProps.TryGetValue(propElement.Name.LocalName, out var entPropName))
                            {
                                var entProperty = entityType.GetProperty(entPropName);
                                if (entProperty != null && entProperty.CanWrite)
                                {
                                    entProperty.SetValue(item, propElement.ToObject(entProperty.PropertyType));
                                }
                            }
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

    internal static class XElementExtentions
    {


        public static T ToObject<T>(this XElement element)
        {
            return (T)element.ToObject(typeof(T));
        }

        public static object ToObject(this XElement element, Type type)
        {
            if (type == typeof(string))
            {
                return element.Value;
            }

            if (string.IsNullOrEmpty(element.Value))
            {
                return GetDefaultValue(type);
            }

            if (type == typeof(int) ||
               type == typeof(int?))
            {
                return ValueAsInt(element);
            }

            if (type == typeof(short) ||
            type == typeof(short?))
            {
                return ValueAsInt(element);
            }

            if (type == typeof(bool) ||
            type == typeof(bool?))
            {
                return ValueAsBool(element);
            }

            if (type == typeof(float) ||
            type == typeof(float?))
            {
                return ValueAsFloat(element);
            }
            
            if (type == typeof(double) ||
            type == typeof(double?))
            {
                return ValueAsDouble(element);
            }

            if (type == typeof(decimal) ||
            type == typeof(decimal?))
            {
                return ValueAsDecimal(element);
            }

            if (type == typeof(DateTime) ||
            type == typeof(DateTime?))
            {
                return ValueAsDateTime(element);
            }

            if (type == typeof(DateTimeOffset) ||
            type == typeof(DateTimeOffset?))
            {
                return ValueAsDateTimeOffset(element);
            }

            if (type == typeof(TimeSpan) ||
            type == typeof(TimeSpan?))
            {
                return ValueAsTimeSpan(element);
            }

            return null;
        }

        private static object GetDefaultValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Expression<Func<object>> e = Expression.Lambda<Func<object>>(
                Expression.Convert(
                    Expression.Default(type), typeof(object)
                )
            );

            return e.Compile()();
        }

        private static int ValueAsInt(XElement element)
        {
            int result = 0;
            int.TryParse(element.Value, out result);
            return result;
        }

        private static bool ValueAsBool(XElement element)
        {
            bool result = false;
            bool.TryParse(element.Value, out result);
            return result;
        }

        private static decimal ValueAsDecimal(XElement element)
        {
            return decimal.Parse(element.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static short ValueAsShort(XElement element)
        {
            return short.Parse(element.Value);
        }

        private static float ValueAsFloat(XElement element)
        {
            return float.Parse(element.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static double ValueAsDouble(XElement element)
        {
            return double.Parse(element.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static DateTime ValueAsDateTime(XElement element)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(element.Value, out result);
            return result;
        }

        private static TimeSpan ValueAsTimeSpan(XElement element)
        {
            TimeSpan result = TimeSpan.MinValue;
            TimeSpan.TryParse(element.Value, out result);
            return result;
        }

        private static DateTimeOffset ValueAsDateTimeOffset(XElement element)
        {
            DateTimeOffset result = DateTimeOffset.MinValue;
            DateTimeOffset.TryParse(element.Value, out result);
            return result;
        }
    }
}
