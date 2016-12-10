using DAL;
using System;
namespace DL
{
    public interface ICsvReader
    {
        string GetNextLatestCsv();
        
        IDataSpecs? ReadLine(string);

        string GetCompanyName(string);
    }
}