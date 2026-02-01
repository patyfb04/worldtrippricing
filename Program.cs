using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from environment variable (strictly from env vars as requested)
var conn = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
           ?? Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

builder.Services.AddDbContext<TripContext>(options =>
    options.UseSqlServer(conn));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(builder=>builder.WithOrigins("*"));
}

app.UseHttpsRedirection();


app.MapGet("/tripPricing/{id}", async (int id, TripContext db) =>
{
    var price = await db.TripPricing.FirstOrDefaultAsync(t => t.TripId == id);
    return price is null ? Results.NotFound() : Results.Ok(price);
})
.WithName("GetTripPricing")
.WithOpenApi();

app.Run();