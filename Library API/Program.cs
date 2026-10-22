
using Library_API;
using Library_API.Data;
using Library_API.Models;
using Library_API.Repository.IRepository;
using Library_API.Repository.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace Library_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingConfig).Assembly);
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IBookCopyRepository, BookCopyRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
