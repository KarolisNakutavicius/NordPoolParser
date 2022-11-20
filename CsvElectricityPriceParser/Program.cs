// See https://aka.ms/new-console-template for more information

using System.Globalization;
using CsvElectricityPriceParser.Models;
using CsvHelper;

Console.WriteLine("Hello, World!");
var path = Path.Combine(GetCurrentDirectory(), "CsvData");

var startingYear = 2015;
var endYear = 2021;


var allYearsList = new List<CorrectedPriceModel>();
var currentYear = 2015;

while (currentYear <= endYear)
{
    using var reader = new StreamReader(Path.Combine(path, $"{currentYear}.csv"));
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    
    var records = csv.GetRecords<PriceModel>().ToList();

    foreach (var record in records)
    {
        var dateTime =
            DateTime.TryParseExact(
                record.DateRange.Split('-')[0], "dd.MM.yyyy HH:mm ", null, DateTimeStyles.None,
                out var dt)
                ? dt
                : DateTime.MinValue;
        var price = float.TryParse(record.Price, out var correctFormat) ? correctFormat : 0;

        if (price == 0 || dateTime == DateTime.MinValue)
        {
            continue;
        }
        
        allYearsList.Add(new CorrectedPriceModel()
        {
            DateTime = dt,
            Value = price
        });
    }

    currentYear++;
}

await using var writer = new StreamWriter(Path.Combine(path, $"{startingYear} - {endYear} MERGED.csv"));
await using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
csvWriter.WriteRecords(allYearsList.OrderBy(c => c.DateTime));





string GetCurrentDirectory() => Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;;