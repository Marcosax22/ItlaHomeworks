
using GameStore.API.Middleware;
using GameStore.API.Middlewares;
using GameStore.Infrastructure.Data;
using GameStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace GameStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            var connFromEnv = Environment.GetEnvironmentVariable("GAMESTORE_CONNECTION");
            var connectionString = !string.IsNullOrEmpty(connFromEnv)
                ? connFromEnv
                : builder.Configuration.GetConnectionString("DefaultConnection");

           
            builder.Services.AddDbContext<GameStoreDbContext>(o =>
            {
                o.UseSqlServer(connectionString);
            });

            builder.Services.AddScoped<GameRepository>();
            builder.Services.AddScoped<UnitOfWork>();

            builder.Services.AddControllers();
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

            app.UseExceptionMiddleware();
            app.UseMiddleware<ResponseWrappingMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}