using DataObject;
using System;
using Util;
namespace DataLayer
{
    public interface ICsvReader
    {
        
        IValueReturnObj<IDataSpecs[]> ReadLines();

        IValueReturnObj<string> GetCompanyName();
    }
}