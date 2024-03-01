using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HtmxBlog.Modules
{
    public static class StaticModule
    {
        public static void RegisterStaticsEndpoints(this IEndpointRouteBuilder endpoints)
        {
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

            endpoints.MapGet(
                "/baseurl",
                (HttpContext context) =>
                {
                    var baseURL = context.Request.Host;
                    var basepath = context.Request.Path;
                    // return Results.Ok($"Base URL: {baseURL} and base path: {basepath} thus, full path: {baseURL + basepath}");
                    return Results.Ok(baseURL);
                }
            );

            endpoints.MapGet(
                "api/static/posts",
                () =>
                {
                    return Results.Ok(varPostist);
                }
            );

            endpoints.MapGet(
                "api/static/posts/{id}",
                (int id) =>
                {
                    var varPost = varPostist.Find(c => c.Id == id);
                    if (varPost == null)
                        return Results.NotFound("Sorry this command doesn't exsists");

                    return Results.Ok(varPost);
                }
            );

            endpoints.MapPut(
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

            endpoints.MapPost(
                "api/static/posts",
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

            // endpoints.MapAutoBogusEndpoint<Person>(
            //     "/people",
            //     rules =>
            //     {
            //         rules.RuleFor(p => p.Id, f => f.IndexGlobal + 1);
            //         rules.RuleFor(p => p.FullName, f => f.Name.FullName());
            //     }
            // );

            endpoints.MapGet(
                "/api/invitation",
                () =>
                {
                    // Logic to handle the invitation and get the senders name
                    //...

                    var sender = "Will";
                    return Results.Content(
                        $"""
                              <head>
                                 <title>Accept Invitation - My App</title>
                              </head>
                              <body style="font-family:Gill Sans, sans-serif; text-align:center;">
                                 <h1 style="font-size:30px;">Thanks for accepting our invitation!</h1>
                                 <h2 style="font-size:26px;">We've let {sender} know you have accepted the invite.</h2>
                              </body>
                            """,
                        "text/html"
                    );
                }
            );

            endpoints.MapGet(
                "/external-html",
                () =>
                {
                    var htmlContent = File.ReadAllText("./wwwroot/cardPost.html");
                    return Results.Text(htmlContent, "text/html");
                }
            );

            endpoints.MapGet(
                "/call-external-api",
                async (HttpClient httpClient) =>
                {
                    var response = await httpClient.GetAsync("https://api.example.com/endpoint");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return Results.Text(content, "application/json");
                    }
                    else
                    {
                        return Results.BadRequest("Error calling external API");
                    }
                }
            );
        }

        public record PostStatic
        {
            public int Id { get; set; }

            public string? Title { get; set; }

            public string? Content { get; set; }
        }
    }
}
