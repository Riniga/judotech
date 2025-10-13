using System;
using System.Collections.Generic;

public sealed class Users
{
    public List<DbUser> AllUsers;

    private static readonly Lazy<Users> lazy = new Lazy<Users>(() => new Users());
    public static Users Instance { get { return lazy.Value; } }
    private Users()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        Logger.Instance.Log("Retrieved database");
        AllUsers = database.ReadAllUsers().Result;
    }
    public void Refresh()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        Logger.Instance.Log("Retrieved database");
        AllUsers = database.ReadAllUsers().Result;
    }
}