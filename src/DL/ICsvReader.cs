using DataObject;
using System;
using Util;
namespace DataLayer
{
    public interface ICsvReader
    {
        
        IValueReturnObj<Nullable<DataSpec>>[] ReadLines();

        IValueReturnObj<string> GetCompanyName();
    }
}