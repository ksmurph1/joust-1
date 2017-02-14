using Supplier;
using Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Joust
{
    internal class Qoute : Interfaces.Joust.IQuote
    {
        private readonly int roomCnt;
        private readonly int hrRate;
        private readonly Guid[] orderIds;
        private readonly AllSupplierView allSuppliers;
        private const decimal MARGIN = .4m;
        private const float SPLITRATE = .5f;
        internal Qoute(int hourlyRate, int roomCnt, Guid[] orderIds)
        {
            this.roomCnt = roomCnt;
            this.orderIds = orderIds;
            this.hrRate = hourlyRate;
            IValueReturnObj<object> statusObj;
            // get view that consolidates all inventory in every company
            allSuppliers = new AllSupplierView(out statusObj);
            // if exception then throw it
            if (statusObj.Exception != null)
            {
                throw statusObj.Exception;
            }
        }

        // Total price including material cost, labor cost, and margin
        public float Price
        {
           get { return (float)((decimal)(MaterialCost + LaborCost) * (MARGIN+1));}
        }

        // Cost of all carpet orders from suppliers
        public float MaterialCost
        {
           
            get
            {
                decimal total=0m;
                byte count=0;
                System.Collections.Concurrent.ConcurrentDictionary<byte,Guid> orderIds=
                new System.Collections.Concurrent.ConcurrentDictionary<byte,Guid>(this.orderIds.ToDictionary(k=>count++,g=>g));
               Parallel.ForEach(allSuppliers.Suppliers.Value,()=>total,(supplier,pls,sum)=>
               {
                   foreach (System.Collections.Generic.KeyValuePair<byte,Guid> kvp in orderIds)
                   {
                       // get inventory by id
                       IValueReturnObj<DataObject.IDataSpecs> statusObj=supplier.GetRow(kvp.Value);
                       if (statusObj.Exception==null)
                       {
                               // add price to total-price could be 0
                           sum += statusObj.Value.Price;
                           Guid value;
                           // remove key, value from dictionary since already found
                           orderIds.TryRemove(kvp.Key,out value);
                       }
                      
                   }
                   return sum;
               },sum=>
               {
                   lock (this.orderIds)
                   {
                      total+=sum;
                   }
               });
                return (float)total;
            }
        }

        // Total cost of installation labor
        public float LaborCost
        {
            get
            {
                // 30 minutes per roll installed plus 30 minutes per room for trimming
                return (float)((decimal)SPLITRATE*hrRate*(roomCnt + orderIds.Length));
            }
        }

        // Inventory IDs of all rolls of carpet to be purchased
        public string[] RollOrders
        {
          get
            {
                return orderIds.Select(g=>g.ToString()).ToArray();
            }
        }
    }

}

