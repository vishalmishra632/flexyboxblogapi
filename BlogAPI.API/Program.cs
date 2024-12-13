using BlogAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Core.Interfaces;
using BlogAPI.Infrastructure.Repositories;
using System.Globalization;
using BlogAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Set culture
Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

// Add services to the container
builder.Services.AddControllers();

// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    // Simplified version reader for better Swagger compatibility
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.SubstituteApiVersionInUrl = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

// Configure Swagger with enhanced documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger document
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Blog API",
        Version = "v1",
        Description = "A simple RESTful API for managing blog posts",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        }
    });

    // Enable XML comments in Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Create XML documentation file if it doesn't exist
    if (!File.Exists(xmlPath))
    {
        File.Create(xmlPath).Dispose();
    }
    options.IncludeXmlComments(xmlPath);

    // Add security definitions if needed
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Organize endpoints by controller
    options.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    options.DocInclusionPredicate((docName, description) => true);
});

// Configure DB Context with retry logic
builder.Services.AddDbContext<BlogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
    }

    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("BlogAPI.Infrastructure");
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(60); // Increase timeout to 60 seconds
    });

    // Enable detailed errors in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

// Register services
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IBlogService, BlogService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Add this after var app = builder.Build();
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // Ensure database exists and apply migrations
        context.Database.EnsureCreated();

        // Log successful connection
        logger.LogInformation("Database connection established successfully");
    }
}
catch (Exception ex)
{
    // Log detailed error information
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while initializing the database. Error details: {Message}", ex.Message);

    // In development, we want to see the full error
    if (app.Environment.IsDevelopment())
    {
        throw; // Rethrow in development to see full error details
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API v1");
        options.RoutePrefix = "swagger"; // This keeps Swagger at /swagger
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });

    // Add development-specific error handling
    app.UseDeveloperExceptionPage();
}

// Enable routing before CORS and other middleware
app.UseRouting();

// Configure HTTPS redirection with proper error handling
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngular");
app.UseAuthorization();

// Map controllers and ensure they're accessible
app.UseEndpoints(endpoints =>
{
    // Default route to index.html
    endpoints.MapFallbackToFile("/index.html");

    // Map controllers
    endpoints.MapControllers();
});

// Add this before app.Run() to serve static files
app.UseDefaultFiles();
app.UseStaticFiles();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithOpenApi();

app.Run();