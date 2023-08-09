using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using text_compare.Core.Interface;
using text_compare.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<ICompareText, CompareText>();

builder.Services.AddSwaggerGen(options =>
{
    var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

    // Add a Swagger document for each discovered API version
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(
            description.GroupName,
            new OpenApiInfo
            {
                Title = "Text Compare",
                Version = description.ApiVersion.ToString(),
            });
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowOrigin");

app.Run();
