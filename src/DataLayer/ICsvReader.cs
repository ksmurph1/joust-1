using DataObject;
using System;
using Util;
namespace DataLayer
{
    public interface ICsvReader
    {
        bool IsDone { get; }
        IValueReturnObj<IDataSpecs[]> ReadLines();

        IValueReturnObj<string> GetCompanyName();
    }
}