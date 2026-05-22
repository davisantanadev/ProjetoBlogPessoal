using BlogPessoal.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfig();
builder.Services.AddSwaggerConfig();
builder.Services.AddDatabaseConfig(builder.Configuration);
builder.Services.AddJwtConfig(builder.Configuration);
builder.Services.AddAiConfig(builder.Configuration);
builder.Services.AddDependencyInjectionConfig();

var app = builder.Build();

app.UseApiConfig();

app.Run();
