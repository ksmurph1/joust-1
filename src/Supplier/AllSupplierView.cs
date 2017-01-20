using Util;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Supplier
{
    public class AllSupplierView
    {
        private IList<IInventory> readOnlyCollection;
        public AllSupplierView(out IValueReturnObj<object> statusObj)
        {
            Stack<IInventory> suppliers = new Stack<IInventory>();
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
                    suppliers.Clear();
                    status = false; // terminate loop
                // store exception if not empty
                  statusObj.Exception = new Exception("AllSupplierView(): "+ex.Message);
                }
            }
            readOnlyCollection= new
                     System.Collections.ObjectModel.ReadOnlyCollection<IInventory>(suppliers.ToArray());
        }

        public IValueReturnObj<IList<IInventory>> Suppliers
        {
            get
            {
                IValueReturnObj<IList<IInventory>> statusObj = new ValueReturnObj<IList<IInventory>>(); 
                if (readOnlyCollection.Count > 0)
                {
                    // if we have some elements store it
                    statusObj.Value = readOnlyCollection;
                }
                else
                {
                    statusObj.Exception = new Exception("Suppliers.get(): " +
                        "There was a problem getting a view, thus no suppliers could be provided");
                }
                return statusObj;
            }
        }
    }
}
