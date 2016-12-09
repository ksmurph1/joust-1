using System.Xml;
using System.IO;
using System.Configuration;
using System.Threading.Task;
using System.Generic.Collections;

namespace DAL
{
    internal static struct FileMetaParser
    {
        private const string filename=
                              ConfigurationManager.AppSettings["meta-filename"];
        
        public static void ParseMetaData()
        {
            XmlReaderSettings options=new XmlReaderSettings
            {
                Async=true,
                CheckCharacters=false,
                CloseInput=true,           
                ConformanceLevel=ConformanceLevel.Document,
                DtdProcessing=DtdProcessing.Ignore,
                IgnoreWhitespace=true,
                IgnoreProcessingInstructions=true,                
                IgnoreComments=true
            };
            using (XmlReader parser=new XmlReader(new StreamReader(filename),options))
            {
                parser.MoveToContent();
                parser.ReadStartElement("file-descriptor");
                bool status=parser.ReadToDescendant("column");
            while (status)
            {
                Task<Tuple<byte,string,Type>> t=new Task<Tuple<byte,string,Type>>(ParseColumnData, parser.ReadSubTree());
                t.ContinueWith((ant)=>MetaData.Add(ant.Item1,ant.Item2, ant.Item3),
                                 TaskContinuationOptions.RunContinuationsAsynchronously);
                t.Start();
                status=parser.ReadToNextSibling("column");
            } 
                
            }
                  
        }

        private static Tuple<byte,string,Type> ParseColumnData(XmlReader reader)
        {
                   reader.Read(); // move to element
                   byte index=Convert.ToByte(reader.GetAttribute("id"));
                   reader.ReadToFollowing("name");
                   string colname=reader.ReadContentAsString();
                   reader.ReadToNextSibling("data-type");
                   Type t=Type.GetType(reader.ReadContentAsString());
                   return new Tuple<byte,string,Type>(index,colname,t);
        }
    }
}