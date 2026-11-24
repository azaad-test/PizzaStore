using System.ComponentModel.DataAnnotations;

namespace PizzaStore.Filters
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var dto = context.Arguments.OfType<T>().FirstOrDefault();
            if (dto is null)
            {
                return Results.BadRequest("Request payload is missing or invalid.");
            }

            var results = new List<ValidationResult>();
            var validationContext = new ValidationContext(dto);

            if (!Validator.TryValidateObject(dto, validationContext, results, validateAllProperties: true))
            {
                var errors = results
                    .SelectMany(r => (r.MemberNames.Any() ? r.MemberNames : new[] { "Error" })
                        .Select(member => new { member, message = r.ErrorMessage ?? "Invalid value." }))
                    .GroupBy(x => x.member)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.message).ToArray()
                    );

                return Results.ValidationProblem(errors);
            }

            // Input is valid: allow the request to enter the endpoint
            return await next(context);
        }
    }
}