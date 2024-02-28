using FluentValidation;

namespace HtmxBlog.Data
{
    public class PersonValidationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next
        )
        {
            var validator =
                context.HttpContext.RequestServices.GetRequiredService<PersonValidator>();
            foreach (var arg in context.Arguments)
            {
                if (arg is not Person person)
                    continue;
                var result = await validator.ValidateAsync(person);
                if (result.IsValid)
                    continue;
                var errors = result
                    .Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return Results.ValidationProblem(errors);
            }

            return await next(context);
        }
    }

    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(m => m.FullName).NotEmpty();
        }
    }
}
