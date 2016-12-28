using Util;
using System.Collections.Generic;
namespace Supplier
{
    public class AllSupplierView
    {
        private readonly List<IInventory> suppliers = new List<IInventory>();
        public AllSupplierView()
        {
            bool status = true;

            IValueReturnObj<IInventory> statusObj= new ValueReturnObj<IInventory>();
            while (status)
            {
                // fill inventory of company
                statusObj=CompanyInventory.FillInventory();
                if (statusObj.Exception == null)
                {
                    // if no errors from filling
                    suppliers.Add(statusObj.Value);
                }
                else
                {
                    // otherwise set loop condition to fail
                    status = false;
                }
            }
            if (statusObj.Exception != null)
            {
                throw statusObj.Exception;
            }
        }

        public IList<IInventory> Suppliers
        {
            get { return suppliers.AsReadOnly(); }
        }
    }
}
