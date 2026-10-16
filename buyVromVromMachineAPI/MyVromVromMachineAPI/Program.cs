using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyVromVromMachineAPI.Data;
using MyVromVromMachineAPI.helpers;
using MyVromVromMachineAPI.Services.UserService;
using MyVromVromMachineAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>();
builder.Services.AddScoped<IUserService, UsersService>();
builder.Services.AddAutoMapper(cnf => { }, typeof(UserMapper));
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
    DataSeeder.SeedCars(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwagger();
    app.MapSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
