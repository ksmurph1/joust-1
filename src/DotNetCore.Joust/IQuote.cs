namespace Interfaces.Joust
{
    public interface IQuote
    {
        // Total price including material cost, labor cost, and margin
        float Price { get; }

        // Cost of all carpet orders from suppliers
        float MaterialCost {get;}

        // Total cost of installation labor
        float LaborCost {get;}

        // Inventory IDs of all rolls of carpet to be purchased
        string[] RollOrders {get;set;}
    }
}
