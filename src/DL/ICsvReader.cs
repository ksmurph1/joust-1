using DAL;
using System;
using System.IO;
using Util;
namespace DL
{
    public interface ICsvReader
    {
        
        IValueReturnObje<Nullable<IDataSpecs>>[] ReadLine();

        IValueReturnObj<string> GetCompanyName();
    }
}