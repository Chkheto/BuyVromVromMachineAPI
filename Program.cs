using FluentValidation;
using MyVromVromMachineAPI.Validators;
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
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

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
