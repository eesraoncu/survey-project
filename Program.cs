using SurveyApp.Services;
using MongoDB.Driver;
using SurveyApp.Models;
using SurveyApp.Infrastructure.Repositories;
using AutoMapper;
using SurveyApp.Application.Profiles;

var builder = WebApplication.CreateBuilder(args);   

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// MongoDB servisini kaydet (var olan servis)
builder.Services.AddSingleton<MongoDBService>();

// MongoDB DI kayıtları (IMongoClient, IMongoDatabase)
var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
if (mongoSettings is not null && !string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
{
    builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
    builder.Services.AddSingleton<IMongoDatabase>(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(mongoSettings.DatabaseName);
    });

    // Repository kayıtları
    builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
    builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
    builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run(); 