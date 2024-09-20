using DisruptionMonitoring.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true; // Optional: for pretty-printed JSON
    });

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add CORS services and define a policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:8002", "http://localhost:8002", "http://localhost:3000", "http://18.141.34.124:3000", "http://localhost:8000", "http://localhost:4200", "http://18.141.34.124:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowSpecificOrigin");

// Remove or comment out HTTPS redirection if not using HTTPS in development
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Configure Swagger UI after MapControllers
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
