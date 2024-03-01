using HtmxBlog.Data;
using HtmxBlog.Modules;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/// Add services to the container.
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

///********************Static API*************************************************************
app.RegisterStaticsEndpoints();

///********************HTML API***************************************************************

app.RegisterPostsHtmlEndpoints();

///////////////////////

///********************Post API***************************************************************

app.RegisterPostsEndpoints();

///********************File Upload API*********************************************************
app.RegisterFileUploadEndpoints();

///********************************************************************************************

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("MyAllowedOrigins");
app.UseAuthorization();
app.UseRouting();
app.MapRazorPages();
app.UseAuthorization();

app.Run();
