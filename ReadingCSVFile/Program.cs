using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

class Program
{
    private static readonly HttpClient client = new HttpClient(); // Global to be used by everybody
    private static readonly string uri = "http://localhost:5000/"; // Default uri

    static async Task Main(string[] args)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.BaseAddress = new Uri(uri);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

        List<string> NewLineList = new List<string>(); // Store new line read
        List<string> ServeDataList = new List<string>(); // Serve searched list to API for client to retrieve GET

        Console.WriteLine("Welcome to Simple Read and Serve API");

        string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string file_path = dir + @"\CsvFile\cameras-defb.csv";
        StreamReader streamReader = new StreamReader(file_path);
        string currentLine = streamReader.ReadLine();

        while ((currentLine = streamReader.ReadLine()) != null)
        {
            if (currentLine.StartsWith("UTR-CM")) // First check if line starts with UTR-CM else skip
            {
                // Get first occurence of line number and save in a temp string to be used later 
                string lineNumber = Regex.Match(currentLine, @"\d+").Value;

                // Read line and convert each occurence of semi-colon (;) to Pipe (|) and save in a Temp variable
                var temp = new Regex(Regex.Escape(";")).Replace(currentLine, " | ");

                // Add all read line to a new list make sure that the number is copied into the beginning of newly added list
                NewLineList.Add(lineNumber + " | " + temp);
            }
        }

        Console.WriteLine("CSV file preloaded, please search for a name");
        string searchText = Console.ReadLine();
        Console.WriteLine("Searching for " +  "'"+ searchText+"'");

        foreach (var line in NewLineList)
        {
            if (line.Contains(searchText))
            {
                ServeDataList.Add(line);
                Console.WriteLine(line);
            }
        }

        await ProcessAndServerData(client, ServeDataList);
        static async Task ProcessAndServerData(HttpClient client, List<string> serveDataList)
        {
            var response = await client.PostAsJsonAsync(uri + "api/cameras", serveDataList); // Serve result as Json
            if (response.IsSuccessStatusCode)
            {
                string resultContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\n" + resultContent);
            }
        }
        Console.ReadLine();
    }
}