using Util;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Supplier
{
    public class AllSupplierView
    {
        private readonly Stack<IInventory> suppliers = new Stack<IInventory>();
        private bool exceptional=false;
        private IList<IInventory> readOnlyCollection;
        public AllSupplierView(out IValueReturnObj<object> statusObj)
        {
            bool status = true;
            statusObj = new ValueReturnObj<object>
            {
                HasVal = true
            };
            /*System.Threading.CancellationTokenSource cToken = new System.Threading.CancellationTokenSource();
            Stack<Task> tasks = new Stack<Task>();*/
            while (status)
            {
                try
                {
                    // fill inventory of company
                    IValueReturnObj<IInventory> result=CompanyInventory.FillInventory();
                         if (result.HasVal)
                         {
                        // if no errors from filling
                        suppliers.Push(result.Value);
                         }
                         else
                         {
                             if (result.Exception != null)
                             {
                              // exception occurred
                                throw result.Exception;
                             }
                         }
                }
                catch (Exception ex)
                {
                    exceptional=true;
                    status = false; // terminate loop
                // store exception if not empty
                  statusObj.Exception = new Exception("AllSupplierView(): "+ex.Message);
                }
            }
        }

        public IValueReturnObj<IList<IInventory>> Suppliers
        {
            get
            {
                IValueReturnObj<IList<IInventory>> statusObj = new ValueReturnObj<IList<IInventory>>(); 
                if (!exceptional && readOnlyCollection == null)
                {
                    // if read only collection does not exist, create it once
                    readOnlyCollection= new
                     System.Collections.ObjectModel.ReadOnlyCollection<IInventory>(suppliers.ToArray());
                    statusObj.Value = readOnlyCollection;
                }
                else if (exceptional)
                {
                    statusObj.Exception = new Exception("Suppliers.get(): " +
                        "There was a problem getting a view, thus no suppliers could be provided");
                }
                else
                {
                    statusObj.Value = readOnlyCollection;
                }
                return statusObj;
            }
        }
    }
}
