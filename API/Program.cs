using CasosDeUso;
using Entidades.Util;
using Infra;
using InterfaceAdapters;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});



// Configuração de Autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options =>
     {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = false, // Ajuste conforme necessário
             ValidateAudience = false, // Ajuste conforme necessário
             ValidateLifetime = true, // Valida o tempo de expiração do token
             ValidateIssuerSigningKey = true, // Valida a chave de assinatura
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abacaxi")) // Substitua pela chave usada no Lambda para assinar o token
         };
     });



    //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //    .AddJwtBearer(options =>
    //    {
    //        options.Authority = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_h9XniQP9F/.well-known/jwks.json"; // Substitua pela URL do User Pool
    //        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    //        {
    //            ValidateIssuer = true, // Valida o emissor
    //            ValidIssuer = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_h9XniQP9F/.well-known/jwks.json", // Deve ser o mesmo Authority
    //            ValidateAudience = true, // Valida a audiência
    //            ValidAudience = "3oee51ddidiamtuk2821qkf4jr", // Substitua pelo Client ID do Cognito
    //            ValidateLifetime = true, // Certifique-se de que o token não está expirado
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abacaxi"))// Chave usada para assinar o JWT no Lambda
    //        };
    //    });

    builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<CognitoService>(provider =>
    new CognitoService("us-east-1_h9XniQP9F"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
   

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

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
            Array.Empty<string>()
        }
    });
});

#region [DI]
CasosDeUsoBootstrapper.Register(builder.Services);
InfraBootstrapper.Register(builder.Services, builder.Configuration);
InterfaceAdaptersBootstrapper.Register(builder.Services);

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Middleware de CORS
app.UseCors("AllowAllOrigins");

// Habilita arquivos estáticos
app.UseStaticFiles();

app.UseRouting();

// Middleware de autenticação e autorização
app.UseAuthentication(); // Adiciona autenticação com JWT
app.UseAuthorization();

// Configuração do Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "McKing");
    c.RoutePrefix = string.Empty;
    c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
});

// Mapeia os controladores
app.MapControllers();

// Middleware de Tratamento de Exceções
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error is RegraNegocioException ex)
        {
            var errorDetails = new
            {
                Title = "Regra de Negócio",
                ex.Message,
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorDetails));
        }
    });
});

// Inicia o aplicativo
app.Run();
