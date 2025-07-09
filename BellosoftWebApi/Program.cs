using BellosoftWebApi.Boot;
using BellosoftWebApi.Responses;
using Microsoft.AspNetCore.Http;
using System.Net;

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

// Configure the HTTP request pipeline for Development
if (app.Environment.IsDevelopment())
{
    app.UseStatusCodePages(async context =>
    {
        const string TextContentType = "text/plain";
        HttpResponse response = context.HttpContext.Response;
        int statusCode = response.StatusCode;
        response.ContentType = TextContentType;

        switch (statusCode)
        {
            case 404:
                await response.WriteAsync("404 NotFound: Verifique se a rota existe, ou se a sessão foi iniciada para acessar rotas protegidas");
                break;
            default:
                string? statusName = Enum.GetName((HttpStatusCode)statusCode) ?? "Status";
                await response.WriteAsync($"{statusCode} {statusName}");
                break;
        }
    });

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
