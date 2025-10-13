using System;
using System.Collections.Generic;

public sealed class Competitions
{
    public List<DbCompetition> AllCompetitions;

    private static readonly Lazy<Competitions> lazy = new Lazy<Competitions>(() => new Competitions());
    public static Competitions Instance { get { return lazy.Value; } }
    private Competitions() 
    { 
        var database = DatabaseBase.GetDefaultDatabase();
        Logger.Instance.Log("Retrieved database");
        AllCompetitions  = database.ReadAllCompetitions().Result;
    }
}