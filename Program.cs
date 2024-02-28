using System.Net.Mime;
using System.Text;
using HtmxBlog.Data;
using HtmxBlog.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Bogus;

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
        Id = 2,
        Title = "Post2",
        Content = "Content2"
    }
};

static string CreateTempfilePath(string fileName)
{
    // var filename = $"{Guid.NewGuid()}.tmp";
    var filename = fileName;
    var directoryPath = Path.Combine("./wwwroot/assests/img", "uploads");
    if (!Directory.Exists(directoryPath))
        Directory.CreateDirectory(directoryPath);

    return Path.Combine(directoryPath, filename);
}

///
// Add services to the container.
builder.Services.AddControllers().AddXmlSerializerFormatters().AddNewtonsoftJson();
builder.Services.AddRazorPages();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});
builder.Services.AddAntiforgery();

builder.Services.AddSingleton(new HtmlOptions { myHTML = "" });
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

app.MapGet(
    "/baseurl",
    (HttpContext context) =>
    {
        var baseURL = context.Request.Host;
        var basepath = context.Request.Path;
        // return Results.Ok($"Base URL: {baseURL} and base path: {basepath} thus, full path: {baseURL + basepath}");
        return Results.Ok(baseURL);
    }
);

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
//https://khalidabuhakmeh.com/generating-bogus-http-endpoints-with-aspnet-core-minimal-apis
//GET http://localhost:5208/people?pageSize=1
//PUT http://localhost:5208/people/1

app.MapAutoBogusEndpoint<Person>("/people", rules =>
{
    rules.RuleFor(p => p.Id, f => f.IndexGlobal + 1);
    rules.RuleFor(p => p.FullName, f => f.Name.FullName());
});



app.MapGet("/api/invitation", () =>
{
    // Logic to handle the invitation and get the senders name
    //...

    var sender = "Will";
    return Results.Content($"""
                              <head>
                                 <title>Accept Invitation - My App</title>
                              </head>
                              <body style="font-family:Gill Sans, sans-serif; text-align:center;">
                                 <h1 style="font-size:30px;">Thanks for accepting our invitation!</h1>
                                 <h2 style="font-size:26px;">We've let {sender} know you have accepted the invite.</h2>
                              </body>
                            """, "text/html");
});
//*********************  HTML API  *********************************************


app.MapGet(
    "/html/post",
    async (HtmlOptions option, AppDbContext db) =>
    {
        string json = JsonConvert.SerializeObject(await db.Posts.ToListAsync());
        List<Post> item = JsonConvert.DeserializeObject<List<Post>>(json);
        string ResponseHTML = null;

        foreach (var post in item)
        {
            //Results.Extensions.HtmlResponse(
            option.myHTML =
                @"
<div id=Post-id-"
                + post.Id
                + "  class='col mb-auto posts-col-"
                + post.Id
                + @"'>

     <div class='card mt-5 card' style='width: 19.5rem'>

        <div class='card-body card-body'>
            <h5 class='card-title xtitlename'>"
                + post.Title
                + @"</h5><p class='card-text xcontentname'>"
                + post.Content
                + @"</p>
            <a href='#' class='btn btn-danger' hx-delete='https://localhost:7137/posts/html/"
                + post.Id
                + @"'    hx-target='.posts-col-"
                + post.Id
                + @"' hx-swap='delete'
                )]'
                 hx-confirm='Are you sure you wish to delete this Post? Titled : "
                + post.Title
                + @"'

            >Delete</a>

            <a href='#' class='btn btn-success' hx-put='https://localhost:7137/posts/html/"
                + post.Id
                + @"' hx-target='.posts-col-"
                + post.Id
                + @"'
              hx-validate='true' hx-include='[name=id],[name=title] ,[name=content]'  >
            Update</a>
        </div>

      </div>


</div>

";
            ResponseHTML = ResponseHTML + option.myHTML;
            option.myHTML = ResponseHTML;
        }
        return Results.Extensions.HtmlResponse(option.myHTML);
    }
);

app.MapPut(
        "/posts/html/{id}",
        async (int id, [FromForm] Post inputPost, AppDbContext db) =>
        {
            var post = await db.Posts.FindAsync(id);

            if (post is null)
                return Results.NotFound();

            post.Title = inputPost.Title;
            post.Content = inputPost.Content;

            await db.SaveChangesAsync();

            return Results.Extensions.HtmlResponse(
                @"
<div id=Post-id-"
                    + post.Id
                    + "  class='col mb-auto posts-col-"
                    + post.Id
                    + @"'>

     <div class='card mt-5 card' style='width: 19.5rem'>

        <div class='card-body card-body'>
            <h5 class='card-title xtitlename'>"
                    + post.Title
                    + @"</h5><p class='card-text xcontentname'>"
                    + post.Content
                    + @"</p>
             <a href='#' class='btn btn-danger' hx-delete='https://localhost:7137/posts/html/"
                    + post.Id
                    + @"'    hx-target='.posts-col-"
                    + post.Id
                    + @"' hx-swap='delete'
                )]'
                 hx-confirm='Are you sure you wish to delete this Post? Titled : "
                    + post.Title
                    + @"'

            >Delete</a>

            <a href='#' class='btn btn-success' hx-put='https://localhost:7137/posts/html/"
                    + post.Id
                    + @"' hx-target='.posts-col-"
                    + post.Id
                    + @"'
             hx-validate='true' hx-include='[name=id],[name=title] ,[name=content]'  >
            Update</a>
        </div>

      </div>


</div>

"
            );
        }
    )
    .DisableAntiforgery();

