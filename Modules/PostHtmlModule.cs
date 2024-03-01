using HtmxBlog.Data;
using HtmxBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public static class ProductsHtmlModule
{
    public static void RegisterPostsHtmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGet(
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

        endpoints
            .MapPut(
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

        endpoints
            .MapPost(
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

        endpoints
            .MapDelete(
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
