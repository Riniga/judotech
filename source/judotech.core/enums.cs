using System;
using System.Collections.Generic;

public sealed class Settings
{
    private static readonly Lazy<Settings> lazy = new Lazy<Settings>(() => new Settings());
    public Dictionary<string, string> Containers;

    public static Settings Instance { get { return lazy.Value; } }

    private Settings()
    {
        Containers = new Dictionary<string, string>();
        Containers.Add("Users", "/email");
        Containers.Add("Competitions", "/name");
        Containers.Add("Logins", "/email");

    }
}