using HtmxBlog.Data;
using HtmxBlog.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

///
var varPostist = new List<PostStatic>
{
    new PostStatic
    {
        Id = 1,
        Title = "Post1",
        Content = "Content1"
    },
    new PostStatic
    {
        Id = 1,
        Title = "Post1",
        Content = "Content1"
    }
};

///
// Add services to the container.
builder.Services.AddControllers().AddXmlSerializerFormatters().AddNewtonsoftJson();
builder.Services.AddRazorPages();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});
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

///********************************************************************************************
app.MapGet(
    "api/static/posts",
    () =>
    {
        return Results.Ok(varPostist);
    }
);

app.MapGet(
    "api/static/posts/{id}",
    (int id) =>
    {
        var varPost = varPostist.Find(c => c.Id == id);
        if (varPost == null)
            return Results.NotFound("Sorry this command doesn't exsists");

        return Results.Ok(varPost);
    }
);

app.MapPut(
    "api/static/posts/{id}",
    (PostStatic UpdatecommListStatic, int id) =>
    {
        var varPost = varPostist.Find(c => c.Id == id);
        if (varPost == null)
            return Results.NotFound("Sorry this command doesn't exsists");

        varPost.Title = UpdatecommListStatic.Title;
        varPost.Content = UpdatecommListStatic.Content;

        return Results.Ok(varPost);
    }
);

app.MapPost(
    "api/static/posts",
    (PostStatic postListStatic) =>
    {
        if (postListStatic.Id != 0 || string.IsNullOrEmpty(postListStatic.Title))
        {
            return Results.BadRequest("Invalid Id or HowTo filling");
        }
        if (
            varPostist.FirstOrDefault(c => c.Title.ToLower() == postListStatic.Title.ToLower())
            != null
        )
        {
            return Results.BadRequest("HowTo Exsists");
        }

        postListStatic.Id = varPostist.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
        varPostist.Add(postListStatic);
        return Results.Ok(varPostist);
    }
);

app.MapDelete(
    "api/static/posts/{id}",
    (int id) =>
    {
        var varPostL = varPostist.Find(c => c.Id == id);
        if (varPostL == null)
            return Results.NotFound("Sorry this command doesn't exsists");
        varPostist.Remove(varPostL);
        return Results.Ok(varPostL);
    }
);

/// Use direct call to DBcontext calling API/////////////////////////

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

class PostStatic
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }
}
