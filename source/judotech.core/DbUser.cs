using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

public class DbUser
{
    public static string ContainerName = "Users";
    [JsonProperty("id")]
    public string Id { get; set; } = default!; 
    [JsonProperty("firstname")]
    public string First { get; set; } = default!;
    [JsonProperty("lastname")]
    public string Lastname { get; set; } = default!;
    [JsonProperty("started")]
    public DateTime Started { get; set; } = default!;
    [JsonProperty("birthdate")]
    public DateTime BirthDate { get; set; } = default!;
    [JsonProperty("age")]
    public float Age { get; set; } = default!;
    

    [JsonProperty("active")]
    public bool Active { get; set; } = default!;
    [JsonProperty("total")]
    public int Total { get; set; } = default!;
    [JsonProperty("grade")]
    public string Grade { get; set; } = default!;
    [JsonProperty("should_have_grade")]
    public string ShouldHaveGrade { get; set; } = default!;
    // [JsonIgnore]
    [JsonProperty("password")]
    public string Password { get; set; } = default!;

    public DbUser() { }
    public DbUser(string email, string fullName, string personnumber, string adress, string postalCode, string city, string primaryPhone, string secondaryPhone,string license, string club, string zone, string password) 
    {
        Email = email;
        FullName = fullName;
        Personnumber = personnumber;
        Adress = adress;
        PostalCode = postalCode;
        City = city;
        PrimaryPhone = primaryPhone;
        SecondaryPhone = secondaryPhone;
        License = license;
        Club = club;
        Zone = zone;
        Roles = new List<string>();
        Password = password;
    }
    public DbUser(string email)
    {
        var database = DatabaseBase.GetDefaultDatabase();
        var userfromdb = database.ReadUser(email).Result;
        if (userfromdb==null) return;
        Email = userfromdb.Email;
        FullName = userfromdb.FullName;
        Personnumber = userfromdb.Personnumber;
        Adress = userfromdb.Adress;
        PostalCode = userfromdb.PostalCode;
        City = userfromdb.City;
        PrimaryPhone = userfromdb.PrimaryPhone;
        SecondaryPhone = userfromdb.SecondaryPhone;
        License = userfromdb.License;
        Club = userfromdb.Club;
        Zone = userfromdb.Zone;
        Roles = userfromdb.Roles;
        Password = userfromdb.Password;
        Attendance = userfromdb.Attendance;
        Active = userfromdb.Active;
        Age = userfromdb.Age;
        Grade = userfromdb.Grade;
        Borde = userfromdb.Borde;
        Diff = userfromdb.Diff;
        
    }
    public bool Create()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        return database.CreateUser(this).Result;
    }
    public bool Update()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        return database.UpdateUser(this).Result;
    }

    public bool Delete()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        return database.DeleteUser(Email).Result;

    }

    public static implicit operator DbUser(FeedResponse<DbLogin> v)
    {
        throw new NotImplementedException();
    }
}