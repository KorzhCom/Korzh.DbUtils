using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Korzh.DbInitializer
{

    public class JsonZipFileLoaderException : Exception
    {
        public JsonZipFileLoaderException(): base()
        {
        }

        public JsonZipFileLoaderException(string message) : base(message)
        {
        }
    }

    public class JsonZipFileLoader : ZipFileLoader
    {

        public JsonZipFileLoader(string fileName) : base(fileName)
        {

        }

        public JsonZipFileLoader(Stream stream) : base(stream)
        {

        }

        public override IEnumerable<IDataItem> LoadEntityData(string entityName)
        {
            var entry = ZipArchive.GetEntry(entityName + ".json") ?? throw new JsonZipFileLoaderException($"File {entityName + ".json"} is not found");

            using (var streamReader = new StreamReader(entry.Open()))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                jsonReader.Read();
                if (jsonReader.TokenType == JsonToken.StartArray)
                {
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                    {
                        var item = new DataItem();
                        var data = JObject.Load(jsonReader);

                        foreach (var property in data.Properties())
                        {
                            item.SetProperty(property.Name, property.Value.ToObject<string>());
                        }
                   
                        yield return item;
                    }
                }
                else
                {
                    throw new JsonZipFileLoaderException($"Wrong file fotmat at {jsonReader.LineNumber}:{jsonReader.LinePosition}");
                }  
            }

            yield break;
        }

        public override IEnumerable<object> LoadEntityData(string entityName, Type entityType)
        {
            var entry = ZipArchive.GetEntry(entityName + ".json") ?? throw new JsonZipFileLoaderException($"File {entityName + ".json"} is not found");

            using (var streamReader = new StreamReader(entry.Open()))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                jsonReader.Read();
                if (jsonReader.TokenType == JsonToken.StartArray)
                {

                    var colProps = GetColumnProperies(entityType);
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                    {

                        var item = Activator.CreateInstance(entityType);
                        var data = JObject.Load(jsonReader);

                        foreach (var property in data.Properties())
                        {
                            if (colProps.TryGetValue(property.Name, out var entPropName))
                            {
                                var entProperty = entityType.GetProperty(entPropName);
                                if (entProperty != null && entProperty.CanWrite)
                                {
                                    entProperty.SetValue(item, property.Value.ToObject(entProperty.PropertyType));
                                }

                            }
                        }

                        yield return item;
                    }
                }
                else
                {
                    throw new JsonZipFileLoaderException($"Wrong file fotmat at {jsonReader.LineNumber}:{jsonReader.LinePosition}");
                }
            }

            yield break;
        }
    }
}
