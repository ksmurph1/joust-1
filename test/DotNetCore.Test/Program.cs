﻿using System;
namespace DotNetCore.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Joust1_ShouldReturnAccurateQuotes test=new Joust1_ShouldReturnAccurateQuotes();
      //      test.AccurateForSingleCarpet();
            test.AccurateForMoreCarpets();
            test.ReturnsNullIfNoneAvalibleInATimelyManner();
            Console.ReadKey();
        }
    }
}
