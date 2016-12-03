using System.Xml;
using System.IO;
using System.Configuration

namespace DAL
{
    internal static struct FileMetaParser
    {
        private const string filename=
                              ConfigurationManager.AppSettings["MetaFileName"];
        
        internal static void ParseMetaData()
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
                
            }
                  
        }
    }
}