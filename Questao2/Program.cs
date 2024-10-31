using Newtonsoft.Json;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        Func<string, int, JsonData?> GetData = (teamPos, page) => {
            var urlBase = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{teamPos}={team}&page={page}";
            var apiClient = new HttpClient();
            var request = apiClient.GetAsync(urlBase);
            request.Wait();
            var response = request.Result.Content.ReadAsStringAsync().Result;
            var resData = JsonConvert.DeserializeObject<JsonData>(response);
            return resData;
        };

        Func<string, int> GetGols = (teamPos) =>
        {
            var GolsTime = GetData(teamPos, 1);
            var Total_pages = GolsTime?.Total_pages ?? 0;
            int CountGolsTime = GolsTime?.Data?.Select(t=> new { Gols = teamPos == "team1" ? t.Team1goals : t.Team2goals }).Sum(t => Convert.ToInt32(t.Gols)) ?? 0;
            for (int i = 2; i <= Total_pages; i++)
            {
                GolsTime = GetData(teamPos, i);
                CountGolsTime += GolsTime?.Data?.Select(t => new { Gols = teamPos == "team1" ? t.Team1goals : t.Team2goals }).Sum(t => Convert.ToInt32(t.Gols)) ?? 0;
            }
            return CountGolsTime;
        };

        int CountGolsTime1 = GetGols("team1");
        int CountGolsTime2 = GetGols("team2");

        return CountGolsTime1 + CountGolsTime2;
    }
    class JsonData
    {
        public string? Page { get; set; }
        public int Per_page { get; set; }
        public int Total { get; set; }
        public int Total_pages { get; set; }
        public Data[]? Data { get; set; }
    }
    class Data
    {
        public string? Competition { get; set; }
        public int Year { get; set; }
        public string? Round { get; set; }
        public string? Team1 { get; set; }
        public string? Team2 { get; set; }
        public string? Team1goals { get; set; }
        public string? Team2goals { get; set; }
    }

}