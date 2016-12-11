using System;
namespace DAO
{
    public interface IInventory
    {
        string CompanyName {get;}
IDataSpecs GetRow<T>(Func<T,bool>);
    }
}