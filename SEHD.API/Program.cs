using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SEHD.API.Data;
using SEHD.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. SERVICE REGISTRATION SECTION (before var app = builder.Build())
// ============================================================================

// Add services to the container
builder.Services.AddControllers();

// Entity Framework - FORCE IN-MEMORY DATABASE FOR TESTING
// Comment out SQL Server and use only in-memory for testing
builder.Services.AddDbContext<SEHDDbContext>(options =>
    options.UseInMemoryDatabase("SEHDTestDb"));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization
builder.Services.AddAuthorization();

// CORS - Updated for testing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:5174",
                "https://localhost:5174"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

// Services
builder.Services.AddScoped<IAdmissionService, AdmissionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITestDataSeeder, TestDataSeeder>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SEHD Admissions API",
        Version = "v1",
        Description = "API for SEHD Admissions Dashboard"
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ============================================================================
// 2. BUILD THE APP (this creates the application instance)
// ============================================================================
var app = builder.Build();

// ============================================================================
// 3. MIDDLEWARE PIPELINE CONFIGURATION (after var app = builder.Build())
// ============================================================================

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SEHD Admissions API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// Remove HTTPS redirect for local testing
// app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ============================================================================
// 4. DATABASE INITIALIZATION (before app.Run())
// ============================================================================

// Initialize database and seed data
using (var initScope = app.Services.CreateScope())
{
    var context = initScope.ServiceProvider.GetRequiredService<SEHDDbContext>();
    var logger = initScope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Ensure database is created
        context.Database.EnsureCreated();
        logger.LogInformation("In-memory database created successfully!");

        // Seed initial users 
        await SeedInitialData(context);
        logger.LogInformation("Initial user data seeded successfully!");

        // ALWAYS seed test data for in-memory testing
        var seeder = initScope.ServiceProvider.GetRequiredService<ITestDataSeeder>();
        await seeder.SeedTestDataAsync();
        logger.LogInformation("Test data seeding completed successfully!");

        // Log what data we have
        var userCount = await context.Users.CountAsync();
        var admissionCount = await context.AdmissionsData.CountAsync();
        var deptCount = await context.Departments.CountAsync();
        var programCount = await context.AcademicPrograms.CountAsync();
        var termCount = await context.AcademicTerms.CountAsync();

        logger.LogInformation($"Database initialized with: {userCount} users, {deptCount} departments, {programCount} programs, {termCount} terms, {admissionCount} admission records");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

// ============================================================================
// 5. START THE APPLICATION (this must be last!)
// ============================================================================
app.Run();

// ============================================================================
// 6. HELPER METHODS (after app.Run())
// ============================================================================

// Helper method to seed initial users (runs every time in development)
static async Task SeedInitialData(SEHDDbContext context)
{
    // Check if users already exist
    if (!await context.Users.AnyAsync())
    {
        var users = new[]
        {
            new SEHD.API.Models.User
            {
                Email = "admin@miami.edu",
                PasswordHash = SEHD.API.Services.AuthService.HashPassword("admin123"),
                FirstName = "Admin",
                LastName = "User",
                Role = "admin",
                IsActive = true
            },
            new SEHD.API.Models.User
            {
                Email = "staff@miami.edu",
                PasswordHash = SEHD.API.Services.AuthService.HashPassword("staff123"),
                FirstName = "Staff",
                LastName = "Member",
                Role = "staff",
                IsActive = true
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }
}