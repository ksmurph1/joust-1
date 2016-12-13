using System;
namespace DAO
{
    public interface IInventory
    {
        string CompanyName {get;}
        void FillInventory(out Util.ValueReturnObj<object>);
IDataSpecs GetRow<T>(Func<T,bool>);
    }
}