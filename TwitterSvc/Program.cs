using System.Xml;
using TwitterSvc;
using Microsoft.EntityFrameworkCore;
using TwitterSvc.Models;
using System.Diagnostics;
using System.Security.Policy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TwitterContext>(opt =>
    opt.UseInMemoryDatabase("TweetList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IStartWorker, StartWorker>();
builder.Services.AddHostedService<TwitterApiTask>();

builder.Services.AddSingleton<ITwitterAccess, TwitterAccess>();
builder.Services.AddSingleton<IDataStore, DataStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
