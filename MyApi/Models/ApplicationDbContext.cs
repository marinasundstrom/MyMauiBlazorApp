using Microsoft.EntityFrameworkCore;

namespace MyApi.Models;

public sealed class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string GivenName { get; set; }

    public string? MiddleName { get; set; }

    public string LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }
}