using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TIEN_LAN_ZU_SHI.Business.Interfaces;
using TIEN_LAN_ZU_SHI.Business.Services;
using TIEN_LAN_ZU_SHI.DataAccess.Interfaces;
using TIEN_LAN_ZU_SHI.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//Lectura de la configuracion de JwtSettings 
var JwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = JwtSettings["SecretKey"];

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuración de Swagger para que agregue seguridad 

builder.Services.AddSwaggerGen(options =>

{
    // 1. Define el esquema de seguridad (Security Scheme) 
    // Esto le dice a Swagger que la API usa autenticación "Bearer" (JWT). 
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // El nombre del header HTTP 
        Type = SecuritySchemeType.ApiKey, // Tipo de esquema 
        Scheme = "bearer", // El nombre del esquema (debe ser minúscula)
        BearerFormat = "JWT", // Formato del token 
        In = ParameterLocation.Header, // Dónde se envía el token 
        Description = "Introduce tu token JWT usando este formato: Bearer {token}"
    });

    // 2. Añade el requisito de seguridad (Security Requirement) 
    // Esto le dice a Swagger que debe aplicar el esquema "Bearer" a los endpoints. 
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }

            },
            new string[] {}

        }

    });

});

//Configuracion del servicio de Autenticacion 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>

{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = JwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = JwtSettings["Audiencie"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))

    };

});






//Registros de Services y Repositories del proyecto
//para inyeccion de dependencias
builder.Services.AddScoped<ICategoryService,CategoryService>();
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();

builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<ICustomerRepository,CustomerRepository>();

builder.Services.AddScoped<IInventoryService,InventoryService>();
builder.Services.AddScoped<IInventoryRepository,InventoryRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IPortionService, PortionService>();
builder.Services.AddScoped<IPortionRepository, PortionRepository>();

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

builder.Services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddScoped<IMovementInventoryRepository, MovementInventoryRepository>();
builder.Services.AddScoped<IMovementInventoryService, MovementInventoryService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
