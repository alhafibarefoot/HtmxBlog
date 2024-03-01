using System.Net.Mime;
using System.Text;
using HtmxBlog.Data;
using HtmxBlog.Models;
using HtmxBlog.Modules;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);

///


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
builder.Services.AddHttpClient();
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

///********************Static API**********************************************************
app.RegisterStaticsEndpoints();

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
            <img class='mx-auto d-block' id='postImageID-"
                    + post.Id
                    + @"' src='./assests/img/uploads/"
                    + post.postImage
                    + @"'  width='100' height='100'>
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
              hx-validate='true' hx-include='[name=id],[name=title] ,[name=content],[name=postImage]'   >
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
    )
    .DisableAntiforgery()
    .Accepts<IFormFile>("multipart/form-data");

app.MapPut(
        "/posts/html/{id}",
        async (int id, [FromForm] Post inputPost, AppDbContext db) =>
        {
            var post = await db.Posts.FindAsync(id);

            if (post is null)
                return Results.NotFound();

            post.Title = inputPost.Title;
            post.Content = inputPost.Content;
            post.postImage = inputPost.postImage;

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
             <img class='mx-auto d-block' id='postImageID-"
                    + post.Id
                    + @"' src='./assests/img/uploads/"
                    + post.postImage
                    + @"'  width='100' height='100'>
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
               hx-validate='true' hx-include='[name=id],[name=title] ,[name=content],[name=postImage]'   >
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
              <img class='mx-auto d-block' id='postImageID-"
                    + post.Id
                    + @"' src='./assests/img/uploads/"
                    + post.postImage
                    + @"' width='100' height='100'>
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
             hx-validate='true' hx-include='[name=id],[name=title] ,[name=content],[name=postImage]'   >
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
app.RegisterPostsEndpoints();



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
