using CsvHelper.Configuration.Attributes;

namespace CsvElectricityPriceParser.Models;

public class PriceModel
{
    [Index(0)]
    public string DateRange { get; set; }
    
    [Index(1)]
    public string Price { get; set; }
}