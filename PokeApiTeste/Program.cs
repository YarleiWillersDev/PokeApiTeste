using Microsoft.EntityFrameworkCore;
using PokeApiTeste.Context;
using PokeApiTeste.Integrations.PokeApi;
using PokeApiTeste.Mapper;
using PokeApiTeste.Service;

var builder = WebApplication.CreateBuilder(args);

var ConnectionStringDataBase = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(ConnectionStringDataBase));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<PokeApiClient>(client =>
{
    var config = builder.Configuration.GetSection("PokeApi");
    client.BaseAddress = new Uri(config["BaseUrl"]!);
    client.DefaultRequestHeaders.Add("User-Agent", config["UserAgent"]!);
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IPokemonService, PokemonService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
