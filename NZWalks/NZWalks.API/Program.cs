using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name ="JWT Authentication",
        Description ="Enter valid JWT bearer token",
        In= ParameterLocation.Header,
        Type =SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat ="JWT",
        Reference =new OpenApiReference
        {
            Id=JwtBearerDefaults.AuthenticationScheme,
            Type= ReferenceType.Schema
        }
    };
    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme,new string[]{} }
    });
});

builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IWalkRepository, WalkRepository>();
builder.Services.AddScoped<IWalkDifficultyRepository, WalkDifficultyRepository>();
builder.Services.AddScoped<ITokenHandler, NZWalks.API.Repository.TokenHandler>();

builder.Services.AddSingleton<IUserRepository,StaticUserRepository>();


builder.Services.
    AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddDbContext<NZWalksDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalks"));
});
builder.Services.AddControllersWithViews()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
       ValidateIssuer  = true,
       ValidateAudience = true,
       ValidateLifetime = true,
       ValidateIssuerSigningKey = true,
       ValidIssuer= builder.Configuration["Jwt:Issuer"],
       ValidAudience=builder.Configuration["Jwt:Audience"],
       IssuerSigningKey= new SymmetricSecurityKey(
           Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
   // app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "NZWalks.API V1"); });
   // app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v3/swagger.json", "NZWalks.API"); });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
