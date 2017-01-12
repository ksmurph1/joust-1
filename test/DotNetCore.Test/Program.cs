using System;
using System.Configuration;
namespace DotNetCore.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Joust1_ShouldReturnAccurateQuotes test=new Joust1_ShouldReturnAccurateQuotes();
            test.AccurateForSingleCarpet();
            Console.ReadKey();
        }
    }
}
