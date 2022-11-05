using NordPoolParser.Models;

var client = new HttpClient();

client.BaseAddress = new Uri("https://www.nordpoolgroup.com");


var dateTimeToSelect = new DateTime(2022, 10, 05);

var result = await client.GetAsync($"/api/marketdata/page/53?currency=,EUR,EUR,EUR&endDate={dateTimeToSelect:dd-MM/yyyy}");

result.EnsureSuccessStatusCode();
var responseMessage = await result.Content.ReadAsStringAsync();

var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<NordPoolResponse>(responseMessage);

Console.WriteLine();



