using Microsoft.AspNetCore.Antiforgery;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace HtmxBlog.Modules
{
    public static class StaticModule
    {
        public static void RegisterStaticsEndpoints(this IEndpointRouteBuilder routes)
        {
            var endpoints = routes.MapGroup("/api/v1/static/posts");
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

            endpoints
                .MapGet(
                    "/baseurl",
                    (HttpContext context) =>
                    {
                        var baseURL = context.Request.Host;
                        var basepath = context.Request.Path;
                        // return Results.Ok($"Base URL: {baseURL} and base path: {basepath} thus, full path: {baseURL + basepath}");
                        return Results.Ok(baseURL);
                    }
                )
                .WithOpenApi(
                    x =>
                        new OpenApiOperation(x)
                        {
                            Summary = "Get Library Books",
                            Description =
                                "Returns information about all the available books from the Amy's library.",
                            Tags = new List<OpenApiTag> { new() { Name = "Amy's Library" } }
                        }
                );

            endpoints
                .MapGet(
                    "/",
                    () =>
                    {
                        return Results.Ok(varPostist);
                    }
                )
                .WithMetadata(new SwaggerOperationAttribute("summary001", "description001"));

            endpoints.MapGet(
                "/{id}",
                (int id) =>
                {
                    var varPost = varPostist.Find(c => c.Id == id);
                    if (varPost == null)
                        return Results.NotFound("Sorry this command doesn't exsists");

                    return Results.Ok(varPost);
                }
            );

            endpoints.MapPut(
                "/{id}",
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

            endpoints.MapPost(
                "/",
                (PostStatic postListStatic) =>
                {
                    if (postListStatic.Id != 0 || string.IsNullOrEmpty(postListStatic.Title))
                    {
                        return Results.BadRequest("Invalid Id or HowTo filling");
                    }
                    if (
                        varPostist.FirstOrDefault(
                            c => c.Title.ToLower() == postListStatic.Title.ToLower()
                        ) != null
                    )
                    {
                        return Results.BadRequest("HowTo Exsists");
                    }

                    postListStatic.Id =
                        varPostist.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
                    varPostist.Add(postListStatic);
                    return Results.Ok(varPostist);
                }
            );

            endpoints.MapDelete(
                "/{id}",
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

            // endpoints.MapAutoBogusEndpoint<Person>(
            //     "/people",
            //     rules =>
            //     {
            //         rules.RuleFor(p => p.Id, f => f.IndexGlobal + 1);
            //         rules.RuleFor(p => p.FullName, f => f.Name.FullName());
            //     }
            // );


            endpoints.MapGet(
                "antiforgery/token",
                (IAntiforgery forgeryService, HttpContext context) =>
                {
                    var tokens = forgeryService.GetAndStoreTokens(context);
                    var xsrfToken = tokens.RequestToken!;
                    return TypedResults.Content(xsrfToken, "text/plain");
                }
            );

            //.RequireAuthorization(); // In a real world scenario, you'll only give this token to authorized users
        }

        public record PostStatic
        {
            public int Id { get; set; }

            public string? Title { get; set; }

            public string? Content { get; set; }
        }
    }

    public record Person(int Id, string FullName)
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Dog Dog { get; set; }
    }

    public record Dog(string Name);
}