app.MapPost(
        "/posts/html",
        async ([FromForm] Post post, AppDbContext db) =>
        {
            db.Posts.Add(post);
            await db.SaveChangesAsync();

            return Results.Extensions.HtmlResponse(
                @"
<div id=Post-id-"
                    + post.Id
                    + "  class='col mb-auto posts-col-"
                    + post.Id
                    + @"'>

     <div class='card mt-5 card' style='width: 19.5rem'>

        <div class='card-body card-body'>
            <h5 class='card-title xtitlename'>"
                    + post.Title
                    + @"</h5><p class='card-text xcontentname'>"
                    + post.Content
                    + @"</p>
            <a href='#' class='btn btn-danger' hx-delete='https://localhost:7137/posts/html/"
                    + post.Id
                    + @"'    hx-target='.posts-col-"
                    + post.Id
                    + @"' hx-swap='delete'
                )]'
                 hx-confirm='Are you sure you wish to delete this Post? Titled : "
                    + post.Title
                    + @"'

            >Delete</a>

            <a href='#' class='btn btn-success' hx-put='https://localhost:7137/posts/html/"
                    + post.Id
                    + @"' hx-target='.posts-col-"
                    + post.Id
                    + @"'
             hx-validate='true' hx-include='[name=id],[name=title] ,[name=content]'   >
            Update</a>
        </div>

      </div>


</div>

"
            );
        }
    )
    .DisableAntiforgery();

app.MapDelete(
        "/posts/html/{id}",
        async (int id, AppDbContext db) =>
        {
            if (await db.Posts.FindAsync(id) is Post post)
            {
                db.Posts.Remove(post);
                await db.SaveChangesAsync();
                return Results.Ok();
            }
            return Results.NotFound("Sorry Item not Exsists");
        }
    )
    .DisableAntiforgery();

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
        async ([FromForm] Post post, AppDbContext db) =>
        {

            db.Posts.Add(post);
            await db.SaveChangesAsync();

            return Results.Created($"/posts/{post.Id}", post);
        }
    )
    .DisableAntiforgery();

app.MapPut(
        "/posts/{id}",
        async (int id, [FromForm] Post inputPost, AppDbContext db) =>
        {
            var post = await db.Posts.FindAsync(id);

            if (post is null)
                return Results.NotFound();

            post.Title = inputPost.Title.ToString();
            post.Content = inputPost.Content.ToString();
            post.postImage = inputPost.postImage.ToString();

            await db.SaveChangesAsync();

            return Results.NoContent();
        }
    )
    .DisableAntiforgery();

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

///*******************************File Upload *************************************************************
///

// Get token
app.MapGet(
    "antiforgery/token",
    (IAntiforgery forgeryService, HttpContext context) =>
    {
        var tokens = forgeryService.GetAndStoreTokens(context);
        var xsrfToken = tokens.RequestToken!;
        return TypedResults.Content(xsrfToken, "text/plain");
    }
);

//.RequireAuthorization(); // In a real world scenario, you'll only give this token to authorized users

app.MapPost(
        "/upload",
        async ([FromForm] IFormFile? file) =>
        {
            String fileName = file.FileName;
            string tempfile = CreateTempfilePath(fileName);
            using var stream = File.OpenWrite(tempfile);
            await file.CopyToAsync(stream);
            return Results.Ok();
        }
    )
    .DisableAntiforgery()
    .Accepts<IFormFile>("multipart/form-data");

app.MapPost(
        "/uploadmany",
        async (IFormFileCollection myFiles) =>
        {
            foreach (var file in myFiles)
            {
                String fileName = file.FileName;
                string tempfile = CreateTempfilePath(fileName);
                using var stream = File.OpenWrite(tempfile);
                await file.CopyToAsync(stream);

                // dom more fancy stuff with the IFormFile
            }
        }
    )
    .DisableAntiforgery()
    .Accepts<IFormFile>("multipart/form-data");

///********************************************************************************************

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("MyAllowedOrigins");
app.UseAuthorization();
app.UseRouting();
app.MapRazorPages();
app.UseAuthorization();

app.Run();

public record PostStatic
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }
}

class CusomtHTMLResult : IResult
{
    private readonly string _htmlContent;

    public CusomtHTMLResult(string htmlContent)
    {
        _htmlContent = htmlContent;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = MediaTypeNames.Text.Html;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(_htmlContent);
        await httpContext.Response.WriteAsync(_htmlContent);
    }
}

static class CustomResultExtensions
{
    public static IResult HtmlResponse(this IResultExtensions extensions, string html)
    {
        return new CusomtHTMLResult(html);
    }
}

public class HtmlOptions
{
    public string? myHTML { get; set; }
}


public record Person(int Id, string FullName)
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dog Dog { get; set; }
}

public record Dog(string Name);