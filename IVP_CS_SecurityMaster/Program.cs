
using IVP_CS_SecurityMaster.AuditRepository;
using IVP_CS_SecurityMaster.Security_Repository.BondOperation;
using IVP_CS_SecurityMaster.Security_Repository.EquityRepository;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IVP_CS_SecurityMaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:3000")
                                      .AllowAnyHeader()
                                       .AllowAnyMethod();
                                  });
            });
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                //.WriteTo.Console()
                //.WriteTo.File("log/MyLog.txt")
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddControllers();
            builder.Services.AddTransient<IEquity,EquityOperation>();
            builder.Services.AddTransient<IBond,BondOperation>();
            builder.Services.AddTransient<IAudit,AuditOperation>();
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
            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
