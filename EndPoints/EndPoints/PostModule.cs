using HtmxBlog.Data;
using HtmxBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class ProductsModule
{
    public static void RegisterPostsEndpoints(this IEndpointRouteBuilder routes)
    {
        /// <summary>
        /// Gets the list of all Posts.
        /// </summary>
        /// <returns>The list of Posts.</returns>
        // GET: api/Employee
        var endpoints = routes.MapGroup("/api/v1/posts");
        endpoints.MapGet("/", async (AppDbContext db) => await db.Posts.ToListAsync());

        //app.MapGet("/posts", async (AppDbContext db) =>  JsonConvert.SerializeObject(await db.Posts.ToListAsync()));
        //app.MapGet("/posts", async (AppDbContext db) =>  new Microsoft.AspNetCore.Mvc.JsonResult(await db.Posts.ToListAsync()));


        endpoints.MapGet(
            "/{id}",
            async (int id, AppDbContext db) =>
                await db.Posts.FindAsync(id) is Post post ? Results.Ok(post) : Results.NotFound()
        );

        endpoints
            .MapPost(
                "/",
                async ([FromForm] Post post, AppDbContext db) =>
                {
                    db.Posts.Add(post);
                    await db.SaveChangesAsync();

                    return Results.Created($"/posts/{post.Id}", post);
                }
            )
            .DisableAntiforgery();

        endpoints
            .MapPut(
                "/{id}",
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

        endpoints.MapDelete(
            "/{id}",
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
    }
}
