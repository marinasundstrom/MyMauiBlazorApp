using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.AspNetCore.Http.TypedResults;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi;

public static class PersonsEndpoints
{
    public static WebApplication MapPersonsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/persons");

        group.MapGet("/", async Task<ItemsResult<Person>> (int page = 1, int pageSize = 10, ApplicationDbContext context = default!, CancellationToken cancellationToken = default!) =>
        {
            var query = context.Persons.AsQueryable();

            var count = await query.CountAsync(cancellationToken);

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return new ItemsResult<Person>(
                Items: await query.ToListAsync(cancellationToken),
                Total: count);
        })
        .WithName("Persons_GetPersons")
        .WithTags("Persons")
        .WithDescription("Gets persons")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Person>, NotFound>> (int id, ApplicationDbContext context = default!, CancellationToken cancellationToken = default!) =>
        {
            var person = await context.Persons.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            return person is null ? NotFound() : Ok(person);
        })
        .WithName("Persons_GetPersonById")
        .WithTags("Persons")
        .WithDescription("Get a person by Id")
        .WithOpenApi();

        group.MapPost("/", async Task<Created<Person>> (CreatePersonRequest request, ApplicationDbContext context = default!, HttpContext httpContext = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!) =>
        {
            var r = context.Persons.Add(new Models.Person
            {
                FirstName = request.FirstName,
                GivenName = request.GivenName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth
            });

            await context.SaveChangesAsync(cancellationToken);

            var link = linkGenerator.GetPathByName(httpContext, "Persons_CreatePerson", new { id = r.Entity.Id });

            return Created(link, r.Entity);
        })
        .Produces<Person>(StatusCodes.Status201Created)
        .Produces<Person>(StatusCodes.Status200OK) // WORKAROUND FOR OpenAPI
        .WithName("Persons_CreatePerson")
        .WithTags("Persons")
        .WithDescription("Create a person")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, ApplicationDbContext context = default!, CancellationToken cancellationToken = default!) =>
        {
            var deleted = await context.Persons.Where(p => p.Id == id).ExecuteDeleteAsync(cancellationToken);

            return deleted > 1 ? Ok() : NotFound();
        })
        .WithName("Persons_DeletePerson")
        .WithTags("Persons")
        .WithDescription("Delete person")
        .WithOpenApi();

        return app;
    }
}

public sealed record ItemsResult<T>(IEnumerable<T> Items, long Total);

public sealed record CreatePersonRequest(string FirstName, string GivenName, string? MiddleName, string LastName, DateOnly DateOfBirth);