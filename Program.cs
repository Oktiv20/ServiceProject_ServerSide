using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using System.Linq;
using System.Dynamic;
using System.Runtime.CompilerServices;
using ServiceProject_ServerSide.Models;
using ServiceProject_ServerSide;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<ServiceProjectDbContext>(builder.Configuration["ServiceProjectDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:5169")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});
var app = builder.Build();
//Add for Cors 
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/checkuser/{uid}", (ServiceProjectDbContext db, string uid) =>
{
    var user = db.Users.Where(x => x.Uid == uid).ToList();
    if (uid == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(user);
    }
});

//Create a User
app.MapPost("/api/user", (ServiceProjectDbContext db, User user) =>
{
    db.Users.Add(user);
    db.SaveChanges();
    return Results.Created($"/api/user/{user.Id}", user);
});

//Get user by uid
app.MapGet("/api/user/{uid}", (ServiceProjectDbContext db, string uid) =>
{
    var user = db.Users.Single(u => u.Uid == uid);
    return user;
});

//Delete a user from a project
app.MapDelete("/api/items/{id}", (ServiceProjectDbContext db, int id, string uid) =>
{
    var project = db.Projects.Where(p => p.Id == id).Include(I => I.Users).FirstOrDefault();
    var user = db.Users.Where(u => u.Uid == uid).FirstOrDefault();
    if (project == null)
    {
        return Results.NotFound("not found");
    }

    project.Users.Remove(user);
    db.SaveChanges();
    return Results.NoContent();
});
//Update a User
app.MapPut("/api/Order/{id}", (ServiceProjectDbContext db, int id, User user) =>
{
    User UserToUpdate = db.Users.SingleOrDefault(user => user.Id == id);
    if (UserToUpdate == null)
    {
        return Results.NotFound();
    }
    UserToUpdate.FirstName = user.FirstName;
    UserToUpdate.LastName = user.LastName;
    UserToUpdate.PhoneNumber = user.PhoneNumber;
    UserToUpdate.Email = user.Email;
    UserToUpdate.Uid = user.Uid;
    UserToUpdate.ProfilePic = user.ProfilePic;
    UserToUpdate.isStaff = user.isStaff;


    db.SaveChanges();
    return Results.NoContent();
});



app.Run();
