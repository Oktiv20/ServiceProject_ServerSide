using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using ServiceProject_ServerSide;
using ServiceProject_ServerSide.Models;
using System.Text.Json.Serialization;

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


//Check if a user exists
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
app.MapDelete("/api/projectusers/{id}", (ServiceProjectDbContext db, int id, string uid) =>
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
app.MapPut("/api/user/{id}", (ServiceProjectDbContext db, int id, User user) =>
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



// PROJECTS ENDPOINTS


// GET PROJECTS

app.MapGet("/api/projects", (ServiceProjectDbContext db) =>
{
    List<Project> projects = db.Projects.Include(p => p.Category).ToList();
    if (projects.Count == 0)
    {
        return Results.NotFound();
    }

    return Results.Ok(projects);
});


// GET SINGLE PROJECT

app.MapGet("/api/projects/{id}", (ServiceProjectDbContext db, int id) =>
{
    Project project = db.Projects
    .Include(p => p.Users)
    .FirstOrDefault(project => project.Id == id);
    if (project == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(project);
});


// GET PROJECT BY CATEGORY

app.MapGet("/api/projectsbycategory/{categoryId}", (ServiceProjectDbContext db, int categoryId) =>
{
    var projects = db.Projects
    .Where(p => p.CategoryId == categoryId)
    .ToList();

    return Results.Ok(projects);
});


// CREATE PROJECT

app.MapPost("/api/projects", (ServiceProjectDbContext db, Project project) =>
{
    try
    {
        db.Add(project);
        db.SaveChanges();
        return Results.Created($"/api/projects/{project.Id}", project);
    }
    catch (DbUpdateException)
    {
        return Results.NotFound();
    }
});


// UPDATE PROJECT

app.MapPut("/api/projects/{projectId}", (ServiceProjectDbContext db, int projectId, Project project) =>
{
    Project updateProject = db.Projects.SingleOrDefault(p => p.Id == projectId);
    if (updateProject == null)
    {
        return Results.NotFound();
    }
    updateProject.Name = project.Name;
    updateProject.Description = project.Description;
    updateProject.Location = project.Location;
    updateProject.Image = project.Image;
    updateProject.Duration = project.Duration;
    updateProject.Date = project.Date;
    updateProject.CategoryId = project.CategoryId;

    db.SaveChanges();
    return Results.NoContent();
});


// DELETE PROJECT

app.MapDelete("/api/projects/{projectId}", (ServiceProjectDbContext db, int projectId) =>
{
    Project deleteProject = db.Projects.FirstOrDefault(p => p.Id == projectId);
    if (deleteProject == null)
    {
        return Results.NotFound();
    }
    db.Remove(deleteProject);
    db.SaveChanges();
    return Results.Ok(deleteProject);
});


// GET USER'S PROJECTS

app.MapGet("/api/userprojects/{userId}", (ServiceProjectDbContext db, int userId) =>
{
    var user = db.Users
        .Include(u => u.Projects)
        .FirstOrDefault(u => u.Id == userId);

    if (user == null)
    {
        return Results.NotFound("User not found");
    }

    var projects = user.Projects.ToList();
    return Results.Ok(projects);
});


// ADD USER TO PROJECT

app.MapPost("/api/projectusers/{projectId}/{userId}", (ServiceProjectDbContext db, int projectId, int userId) =>
{
    var project = db.Projects
        .Include(p => p.Users)
        .FirstOrDefault(p => p.Id == projectId);

    var user = db.Users.FirstOrDefault(u => u.Id == userId);

    if (project == null || user == null)
    {
        return Results.NotFound("Project or User not found");
    }

    if (project.Users.Any(u => u.Id == userId))
    {
        return Results.BadRequest("User is already associated with this project");
    }

    project.Users.Add(user);
    db.SaveChanges();

    return Results.Ok(user);
});

app.Run();
