using DAL;
using System;
using System.IO;
using Util;
namespace DL
{
    public interface ICsvReader
    {
        IValueReturnObj<FileInfo> GetNextLatestCsv();
        
        IValueReturnObje<Nullable<IDataSpecs>> ReadLine(FileInfo);

        IValueReturnObj<string> GetCompanyName(FileInfo);
    }
}