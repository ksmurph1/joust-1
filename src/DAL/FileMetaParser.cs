using System.Xml;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using System;

namespace DAL
{
    internal struct FileMetaParser
    {
        private static readonly string filename =
                              ConfigurationManager.AppSettings["meta-filename"];

        public static void ParseMetaData()
        {
            XmlReaderSettings options = new XmlReaderSettings
            {
                Async = true,
                CheckCharacters = false,
                CloseInput = true,
                ConformanceLevel = ConformanceLevel.Document,
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true,
                IgnoreComments = true
            };
            using (XmlReader parser = XmlReader.Create(new StreamReader(filename), options))
            {
                parser.MoveToContent();
                parser.ReadStartElement("file-descriptor");
                bool status = parser.ReadToDescendant("column");
                while (status)
                {
                    Task<Tuple<byte, string, Type>> t = new Task<Tuple<byte, string, Type>>(
                        s => ParseColumnData((XmlReader)s), parser.ReadSubtree());
                    t.ContinueWith<Tuple<byte, string, Type>>((ant) => MetaData.Add(ant.Result.Item1,
                                                                  ant.Result.Item2, ant.Result.Item3),
                                     TaskContinuationOptions.NotOnFaulted);
                    t.Start();
                    status = parser.ReadToNextSibling("column");
                }

            }

        }

        private static Tuple<byte, string, Type> ParseColumnData(XmlReader reader)
        {
            reader.Read(); // move to element
            byte index = Convert.ToByte(reader.GetAttribute("id"));
            reader.ReadToFollowing("name");
            string colname = reader.ReadContentAsString();
            reader.ReadToNextSibling("data-type");
            Type t = Type.GetType(reader.ReadContentAsString());
            return new Tuple<byte, string, Type>(index, colname, t);
        }
    }
}