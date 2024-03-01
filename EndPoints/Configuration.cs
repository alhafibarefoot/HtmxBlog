using HtmxBlog.Data;
using Microsoft.EntityFrameworkCore;

public static class Configuration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {

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

        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
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
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
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
        app.UseCors("MyAllowedOrigins");
        app.UseAuthorization();
        app.UseRouting();
        app.MapRazorPages();
        app.UseAuthorization();
    }
}
