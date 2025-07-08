using BellosoftWebApi.Boot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var boot = new AppBootServices(builder);
boot.AddDatabase();
boot.AddAuth();
boot.AddControllers();
boot.AddSwagger();
boot.AddInfrastructureServices();
boot.AddApplicationServices();
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
