using System.Collections.Generic;
using Interfaces.Joust;
using Util;
using DAO;
namespace Joust
{
    public sealed class LowestCostCalculator : IJoust
    {
        private List<IInventory> suppliers = new List<IInventory>();
        //0,sqftreq 1,roomcount 2,hourlylabor 3,desiredgrade
         private enum InputNames: byte {SQFTREQ=0, ROOMCNT, HRLYLABOR, GRADE};

         public LowestCostCalculator()
         {
             ValueReturnObj<IInventory> status=CompanyInventory.CreateInstance();
             while(status.Value != null && status.Exception == null)
             {
                suppliers.Add(status.Value);
                status=CompanyInventory.CreateInstance();
             } 
             if (status.Exception != null)
             {
                 throw status.Exception;
             }

         }
         public IQuote GetQuote(int[] input)
         {
             foreach (IInventory supplier in suppliers)
             {
                supplier.ApplyOperation(d=>d.Price/(double)(d.Width*d.Length),
                d=>d.Grade >= input[(byte)InputNames.GRADE]);
             }
         }
    }

}