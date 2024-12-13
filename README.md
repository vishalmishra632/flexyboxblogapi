# FlexBox Blog API - Clean Architecture

A robust RESTful API built with .NET Core 8 following Clean Architecture principles, providing backend services for a blogging platform.

## Architecture Overview

This project implements Clean Architecture, separating concerns into distinct layers:

### 1. Core Layer (BlogAPI.Core)
Contains business logic and domain entities:
```csharp
BlogAPI.Core/
├── Entities/         # Domain models
│   └── BlogPost.cs
├── Interfaces/       # Repository interfaces
│   └── IBlogPostRepository.cs
├── DTOs/            # Data transfer objects
│   ├── BlogPostDto.cs
│   ├── CreateBlogPostDto.cs
│   └── UpdateBlogPostDto.cs
└── Common/          # Shared utilities
    └── Result.cs    # Generic result wrapper
```

### 2. Infrastructure Layer (BlogAPI.Infrastructure)
Implements data access and external services:
```csharp
BlogAPI.Infrastructure/
├── Data/
│   ├── BlogDbContext.cs
│   └── Configurations/
│       └── BlogPostConfiguration.cs
├── Repositories/
│   └── BlogPostRepository.cs
└── Services/
    └── BlogService.cs
```

### 3. API Layer (BlogAPI.API)
Handles HTTP requests and API configuration:
```csharp
BlogAPI.API/
├── Controllers/
│   └── BlogPostsController.cs
├── Program.cs
└── appsettings.json
```

## Technology Stack & Packages

### Core Frameworks
- .NET Core 8.0
- Entity Framework Core 8.0
- ASP.NET Core Web API

### NuGet Packages
```xml
<!-- Entity Framework Core -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />

<!-- API Documentation -->
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Versioning" Version="5.1.0" />

<!-- Additional Tools -->
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
```

## Database Setup

### Connection String
Update `appsettings.json` with your SQL Server connection:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BlogDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### Database Migration Commands
Run these commands in the Package Manager Console:

```powershell
# Create initial migration
Add-Migration InitialCreate -Project BlogAPI.Infrastructure

# Apply migration to database
Update-Database
```

Or using .NET CLI:
```bash
# Create migration
dotnet ef migrations add InitialCreate --project BlogAPI.Infrastructure

# Update database
dotnet ef database update
```

## API Endpoints

### Blog Posts
```
GET    /api/v1/blogposts     - Get all posts
GET    /api/v1/blogposts/{id} - Get single post
POST   /api/v1/blogposts     - Create post
PUT    /api/v1/blogposts/{id} - Update post
DELETE /api/v1/blogposts/{id} - Delete post
```

## Running the Application

1. Clone the repository:
```bash
git clone https://github.com/vishalmishra632/flexyboxblogapi.git
cd flexyboxblogapi
```

2. Set up the database:
```bash
dotnet ef database update
```

3. Run the application:
```bash
dotnet run --project BlogAPI.API
```

4. Access Swagger UI:
```
[https://localhost:7084/swagger](http://localhost:5090/swagger/index.html)
```

## Project Setup from Scratch

If you want to create this project structure from scratch:

1. Create solution and projects:
```bash
dotnet new sln -n BlogAPI
dotnet new classlib -n BlogAPI.Core
dotnet new classlib -n BlogAPI.Infrastructure
dotnet new webapi -n BlogAPI.API
```

2. Add projects to solution:
```bash
dotnet sln add BlogAPI.Core/BlogAPI.Core.csproj
dotnet sln add BlogAPI.Infrastructure/BlogAPI.Infrastructure.csproj
dotnet sln add BlogAPI.API/BlogAPI.API.csproj
```

3. Set up project references:
```bash
cd BlogAPI.API
dotnet add reference ../BlogAPI.Core/BlogAPI.Core.csproj
dotnet add reference ../BlogAPI.Infrastructure/BlogAPI.Infrastructure.csproj

cd ../BlogAPI.Infrastructure
dotnet add reference ../BlogAPI.Core/BlogAPI.Core.csproj
```

## Development Guidelines

### Adding New Features

1. Define entity in Core layer:
```csharp
public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
```

2. Create repository interface in Core layer:
```csharp
public interface IBlogPostRepository
{
    Task<IEnumerable<BlogPost>> GetAllAsync();
    Task<BlogPost> GetByIdAsync(int id);
    // ... other methods
}
```

3. Implement repository in Infrastructure layer:
```csharp
public class BlogPostRepository : IBlogPostRepository
{
    private readonly BlogDbContext _context;
    
    public BlogPostRepository(BlogDbContext context)
    {
        _context = context;
    }
    // ... implement methods
}
```

## Error Handling

The API uses a consistent error response format:
```json
{
    "isSuccess": false,
    "data": null,
    "message": "Error message here",
    "errors": ["Detailed error 1", "Detailed error 2"]
}
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.

## Contact

Vishal Mishra
GitHub: [@vishalmishra632](https://github.com/vishalmishra632)
