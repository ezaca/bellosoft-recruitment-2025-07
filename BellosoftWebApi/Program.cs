using BellosoftWebApi.Boot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var boot = new AppBootServices(builder);
boot.SetupDatabase();
boot.SetupAuth();
boot.SetupControllers();
boot.SetupSwagger();
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
