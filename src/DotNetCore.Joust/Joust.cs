using System.Collections.Generic;
using Interfaces.Joust;
using Util;
using DAO;
namespace Joust
{
    public sealed class LowestCostCalculator : IJoust
    {
        private List<IInventory> inventory = new List<IInventory>();
        //0,sqftreq 1,roomcount 2,hourlylabor 3,desiredgrade
         private enum InputNames: byte {SQFTREQ=0, ROOMCNT, HRLYLABOR, DESGRADE};

         public LowestCostCalculator()
         {
             ValueReturnObj<IInventory> status=CompanyInventory.CreateInstance();
             while(status.Value != null && status.Exception == null)
             {
                inventory.Add(status.Value);
                status=CompanyInventory.CreateInstance();
             } 
             if (status.Exception != null)
             {
                 throw status.Exception;
             }

         }
         public IQuote GetQuote(int[] input)
         {
                input[(byte)InputNames.DESGRADE]
         }
    }

}