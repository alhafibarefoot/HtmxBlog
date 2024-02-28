using AutoBogus;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace HtmxBlog.Data
{
    public static class BogusEndpointsExtensions
    {
        public static RouteGroupBuilder MapAutoBogusEndpoint<TResource>(
            this WebApplication app,
            PathString prefix,
            Action<AutoFaker<TResource>>? builder = null
        )
            where TResource : class
        {
            var group = app.MapGroup(prefix).WithGroupName($"{typeof(TResource).FullName}_Bogus");

            var faker = new AutoFaker<TResource>();
            builder?.Invoke(faker);

            // generate a working collection
            // will allocate objects in memory
            var db = faker.Generate(1000);

            // INDEX
            group
                .MapGet(
                    "",
                    (int? pageSize, int? page) =>
                    {
                        var result = db.ToPagedList(
                            page.GetValueOrDefault(1),
                            pageSize.GetValueOrDefault(10)
                        );

                        return new
                        {
                            results = result,
                            page = result.PageNumber,
                            pageSize = result.PageSize,
                            totalItemCount = result.TotalItemCount
                        };
                    }
                )
                .WithName($"{typeof(TResource).FullName}_Bogus+List");

            // SHOW
            group
                .MapGet(
                    "{id}",
                    (string id) =>
                    {
                        var result = db.FirstOrDefault(t => FindById(t, id));
                        return result != null ? Results.Ok(result) : Results.NotFound();
                    }
                )
                .WithName($"{typeof(TResource).FullName}_Bogus+Show");

            // POST
            group
                .MapPost(
                    "",
                    (TResource item) =>
                    {
                        try
                        {
                            dynamic generated = faker.Generate(1)[0];
                            SetId(item, generated.Id);
                            db.Add(item);
                            return Results.CreatedAtRoute(
                                $"{typeof(TResource).FullName}_Bogus+Show",
                                new { id = generated.Id },
                                item
                            );
                        }
                        catch
                        {
                            return Results.Ok(item);
                        }
                    }
                )
                .WithName($"{typeof(TResource).FullName}_Bogus+Create");

            // PUT
            group
                .MapPut(
                    "{id}",
                    (string id, [FromBody] TResource item) =>
                    {
                        var index = db.FindIndex(t => FindById(t, id));
                        if (index < 0)
                            return Results.NotFound();
                        SetId(id, item);
                        db[index] = item;
                        return Results.Ok(item);
                    }
                )
                .WithName($"{typeof(TResource).FullName}_Bogus+Update");

            // DELETE
            group
                .MapDelete(
                    "{id}",
                    (string id) =>
                    {
                        db.RemoveAll(t => FindById(t, id));
                        return Results.Accepted();
                    }
                )
                .WithName($"{typeof(TResource).FullName}_Bogus+Delete");

            return group;
        }

        private static bool FindById<TResource>(TResource target, object? id)
        {
            if (id is null)
                return false;

            var type = typeof(TResource);
            var identifier = type.GetProperties().FirstOrDefault(p => p.Name == "Id");

            if (identifier == null)
                return false;

            object? converted = Convert.ChangeType(id, identifier.PropertyType);
            if (converted == null)
                return false;
            var value = identifier.GetValue(target);
            var result = converted.Equals(value);
            return result;
        }

        private static void SetId<TResource>(TResource target, object? id)
        {
            if (id is null)
                return;

            var type = typeof(TResource);
            var identifier = type.GetProperties().FirstOrDefault(p => p.Name == "Id");

            if (identifier == null)
                return;

            object? converted = Convert.ChangeType(id, identifier.PropertyType);
            if (converted == null)
                return;
            identifier.SetValue(target, converted);
        }
    }
}
