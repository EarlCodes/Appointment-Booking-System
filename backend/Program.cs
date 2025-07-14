using BookingApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS policy
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAngular", policy =>
    {
        policy
          .WithOrigins("http://localhost:4200", "http://angular-app")
          .AllowAnyMethod()
          .AllowAnyHeader();
    });
});

// 2. EF Core
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// 3. Authentication/JWT
var jwtKey = builder.Configuration["Jwt:Key"];
Console.WriteLine($"JWT Key length: {jwtKey?.Length ?? 0}");
Console.WriteLine($"JWT Key: {jwtKey}");

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured. Please check your appsettings.json file.");
}

if (jwtKey.Length < 32)
{
    throw new InvalidOperationException($"JWT Key must be at least 32 characters long. Current length: {jwtKey.Length}");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opts =>
  {
      opts.RequireHttpsMetadata = false;
      opts.SaveToken = true;
      opts.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey         = new SymmetricSecurityKey(key),
          ValidateIssuer           = false,
          ValidateAudience         = false,
          ClockSkew                = TimeSpan.Zero
      };
  });

// 4. Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });
});

var app = builder.Build();
// 8. CORS
app.UseCors("AllowAngular");

// Apply EF Core migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
// 5. Swagger UI at root
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
    c.RoutePrefix = string.Empty;  // serve at http://localhost:5000/
});


// 7. Routing
app.UseRouting();



// 9. AuthN / AuthZ
app.UseAuthentication();
app.UseAuthorization();

// 10. Endpoints
app.MapControllers();

app.Run();
