# Joust 1: It's Carpets All the Way Down!
You've been contracted by a commercial carpet installation business to create a service they can plug into their CRM to help generate realtime quotes for sales people in the field. Specifically, your code will use price and item data from multiple vendors to generate a lowest cost quote for the customer.

A salesperson will enter a quantity of carpet required (in sf), a number of rooms, hourly cost of labor, and a carpet grade (1-5) for a particular job. Your service will estimate the cost of required supplies (rolls of carpet) and labor, outputing supplier ordering information as well as a quote for the job.

### Requirements
* Input will be an int[4] containing: [square footage required, number of rooms, hourly labor cost, desired grade of carpet]
* Supplier Inventory Data
  * Data will be provided in CSV format with the following columns:
    * Inventory Id (string)
    * Grade (whole number)
    * Length of roll (whole number)
    * Width of roll (whole number)
    * Unit price (decimal, #.##)
  * Retrieving the CSV files from the vendors is not in scope for this effort. For testing, sample files will be provided in the data directory.
  * Files will be named in the format: [Company Name].[yyyy.mm.dd].csv where [yyyy.mm.dd] is the date the data was published. **Only the most recent data from a particular company should be used.**
* A grade of carpet from one company can be assumed to be equivalent to the same grade from another supplier. One quote might use rolls from several different companies.
* Buying too much carpet is not a problem - all we care about is the lowest cost for the job
* Labor cost should be estimated at 30 minutes per roll installed plus 30 minutes per room for trimming
* Once you have a total cost, add a 40% margin to get a final price

### Scoring Considerations (in order of weight)
 * Service correctly identifies most recent company files and parses appropriate inventory data
 * Service accurately calculates labor costs for a given job and number of carpet rolls
 * Service is fully functional and returns a logically coherent quote
 * Service returns an optimal quote in all circumstances
 * Readability & Style
 * Optimization
 * Input & Edge Case Validation
 * Elegance

### Resources
[See this gist to understand how test data was generated or to make some of your own.](https://gist.github.com/nixondanielj/48e569fd900c0d18c3948d36d7507086)
