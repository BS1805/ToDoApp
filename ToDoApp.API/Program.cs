using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.Domain.Entities;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Services;
using ToDoApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Register application services and repository
builder.Services.AddScoped<IToDoService, ToDoService>();
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<PermissionService>();

// Register DataSeeder
builder.Services.AddScoped<DataSeeder>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<DataSeeder>();
    await DataSeeder.SeedAll(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
