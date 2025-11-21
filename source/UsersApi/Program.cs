using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// In-memory "DB"
var users = new ConcurrentDictionary<string, User>();

app.MapGet("/users", () => Results.Ok(users.Values));

app.MapGet("/users/{id}", ([FromRoute] string id) =>
    users.TryGetValue(id, out var u) ? Results.Ok(u) : Results.NotFound());

app.MapPost("/users", ([FromBody] User user) =>
{
    if (!users.TryAdd(user.Id, user))
        return Results.Conflict($"User '{user.Id}' exists");
    return Results.Created($"/users/{user.Id}", user);
});

app.MapPut("/users/{id}", ([FromRoute] string id, [FromBody] User user) =>
{
    if (id != user.Id) return Results.BadRequest("ID mismatch");
    if (!users.ContainsKey(id)) return Results.NotFound();
    users[id] = user; return Results.Ok(user);
});

app.MapDelete("/users/{id}", ([FromRoute] string id) =>
    users.TryRemove(id, out _) ? Results.NoContent() : Results.NotFound());

app.Run();

public record User(string Id, string Name, string Email);
