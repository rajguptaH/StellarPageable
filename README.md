# StellarPageable

**StellarPageable** is a lightweight C# library designed to simplify filtering, ordering, and paginating data from an `IQueryable` source. This library is ideal for use with Entity Framework Core and provides an efficient way to handle paginated API responses.

---

## Features

- **Dynamic Filtering**: Apply multiple filters with simple syntax.
- **Dynamic Ordering**: Easily sort data by any property.
- **Pagination**: Effortlessly paginate large datasets with customizable page size and number.
- **Asynchronous Execution**: Optimized for scalability with async methods.
- **Error Handling**: Descriptive error messages for invalid inputs.

---

## Installation

1. Clone or download the repository.
2. Add the project reference to your solution.
3. Alternatively, you can compile it into a DLL and include it in your project.

_Note: NuGet package support coming soon!_

---

## Usage

### 1. Basic Setup
Add the following model classes to your project if they are not already included:

```csharp
public class PaginatedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Filter { get; set; } // Example: "Name eq 'John'; Age gt 25"
    public string? OrderBy { get; set; } // Example: "Name desc"
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
```

### 2. Queryable Extension
Use the extension method to retrieve paginated data from your `IQueryable` source:

```csharp
var paginatedResult = await dbContext.YourEntity
    .GetPaginatedAsync(new PaginatedRequest
    {
        PageNumber = 1,
        PageSize = 10,
        OrderBy = "Name desc",
        Filter = "Age gt 30; Name eq 'John'"
    });
```

### 3. Response
The `GetPaginatedAsync` method will return a `PaginatedResponse<T>` with the following fields:
- **Data**: The list of items for the current page.
- **TotalRecords**: Total number of records in the dataset.
- **TotalPages**: Total number of pages available.
- **PageNumber**: Current page number.
- **PageSize**: Number of items per page.

---

## Example Usage

Here's a simple example to integrate **StellarPageable** with an API endpoint:

```csharp
[HttpGet]
public async Task<IActionResult> GetUsers([FromQuery] PaginatedRequest request)
{
    var paginatedUsers = await _dbContext.Users.GetPaginatedAsync(request);
    return Ok(paginatedUsers);
}
```

---

## Roadmap

- NuGet package support.
- Support for Dapper and other ORMs.
- Additional filtering options for advanced scenarios.

---

## Contact

For questions or feedback, feel free to reach out:
- **Email**: [raj@fixelr.in](mailto:raj@fixelr.in)
- **Instagram**: [@raj__rr](https://instagram.com/raj__rr)

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
