using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace HtmxBlog.Modules
{
    public static class UploadFilesModules
    {

        static string CreateTempfilePath(string fileName)
        {
            // var filename = $"{Guid.NewGuid()}.tmp";
            var filename = fileName;
            var directoryPath = Path.Combine("./wwwroot/assests/img", "uploads");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath, filename);
        }

        public static void RegisterFileUploadEndpoints(this IEndpointRouteBuilder routes)
        {
              var endpoints = routes.MapGroup("/api/v1/File");
            endpoints
                .MapPost(
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

            endpoints
                .MapPost(
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
        }
    }
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
