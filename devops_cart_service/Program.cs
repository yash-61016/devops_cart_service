using AutoMapper;
using devops_cart_service;
using devops_cart_service.Data;
using devops_cart_service.Endpoints;
using devops_cart_service.Filters;
using devops_cart_service.Models.Dto;
using devops_cart_service.Repository;
using devops_cart_service.Repository.IRepository;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICartOverviewRepository, CartOverviewRepository>();
builder.Services.AddScoped<ICartProductRepository, CartProductRepository>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// resolve dependency injection for iendpointfilter
builder.Services.AddScoped(typeof(IValidator<>), typeof(BasicValidator<>));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

ApplyMigration();
app.ConfigureCartEndpoints();
app.Run();


void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}