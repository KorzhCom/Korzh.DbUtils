using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Korzh.DbUtils.Loaders
{

    public class JsonZipFileLoaderException : ZipFileLoaderException
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

        public override IEnumerable<IDataRecord> LoadTableData(string tableName)
        {
            var entry = ZipArchive.GetEntry(tableName + ".json") ?? throw new JsonZipFileLoaderException($"File {tableName + ".json"} is not found");

            using (var streamReader = new StreamReader(entry.Open()))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                jsonReader.Read();
                if (jsonReader.TokenType == JsonToken.StartArray)
                {
                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                    {
                        var item = new DataRecord();
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
    }
}
