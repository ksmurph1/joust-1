using System.Xml;
using System.IO;
using System.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Descriptor
{
    internal struct FileMetaParser
    {
        private static readonly string filename =
                              ConfigurationManager.AppSettings["meta-filename"];

        public static void ParseMetaData()
        {
            XmlReaderSettings options = new XmlReaderSettings
            {
                Async = false,
                CheckCharacters = false,
                CloseInput = true,
                ConformanceLevel = ConformanceLevel.Document,
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true,
                IgnoreComments = true
            };
            XmlReaderSettings fragOptions = new XmlReaderSettings
            {
                Async = false,
                CheckCharacters = false,
                CloseInput = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreWhitespace = true,
                IgnoreProcessingInstructions = true,
                IgnoreComments = true
            };

            Stack<Task> tasks = new Stack<Task>();
            using (XmlReader parser = XmlReader.Create(new StreamReader(filename), options))
            {
                parser.MoveToContent();
                parser.ReadStartElement("file-descriptor");

                bool status = parser.Name == "column";
                while (status)
                {
                    Task<Tuple<string, Type>> task=Task.Factory.StartNew(reader=>ParseColumnData((XmlReader)reader),
                        XmlReader.Create(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(parser.ReadOuterXml())),
                        fragOptions),TaskCreationOptions.PreferFairness);
                    tasks.Push(task.ContinueWith(t=>MetaData.Add(t.Result.Item1, t.Result.Item2),TaskContinuationOptions.NotOnFaulted|
                        TaskContinuationOptions.ExecuteSynchronously));

                    status = parser.Name == "column" && parser.IsStartElement();
                }
            }
            // wait until all tasks finish
            Task.WaitAll(tasks.ToArray(),1);

        }

        private static Tuple<string, Type> ParseColumnData(XmlReader reader)
        {
            reader.Read(); // move to element
            byte index = Convert.ToByte(reader.GetAttribute("id"));
            reader.ReadToDescendant("name");
            string colname = (string)reader.ReadElementContentAs(typeof(string),null);
            if (reader.Name != "data-type")
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name +
                                     ": " + " Found " + reader.Name+" instead of data-type in xml");
            }
            Type t = Type.GetType((string)reader.ReadElementContentAs(typeof(string),null));
            return new Tuple<string, Type>(colname, t);
        }
    }
}