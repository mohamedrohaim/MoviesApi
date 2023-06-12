using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MoviesApi.Models;

namespace MoviesApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connctionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDBContext>(options=>
            options.UseSqlServer(connctionString)
            );
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            // allow Cors
            builder.Services.AddCors();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(name: "v1", info: new OpenApiInfo {
                    Version = "v1",
                    Title = "Test Api",
                    Description = "My First Api",
                    TermsOfService = new Uri("https://www.linkedin.com/in/mohamed-rohiem-310927200/"),
                    Contact = new OpenApiContact {
                        Name = "Mohamed Rohiem",
                        Email = "mohamedrohiem597@gmail.com"
                    }
                });

                // make token for header request
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter Your Api Key",
                });
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement {
                        {
                        new OpenApiSecurityScheme{

                        Reference=new OpenApiReference{

                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer",

                        },
                        Name="Bearer",
                        In=ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    }
                    );
            
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            // add midelware of Cors 
            app.UseCors(c =>
            c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
            );
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}