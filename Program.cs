using HtmxBlog.Data;
using HtmxBlog.Models;
using Microsoft.AspNetCore.Mvc;
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
                    "https://localhost/*",
                    "http://localhost/*",
                    "http://127.0.0.1/*",
                    "https://www.alhafi.org"
                )
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
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

///********************************************************************************************
//Creaeting Hello simple API call

// app.MapGet("/hello", () => "[get] hello world!");
// app.MapPost("/hello", () => "[post] hello world!");
// app.MapPut("/hello", () => "[put] hello world!");
// app.MapDelete("/hello", () => "[delete] hello world!");
// app.MapGet(
//     "/html",
//     () =>
//     {
//         var html = System.IO.File.ReadAllText(@"./wwwroot/assests/card.html");

//         return Results.Content(html, "text/html");

//         // var reader = File.OpenText("Words.txt");
//         // var fileText = await reader.ReadToEndAsync();
//     }
// );

///********************************************************************************************

/// Use direct access by calling API withen to DBcontext /////////////////////////
///


app.MapGet("/posts", async (AppDbContext db) => await db.Posts.ToListAsync());

//app.MapGet("/posts", async (AppDbContext db) =>  JsonConvert.SerializeObject(await db.Posts.ToListAsync()));
//app.MapGet("/posts", async (AppDbContext db) =>  new Microsoft.AspNetCore.Mvc.JsonResult(await db.Posts.ToListAsync()));


app.MapGet(
    "/posts/{id}",
    async (int id, AppDbContext db) =>
        await db.Posts.FindAsync(id) is Post post ? Results.Ok(post) : Results.NotFound()
);

app.MapPost(
    "/posts",
    async (Post post, AppDbContext db) =>
    {
        db.Posts.Add(post);
        await db.SaveChangesAsync();

        return Results.Created($"/posts/{post.Id}", post);
    }
);

app.MapPut(
    "/posts/{id}",
    async (int id, Post inputPost, AppDbContext db) =>
    {
        var post = await db.Posts.FindAsync(id);

        if (post is null)
            return Results.NotFound();

        post.Title = inputPost.Title;
        post.Content = inputPost.Content;
        post.Image = inputPost.Image;

        await db.SaveChangesAsync();

        return Results.NoContent();
    }
);

app.MapDelete(
    "/posts/{id}",
    async (int id, AppDbContext db) =>
    {
        if (await db.Posts.FindAsync(id) is Post post)
        {
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    }
);

///********************************************************************************************
///
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("MyAllowedOrigins");
app.UseAuthorization();
app.UseRouting();
app.MapRazorPages();
app.UseAuthorization();


app.Run();
