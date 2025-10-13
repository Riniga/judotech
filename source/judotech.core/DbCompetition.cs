using Newtonsoft.Json;

public class DbCompetition
{
    public static string ContainerName = "Competitions";
    [JsonProperty("id")]
    public string Id { get; set; } = default!;
    [JsonProperty("responsibleemail")]
    public string ResponsibleEmail { get; set; } = default!;
    [JsonProperty("name")]
    public string Name { get; set; } = default!;
    [JsonProperty("startdate")]
    public DateTime StartDate { get; set; } = default!;
    [JsonProperty("enddate")]
    public DateTime EndDate { get; set; } = default!;

    

    public DbCompetition() { }
    public DbCompetition(string responsible, string name, DateTime startDate, DateTime endDate) 
    {
        ResponsibleEmail = responsible;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }
    public DbCompetition(string name)
    {
        var database = DatabaseBase.GetDefaultDatabase();
        var competitionfromdb = database.ReadCompetition(name).Result;
        ResponsibleEmail = competitionfromdb.ResponsibleEmail;
        Name = competitionfromdb.Name;
        StartDate = competitionfromdb.StartDate;
        EndDate = competitionfromdb.EndDate;
    }
    public bool Create()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        return database.CreateCompetition(this).Result;

    }
    public bool Update()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        return database.UpdateCompetition(this).Result;
    }

    public bool Delete()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        return database.DeleteCompetition(Name).Result;

    }
}