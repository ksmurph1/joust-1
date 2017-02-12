
using Xunit;
using Interfaces.Joust;
namespace DotNetCore.Test
{
    public class Joust1_ShouldReturnAccurateQuotes
    {
        private readonly IJoust _joust;
        public Joust1_ShouldReturnAccurateQuotes()
        {
            //TO-DO: set _joust equal to an instance of your IJoust implementation
            _joust = new DotNetCore.Joust.LowestCostCalculator();
        }

        [Fact]
        public void AccurateForSingleCarpet()
        {
            //e181f408-3f56-40d8-9149-d12a06aeea18	9	20	70	1400	$4,251.46
            //grade 9 
            //1 room, 1 carpet, at 100 per hour of labor should be 100m labor
            float expectedPriceResult = (float)((4251.46m + 100m) * 1.4m);
            string[] expectedCarpetIdArray = { "e181f408-3f56-40d8-9149-d12a06aeea18" };

            IQuote lowestQuote = _joust.GetQuote(new int[] { 1000, 1, 100, 9 });
            Assert.Equal(4251.46f, lowestQuote.MaterialCost);
            Assert.Equal(100f, lowestQuote.LaborCost);
            Assert.Equal(expectedPriceResult, lowestQuote.Price);
            Assert.Equal(expectedCarpetIdArray, lowestQuote.RollOrders);
        }

        [Fact]
        public void AccurateForMoreCarpets()
        {
            //6b746c7d - 754d - 4049 - 9c5c - f2abfe73f9d3 	  9	  80  25  2000    $6,668.67
            //67b751bb - de92 - 4d4d - 8e33 - 13b535fc9bbd    9   25  75  1875    $5,388.74
            //de4d199e - ce50 - 47e9 - 993f - ffb47d071db2    9   90  20  1800    $4,672.48
            //e181f408 - 3f56 - 40d8 - 9149 - d12a06aeea18    9   20  70  1400    $4,251.46
            //grade 9 
            float expectedPriceResult = (float)((6668.67m + 5388.74m + 4672.48m + 4251.46m) * 1.4m);
            string[] expectedCarpetIdArray =
                {
                    "6b746c7d-754d-4049-9c5c-f2abfe73f9d3",
                    "67b751bb-de92-4d4d-8e33-13b535fc9bbd",
                    "de4d199e-ce50-47e9-993f-ffb47d071db2",
                    "e181f408-3f56-40d8-9149-d12a06aeea18"
                };
            IQuote lowestQuote = _joust.GetQuote(new int[] { 7000, 0, 0, 9 });
            Assert.Equal(expectedPriceResult, lowestQuote.Price);
            Assert.Equal(expectedCarpetIdArray.Length, lowestQuote.RollOrders.Length);
            Assert.Contains<string>(expectedCarpetIdArray[0], lowestQuote.RollOrders);
            Assert.Contains<string>(expectedCarpetIdArray[1], lowestQuote.RollOrders);
            Assert.Contains<string>(expectedCarpetIdArray[2], lowestQuote.RollOrders);
            Assert.Contains<string>(expectedCarpetIdArray[3], lowestQuote.RollOrders);
        }

        [Fact]
        public void ReturnsNullIfNoneAvalibleInATimelyManner()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            IJoust speedTest = new DotNetCore.Joust.LowestCostCalculator();
            //TO-DO: set speedTest equal to an instance of your IJoust implementation
            IQuote lowestQuote = speedTest.GetQuote(new int[] { int.MaxValue, 100, 100, 1 });
            timer.Stop();
            Assert.True(timer.Elapsed.TotalMinutes < 1);
            Assert.Null(lowestQuote);
        }
    }
}
