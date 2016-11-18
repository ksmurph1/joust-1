public interface IQuote
{
    float Price { get; }
    string[] RollOrders {get;set;}
}