using System.Xml;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Util;

namespace Descriptor
{
    internal struct FileMetaParser
    {
        private static readonly string filename =
                              Utilities.AppSettings["meta-filename"];

        public static async void  ParseMetaData()
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

            using (XmlReader parser = XmlReader.Create(new StreamReader(File.OpenRead(filename)), options))
            {
                parser.MoveToContent();
                parser.ReadStartElement("file-descriptor");

                bool status = parser.Name == "column";
               
            
                    Stack<Task> tasks = new Stack<Task>();
                    while (status)
                    {
                        Task<Tuple<byte, string, Type>> task = Task.Factory.StartNew(reader => ParseColumnData((XmlReader)reader),
                            XmlReader.Create(new MemoryStream(System.Text.Encoding.ASCII.GetBytes(parser.ReadOuterXml())),
                            fragOptions), TaskCreationOptions.PreferFairness);
                        tasks.Push(task.ContinueWith(t => MetaData.Add(t.Result.Item1,t.Result.Item2, t.Result.Item3), 
                                                      TaskContinuationOptions.NotOnFaulted));

                        status = parser.Name == "column" && parser.IsStartElement();
                    }

                // wait asyncronously and set flag when finished
                await Task.WhenAll(tasks.ToArray()).ContinueWith(t=>MetaData.IsCompleted=true);
            }
           
        }

        private static Tuple<byte, string, Type> ParseColumnData(XmlReader reader)
        {
            reader.Read(); // move to element
            byte index = Convert.ToByte(reader.GetAttribute("id"));
            reader.ReadToDescendant("name");
            string colname = (string)reader.ReadElementContentAs(typeof(string),null);
            if (reader.Name != "data-type")
            {
                throw new Exception("ParseColumnData: Found " + reader.Name+" instead of data-type in xml");
            }
            Type t = Type.GetType((string)reader.ReadElementContentAs(typeof(string),null));
            return new Tuple<byte,string, Type>(index,colname, t);
        }
    }
}