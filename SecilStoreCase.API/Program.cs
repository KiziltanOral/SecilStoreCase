using Microsoft.EntityFrameworkCore; 
using SecilStoreCase.Application.Interfaces;
using SecilStoreCase.Application.Services;
using SecilStoreCase.ConfigurationLibrary;
using SecilStoreCase.Infrastructure.DbContext;
using SecilStoreCase.Infrastructure.Repositories;
using AutoMapper;
using SecilStoreCase.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Add services to the container.
builder.Services.AddControllers(); 
builder.Services.AddDbContext<ApplicationDbContext>(options => 
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IConfigService, ConfigService>();
 

builder.Services.AddSingleton(provider =>
{
    var applicationName = builder.Configuration["ApplicationSettings:ApplicationName"];
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var refreshTimerIntervalInMs = 60000;

    return new ConfigurationReader(applicationName, connectionString, refreshTimerIntervalInMs);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
