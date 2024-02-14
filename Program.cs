using HtmxBlog.Data;
using HtmxBlog.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new()
        {
            Title = builder.Environment.ApplicationName,
            Version = "v1",
            Contact = new()
            {
                Name = "Alhafi.BareFoot",
                Email = "alhafi@hotmail.com",
                Url = new Uri("https://www.alhafi.org/")
            },
            Description = " Blog_Posrs Minimal API - Swagger",
            License = new Microsoft.OpenApi.Models.OpenApiLicense(),
            TermsOfService = new("https://www.alhafi.org/")
        }
    );
});

builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "MyAllowedOrigins",
        policy =>
        {
            policy
                .WithOrigins(
                    "https://localhost:7137",
                    "http://localhost:5289",
                    "http://127.0.0.1:5501",
                    "https://www.alhafi.org"
                )
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

builder.Services.AddDbContext<AppDbContext>(
    x => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsStaging())
{
    // your code here
}
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("MyAllowedOrigins");

app.UseAuthorization();

app.MapRazorPages();

///********************************************************************************************
//Creaeting Hello simple API call

app.MapGet("/hello", () => "[get] hello world!");
app.MapPost("/hello", () => "[post] hello world!");
app.MapPut("/hello", () => "[put] hello world!");
app.MapDelete("/hello", () => "[delete] hello world!");
app.MapGet(
    "/html",
    () =>
    {
        var html = System.IO.File.ReadAllText(@"./wwwroot/assests/card.html");

        return Results.Content(html, "text/html");

        // var reader = File.OpenText("Words.txt");
        // var fileText = await reader.ReadToEndAsync();
    }
);

///********************************************************************************************

/// Use direct call to DBcontext calling API/////////////////////////
app.MapGet(
    "/api/dbcontext/v1/posts",
    async (AppDbContext context) =>
    {
        var commands = await context.Posts.ToListAsync();
        return Results.Ok(commands);
    }
);
app.MapGet(
    "/api/dbcontext/v1/posts/{id}",
    async (AppDbContext context, int id) =>
    {
        var command = await context.Posts.FirstOrDefaultAsync(c => c.Id == id);
        if (command != null)
        {
            string strValue = JsonConvert.SerializeObject(command, Formatting.Indented);
            return Results.Text(strValue, "application/json", null);
            //return Results.Ok(command);
        }
        return Results.NotFound();
    }
);

app.MapPost(
    "/api/dbcontext/v1/posts/{id}",
    async (AppDbContext context, Post comm) =>
    {
        await context.Posts.AddAsync(comm);
        await context.SaveChangesAsync();
        return Results.Created($"api/v1/commands/{comm.Id}", comm);
    }
);
app.MapPut(
    "/api/dbcontext/v1/posts/{id}",
    async (AppDbContext context, int id, Post comm) =>
    {
        var command = await context.Posts.FirstOrDefaultAsync(c => c.Id == id);
        if (command != null)
        {
            command.Title = comm.Title;
            command.Content = comm.Content;
            command.Image = comm.Image;
            await context.SaveChangesAsync();
            return Results.NoContent();
        }
        return Results.NotFound();
    }
);
app.MapDelete(
    "api/dbcontext/v1/posts/{id}",
    async (AppDbContext context, int id) =>
    {
        var command = await context.Posts.FirstOrDefaultAsync(c => c.Id == id);
        if (command != null)
        {
            context.Posts.Remove(command);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }
        return Results.NotFound();
    }
);

app.Run();
