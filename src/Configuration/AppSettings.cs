using System;
using System.Xml;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Configuration
{
    public sealed class AppSettings
    {
        private const string APPFILE = @"C:\Users\kerry\joust-1\App.config";
        private readonly static ReadOnlyDictionary<string,string> settings;
        static AppSettings()
        {
           Dictionary<string,string> newSettings=new Dictionary<string,string>();
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

            using (XmlReader parser = XmlReader.Create(new StreamReader(File.OpenRead(APPFILE)), options))
            {
                parser.MoveToContent();
                parser.ReadToFollowing("appSettings");
                parser.ReadStartElement();

                bool status=parser.Name == "add";
            
                    while (status)
                    {
                        string varName=parser.GetAttribute("key");
                        string varValue=parser.GetAttribute("value");
                        status=parser.ReadToNextSibling("add");
                        if (!String.IsNullOrWhiteSpace(varName))
                        {
                            // if valid name, add to Dictionary
                            newSettings[varName]=varValue;
                        }
                    }

                }
                settings=new ReadOnlyDictionary<string,string>(newSettings);
        }

     
        public string this[string key]
        {
            get 
            {
            string value;
            //try to get value out of dict that matches key
            bool status=settings.TryGetValue(key,out value);
            if (!status)
            {
                return null;
            }
                return value;
            }
        }
    }
}