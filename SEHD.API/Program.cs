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

// Entity Framework - choose one based on your needs:

// Option A: Use SQL Server (for production/real database testing)
builder.Services.AddDbContext<SEHDDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Option B: Use In-Memory Database (for testing without real database)
// Uncomment this if you want to test without SQL Server connection

if (builder.Environment.IsDevelopment() && args.Contains("--use-memory-db"))
{
    builder.Services.AddDbContext<SEHDDbContext>(options =>
        options.UseInMemoryDatabase("SEHDTestDb"));
}
else
{
    builder.Services.AddDbContext<SEHDDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}


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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://localhost:5173",
                "https://umsehdhub.azurewebsites.net"
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

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ============================================================================
// 4. DATABASE INITIALIZATION (before app.Run())
// ============================================================================

// Initialize database and seed data
using (var initScope = app.Services.CreateScope()) // Changed from 'scope' to 'initScope'
{
    var context = initScope.ServiceProvider.GetRequiredService<SEHDDbContext>();
    try
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Seed initial users if they don't exist (always runs in development)
        await SeedInitialData(context);

        // Conditional test data seeding (only with --seed-data flag)
        if (app.Environment.IsDevelopment() && args.Contains("--seed-data"))
        {
            var seeder = initScope.ServiceProvider.GetRequiredService<ITestDataSeeder>(); // Use initScope here too
            await seeder.SeedTestDataAsync();

            var logger = initScope.ServiceProvider.GetRequiredService<ILogger<Program>>(); // And here
            logger.LogInformation("Test data seeding completed successfully!");
        }
    }
    catch (Exception ex)
    {
        var logger = initScope.ServiceProvider.GetRequiredService<ILogger<Program>>(); // And here
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