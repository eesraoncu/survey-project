using MongoDB.Driver;
using SurveyApp.Models;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Services;
using AutoMapper;
using SurveyApp.Application.Profiles;

var builder = WebApplication.CreateBuilder(args);   

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// MongoDB DI kayıtları (IMongoClient, IMongoDatabase)
var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDBSettings>();
if (mongoSettings is not null && !string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
{
    builder.Services.AddSingleton<IMongoClient>(_ => 
    {
        var settings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        
        // SSL'i tamamen devre dışı bırak
        settings.SslSettings = new SslSettings
        {
            EnabledSslProtocols = System.Security.Authentication.SslProtocols.None,
            CheckCertificateRevocation = false
        };
        
        // Timeout ayarları
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(60);
        settings.ConnectTimeout = TimeSpan.FromSeconds(60);
        settings.SocketTimeout = TimeSpan.FromSeconds(60);
        settings.HeartbeatTimeout = TimeSpan.FromSeconds(60);
        
        return new MongoClient(settings);
    });
    
    builder.Services.AddSingleton<IMongoDatabase>(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(mongoSettings.DatabaseName);
    });
    
    // AutoIncrementService'i ekle
    builder.Services.AddScoped<AutoIncrementService>();
}

// Service'leri kaydet
builder.Services.AddScoped<IAddressService, AddressService>();

// Repository'leri kaydet
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

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