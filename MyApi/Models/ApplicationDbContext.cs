using Microsoft.EntityFrameworkCore;

namespace MyApi.Models;

public sealed class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; } = default!;

    public string GivenName { get; set; } = default!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = default!;

    public DateOnly DateOfBirth { get; set; }
}