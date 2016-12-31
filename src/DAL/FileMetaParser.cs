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

        public static void  ParseMetaData()
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

            
            using (XmlReader parser = XmlReader.Create(new StreamReader(filename), options))
            {
                parser.MoveToContent();
                parser.ReadStartElement("file-descriptor");

                bool status = parser.Name == "column";
               
                System.Threading.ThreadPool.QueueUserWorkItem((p) =>
                {
                    Stack<Task> tasks = new Stack<Task>();
                    while (status)
                    {
                        Task<Tuple<string, Type>> task = Task.Factory.StartNew(reader => ParseColumnData((XmlReader)reader),
                            XmlReader.Create(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(((XmlReader)p).ReadOuterXml())),
                            fragOptions), TaskCreationOptions.PreferFairness);
                        tasks.Push(task.ContinueWith(t => MetaData.Add(t.Result.Item1, t.Result.Item2), TaskContinuationOptions.NotOnFaulted));

                        status = ((XmlReader)p).Name == "column" && ((XmlReader)p).IsStartElement();
                    }
                    Task.WaitAll(tasks.ToArray());
                    MetaData.IsCompleted = true;
                },parser);
               
            }
            // wait until all tasks finish
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