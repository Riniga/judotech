using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

public class DbUser
{
    public static string ContainerName = "Users";
    [JsonProperty("id")]
    public string Id { get; set; } = default!; 
    [JsonProperty("email")]
    public string Email { get; set; } = default!;
    [JsonProperty("fullname")]
    public string FullName { get; set; } = default!;
    [JsonProperty("personnumber")]    
    public string Personnumber { get; set; } = default!;
    [JsonProperty("age")]
    public float Age { get; set; } = default!;
    [JsonProperty("adress")]
    public string Adress { get; set; } = default!;
    [JsonProperty("postalcode")]
    public string PostalCode { get; set; } = default!;
    [JsonProperty("city")]
    public string City { get; set; } = default!;
    [JsonProperty("primaryphone")]
    public string PrimaryPhone { get; set; } = default!;
    [JsonProperty("secondaryphone")]
    public string SecondaryPhone { get; set; } = default!;
    [JsonProperty("active")]
    public bool Active { get; set; } = default!;
    [JsonProperty("attendance")]
    public int Attendance { get; set; } = default!;
    [JsonProperty("grade")]
    public string Grade { get; set; } = default!;
    [JsonProperty("borde")]
    public string Borde { get; set; } = default!;
    [JsonProperty("diff")]
    public string Diff { get; set; } = default!;
    [JsonProperty("license")]
    public string License { get; set; } = default!;
    [JsonProperty("club")]
    public string Club { get; set; } = default!;
    [JsonProperty("zone")]
    public string Zone { get; set; } = default!;
    [JsonProperty("roles")]
    public List<string> Roles { get; set; } = default!;
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