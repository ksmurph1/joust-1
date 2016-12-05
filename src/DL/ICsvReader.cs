namespace DL
{
    public interface ICsvReader
    {
        string GetNextLatestCsv();
        
        IDataTable ParseFile(string);
    }
}