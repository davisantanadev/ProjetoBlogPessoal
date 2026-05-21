using BlogPessoal.Data;
using BlogPessoal.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => 
        options.JsonSerializerOptions
            .ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string MySqlConecction = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(MySqlConecction,
                    ServerVersion.AutoDetect(MySqlConecction)));


builder.Services.AddScoped<ITemaRepository, TemaRepository>();
builder.Services.AddScoped<IPostagemRepository, PostagemRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
