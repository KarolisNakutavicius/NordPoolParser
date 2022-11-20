using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using NordPoolParser.Models;

var client = new HttpClient();

client.BaseAddress = new Uri("https://www.nordpoolgroup.com");

await CreateTrainingCsv();
//await CreateTestCsv();

Console.WriteLine();


async Task CreateTestCsv()
{
    Console.WriteLine("CREATING TEST DATA");
    
    var startingDate = new DateTime(2022, 09, 1);
    var endDate = DateTime.Now;

    await CreateCsv(startingDate, endDate, $"{Directory.GetCurrentDirectory()}\\testData.csv");
}

async Task CreateTrainingCsv()
{
    Console.WriteLine("CREATING TRAINING DATA");
    
    var startingDate = new DateTime(2022, 1, 1);
    var endDate = new DateTime(2022, 11, 1);

    await CreateCsv(startingDate, endDate, $"{Directory.GetCurrentDirectory()}\\trainingData.csv");
}

async Task CreateCsv(DateTime startingDate, DateTime endDate, string path)
{
    startingDate = startingDate.AddDays(-1);
    var csvList = new List<CsvDataModel>();

    while (startingDate.Date <= endDate.Date)
    {
        startingDate = startingDate.AddDays(8);
        var result = await client.GetAsync($"/api/marketdata/page/53?currency=,EUR,EUR,EUR&endDate={startingDate:dd-MM-yyyy}");

        if (!result.IsSuccessStatusCode)
        {
            Debugger.Break();
        }
        
        var responseMessage = await result.Content.ReadAsStringAsync();
        var response = Newtonsoft.Json.JsonConvert.DeserializeObject<NordPoolResponse>(responseMessage);

        var rows = response!.Data.Rows.Take(24);

        foreach (var row in rows)
        {
            foreach (var column in row.Columns)
            {
                var date = DateTime.ParseExact(column.Name, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                date = date.AddHours(row.EndTime.Hour);
                
                if(!float.TryParse(column.Value.Replace(',', '.'), out float value))
                {
                    Debugger.Break();
                    
                    continue;
                }
                
                csvList.Add(new CsvDataModel
                {
                    DateTime = date,
                    Value = value
                });
            }
        }
        
        Console.WriteLine($"Progress {(float)startingDate.DayOfYear / endDate.DayOfYear * 100} %");
    }
    
    await using var writer = new StreamWriter(path);
    await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
    csv.WriteRecords(csvList.OrderBy(c => c.DateTime));
}
